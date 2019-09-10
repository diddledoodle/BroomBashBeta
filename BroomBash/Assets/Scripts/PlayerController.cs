using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float baseSpeed = 15f;
    public float minimumSpeed = 1f;
    public float maximumSpeed = 25f;
    public float accelerationTime = 5f; // Arbitrary
    public float rotattionSpeedX = 1f;
    public float rotattionSpeedY = 1f;
    public float yawMaxRotationAmount = 20f; // Degrees

    private float speed;
    private float lastSpeed;

    private InputHandler inputHandler;

    private void Start()
    {
        // Get the input handler
        inputHandler = GameObject.FindObjectOfType<InputHandler>();
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
        if(maxX > 90 && maxX < 70 || maxX < 270 && maxX > 290)
        {
            moveVector += dir;
            transform.rotation = Quaternion.LookRotation(moveVector);
        }

        this.transform.position += moveVector * Time.deltaTime;

        RotateTowardSteer();
    }

    private void Update()
    {
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

    private void RotateTowardSteer()
    {
        float _currentSteer = inputHandler.Steer;

        // Tilt to the right if steering right
        if(_currentSteer > inputHandler.controllerDeadZone)
        {
            this.transform.localEulerAngles = Quaternion.Euler(new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, -yawMaxRotationAmount)).eulerAngles;
        }
        // Tilt to the left if steering left
        else if (_currentSteer < -inputHandler.controllerDeadZone)
        {
            this.transform.localEulerAngles = Quaternion.Euler(new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, yawMaxRotationAmount)).eulerAngles;
        }
        // Tilt back to 0 if not steering
        else
        {
            this.transform.localEulerAngles = Quaternion.Euler(new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, 0)).eulerAngles;
        }
    }
}
