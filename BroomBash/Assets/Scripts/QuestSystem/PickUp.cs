using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private QuestController questController;
    private float timePlayerEnteredTrigger = -1;
    private bool playerEnteredTrigger = false;

    public void Initialize(QuestController _qc)
    {
        // Assign the quest controller
        questController = _qc;
    }

    /*jpost audio*/
    //re-enabling this ontrigger enter for audio purposes
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //debug
            Debug.Log("player has entered pickup zone");
            //play the enter active pickup wwise sound
            AkSoundEngine.PostEvent("play_bb_sx_game_int_enter_active_pickup", gameObject);
            //timePlayerEnteredTrigger = questController.timeSinceStart; legacy trigger for quest controller?
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.GetComponent<PlayerController>())
    //    {
    //        if(questController.timeSinceStart - timePlayerEnteredTrigger >= questController.timeToStayForPickUp)
    //        {
    //            if(playerEnteredTrigger == false)
    //            {
    //                questController.PlayerArrivedAtPickUpLocation(this);
    //                playerEnteredTrigger = true;
    //            }
    //        }
    //    }
    //}

    /*jpost audio*/
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //debug
            Debug.Log("exited the pickupzone");
            //stop the enter active pickup sound
            AkSoundEngine.PostEvent("stop_bb_sx_game_int_enter_active_pickup", gameObject);
            //play the exit active pickup sound
            AkSoundEngine.PostEvent("play_bb_sx_game_int_exit_active_pickup", gameObject);
            //timePlayerEnteredTrigger = -1; legacy trigger for quest controller?
            //playerEnteredTrigger = false; legacy trigger for quest controller?
        }
    }
}
