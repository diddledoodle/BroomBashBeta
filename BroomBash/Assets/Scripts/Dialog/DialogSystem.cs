using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{

    public enum DialogType { PICKUP, DROPOFF}

    public DialogType dialogType;

    public List<string> dialog = new List<string>();

    private System.Random rand = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetRandomQuestDialog()
    {
        string _dialog = string.Empty;
        if(dialog.Count > 1)
        {
            _dialog = dialog[rand.Next(0, dialog.Count)];
        }
        else if(dialog.Count == 1)
        {
            _dialog = dialog[0];
        }
        else if(dialog.Count == 0)
        {
            _dialog = "There are no dialog sentences...";
        }

        return _dialog;
    }
}
