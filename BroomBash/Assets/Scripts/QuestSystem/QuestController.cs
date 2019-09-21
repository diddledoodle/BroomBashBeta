using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class QuestController : MonoBehaviour
{

    public List<Quest> quests = new List<Quest>();
    [DisableInEditorMode]
    [DisableInPlayMode]
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

    // UI
    public Text timerText;

    [Tooltip("The maximum distance from the player in meters to be considered an [easy] quest")]
    public float easyQuestDistance = 75f;
    [Tooltip("The maximum distance from the player in meters to be considered an [medium] quest")]
    public float mediumQuestDistance = 150f;
    [Tooltip("The maximum distance from the player in meters to be considered an [hard] quest")]
    public float hardQuestDistance = 300f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the player object
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        // Initialize all of the quest objects
        if(quests.Count > 0)
        {
            foreach(Quest q in quests)
            {
                q.Initialize(this);
            }
        }
        // Set the timer
        timeLeft = startingTimeLimit;
    }

    // Update is called once per frame
    void Update()
    {
        // Count down the time
        CountdownTimer();
        // TODO: End the game when the timer hits zero

        // Update the player UI - needs to be optimized, no need to do this every frame
        UpdatePlayerUI();
    }

    private float CountdownTimer()
    {
        float _timeLeft = timeLeft -= Time.deltaTime;
        return _timeLeft;
    }

    private void UpdatePlayerUI()
    {
        timerText.text = $"{timeLeft.ToString("F0")} seconds";
    }
}
