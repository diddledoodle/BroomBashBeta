using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{

    public List<Button> menuButtons;

    private int currentSelectedIndex = 0;

    private InputHandler inputHandler;

    // Start is called before the first frame update
    void Start()
    {
        // Select the first button
        menuButtons[0].Select();
        // Get the inputhandler
        inputHandler = GameObject.FindObjectOfType<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from the player
        if (inputHandler.MenuUp)
        {
            SelectNextButtonInList(1);
        } else if (inputHandler.MenuDown)
        {
            SelectNextButtonInList(-1);
        }
    }

    private void SelectNextButtonInList(int _nextMove)
    {
        int _selectedIndex = currentSelectedIndex + _nextMove;
        if(_selectedIndex < 0)
        {
            _selectedIndex = menuButtons.Count - 1;
        }
        else if(_selectedIndex > menuButtons.Count - 1)
        {
            _selectedIndex = 0;
        }

        // Select the button of the selected index 
        menuButtons[_selectedIndex].Select();
        currentSelectedIndex = _selectedIndex;

        /*jpost audio*/
        //play the menu navigation UI sound from wwise
        AkSoundEngine.PostEvent("play_bb_sx_menu_ui_main_navigation", gameObject);
    }

    public void SelectFirstIndexOnEnable()
    {
        // Weird hack to deselect and reselect - sorry single buttons :/
        if(menuButtons.Count > 1)
        {
            menuButtons[1].Select();
        }
        menuButtons[0].Select();
    }
}
