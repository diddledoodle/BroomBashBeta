using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{

    public TextMeshProUGUI timer;
    public Text levelText;
    public Text xpText;
    //public Text levelText;
    //public Text livesText;
    public List<Image> starImages = new List<Image>();

    public GameObject miniMap;
    public GameObject notifcationPanel;
    public GameObject dialogPanel;
    public GameObject startGameInstructions;
    public Texture miniMapRenderTexture;
    private QuestController questController;
    private InputHandler inputHandler;

    private LevelSystem playerLevelSystem;
    private int activeLiveStars = 3;

    // Start is called before the first frame update
    void Start()
    {


        // Assign the render texture to the mini-map
        miniMap.GetComponent<RawImage>().texture = miniMapRenderTexture;
        // Get the questController
        questController = GameObject.FindObjectOfType<QuestController>();
        // Turn of text elements if the quest controller is null
        if(questController == null)
        {
            timer.text = string.Empty;
            foreach(Image s in starImages)
            {
                s.enabled = false;
            }
            levelText.text = string.Empty;
        }
        // Get the player leveling system
        playerLevelSystem = (questController != null) ? questController.player.GetComponent<LevelSystem>() : null;
    }

    // Update is called once per frame
    void Update()
    {
        if (questController != null && questController.countdownTimerIsActive)
        {
            timer.text = $"<b>{questController.timeLeft.ToString("F0")} seconds</b>";
        }
        else
        {
            timer.text = string.Empty;
        }
        // Update the xp and level text
        if(playerLevelSystem != null)
        {
            xpText.text = $"<b>XP {playerLevelSystem.xp}</b>";
            levelText.text = $"<b>Level {playerLevelSystem.currentLevel + 1}</b>";
        }
        else
        {
            xpText.text = string.Empty;
            levelText.text = string.Empty;
        }
        
        //livesText.text = $"<b>Lives: {questController.currentPlayerFailedQuests}</b>";
    }

    public void SubtractLiveStar()
    {
        activeLiveStars--;
        for(int i = 0; i < starImages.Count; i++)
        {
            if(i <= activeLiveStars - 1)
            {
                starImages[i].enabled = true;
            }
            else
            {
                starImages[i].enabled = false;
            }
        }
    }
}
