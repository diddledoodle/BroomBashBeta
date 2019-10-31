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

    /*jpost audio*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<SphereCollider>())
        {
            //debug
            Debug.Log("player has entered dropoff zone");
            //play the enter active dropoff wwise sound
            AkSoundEngine.PostEvent("play_bb_sx_game_int_delivery_dropoff", gameObject);
            //timePlayerEnteredTrigger = questController.timeSinceStart; legacy trigger for quest controller?
        }
    }
}
