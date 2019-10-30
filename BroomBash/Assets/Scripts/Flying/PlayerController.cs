using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("The base speed that the player flies at without any input")]
    public float baseSpeed = 15f;
    [Tooltip("The minimum speed that the player can fly at when the 'brakes' are pressed")]
    public float minimumSpeed = 1f;
    [Tooltip("The maximum speed that the player can fly at when the 'throttle' is pressed")]
    public float maximumSpeed = 25f;
    [Tooltip("The speed at which the player steers their flying direction on the X plane")]
    public float rotattionSpeedX = 1f;
    [Tooltip("The speed that the player can pitch their flying direction on the y plane")]
    public float rotattionSpeedY = 1f;
    [Tooltip("The Z rotation of the whole player object in local space when that player steers left and right")]
    public float yawMaxSteerRotationAmount = 20f; // Degrees
    [Tooltip("The linear interpolation speed (smoothing) of the Z rotation steer mutiplied by Time.deltaTime and steer input")]
    public float yawMaxSteerRotationSpeed = 5f;
    [Tooltip("The speed at which the player object will level out when stopped")]
    public float stoppedLevelingRotattionSpeed = 4f;
    [Tooltip("The amount of noise in the broom hover when the player is stopped. (Best to keep this low)")]
    public float stoppedHoverNoiseAmount = 0.01f;
    [Tooltip("[WORKAROUND] Child object to rotate with steer - PlayerGO/MeshGO/<All necessary meshes>")]
    public GameObject childObjectToRotateTowardSteer;

    //[HideInInspector]
    public bool stopPlayer = false;

    

    private float speed;
    private float lastSpeed;
    private Vector3 playerStartingPosition = Vector3.zero;

    [HideInInspector]
    public InputHandler inputHandler;
    [HideInInspector]
    public QuestController questController;

    /*jpost audio*/
    //audio related fields
    private bool hasAccelerated = false;
    private bool hasSlowedDown = false;
    private bool hasStopped = false;
    private bool isFlyingNormal = false;

    private void Start()
    {
        // Get the input handler
        inputHandler = this.gameObject.AddComponent<InputHandler>() as InputHandler;
        // Get the quest controller
        questController = GameObject.FindObjectOfType<QuestController>();
        // Get the players starting poosition
        playerStartingPosition = this.gameObject.transform.position;
    }

    private void OnGUI()
    {
        // Print directions on the screen - temporary
        if(!questController.playerUIManager.dialogSystemIsActive && !questController.playerUIManager.notificationSystemIsActive)
        {
            GUIStyle textStyle = new GUIStyle((GUIStyle)"label");
            textStyle.fontSize = 22;
            GUI.color = Color.green;
            GUI.Box(new Rect(10.0f, Screen.height - 240, 400.0f, 40.0f), "Left Stick - Flight Control", textStyle);
            GUI.Box(new Rect(10.0f, Screen.height - 200, 400.0f, 40.0f), "Right Trigger - Speed up", textStyle);
            GUI.Box(new Rect(10.0f, Screen.height - 160, 400.0f, 40.0f), "Left Trigger - Slow Down", textStyle);
            GUI.Box(new Rect(10.0f, Screen.height - 120, 400.0f, 40.0f), "Left Bumper (L1) - Stop", textStyle);
            GUI.Box(new Rect(10.0f, Screen.height - 80, 400.0f, 40.0f), "R - reset position to scene origin", textStyle);
            GUI.Box(new Rect(10.0f, Screen.height - 40, 400.0f, 40.0f), "Esc - Exit to Main Menu", textStyle);
        }
    }

    private void FixedUpdate()
    {
		FlightMechanics();

		// Reset the player to the origin - will be removed in the future
		if (Input.GetKeyDown(KeyCode.R))
		{
			this.gameObject.transform.position = playerStartingPosition;
		}
	}

    private void FlightMechanics()
	{
        if (!stopPlayer)
        {
            speed = GetWantedSpeed(inputHandler.SpeedControl);
        }
        else
        {
            speed = 0;
        }

		// Forward velocity
		Vector3 moveVector = transform.forward * speed;
		Vector3 yaw = inputHandler.Steer * transform.right * rotattionSpeedX * Time.deltaTime;
		Vector3 pitch = -(inputHandler.Pitch) * transform.up * rotattionSpeedY * Time.deltaTime; // Need to negate pitch input to meet design doc specifications
		Vector3 dir = yaw + pitch;

		// Limit rotation

		float maxX = (moveVector + dir != Vector3.zero) ? Quaternion.LookRotation(moveVector + dir).eulerAngles.x : 0; // Need to get rid of that annoying debug from Quaternion.LookRotation taking in a 0 vector



		if (speed != 0)
		{
			this.transform.position += moveVector * Time.deltaTime;
			if (maxX < 90 && maxX > 70 || maxX > 270 && maxX < 290)
			{
				// TODO: Maybe to some sort of falloff if angle is exceeded to add difficulty?
			}
			else
			{
				moveVector += dir;
				transform.rotation = Quaternion.LookRotation(moveVector);
                
            }
		}
		else if (speed == 0)
		{
			// Hover noise
			this.transform.position = new Vector3(this.transform.position.x, Mathf.Lerp(this.transform.position.y - stoppedHoverNoiseAmount, this.transform.position.y + stoppedHoverNoiseAmount, Mathf.PingPong(Time.time, 1)), this.transform.position.z);
		}

		RotateChildTowardSteer();
	}

    public void StopPlayer()
    {
        stopPlayer = true;
        Debug.Log("The player was stopped", this.gameObject);
    }

    public void UnstopPlayer() // Lol is that even a word?
    {
        stopPlayer = false;
        Debug.Log("The player was released", this.gameObject);
    }

    private float GetWantedSpeed(float _speedControlInput)
    {
        float _wantedSpeed = 0f;

        // Speed up
        if(_speedControlInput > inputHandler.controllerDeadZone && !inputHandler.Stop)
        {
            _wantedSpeed = maximumSpeed;

            /*jpost audio*/
            if (!hasAccelerated)
            {
                //play the broom accelerate sound from wwise
                AkSoundEngine.PostEvent("play_bb_sx_game_plr_broom_accelerate", gameObject);
                //stop the flying normal sound
                AkSoundEngine.PostEvent("stop_bb_sx_game_plr_broom_flying", gameObject);
                hasAccelerated = true;
                hasSlowedDown = false;
                isFlyingNormal = false;
            }
            
        }
        // Slow down
        else if(_speedControlInput < -inputHandler.controllerDeadZone && !inputHandler.Stop)
        {
            _wantedSpeed = minimumSpeed;
            /*jpost audio*/
            if (!hasSlowedDown)
            {
                //play the broom deccelerate sound from wwise
                AkSoundEngine.PostEvent("play_bb_sx_game_plr_broom_decelerate", gameObject);
                hasSlowedDown = true;
            }
        }
        // Go back to base speed
        else if (_speedControlInput < inputHandler.controllerDeadZone && _speedControlInput > -inputHandler.controllerDeadZone && !inputHandler.Stop)
        {
            _wantedSpeed = baseSpeed;
            /*jpost audio*/
            //reset hasAccelerated
            hasAccelerated = false;
            //reset hasSlowedDown
            hasSlowedDown = true;
            //reset hasStopped
            hasStopped = false;
            /*jpost audio*/
            if (!isFlyingNormal)
            {
                //play normal flying sound
                AkSoundEngine.PostEvent("play_bb_sx_game_plr_broom_flying", gameObject);
                isFlyingNormal = true;
            }
        }

        else if(inputHandler.Stop)
        {
            _wantedSpeed = 0;
            /*jpost audio*/
            //if the player hasn't stopped
            if (!hasStopped)
            {
                //play the broom stop sound from wwise
                AkSoundEngine.PostEvent("play_bb_sx_game_plr_broom_stop", gameObject);
                //set hasStopped to true
                hasStopped = true;
                //stop the flying normal sound
                AkSoundEngine.PostEvent("stop_bb_sx_game_plr_broom_flying", gameObject);
                isFlyingNormal = false;
            }


        }

        return _wantedSpeed;
    }

    private void RotateChildTowardSteer()
    {
        float _currentSteer = inputHandler.Steer;
        Vector3 _wantedAngle = Vector3.zero;

        // Tilt to the right if steering right
        if(_currentSteer > inputHandler.controllerDeadZone)
        {
            _wantedAngle = Quaternion.Euler(new Vector3(childObjectToRotateTowardSteer.transform.eulerAngles.x, childObjectToRotateTowardSteer.transform.eulerAngles.y, -yawMaxSteerRotationAmount)).eulerAngles;
            childObjectToRotateTowardSteer.transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(childObjectToRotateTowardSteer.transform.eulerAngles), Quaternion.Euler(_wantedAngle), Time.deltaTime * Mathf.Abs(_currentSteer) * yawMaxSteerRotationSpeed).eulerAngles;
        }
        // Tilt to the left if steering left
        else if (_currentSteer < -inputHandler.controllerDeadZone)
        {
            _wantedAngle = Quaternion.Euler(new Vector3(childObjectToRotateTowardSteer.transform.eulerAngles.x, childObjectToRotateTowardSteer.transform.eulerAngles.y, yawMaxSteerRotationAmount)).eulerAngles;
            childObjectToRotateTowardSteer.transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(childObjectToRotateTowardSteer.transform.eulerAngles), Quaternion.Euler(_wantedAngle), Time.deltaTime * Mathf.Abs(_currentSteer) * yawMaxSteerRotationSpeed).eulerAngles;
        }
        // Tilt back to 0 if not steering
        else
        {
            _wantedAngle = Quaternion.Euler(new Vector3(childObjectToRotateTowardSteer.transform.eulerAngles.x, childObjectToRotateTowardSteer.transform.eulerAngles.y, 0)).eulerAngles;
            childObjectToRotateTowardSteer.transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(childObjectToRotateTowardSteer.transform.eulerAngles), Quaternion.Euler(_wantedAngle), Time.deltaTime * yawMaxSteerRotationSpeed).eulerAngles;
        }

        // Rotate back level if player is stopped
        if(speed == 0)
        {
            this.transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(this.transform.eulerAngles), Quaternion.Euler(0, this.transform.eulerAngles.y, this.transform.eulerAngles.z), Time.deltaTime * stoppedLevelingRotattionSpeed).eulerAngles;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (questController.countdownTimerIsActive)
        {
            questController.PlayerCollidedWithObjectDuringQuest();
        }

        /*jpost audio*/
        //collision sounds for buildings
        if (collision.gameObject.GetComponent<MeshCollider>())
        {
            if(collision.gameObject.GetComponent<MeshCollider>().name == "polySurface99")
            {
                //play wwise sound for building collision at location of collision
                AkSoundEngine.PostEvent("play_bb_sx_game_plr_impact_building", collision.gameObject);
            }                
        }
        //collision sounds for water
        if (collision.gameObject.name == "Water")
        {
            //play wwise sound for water collision at location of collision
            AkSoundEngine.PostEvent("play_bb_sx_game_plr_impact_water", collision.gameObject);
        }
        //collision sounds for ground
        if (collision.gameObject.tag == "Ground")
        {
            //play wwise sound for water collision at location of collision
            AkSoundEngine.PostEvent("play_bb_sx_game_plr_impact_ground", gameObject);            
        }
            
    }
}
