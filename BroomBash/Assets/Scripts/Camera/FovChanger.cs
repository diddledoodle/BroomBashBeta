using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FovChanger : MonoBehaviour
{
    public float baseFov = 40;
    public float slowDownFov = 50f;
    public float speedUpFov = 110f;
    public float speedChangeMultiplier = 1f;


    private CinemachineVirtualCamera myCamera;
    private InputHandler inputHandler;

    // Update is called once per frame
    void Update()
    {
        if (inputHandler == null)
        {
            inputHandler = GameObject.FindObjectOfType<InputHandler>();
            //Debug.Log("no input handler");
            return;
        }

        if(myCamera == null)
        {
            myCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            //Debug.Log("no vcam");
            return;
        }
        
        ChangeCameraFovBasedOnInput();
    }

    private void ChangeCameraFovBasedOnInput()
    {
        if(inputHandler.SpeedControl > inputHandler.controllerDeadZone)
        {
            myCamera.m_Lens.FieldOfView = Mathf.Lerp(myCamera.m_Lens.FieldOfView, speedUpFov, Time.deltaTime * speedChangeMultiplier);
        }

        else if(inputHandler.SpeedControl < -inputHandler.controllerDeadZone)
        {
            myCamera.m_Lens.FieldOfView = Mathf.Lerp(myCamera.m_Lens.FieldOfView, slowDownFov, Time.deltaTime * speedChangeMultiplier);
        }

        else
        {
            myCamera.m_Lens.FieldOfView = Mathf.Lerp(myCamera.m_Lens.FieldOfView, baseFov, Time.deltaTime * speedChangeMultiplier);
        }
    }
}
