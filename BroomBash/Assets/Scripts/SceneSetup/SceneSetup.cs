using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class SceneSetup : MonoBehaviour
{
    // Cameras
    public GameObject mainCamera;
    public GameObject miniMapCamera;
    public GameObject cinemachineVCam;
    public Texture miniMapRenderTexture;
    // UI
    public GameObject playerUI;
    // Input
    public GameObject inControl;
    // Post processing
    public GameObject postProcessing;

    // Start is called before the first frame update
    void Start()
    {
        // Input
        GameObject _inControl = Instantiate(inControl);
        // Set up the cameras
        GameObject _mainCamera = Instantiate(mainCamera);
        CinemachineVirtualCamera _cinemachineVCam = Instantiate(cinemachineVCam).GetComponent<CinemachineVirtualCamera>();
        _cinemachineVCam.m_Follow = this.transform;
        _cinemachineVCam.m_LookAt = this.gameObject.GetComponentInChildren<CameraTarget>().transform;
        GameObject _miniMapCamera = Instantiate(miniMapCamera);
        MiniMap _miniMap = _miniMapCamera.GetComponent<MiniMap>();
        _miniMap.player = this.gameObject.transform;
        // Set up the player UI
        GameObject _playerUI = Instantiate(playerUI);
        // Reference the player UI in the quest manager
        if(GameObject.FindObjectOfType<QuestController>()) GameObject.FindObjectOfType<QuestController>().playerUIManager = _playerUI.GetComponent<PlayerUIManager>();
        // Instantiate post processing
        GameObject _pp = Instantiate(postProcessing);
    }

    private void InitializeCameraFovChanger()
    {

    }
}
