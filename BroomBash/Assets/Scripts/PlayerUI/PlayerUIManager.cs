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
    public GameObject dialogPanel;
    public Text dialogText;
    public Texture miniMapRenderTexture;
    private QuestController questController;
    private InputHandler inputHandler;

    [HideInInspector]
    public bool dialogSystemIsActive = false;
    [HideInInspector]
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
    }

    public void RunDialogSystem(string _dialogMessage)
    {
        dialogSystemIsActive = true;
        dialogPanel.SetActive(true);
        dialogText.text = _dialogMessage;
    }

    private void CloseDialogSystem()
    {
        if (dialogSystemIsActive)
        {
            if(inputHandler.Accept > inputHandler.controllerDeadZone)
            {
                dialogText.text = string.Empty;
                dialogPanel.SetActive(false);
                // TODO: Bring up the notifiction system
            }
        }
    }
}
