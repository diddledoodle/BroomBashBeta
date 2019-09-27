using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DropOff : MonoBehaviour
{
    public string dropOffName = "NO NAME";
    private float timePlayerEnteredTrigger = -1f;
    public enum QuestDifficulty { EASY, MEDIUM, HARD}
    [DisableInPlayMode]
    [DisableInEditorMode]
    public QuestDifficulty questDifficulty = QuestDifficulty.EASY;

    private QuestController questController;
    private GameObject player;
    private float playerDistance = 0;

    // Start is called before the first frame update
    public void Initialize(QuestController _qc)
    {
        // Assign the quest controller
        questController = _qc;
        player = _qc.player;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the quest difficulty based on current distance from the player
        CalculateQuestDifficulty(playerDistance = CalculateDistanceToPlayer());
    }

    private float CalculateDistanceToPlayer()
    {
        float _distance = Vector3.Distance(this.transform.position, player.transform.position);
        return _distance;
    }

    private void CalculateQuestDifficulty(float _distanceToPlayer)
    {
        if(_distanceToPlayer > 0 &&_distanceToPlayer < questController.easyQuestDistance)
        {
            questDifficulty = QuestDifficulty.EASY;
        }
        else if(_distanceToPlayer > questController.easyQuestDistance && _distanceToPlayer < questController.mediumQuestDistance)
        {
            questDifficulty = QuestDifficulty.MEDIUM;
        }
        else if(_distanceToPlayer > questController.mediumQuestDistance)
        {
            questDifficulty = QuestDifficulty.HARD;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            timePlayerEnteredTrigger = questController.timeLeft;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            if(timePlayerEnteredTrigger - questController.timeLeft >= questController.timeToStayForDelivery)
            {
                questController.PlayerArrivedAtDeliveryLocation(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            timePlayerEnteredTrigger = -1;
        }
    }
}
