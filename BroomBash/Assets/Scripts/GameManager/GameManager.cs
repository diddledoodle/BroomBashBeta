using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class GameManager : MonoBehaviour
{

    public TutorialController tutorialController;
    public QuestController questController;
    private DialogueSystemController dsc;

    [SerializeField]
    private bool tutorialIsActive = true;
    private bool tutorialEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        dsc = GameObject.FindObjectOfType<DialogueSystemController>();
        // Make sure the tutorial var in dialogue lua is true
        DialogueLua.SetVariable("endTutorial", false);
        Invoke("StartTutorial", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialIsActive)
        {
            // Disable the tutorial and allow the player full reign of the game world
            if (DialogueLua.GetVariable("endTutorial").asBool == true)
            {
                // Disable the tutorial
                tutorialController.enabled = false;
                // Enable the quest controller objects
                questController.NoQuestActiveGameObjects();
                tutorialIsActive = false;
                dsc.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Optional;
                GameObject.FindObjectOfType<PlayerController>().GetComponentInChildren<TargetIndicator>().tutorial = false;
            }
        }

        // Check for tutorial end
        if(tutorialController.enabled == false && tutorialEnded == false)
        {
            // Enable the quest controller objects
            questController.NoQuestActiveGameObjects();
            tutorialIsActive = false;
            tutorialEnded = true;
            dsc.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Optional;
            GameObject.FindObjectOfType<PlayerController>().GetComponentInChildren<TargetIndicator>().tutorial = false;
            questController.AddTutorialXpToPlayerLevelingSystem(5);
        }
    }

    private void StartTutorial()
    {
        questController.TurnOffAllLocations();
    }
}
