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

    // Player action axes
    public PlayerOneAxisAction steer;
    public PlayerOneAxisAction pitch;
    public PlayerOneAxisAction speedControl;

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


        // Create the player action axes
        steer = CreateOneAxisPlayerAction(steerLeft, steerRight);
        pitch = CreateOneAxisPlayerAction(pitchUp, pitchDown);
        speedControl = CreateOneAxisPlayerAction(slowDown, speedUp);
    }
}
