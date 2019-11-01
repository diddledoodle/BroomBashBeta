using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{

    public PlayerController playerController;
    public float firstDialogueTriggerTime = 2f;
    [Header("Flight tutorial convos")]
    public DialogueSystemTrigger introduction0;
    public DialogueSystemTrigger introduction1;
    public DialogueSystemTrigger stop;
    public DialogueSystemTrigger slowDown;
    public DialogueSystemTrigger speedUp;
    public DialogueSystemTrigger steer;
    public DialogueSystemTrigger pitch;
    public DialogueSystemTrigger pause;
    [Header("Gameplay tutorial convos")]
    public DialogueSystemTrigger start;
    public DialogueSystemTrigger end;
    [Header("Tutorial locations")]
    public GameObject pickUpLocation;
    public GameObject dropOffLocation;

    private InputHandler inputHandler;

    private bool stopConvoOver = false;
    private bool slowDownConvoOver = false;
    private bool speedUpConvoOver = false;
    private bool steerConvoOver = false;
    private bool pitchConvoOver = false;
    private bool pauseConvoOver = false;

    private bool stopCompleted = false;
    private bool slowDownCompleted = false;
    private bool speedUpCompleted = false;
    private bool steerCompleted = false;
    private bool steerRightCompleted = false;
    private bool steerLeftCompleted = false;
    private bool pitchCompleted = false;
    private bool pitchUpCompleted = false;
    private bool pitchDownCompleted = false;
    private bool pauseCompleted = false;


    // Start is called before the first frame update
    void Start()
    {
        // Get the inputHandler
        Invoke("GetInputHandler", 0.3f);
        // Stop the player 
        playerController.StopPlayer();
        // Set the tutorial location active false
        pickUpLocation.SetActive(false);
        dropOffLocation.SetActive(false);
        // Trigger the first dialogue
        Invoke("TriggerFirstDialogue", firstDialogueTriggerTime);
        // Add event listeners
        stop.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { UnstopPlayer(); stopConvoOver = true; });
        slowDown.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { UnstopPlayer(); slowDownConvoOver = true; });
        speedUp.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { UnstopPlayer(); speedUpConvoOver = true; });
        steer.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { UnstopPlayer(); steerConvoOver = true; });
        pitch.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { UnstopPlayer(); pitchConvoOver = true; });
        pause.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate {
            playerController.StopPlayer();
            playerController.transform.position = playerController.playerStartingPosition;
            playerController.transform.eulerAngles = Vector3.zero;
            pauseConvoOver = true;
            StartGameplayTutorial();
            });
        start.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { UnstopPlayer(); pickUpLocation.SetActive(true); playerController.GetComponentInChildren<TargetIndicator>().target = pickUpLocation.transform; });
        end.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { StartCoroutine(BackToMenu()); });
    }

    private void GetInputHandler()
    {
        // Get the input handler
        inputHandler = playerController.gameObject.GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        // Return if input handler is not found
        if(inputHandler == null)
        {
            return;
        }

        // Check for stop completed
        CheckForStop();
        // Check for slow down completed
        CheckForSlowDown();
        // Check for speed up completed
        CheckForSpeedUp();
        // Check for steer completed
        CheckForSteering();
        // Check for pitch completed
        CheckForPitch();
    }

    private void TriggerFirstDialogue()
    {
        introduction0.OnUse();
        introduction0.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { TriggerDialogue(introduction1); });
        introduction1.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { playerController.UnstopPlayer(); StartCoroutine(TriggerDialogueDelay(stop, 5)); });
    }

    private void TriggerDialogue(DialogueSystemTrigger _trigger)
    {
        playerController.StopPlayer();
        _trigger.OnUse();
    }

    IEnumerator TriggerDialogueDelay(DialogueSystemTrigger _trigger, float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        Debug.Log("delay");
        playerController.StopPlayer();
        Debug.Log("Stopped player");
        _trigger.OnUse();
        Debug.Log($"Triggered convo: {_trigger.gameObject.name}");
    }

    private void CheckForStop()
    {
        if (stopCompleted == false && stopConvoOver == true)
        {
            if(playerController.speed < 2 && inputHandler.SpeedControl < -0.95)
            {
                stopCompleted = true;
                // Trigger slow down convo
                TriggerDialogue(slowDown);
            }
        }
    }

    private void CheckForSlowDown()
    {
        if(slowDownCompleted == false && slowDownConvoOver == true)
        {
            if(inputHandler.SpeedControl < -0.7)
            {
                slowDownCompleted = true;
                // Trigger speed up convo
                StartCoroutine(TriggerDialogueDelay(speedUp, 2f));
            }
        }
    }

    private void CheckForSpeedUp()
    {
        if(speedUpCompleted == false && speedUpConvoOver == true)
        {
            if(inputHandler.SpeedControl > 0.7)
            {
                speedUpCompleted = true;
                // Trigger steering convo
                StartCoroutine(TriggerDialogueDelay(steer, 2f));
            }
        }
    }

    private void CheckForSteering()
    {
        if(steerCompleted == false && steerConvoOver == true)
        {
            if(inputHandler.Steer > 0.7f)
            {
                steerRightCompleted = true;
            }
            if(inputHandler.Steer < -0.7f)
            {
                steerLeftCompleted = true;
            }
            if(steerRightCompleted && steerLeftCompleted)
            {
                steerCompleted = true;
                // Trigger Pitch Convo
                StartCoroutine(TriggerDialogueDelay(pitch, 2f));
            }
        }
    }

    private void CheckForPitch()
    {
        if (pitchCompleted == false && pitchConvoOver == true)
        {
            if (inputHandler.Pitch > 0.7f)
            {
                pitchUpCompleted = true;
            }
            if (inputHandler.Pitch < -0.7f)
            {
                pitchDownCompleted = true;
            }
            if (pitchUpCompleted && pitchDownCompleted)
            {
                pitchCompleted = true;
                // Trigger Pause Convo
                StartCoroutine(TriggerDialogueDelay(pause, 2f));
            }
        }
    }

    private void StartGameplayTutorial()
    {
        // Trigger the gameplay start tutorial
        StartCoroutine(TriggerDialogueDelay(start, 2f));
    }

    public void SetTargetArrowTarget()
    {
        playerController.GetComponentInChildren<TargetIndicator>().target = null;
    }

    public void SetTargetToDropOff()
    {
        pickUpLocation.SetActive(false);
        dropOffLocation.SetActive(true);
        playerController.UnstopPlayer();
        playerController.GetComponentInChildren<TargetIndicator>().target = dropOffLocation.transform;
    }

    public void TriggerEndConvo()
    {
        dropOffLocation.SetActive(false);
        playerController.GetComponentInChildren<TargetIndicator>().target = null;
        TriggerDialogue(end);
    }

    IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(0);
    }


    private void UnstopPlayer()
    {
        playerController.UnstopPlayer();
    }
}
