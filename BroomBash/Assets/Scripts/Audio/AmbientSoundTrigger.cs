using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //play the ambient sound at random intervals between 10 and 20 seconds
        InvokeRepeating("PlayAmbientSound", Random.Range(10, 20), Random.Range(10, 20));
    }


    private void PlayAmbientSound()
    {
        //replace string with wwise event that you want to trigger at a random interval
        AkSoundEngine.PostEvent("play_bb_sx_game_amb_meowing", gameObject);
    }
}
