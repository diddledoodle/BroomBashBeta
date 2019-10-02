using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public Text timer;
    public Text xpText;
    public Text levelText;
    public GameObject miniMap;
    public GameObject notifcationPanel;
    public Text notifictionText;
    public Text notificationDeclineText;
    public Text notificationAcceptText;
    public GameObject dialogPanel;
    public Text dialogText;
    public Texture miniMapRenderTexture;
    public QuestController questController;
    private InputHandler inputHandler;

    public enum QuestStatus { STANDBY, START, END, FAIL}
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
            timer.text = $"{questController.timeLeft.ToString("F0")} seconds";
        }
        else
        {
            timer.text = string.Empty;
        }
        // Update the xp and level text
        xpText.text = $"{playerLevelSystem.xp} XP";
        levelText.text = $"Level {playerLevelSystem.currentLevel}";

        // Dialog System
        CloseDialogSystem();
        // Notification system
        CloseNotificationSystem();
    }

    public void RunDialogSystem(string _dialogMessage, QuestStatus _qs)
    {
        dialogSystemIsActive = true;
        dialogPanel.SetActive(true);
        dialogText.text = _dialogMessage;
        questStatus = _qs;
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
        }
        
    }

    private void CloseNotificationSystem()
    {
        if (notificationSystemIsActive)
        {
            switch (questStatus)
            {
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
            }
           
        }
    }
}
