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

    // Start is called before the first frame update
    void Start()
    {   
        // Set up the cameras
        GameObject _mainCamera = Instantiate(mainCamera);
        CinemachineVirtualCamera _cinemachineVCam = Instantiate(cinemachineVCam).GetComponent<CinemachineVirtualCamera>();
        _cinemachineVCam.m_Follow = this.gameObject.transform;
        _cinemachineVCam.m_LookAt = this.gameObject.transform;
        GameObject _miniMapCamera = Instantiate(miniMapCamera);
        MiniMap _miniMap = _miniMapCamera.GetComponent<MiniMap>();
        _miniMap.player = this.gameObject.transform;
        // Set up the player UI
        GameObject _playerUI = Instantiate(playerUI);
        // Set up miniMap
        GameObject.Find("MiniMap").GetComponent<RawImage>().texture = miniMapRenderTexture;
        // Input
        GameObject _inControl = Instantiate(inControl);
    }
}
