using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName;
    public GameObject creditsMenuPanel;
    public GameObject titleMenuPanel;

    public void LoadGameScene()
    {
        SceneManager.LoadSceneAsync(gameSceneName);
    }
    public void ShowCredits()
    {
        creditsMenuPanel.SetActive(true);
        creditsMenuPanel.GetComponent<MenuNavigation>().SelectFirstIndexOnEnable();
    }
    public void ShowTitle()
    {
        titleMenuPanel.SetActive(true);
        titleMenuPanel.GetComponent<MenuNavigation>().SelectFirstIndexOnEnable();
    }
    public void ExitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
