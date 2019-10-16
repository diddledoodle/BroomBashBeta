using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputHandler : MonoBehaviour {

    // Input Handler settings
    public float controllerDeadZone = 0.2f;

    // Player inputs
    public float Pitch = 0;
    public float Steer = 0;
    public float SpeedControl = 0;
    public bool Stop = false;
    public bool Accept = false;
    public bool Decline = false;
   
    private PlayerControlActions playerControlActions;
    private InputDevice currentInputDevice;

    void Start () {

        // Define playerControlActions
        playerControlActions = new PlayerControlActions();

        // Player default bindings - Controller
        playerControlActions.pitchDown.AddDefaultBinding(InputControlType.LeftStickUp);
        playerControlActions.pitchUp.AddDefaultBinding(InputControlType.LeftStickDown);
        playerControlActions.steerLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
        playerControlActions.steerRight.AddDefaultBinding(InputControlType.LeftStickRight);
        playerControlActions.speedUp.AddDefaultBinding(InputControlType.RightTrigger);
        playerControlActions.slowDown.AddDefaultBinding(InputControlType.LeftTrigger);
        playerControlActions.stop.AddDefaultBinding(InputControlType.LeftBumper);
        playerControlActions.accept.AddDefaultBinding(InputControlType.Action1);
        playerControlActions.decline.AddDefaultBinding(InputControlType.Action2);
        // Player default bindings - Keyboard
        playerControlActions.pitchDown.AddDefaultBinding(Key.W);
        playerControlActions.pitchUp.AddDefaultBinding(Key.S);
        playerControlActions.steerLeft.AddDefaultBinding(Key.A);
        playerControlActions.steerRight.AddDefaultBinding(Key.D);
        playerControlActions.speedUp.AddDefaultBinding(Key.Shift);
        playerControlActions.slowDown.AddDefaultBinding(Key.LeftControl);
        playerControlActions.stop.AddDefaultBinding(Key.Space);
        playerControlActions.accept.AddDefaultBinding(Key.Return);
        playerControlActions.decline.AddDefaultBinding(Key.Backspace);

    }
	
	// Update is called once per frame
	void Update () {

        // Update current device to the input device receiving input
        currentInputDevice = InputManager.ActiveDevice;

        // Update inputs
        Pitch = playerControlActions.pitch.Value;
        Steer = playerControlActions.steer.Value;
        SpeedControl = playerControlActions.speedControl.Value;
        Accept = playerControlActions.accept.WasPressed;//(playerControlActions.accept.Value > controllerDeadZone) ? true : false;
        Decline = playerControlActions.decline.WasPressed;
        // Toggle stopped based on the stop button
        /*if (playerControlActions.stop.WasPressed)
        {
            Stop = (Stop == true) ? false : true;
        }
        // Accelerate from stop if speed control is changed from "0"
        if(SpeedControl > controllerDeadZone || SpeedControl < -controllerDeadZone)
        {
            Stop = false;
        }*/
        // Check for player stopped
        if (this.SpeedControl > -0.9f)
        {
            Stop = false;
        }
        else
        if (this.SpeedControl < -this.controllerDeadZone && this.SpeedControl < -0.9f)
        {
            Stop = true;
        }
    }
}
