using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string gameSceneName;
    [SerializeField]
    private GameObject creditsMenuPanel;
    [SerializeField]
    private GameObject titleMenuPanel;
    public void LoadGameScene()
    {
        SceneManager.LoadSceneAsync(gameSceneName);
    }
    public void ShowCredits()
    {
        creditsMenuPanel.SetActive(true);
    }
    public void ShowTitle()
    {
        titleMenuPanel.SetActive(true);
    }
    public void ExitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    /*jpost audio*/
    //play start game sound
    public void PlayStartSound()
    {
        //plays the wwise event at the location of the second argument (gameObject)
        AkSoundEngine.PostEvent("play_bb_sx_menu_ui_main_start", gameObject);
        //fades out the main menu music if it is still playing
        AkSoundEngine.PostEvent("stop_bb_mx_menu", gameObject);
    }
}
