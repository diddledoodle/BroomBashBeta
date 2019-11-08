using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public float targetMoveDistance = 5;
    public float lerpSpeed = 2;

    private InputHandler inputHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inputHandler == null)
        {
            // Get the input handler
            inputHandler = this.transform.parent.GetComponent<InputHandler>();

            return;
        }
        Vector3 _position = this.transform.localPosition;

        MoveTargetAxisX(_position);
        //MoveTargetAxisY(_position);
    }

    private void MoveTargetAxisX(Vector3 _position)
    {
        if(inputHandler.CameraAxisX > inputHandler.controllerDeadZone)
        {
            this.transform.localPosition = Vector3.Lerp(_position, new Vector3(targetMoveDistance, _position.y, _position.z), Time.deltaTime * lerpSpeed * Mathf.Abs(inputHandler.CameraAxisX));
        }
        else if(inputHandler.CameraAxisX < -inputHandler.controllerDeadZone)
        {
            this.transform.localPosition = Vector3.Lerp(_position, new Vector3(-targetMoveDistance, _position.y, _position.z), Time.deltaTime * lerpSpeed * Mathf.Abs(inputHandler.CameraAxisX));

        }
        else if (inputHandler.CameraAxisX < inputHandler.controllerDeadZone && inputHandler.CameraAxisX > -inputHandler.controllerDeadZone)
        {
            this.transform.localPosition = Vector3.Lerp(_position, new Vector3(0, _position.y, _position.z), Time.deltaTime * lerpSpeed * 2);
        }
    }

    private void MoveTargetAxisY(Vector3 _position)
    {
        if (inputHandler.CameraAxisY > inputHandler.controllerDeadZone)
        {
            this.transform.localPosition = Vector3.Lerp(_position, new Vector3(_position.x, targetMoveDistance, _position.z), Time.deltaTime * lerpSpeed * Mathf.Abs(inputHandler.CameraAxisY));
        }
        else if (inputHandler.CameraAxisY < -inputHandler.controllerDeadZone)
        {
            this.transform.localPosition = Vector3.Lerp(_position, new Vector3(_position.x, -targetMoveDistance, _position.z), Time.deltaTime * lerpSpeed * Mathf.Abs(inputHandler.CameraAxisY));

        }
        else if (inputHandler.CameraAxisY < inputHandler.controllerDeadZone && inputHandler.CameraAxisY > -inputHandler.controllerDeadZone)
        {
            this.transform.localPosition = Vector3.Lerp(_position, new Vector3(_position.x, 0, _position.z), Time.deltaTime * lerpSpeed * 2);
        }
    }
}
