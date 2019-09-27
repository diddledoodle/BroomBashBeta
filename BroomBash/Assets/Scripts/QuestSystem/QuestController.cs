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

    [Tooltip("The amount of time the player has at the start of the game in seconds")]
    public float startingTimeLimit = 90f; // seconds
    [Tooltip("The amount of time added to the time limit for [easy] quests in seconds")]
    public float easyTimeAddition = 15f; // Seconds
    [Tooltip("The amount of time added to the time limit for [medium] quests in seconds")]
    public float mediumTimeAddition = 25f; // Seconds
    [Tooltip("The amount of time added to the time limit for [hard] quests in seconds")]
    public float hardTimeAddition = 35f; // Seconds
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

    [Button]
    [DisableInEditorMode]
    [DisableIf("playerHasQuest")]
    public void AssignNewQuestToPlayer()
    {
        AssignQuestToPlayer();
    }

    [SerializeField]
    [DisableInPlayMode]
    [DisableInEditorMode]
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
        // Set the timer
        timeLeft = startingTimeLimit;
        // Test get random quest
        AssignQuestToPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        // Count down the time
        CountdownTimer();
        // TODO: End the game when the timer hits zero
    }

    public void PlayerArrivedAtPickUpLocation(PickUp _pickUpLocation)
    {
        if(_pickUpLocation == closestPickUp && playerHasQuest && playerHasDelivery == false)
        {
            playerHasDelivery = true;
            Debug.Log("<color=red>Player picked up a delivery!</color>");
        }
    }

    public void PlayerArrivedAtDeliveryLocation(DropOff _questLocation)
    {
        if(_questLocation == currentQuest && playerHasQuest && playerHasDelivery)
        {
            playerHasQuest = false;
            playerHasDelivery = false;
            // Add time to timeLeft
            switch (_questLocation.questDifficulty)
            {
                case DropOff.QuestDifficulty.EASY:
                    timeLeft += easyTimeAddition;
                    break;
                case DropOff.QuestDifficulty.MEDIUM:
                    timeLeft += mediumTimeAddition;
                    break;
                case DropOff.QuestDifficulty.HARD:
                    timeLeft += hardTimeAddition;
                    break;
            }
            Debug.Log("<color=blue>Player made a delivery!</color>");
        }
    }

    private float CountdownTimer()
    {
        float _timeLeft = timeLeft -= Time.deltaTime;
        return _timeLeft;
    }

    private void AssignQuestToPlayer()
    {
        // Get random quest from available quests that wasn't the last quest
        GetRandomQuest();
        // Get the closest pick up location from list of pick up locations
        GetClosestPickUpLocation();
        // Tell the manager that the player does have a quest
        playerHasQuest = true;
    }

    private void GetRandomQuest()
    {
        if(dropOffs.Count > 1)
        {
            int _questAssignmentIndex = randomNumber.Next(0, dropOffs.Count);
            // Make sure we don't get the same quest as the last time
            if(_questAssignmentIndex == lastQuestIndex)
            {
                int _flipCoin = randomNumber.Next(0, 2);
                if (_questAssignmentIndex == dropOffs.Count - 1)
                {
                    
                    _questAssignmentIndex = (_flipCoin == 1) ? 0 : _questAssignmentIndex -= 1;
                }
                else if(_questAssignmentIndex == 0)
                {
                    _questAssignmentIndex = (_flipCoin == 1) ? 1 : dropOffs.Count - 1;
                }
                else
                {
                    _questAssignmentIndex = (_flipCoin == 1) ? _questAssignmentIndex += 1 : _questAssignmentIndex -= 1;
                }
            }
            currentQuest = dropOffs[_questAssignmentIndex];
            lastQuestIndex = _questAssignmentIndex;
        }
        else if(dropOffs.Count == 1)
        {
            currentQuest = dropOffs[0];
            // Set the last quest so we can't get it next time
            lastQuestIndex = 0;
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
