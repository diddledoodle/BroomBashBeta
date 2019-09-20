using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public Text timer;
    public GameObject miniMap;
    public GameObject notifcationPanel;
    public GameObject dialogPanel;
    public Texture miniMapRenderTexture;

    // Start is called before the first frame update
    void Start()
    {
        // Assign the render texture to the mini-map
        miniMap.GetComponent<RawImage>().texture = miniMapRenderTexture;
        // Turn off all components that arent used at the start of the game
        notifcationPanel.SetActive(false);
        dialogPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
