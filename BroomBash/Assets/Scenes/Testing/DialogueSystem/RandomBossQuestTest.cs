using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class RandomBossQuestTest : MonoBehaviour
{
    public DialogueSystemTrigger dialogueSystemTrigger;

    // Start is called before the first frame update
    void Start()
    {
        dialogueSystemTrigger = this.gameObject.GetComponent<DialogueSystemTrigger>();
        Invoke("StartConversation", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartConversation()
    {
        dialogueSystemTrigger.OnUse();
    }
}
