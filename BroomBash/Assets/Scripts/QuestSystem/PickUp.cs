using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private QuestController questController;

    public void Initialize(QuestController _qc)
    {
        // Assign the quest controller
        questController = _qc;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            questController.PlayerArrivedAtPickUpLocation(this);
        }
    }
}
