using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DropOff : MonoBehaviour
{ 
    private QuestController questController;

    // Start is called before the first frame update
    public void Initialize(QuestController _qc)
    {
        // Assign the quest controller
        questController = _qc;
    }

    public void PlayerArrivedAtDeliveryLocation()
    {
        questController.PlayerArrivedAtDeliveryLocation(this);
    }
}
