using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class QuestController : MonoBehaviour
{
    public List<PickUp> pickUps = new List<PickUp>();
    public List<DropOff> dropOffs = new List<DropOff>();
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public PlayerController playerController;
    [HideInInspector]
    public LevelSystem playerLevelSystem;

    [HideInInspector]
    public PlayerUIManager playerUIManager;

    [Tooltip("The amount of time the player has to complete the [easy] quest in seconds")]
    public float easyQuestTimeLimit = 60f;
    [Tooltip("The amount of time the player has to complete the [medium] quest in seconds")]
    public float mediumQuestTimeLimit = 45f;
    [Tooltip("The amount of time the player has to complete the [hard] quest in seconds")]
    public float hardQuestTimeLimit = 30f;
    [Tooltip("The amount of points the player receives for completing [easy] quests")]
    public int easyQuestCompletionPoints = 10;
    [Tooltip("The amount of points the player receives for completing [medium] quests")]
    public int mediumQuestCompletionPoints = 25;
    [Tooltip("The amount of points the player receives for completing [hard] quests")]
    public int hardQuestCompletionPoints = 50;
    [HideInInspector]
    public float timeSinceStart = 0;
    [DisableInPlayMode]
    [DisableInEditorMode]
    public float timeLeft = 0;
    [Tooltip("The time in seconds the player has to stay in the pick up zone to make the pick up successful")]
    public float timeToStayForPickUp = 2f;
    [Tooltip("The time in seconds the player has to stay in the delivery zone to make the delivery successful")]
    public float timeToStayForDelivery = 2f;

    [Tooltip("The maximum distance from the player in meters to be considered an [easy] quest")]
    public float easyQuestDistance = 75f;
    [Tooltip("The maximum distance from the player in meters to be considered an [medium] quest")]
    public float mediumQuestDistance = 150f;
    [Tooltip("The maximum distance from the player in meters to be considered an [hard] quest")]
    public float hardQuestDistance = 300f;

    [Tooltip("The required level the player has to hit before they get [medium] quests")]
	public int easyQuestLevelCap = 3;
    [Tooltip("The required level the player has to hit before they get [hard] quests")]
	public int mediumQuestLevelCap = 6;
    // 0 = easy, 1 = medium, 2 = hard
	private int currentPlayerDifficulty = 0;

    [Tooltip("The amount of times the player can fail a quest before a game over")]
    public int maxPlayerFailedQuests = 3;
    [Tooltip("The amount of collisions the player can make with other objects during a quest")]
    public int maxPlayerCollisionsPerDelivery = 3;
    private int currentPlayerFailedQuests = 3;
    [SerializeField]
    private int currentPlayerCollisionsPerDelivery = 3;

	[HideInInspector]
    public bool countdownTimerIsActive = false;

    private bool playerHasQuest = false;
    [SerializeField]
    [DisableInPlayMode]
    [DisableInEditorMode]
    private bool playerHasDelivery = false;
    [SerializeField]
    [DisableInPlayMode]
    [DisableInEditorMode]
    private DropOff currentQuest = null;
    [SerializeField]
    [DisableInPlayMode]
    [DisableInEditorMode]
    private int lastQuestIndex = -1;
    [SerializeField]
    [DisableInPlayMode]
    [DisableInEditorMode]
    private PickUp closestPickUp = null;

    private System.Random randomNumber = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        // Get the player object
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        playerController = player.GetComponent<PlayerController>();
        playerLevelSystem = player.GetComponent<LevelSystem>();
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
            }
        }
        // Set max failed quests and player collisions
        currentPlayerFailedQuests = maxPlayerFailedQuests;
        currentPlayerCollisionsPerDelivery = maxPlayerCollisionsPerDelivery;
        // Test get random quest
        //AssignQuestToPlayer();
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

        // FIX: Really dirty hardcode for player stop
        if (playerUIManager.dialogSystemIsActive || playerUIManager.notificationSystemIsActive)
        {
            playerController.stopPlayer = true;
        }
        else playerController.stopPlayer = false;

        // TODO: End the game when the timer hits zero
        CheckForEndQuestFromFailure();
    }

    private void CheckForEndQuestFromFailure()
    {
        if(currentPlayerCollisionsPerDelivery <= 0)
        {
            EndQuestFromFailure();
        }
    }

    private void CheckForGameOver()
    {

    }

    public void PlayerArrivedAtPickUpLocation(PickUp _pickUpLocation)
    {
        if(!playerHasQuest && !playerHasDelivery)
        {
            playerUIManager.RunDialogSystem(_pickUpLocation.GetComponent<DialogSystem>().GetRandomQuestDialog(), PlayerUIManager.QuestStatus.START);
        }
    }
    
    public void StartQuest()
    {
        if (!playerHasQuest && !playerHasDelivery)
        {
            playerHasDelivery = true;
            Debug.Log("<color=red>Player picked up a delivery!</color>");
            // Set the 
            // Assign drop off within player level difficulty range
            GetQuestBasedOnCurrentPlayerDifficulty(currentPlayerDifficulty);
            // Start the countdown timer
            SetTimeLeftBasedOnPlayerDifficulty(currentPlayerDifficulty);
            countdownTimerIsActive = true;
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
        if (_questLocation == currentQuest && playerHasQuest && playerHasDelivery)
        {
            countdownTimerIsActive = false;
            playerUIManager.RunDialogSystem(_questLocation.GetComponent<DialogSystem>().GetRandomQuestDialog(), PlayerUIManager.QuestStatus.END);
        }
    }

    public void EndQuest()
    {
        playerHasQuest = false;
        playerHasDelivery = false;
        // Reset player collisions during quest
        currentPlayerCollisionsPerDelivery = maxPlayerCollisionsPerDelivery;
        // Add XP to player leveling system
        AddXpToPlayerLevelingSystem(currentPlayerDifficulty);
        // Csalculate the players current difficulty based on current level from xp gain
        CalculatePlayersCurrentDifficulty(playerLevelSystem.currentLevel);
        Debug.Log("<color=blue>Player made a delivery!</color>");
    }

    public void EndQuestFromFailure()
    {
        countdownTimerIsActive = false;
        playerHasQuest = false;
        playerHasDelivery = false;
        SubXpToPlayerLevelingSystem(currentPlayerDifficulty);
        CalculatePlayersCurrentDifficulty(playerLevelSystem.currentLevel);
        currentPlayerFailedQuests -= 1;
        currentPlayerCollisionsPerDelivery = maxPlayerCollisionsPerDelivery;
        playerUIManager.RunDialogSystem("You failed this quest! :(", PlayerUIManager.QuestStatus.FAIL);
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

    public void SubXpToPlayerLevelingSystem(int _playerDifficulty)
    {
        int _currentXP = playerLevelSystem.xp;
        int _xpSubstraction = 0;
        switch (_playerDifficulty)
        {
            case 0:
                _xpSubstraction = -easyQuestCompletionPoints / 2;
                if(_currentXP - _xpSubstraction > 0)
                {
                    playerLevelSystem.AddXpToPlayerLevel(_xpSubstraction);
                }
                break;
            case 1:
                _xpSubstraction = -mediumQuestCompletionPoints / 2;
                if (_currentXP - _xpSubstraction > 0)
                {
                    playerLevelSystem.AddXpToPlayerLevel(_xpSubstraction);
                }
                break;
            case 2:
                _xpSubstraction = -hardQuestCompletionPoints / 2;
                if (_currentXP - _xpSubstraction > 0)
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

    private void CalculatePlayersCurrentDifficulty(int _difficulty)
	{

	}

    //private void AssignQuestToPlayer()
    //{
    //    // Get random quest from available quests that wasn't the last quest
    //    GetRandomQuest();
    //    // Get the closest pick up location from list of pick up locations
    //    GetClosestPickUpLocation();
    //    // Tell the manager that the player does have a quest
    //    playerHasQuest = true;
    //}

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
            currentQuest = _dropOffList[_questAssignmentIndex];
            lastQuestIndex = _questAssignmentIndex;
        }
        else if(_dropOffList.Count == 1)
        {
            currentQuest = _dropOffList[0];
            // Set the last quest so we can't get it next time
            lastQuestIndex = 0;
        }
        playerHasQuest = true;
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
