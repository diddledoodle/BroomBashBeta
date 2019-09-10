using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputHandler : MonoBehaviour {

    // Input Handler settings
    public float controllerDeadZone;

    // Player inputs
    public float Pitch = 0;
    public float Steer = 0;
    public float SpeedControl = 0;
   
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
        // Player default bindings - Keyboard
        playerControlActions.pitchDown.AddDefaultBinding(Key.W);
        playerControlActions.pitchUp.AddDefaultBinding(Key.S);
        playerControlActions.steerLeft.AddDefaultBinding(Key.A);
        playerControlActions.steerRight.AddDefaultBinding(Key.D);
        playerControlActions.speedUp.AddDefaultBinding(Key.Shift);
        playerControlActions.slowDown.AddDefaultBinding(Key.LeftControl);

    }
	
	// Update is called once per frame
	void Update () {

        // Update current device to the input device receiving input
        currentInputDevice = InputManager.ActiveDevice;

        // Update inputs
        Pitch = playerControlActions.pitch.Value;
        Steer = playerControlActions.steer.Value;
        SpeedControl = playerControlActions.speedControl.Value;
    }
}
