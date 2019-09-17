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
    [Tooltip("[WORKAROUND] Child object to rotate with steer - PlayerGO/MeshGO/<All necessary meshes>")]
    public GameObject childObjectToRotateTowardSteer;

    private float speed;
    private float lastSpeed;

    private InputHandler inputHandler;

    private void Start()
    {
        // Get the input handler
        inputHandler = GameObject.FindObjectOfType<InputHandler>();
    }

    private void OnGUI()
    {
        // Print directions on the screen - temporary
        GUIStyle textStyle = new GUIStyle((GUIStyle)"label");
        textStyle.fontSize = 22;
        GUI.color = Color.black;
        GUI.Box (new Rect (10.0f, 10.0f, 400.0f, 40.0f), "A,W,S,D/Left Stick - Main Control", textStyle);
        GUI.Box (new Rect (10.0f, 50.0f, 400.0f, 40.0f), "Left Shift/Right Trigger - Speed up", textStyle);
        GUI.Box (new Rect (10.0f, 90.0f, 400.0f, 40.0f), "Left Control/Left Trigger - Slow Down", textStyle);
    }

    private void FixedUpdate()
    {
        speed = GetWantedSpeed(inputHandler.SpeedControl);

        // Forward velocity
        Vector3 moveVector = transform.forward * speed;
        Vector3 yaw = inputHandler.Steer * transform.right * rotattionSpeedX * Time.deltaTime;
        Vector3 pitch = -(inputHandler.Pitch) * transform.up * rotattionSpeedY * Time.deltaTime; // Need to negate pitch input to meet design doc specifications
        Vector3 dir = yaw + pitch;

        // Limit rotation
        float maxX = Quaternion.LookRotation(moveVector + dir).eulerAngles.x;
        if(maxX < 90 && maxX > 70 || maxX > 270 && maxX < 290)
        {
            // TODO: Maybe to some sort of falloff if angle is exceeded to add difficulty?
        }
        else
        {
            moveVector += dir;
            transform.rotation = Quaternion.LookRotation(moveVector);
        }

        this.transform.position += moveVector * Time.deltaTime;

        RotateChildTowardSteer();
    }

    private void Update()
    {
        // Change colors of the broom to see what is happening - Temporary will be taken out in later build
        if (inputHandler.SpeedControl > inputHandler.controllerDeadZone)
        {
            this.GetComponentInChildren<Renderer>().material.color = Color.green;
        }
        // Slow down
        else if (inputHandler.SpeedControl < -inputHandler.controllerDeadZone)
        {
            this.GetComponentInChildren<Renderer>().material.color = Color.red;
        }
        // Go back to base speed
        else
        {
            this.GetComponentInChildren<Renderer>().material.color = new Color32(77, 41, 0, 255);
        }
    }

    private float GetWantedSpeed(float _speedControlInput)
    {
        float _wantedSpeed = 0f;

        // Speed up
        if(_speedControlInput > inputHandler.controllerDeadZone)
        {
            _wantedSpeed = maximumSpeed;
        }
        // Slow down
        else if(_speedControlInput < -inputHandler.controllerDeadZone)
        {
            _wantedSpeed = minimumSpeed;
        }
        // Go back to base speed
        else
        {
            _wantedSpeed = baseSpeed;
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
    }
}
