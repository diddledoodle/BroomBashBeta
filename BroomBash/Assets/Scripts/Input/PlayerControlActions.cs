using InControl;

public class PlayerControlActions : PlayerActionSet
{

    // Player Actions
    public PlayerAction pitchDown;
    public PlayerAction pitchUp;
    public PlayerAction steerRight;
    public PlayerAction steerLeft;
    public PlayerAction speedUp;
    public PlayerAction slowDown;
    public PlayerAction stop;
    public PlayerAction accept;
    public PlayerAction decline;
    public PlayerAction cameraXAxisPositive;
    public PlayerAction cameraXAxisNegative;
    public PlayerAction cameraYAxisPositive;
    public PlayerAction cameraYAxisNegative;

    // Menu navigation Actions
    public PlayerAction menuUp;
    public PlayerAction menuDown;
    public PlayerAction pause;

    // Player action axes
    public PlayerOneAxisAction steer;
    public PlayerOneAxisAction pitch;
    public PlayerOneAxisAction speedControl;
    public PlayerOneAxisAction cameraAxisX;
    public PlayerOneAxisAction cameraAxisY;

	public PlayerControlActions ()
	{
		// Create the player actions
		pitchDown = CreatePlayerAction("PitchDown");
        pitchUp = CreatePlayerAction("PitchUp");
        steerRight = CreatePlayerAction("SteerRight");
        steerLeft = CreatePlayerAction("SteerLeft");
        speedUp = CreatePlayerAction("SpeedUp");
        slowDown = CreatePlayerAction("SlowDown");
        stop = CreatePlayerAction("Stop");
        accept = CreatePlayerAction("Accept");
        decline = CreatePlayerAction("Decline");
        cameraXAxisPositive = CreatePlayerAction("CameraxAxisPositive");
        cameraXAxisNegative = CreatePlayerAction("CameraXAxisNegative");
        cameraYAxisPositive = CreatePlayerAction("CameraYAxisPositive");
        cameraYAxisNegative = CreatePlayerAction("CameraYAxisNegative");

        // Create menu navigation actions
        menuUp = CreatePlayerAction("MenuUp");
        menuDown = CreatePlayerAction("MenuDown");
        pause = CreatePlayerAction("Pause");

        // Create the player action axes
        steer = CreateOneAxisPlayerAction(steerLeft, steerRight);
        pitch = CreateOneAxisPlayerAction(pitchUp, pitchDown);
        speedControl = CreateOneAxisPlayerAction(slowDown, speedUp);
        cameraAxisX = CreateOneAxisPlayerAction(cameraXAxisNegative, cameraXAxisPositive);
        cameraAxisY = CreateOneAxisPlayerAction(cameraYAxisNegative, cameraYAxisPositive);
    }
}
