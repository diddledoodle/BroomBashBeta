using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public bool isPaused = false;

    public GameObject inGameInformation;
    public GameObject pauseMenu;
    public MenuNavigation menuNavigation;

    private InputHandler inputHandler;

    // Start is called before the first frame update
    void Start () {
        // Set the in game information active 
        if (inGameInformation != null) {
            inGameInformation.SetActive (true);
        }
        // Set the pause menu not active in case it was active in design time
        if (pauseMenu != null) {
            pauseMenu.SetActive (false);
        }
        // Get the input handler
        inputHandler = GameObject.FindObjectOfType<InputHandler> ();
    }

    // Update is called once per frame
    void Update () {
        // Return if pause menu is null
        if (pauseMenu == null) {
            return;
        }

        // Get the players input
        if (inputHandler.Pause) {
            if (isPaused) {
                ResumeGame ();
            } else if (!isPaused) {
                PauseGame ();
            }
        }
    }

    public void ResumeGameFromUI () {
        ResumeGame ();
    }

    public void ExitToMainMenu () {
        /*jpost audio*/
        //play the exit game sound from wwise
        AkSoundEngine.PostEvent ("play_bb_sx_game_ui_exitgame", gameObject);
        //stop the level music from playing
        AkSoundEngine.PostEvent("stop_bb_mx_level", gameObject);
        // Make sure time is set to 1 
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync ("MainMenu");
    }

    private void ResumeGame () {
        inGameInformation.SetActive (true);
        pauseMenu.SetActive (false);
        Time.timeScale = 1;
        isPaused = false;
    }

    private void PauseGame () {
        inGameInformation.SetActive (false);
        pauseMenu.SetActive (true);
        Time.timeScale = 0;
        isPaused = true;
        menuNavigation.SelectFirstIndexOnEnable ();
    }
}