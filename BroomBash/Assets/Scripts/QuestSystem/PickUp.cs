using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private QuestController questController;
    private float timePlayerEnteredTrigger = -1;

    public void Initialize(QuestController _qc)
    {
        // Assign the quest controller
        questController = _qc;
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
            if(timePlayerEnteredTrigger - questController.timeLeft >= questController.timeToStayForPickUp)
            {
                questController.PlayerArrivedAtPickUpLocation(this);
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
