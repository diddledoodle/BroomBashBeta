using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSurfaceBelowObject : MonoBehaviour
{
    /*jpost audio*/
    //fields
    private bool hasCollidedWithWater = false;    

    private void OnTriggerEnter(Collider other)
    {
        //if the object is colliding with water then play water/wave ambient sounds from wwise
        if (other.gameObject.name == "Water")
        {
            //debug
            Debug.Log("colliding with water!");
            if (!hasCollidedWithWater)
            {
                AkSoundEngine.PostEvent("play_bb_sx_game_amb_water_lap_stone", gameObject);
                hasCollidedWithWater = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if the object is no longer colliding with water, stop the water/wave ambient sounds playing from wwise
        if (other.gameObject.name == "Water")
        {
            Debug.Log("no longer colliding with water!");
            if (hasCollidedWithWater)
            {
                AkSoundEngine.PostEvent("stop_bb_sx_game_amb_water_lap_stone", gameObject);
                hasCollidedWithWater = false;
            }
        }
    }
    
}
