using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public MenuNavigation otherMenu;

    private InputHandler inputHandler;

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = GameObject.FindObjectOfType<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inputHandler != null)
        {
            if (inputHandler.Decline)
            {
                if(otherMenu != null)
                {
                    otherMenu.SelectFirstIndexOnEnable();
                }
                this.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
