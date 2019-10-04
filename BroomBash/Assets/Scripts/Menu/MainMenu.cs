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
}
