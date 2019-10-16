using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    public Text timer;
    public Text xpText;
    public Text levelText;
    public Text livesText;
    public GameObject miniMap;
    public GameObject notifcationPanel;
    public Text notifictionText;
    public Text notificationDeclineText;
    public Text notificationAcceptText;
    public GameObject dialogPanel;
    public GameObject startGameInstructions;
    public Text dialogText;
    public Texture miniMapRenderTexture;
    private QuestController questController;
    private InputHandler inputHandler;

    public enum QuestStatus { GAMESTART, STANDBY, START, END, FAIL, GAMEOVER}
    [HideInInspector]
    public QuestStatus questStatus = QuestStatus.STANDBY;

    //[HideInInspector]
    public bool dialogSystemIsActive = false;
    //[HideInInspector]
    public bool notificationSystemIsActive = false;

    private LevelSystem playerLevelSystem;

    // Start is called before the first frame update
    void Start()
    {
        // Assign the render texture to the mini-map
        miniMap.GetComponent<RawImage>().texture = miniMapRenderTexture;
        // Turn off all components that arent used at the start of the game
        notifcationPanel.SetActive(false);
        dialogPanel.SetActive(false);

        // Get the questController
        questController = GameObject.FindObjectOfType<QuestController>();
        // Get the player leveling system
        playerLevelSystem = questController.player.GetComponent<LevelSystem>();
        // Get the input handler
        inputHandler = questController.player.GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (questController.countdownTimerIsActive)
        {
            timer.text = $"<b>{questController.timeLeft.ToString("F0")} seconds</b>";
        }
        else
        {
            timer.text = string.Empty;
        }
        // Update the xp and level text
        xpText.text = $"<b>XP: {playerLevelSystem.xp}</b>";
        levelText.text = $"<b>Level: {playerLevelSystem.currentLevel}</b>";
        livesText.text = $"<b>Lives: {questController.currentPlayerFailedQuests}</b>";
        // Dialog System
        CloseDialogSystem();
        // Notification system
        CloseNotificationSystem();

        // Go to main menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync("MainMenu");
            Debug.Log("Okla");
        }
    }

    public void RunDialogSystem(string _dialogMessage, QuestStatus _qs)
    {
        dialogSystemIsActive = true;
        dialogPanel.SetActive(true);
        dialogText.text = _dialogMessage;
        questStatus = _qs;
        if(questStatus == QuestStatus.GAMESTART)
        {
            startGameInstructions.SetActive(true);
        }
        else
        {
            startGameInstructions.SetActive(false);
        }
    }

    private void CloseDialogSystem()
    {
        if (dialogSystemIsActive)
        {
            if(inputHandler.Accept)
            {
                dialogText.text = string.Empty;
                dialogPanel.SetActive(false);
                // TODO: Bring up the notifiction system
                Invoke("RunNotificationSystem", 0.1f); // Need to invoke because the accept was being passed to the notification syustem
            }
        }
    }

    private void RunNotificationSystem()
    {
        dialogSystemIsActive = false;
        notificationSystemIsActive = true;
        notifcationPanel.SetActive(true);
        switch (questStatus)
        {
            case QuestStatus.GAMESTART:
                notificationAcceptText.enabled = true;
                notificationDeclineText.enabled = false;
                notificationAcceptText.text = "<color=green>Start -> A/Enter</color>";
                notifictionText.text = "Are you ready to start?";
                break;
            case QuestStatus.START:
                notificationAcceptText.enabled = true;
                notificationDeclineText.enabled = true;
                notificationAcceptText.text = "<color=green>Accept -> A/Enter</color>";
                notifictionText.text = "Do you want to accept this quest?";
                break;
            case QuestStatus.END:
                notificationAcceptText.enabled = true;
                notificationDeclineText.enabled = false;
                notificationAcceptText.text = "<color=green>Collect -> A/Enter</color>";
                notifictionText.text = "Collect Reward.";
                break;
            case QuestStatus.FAIL:
                notificationAcceptText.enabled = true;
                notificationDeclineText.enabled = false;
                notificationAcceptText.text = "<color=green>OK -> A/Enter</color>";
                notifictionText.text = "You lost some XP";
                break;
            case QuestStatus.GAMEOVER:
                notificationAcceptText.enabled = true;
                notificationDeclineText.enabled = false;
                notificationAcceptText.text = "<color=green>OK -> A/Enter</color>";
                notifictionText.text = $"<b>Game Over</b>\nYou finished the game with <i>{playerLevelSystem.xp}</i> XP and you were level <i>{playerLevelSystem.currentLevel}</i>!";
                break;
        }
        
    }

    private void CloseNotificationSystem()
    {
        if (notificationSystemIsActive)
        {
            switch (questStatus)
            {
                case QuestStatus.GAMESTART:
                    if (inputHandler.Accept)
                    {
                        notifictionText.text = string.Empty;
                        notifcationPanel.SetActive(false);
                        notificationSystemIsActive = false;
                    }
                    break;
                case QuestStatus.START:
                    if (inputHandler.Accept)
                    {
                        notifictionText.text = string.Empty;
                        notifcationPanel.SetActive(false);
                        notificationSystemIsActive = false;
                        // Start the quest
                        questController.StartQuest();
                    }
                    else if (inputHandler.Decline)
                    {
                        notifictionText.text = string.Empty;
                        notifcationPanel.SetActive(false);
                        notificationSystemIsActive = false;
                    }
                    break;
                case QuestStatus.END:
                    if (inputHandler.Accept)
                    {
                        notifictionText.text = string.Empty;
                        notifcationPanel.SetActive(false);
                        notificationSystemIsActive = false;
                        // End the quest
                        questController.EndQuest();
                    }
                    break;
                case QuestStatus.FAIL:
                    if (inputHandler.Accept)
                    {
                        notifictionText.text = string.Empty;
                        notifcationPanel.SetActive(false);
                        notificationSystemIsActive = false;
                    }
                    break;
                // TODO: Do this the right way somewhere else. This is hella gross
                case QuestStatus.GAMEOVER:
                    if (inputHandler.Accept)
                    {
                        SceneManager.LoadSceneAsync("MainMenu");
                    }
                    break;
            }
           
        }
    }
}
