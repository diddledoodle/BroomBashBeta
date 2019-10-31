using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using PixelCrushers.DialogueSystem;

public class QuestController : MonoBehaviour
{
    [Header("Pick up and drop off scene objects")]
    public List<PickUp> pickUps = new List<PickUp>();
    public List<DropOff> dropOffs = new List<DropOff>();

    [Header("Location Materials")]
    public Material pickUpLocationMaterial;
    public Material dropOffLocationMaterial;
    public Material dropOffLocationActiveMaterial;
    public Material invisibleMaterial;

    [Header("Time Limits")]
    [Tooltip("The amount of time the player has to complete the [easy] quest in seconds")]
    public float easyQuestTimeLimit = 60f;
    [Tooltip("The amount of time the player has to complete the [medium] quest in seconds")]
    public float mediumQuestTimeLimit = 45f;
    [Tooltip("The amount of time the player has to complete the [hard] quest in seconds")]
    public float hardQuestTimeLimit = 30f;

    [Header("Boss Quests")]
    [Tooltip("Boss conversations list")]
    public List<DialogueSystemTrigger> bossConversations = new List<DialogueSystemTrigger>();
    [Tooltip("The frequency the boss asks you to do higher rated quests. Ex. For every x quest the boss asks the player")]
    public int bossQuestInquiryRate = 3;
    [Tooltip("The percent of the quest XP to be added on top. Ex. 0.15 is 15%")]
    public float bossQuestXpBonusPercent = 0.15f;

    [Header("Points for Deliveries")]
    [Tooltip("The amount of points the player receives for completing [easy] quests")]
    public int easyQuestCompletionPoints = 10;
    [Tooltip("The amount of points the player receives for completing [medium] quests")]
    public int mediumQuestCompletionPoints = 25;
    [Tooltip("The amount of points the player receives for completing [hard] quests")]
    public int hardQuestCompletionPoints = 50;
    
    [Header("Pick up and drop off times")]
    [Tooltip("The time in seconds the player has to stay in the pick up zone to make the pick up successful")]
    public float timeToStayForPickUp = 2f;
    [Tooltip("The time in seconds the player has to stay in the delivery zone to make the delivery successful")]
    public float timeToStayForDelivery = 2f;

    [Header("Quest difficulty distances")]
    [Tooltip("The maximum distance from the player in meters to be considered an [easy] quest")]
    public float easyQuestDistance = 75f;
    [Tooltip("The maximum distance from the player in meters to be considered an [medium] quest")]
    public float mediumQuestDistance = 150f;
    [Tooltip("The maximum distance from the player in meters to be considered an [hard] quest")]
    public float hardQuestDistance = 300f;

    [Header("Quest difficulty caps")]
    [Tooltip("The required level the player has to hit before they get [medium] quests")]
	public int easyQuestLevelCap = 3;
    [Tooltip("The required level the player has to hit before they get [hard] quests")]
	public int mediumQuestLevelCap = 6;
    // 0 = easy, 1 = medium, 2 = hard
	private int currentPlayerDifficulty = 0;

    [Header("Player quest failing")]
    [Tooltip("The amount of times the player can fail a quest before a game over")]
    public int maxPlayerFailedQuests = 3;
    [Tooltip("The amount of collisions the player can make with other objects during a quest")]
    public int maxPlayerCollisionsPerDelivery = 3;
    [HideInInspector]
    public int currentPlayerFailedQuests = 3;
    private int currentPlayerCollisionsPerDelivery = 3;

    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public PlayerController playerController;
    [HideInInspector]
    public LevelSystem playerLevelSystem;
    [HideInInspector]
    public PlayerUIManager playerUIManager;
    [HideInInspector]
    public float timeSinceStart = 0;
    [HideInInspector]
    public float timeLeft = 0;
    [HideInInspector]
    public bool countdownTimerIsActive = false;
    [HideInInspector]
    public GameObject currentQuest = null;

    private bool playerHasQuest = false;
    private bool playerHasBossQuest = false;
    private bool playerHasDelivery = false;
    private bool playerHasBossDelivery = false;
    private int lastQuestIndex = -1;
    private PickUp closestPickUp = null;
    private System.Random randomNumber = new System.Random();
    private bool startOfGame = true;
    [SerializeField]
    private int normalCompletedQuests = 0;
    private bool bossCanInquire = true;
    private bool bossQuestIsActive;

    /*jpost audio*/
    bool hasRunOutOfTime = false;
    // Start is called before the first frame update
    void Start()
    {
        // Get the player object
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        playerController = player.GetComponent<PlayerController>();
        playerLevelSystem = player.GetComponent<LevelSystem>();
        currentPlayerDifficulty = 0;
        // Initialize all of the pick up locations
        if(pickUps.Count > 0)
        {
            foreach(PickUp pu in pickUps)
            {
                pu.Initialize(this);
            }
        }
        // Initialize all of the quest objects
        if(dropOffs.Count > 0)
        {
            foreach(DropOff d in dropOffs)
            {
                d.Initialize(this);
                // Make all frop offs invisible
                d.gameObject.GetComponent<Renderer>().material = invisibleMaterial;
            }
        }
        // Set max failed quests and player collisions
        currentPlayerFailedQuests = maxPlayerFailedQuests;
        currentPlayerCollisionsPerDelivery = maxPlayerCollisionsPerDelivery;
        // Enable/Disable gameobjects
        NoQuestActiveGameObjects();

        // Stop the player
        //playerController.stopPlayer = true;
        // Run the game instructions on start
        //Invoke("RunStartInstructions", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Keep track of time since start
        CountupTimer();
        // Count down the time
        if (countdownTimerIsActive)
        {
            CountdownTimer();
        }

        // Check for boss required quests
        BossRelatedQuestAssignment();
        // End the game when the timer hits zero
        CheckForEndQuestFromFailure();
    }

    private void CheckForEndQuestFromFailure()
    {
        if((currentPlayerCollisionsPerDelivery <= 0 && countdownTimerIsActive)|| (timeLeft <= 0 && countdownTimerIsActive))
        {
            // End the game if all lives are exhausted
            if (currentPlayerFailedQuests < 1)
            {
                EndGameFromFailure();
            }
            // End the quest if lives > 1
            else
            {
                EndQuestFromFailure();
            }
            // Change quest material back to idle
            currentQuest.gameObject.GetComponent<Renderer>().material = dropOffLocationMaterial;
            
            /*jpost audio*/ 
            if (!hasRunOutOfTime)
            {
                //play the timer out sound from wwise
                AkSoundEngine.PostEvent("play_bb_sx_game_ui_timer_out", gameObject);
            }
        }
    }

    private void BossRelatedQuestAssignment()
    {
        if(normalCompletedQuests != 0 && normalCompletedQuests % bossQuestInquiryRate == 0 && bossCanInquire)
        {
            bossCanInquire = false;
            Invoke("AskPlayerAboutBossQuest", 3f);
        }
    }

    private void AskPlayerAboutBossQuest()
    {
        // TODO: Need to ask the player if they want to do a quest for a greater xp increase
        int _randBossConversationIndex = randomNumber.Next(0, bossConversations.Count);

        bossConversations[_randBossConversationIndex].OnUse();
    }

    public void CheckIfPlayerAcceptedBossQuest()
    {
        bool _playerChoice = DialogueLua.GetVariable("BossQuestIsActive").asBool;

        if (_playerChoice == true)
        {
            // TODO: get the closest pick up location
            GetClosestPickUpLocation();
            // Set the current quest to the pick up location
            currentQuest = closestPickUp.gameObject;
            // Start the countdown timer
            SetTimeLeftBasedOnPlayerDifficulty(currentPlayerDifficulty);
            countdownTimerIsActive = true;
        }
        else if (_playerChoice == false)
        {
            Debug.Log("Player declined the boss quest");
        }
    }

    public void CheckQuestType()
    {
        bool _playerChoice = DialogueLua.GetVariable("BossQuestIsActive").asBool;

        if (_playerChoice == true)
        {
            StartBossQuest();
        }
        else if (_playerChoice == false)
        {
            StartQuest();
        }
    }
    
    public void StartQuest()
    {
        if (!playerHasQuest && !playerHasDelivery)
        {
            playerHasDelivery = true;
            // Assign drop off within player level difficulty range
            GetQuestBasedOnCurrentPlayerDifficulty(currentPlayerDifficulty);
            // Start the countdown timer
            SetTimeLeftBasedOnPlayerDifficulty(currentPlayerDifficulty);
            countdownTimerIsActive = true;
            // Enable/Disable gameobjects
            ActiveQuestActiveGameObjects();

            /*jpost audio*/
            //play the accept quest sound from wwise
            AkSoundEngine.PostEvent("play_bb_sx_game_int_delivery_pickup", gameObject);
        }
        else
        {
            Debug.Log("Player already has a quest and/or delivery", this.gameObject);
            return;
        }
    }

    public void StartBossQuest()
    {
        if(!playerHasBossQuest && !playerHasBossDelivery)
        {
            bossQuestIsActive = true;
            playerHasBossDelivery = true;
            // Assign drop off within player level difficulty range
            GetQuestBasedOnCurrentPlayerDifficulty(currentPlayerDifficulty);
            // Enable/Disable gameobjects
            ActiveQuestActiveGameObjects();
        }
    }

    public void PlayerCollidedWithObjectDuringQuest()
    {
        currentPlayerCollisionsPerDelivery -= 1;
    }

    private void SetTimeLeftBasedOnPlayerDifficulty(int _difficulty)
    {
        switch (_difficulty)
        {
            case 0:
                timeLeft = easyQuestTimeLimit;
                break;
            case 1:
                timeLeft = mediumQuestTimeLimit;
                break;
            case 2:
                timeLeft = hardQuestTimeLimit;
                break;
        }
    }

    public void PlayerArrivedAtDeliveryLocation(DropOff _questLocation)
    {
        if (_questLocation.gameObject == currentQuest && ((playerHasQuest && playerHasDelivery) || playerHasBossQuest && playerHasBossDelivery))
        {
            countdownTimerIsActive = false;
            //playerUIManager.RunDialogSystem(_questLocation.GetComponent<DialogSystem>().GetRandomQuestDialog(), PlayerUIManager.QuestStatus.END);
        }
    }

    public void EndQuest()
    { 
        bool _playerChoice = DialogueLua.GetVariable("BossQuestIsActive").asBool;

        // Stuff for boss quest
        if (_playerChoice == true)
        {
            playerHasBossQuest = false;
            playerHasBossDelivery = false;
            EndBossQuest();
        }
        // Stuff for normal quest
        else if (_playerChoice == false)
        {
            playerHasQuest = false;
            playerHasDelivery = false;
            // Add XP to player leveling system
            AddXpToPlayerLevelingSystem(currentPlayerDifficulty);
            // Add one completed quest to completed quests
            normalCompletedQuests += 1;
            // Make sure the boss can inquire only after normal quests
            bossCanInquire = true;

            /*jpost audio*/
            //play add xp sound from wwise
            AkSoundEngine.PostEvent("play_bb_sx_game_ui_xp_gained", gameObject);
            //debug
            Debug.Log("xp sound should play");
        }

        // Reset player collisions during quest
        currentPlayerCollisionsPerDelivery = maxPlayerCollisionsPerDelivery;
        // Calculate the players current difficulty based on current level from xp gain
        CalculatePlayersCurrentDifficulty();
        // Enable/Disable gameobjects
        NoQuestActiveGameObjects();
    }

    public void EndBossQuest()
    {
        // Add boss quest xp bonus
        AddBossXpBonusToPlayerLevelingSystem(currentPlayerDifficulty);
        bossQuestIsActive = false;
        // Need to make sure the boss quest is not active in the dialogue system
        DialogueLua.SetVariable("BossQuestIsActive", false);
    }

    public void EndQuestFromFailure()
    {
        countdownTimerIsActive = false;
        playerHasQuest = false;
        playerHasBossQuest = false;
        playerHasDelivery = false;
        playerHasBossDelivery = false;
        SubXpToPlayerLevelingSystem(currentPlayerDifficulty);
        CalculatePlayersCurrentDifficulty();
        currentPlayerFailedQuests -= 1;
        currentPlayerCollisionsPerDelivery = maxPlayerCollisionsPerDelivery;
        //playerUIManager.RunDialogSystem("You failed this quest! :(", PlayerUIManager.QuestStatus.FAIL);
        // Enable/Disable gameobjects
        NoQuestActiveGameObjects();
    }

    public void EndGameFromFailure()
    {
        countdownTimerIsActive = false;
        playerHasQuest = false;
        playerHasDelivery = false;
        //playerUIManager.RunDialogSystem("<b>Game Over!</b>\n You failed too many quests.", PlayerUIManager.QuestStatus.GAMEOVER);
        // Enable/Disable gameobjects
        NoQuestActiveGameObjects();
    }

    private void NoQuestActiveGameObjects()
    {
        // Disable all drop off locations
        foreach(DropOff d in dropOffs)
        {
            d.GetComponent<Renderer>().material = invisibleMaterial;
            d.gameObject.SetActive(false);
        }
        // Enable all pick up locations
        foreach(PickUp p in pickUps)
        {
            p.gameObject.SetActive(true);
        }
    }

    private void ActiveQuestActiveGameObjects()
    {
        // Enable current drop off location
        foreach (DropOff d in dropOffs)
        {
            if(d.gameObject == currentQuest)
            {
                d.GetComponent<Renderer>().material = dropOffLocationActiveMaterial;
                d.gameObject.SetActive(true);
            }
        }
        // Enable all pick up locations
        foreach (PickUp p in pickUps)
        {
            p.gameObject.SetActive(false);
        }
    }

    private void CalculatePlayersCurrentDifficulty()
    {
        if(playerLevelSystem.currentLevel < easyQuestLevelCap)
        {
            currentPlayerDifficulty = 0;
        }
        else if(playerLevelSystem.currentLevel >= easyQuestLevelCap && playerLevelSystem.currentLevel < mediumQuestLevelCap)
        {
            currentPlayerDifficulty = 1;
        }
        else if (playerLevelSystem.currentLevel >= mediumQuestLevelCap)
        {
            currentPlayerDifficulty = 2;
        }
    }

    public void AddXpToPlayerLevelingSystem(int _playerDifficulty)
    {
        switch (_playerDifficulty)
        {
            case 0:
                playerLevelSystem.AddXpToPlayerLevel(easyQuestCompletionPoints);
                break;
            case 1:
                playerLevelSystem.AddXpToPlayerLevel(mediumQuestCompletionPoints);
                break;
            case 2:
                playerLevelSystem.AddXpToPlayerLevel(hardQuestCompletionPoints);
                break;
        }
    }

    public void AddBossXpBonusToPlayerLevelingSystem(int _playerDifficulty)
    {
        switch (_playerDifficulty)
        {
            case 0:
                playerLevelSystem.AddXpToPlayerLevel((int)((easyQuestCompletionPoints * bossQuestXpBonusPercent) + easyQuestCompletionPoints));
                break;
            case 1:
                playerLevelSystem.AddXpToPlayerLevel((int)((mediumQuestCompletionPoints * bossQuestXpBonusPercent) + mediumQuestCompletionPoints));
                break;
            case 2:
                playerLevelSystem.AddXpToPlayerLevel((int)((hardQuestCompletionPoints * bossQuestXpBonusPercent) + hardQuestCompletionPoints ));
                break;
        }
    }

    public void SubXpToPlayerLevelingSystem(int _playerDifficulty)
    {
        int _currentXP = playerLevelSystem.xp;
        int _xpSubstraction = 0;
        switch (_playerDifficulty)
        {
            case 0:
                _xpSubstraction = -easyQuestCompletionPoints / 2;
                if(_currentXP - Mathf.Abs(_xpSubstraction) > 0)
                {
                    playerLevelSystem.AddXpToPlayerLevel(_xpSubstraction);
                }
                break;
            case 1:
                _xpSubstraction = -mediumQuestCompletionPoints / 2;
                if (_currentXP - Mathf.Abs(_xpSubstraction) > 0)
                {
                    playerLevelSystem.AddXpToPlayerLevel(_xpSubstraction);
                }
                break;
            case 2:
                _xpSubstraction = -hardQuestCompletionPoints / 2;
                if (_currentXP - Mathf.Abs(_xpSubstraction) > 0)
                {
                    playerLevelSystem.AddXpToPlayerLevel(_xpSubstraction);
                }
                break;
        }
    }

    private float CountupTimer()
    {
        float _timeSinceStart = timeSinceStart += Time.deltaTime;
        return _timeSinceStart;
    }

    private float CountdownTimer()
    {
        float _timeLeft = timeLeft -= Time.deltaTime;
        return _timeLeft;
    }

    // private void AssignQuestToPlayer()
    // {
    //    // Get random quest from available quests that wasn't the last quest
    //    GetRandomQuest();
    //    // Get the closest pick up location from list of pick up locations
    //    GetClosestPickUpLocation();
    //    // Tell the manager that the player does have a quest
    //    playerHasQuest = true;
    // }

    private void GetQuestBasedOnCurrentPlayerDifficulty(int _difficulty)
    {
        List<DropOff> _d = new List<DropOff>();
        // Get all the drop off locations within the difficulty distance
        switch (_difficulty)
        {
            case 0:
                _d = MeasureDropOffDistances(1, easyQuestDistance);
                break;
            case 1:
                _d = MeasureDropOffDistances(easyQuestDistance, mediumQuestDistance);
                break;
            case 2:
                _d = MeasureDropOffDistances(mediumQuestDistance, hardQuestDistance);
                break;
        }
        // If there are no drop offs within range then randomly choose one from all of them
        if(_d.Count == 0)
        {
            _d = dropOffs;
        }
        // Assign a random drop off within the difficulty distance
        GetRandomQuestInDistance(_d);
    }

    private List<DropOff> MeasureDropOffDistances(float _minDistance, float _maxDistance)
    {
        List<DropOff> _d = new List<DropOff>();
        foreach(DropOff d in dropOffs)
        {
            float _distance = Vector3.Distance(player.transform.position, d.transform.position);
            if (_distance >= _minDistance && _distance <= _maxDistance)
            {
                _d.Add(d);
            }
        }
        return _d;
    }

    private void GetRandomQuestInDistance(List<DropOff> _dropOffList)
    {
        if(_dropOffList.Count > 1)
        {
            int _questAssignmentIndex = randomNumber.Next(0, _dropOffList.Count);
            // Make sure we don't get the same quest as the last time
            if(_questAssignmentIndex == lastQuestIndex)
            {
                int _flipCoin = randomNumber.Next(0, 2);
                if (_questAssignmentIndex == _dropOffList.Count - 1)
                {
                    
                    _questAssignmentIndex = (_flipCoin == 1) ? 0 : _questAssignmentIndex -= 1;
                }
                else if(_questAssignmentIndex == 0)
                {
                    _questAssignmentIndex = (_flipCoin == 1) ? 1 : _dropOffList.Count - 1;
                }
                else
                {
                    _questAssignmentIndex = (_flipCoin == 1) ? _questAssignmentIndex += 1 : _questAssignmentIndex -= 1;
                }
            }
            currentQuest = _dropOffList[_questAssignmentIndex].gameObject;
            lastQuestIndex = _questAssignmentIndex;
        }
        else if(_dropOffList.Count == 1)
        {
            currentQuest = _dropOffList[0].gameObject;
            // Set the last quest so we can't get it next time
            lastQuestIndex = 0;
        }

        // Set player has quest type
        bool _playerChoice = DialogueLua.GetVariable("BossQuestIsActive").asBool;

        if (_playerChoice == true)
        {
            playerHasBossQuest = true;
        }
        else if (_playerChoice == false)
        {
            playerHasQuest = true;
        }
    }

    private void GetClosestPickUpLocation()
    {
        if(pickUps.Count > 1)
        {
            float _shortestDistance = float.MaxValue;
            PickUp _shortestDistnacePickUp = null;
            foreach (PickUp pu in pickUps)
            {
                float _distance = Vector3.Distance(player.transform.position, pu.transform.position);
                if (_distance < _shortestDistance)
                {
                    _shortestDistance = _distance;
                    _shortestDistnacePickUp = pu;
                }
            }
            // Assign the pick up location
            closestPickUp = _shortestDistnacePickUp;
        }
        else if(pickUps.Count == 1)
        {
            closestPickUp = pickUps[0];
        }
       
    }
}
