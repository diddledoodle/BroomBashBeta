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
    }
}
