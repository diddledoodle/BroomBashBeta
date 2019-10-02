using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{

    public Transform target;
    public GameObject meshes;
    public float hideDistance;

    private QuestController questController;

    // Start is called before the first frame update
    void Start()
    {
        questController = GameObject.FindObjectOfType<QuestController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (questController.countdownTimerIsActive)
        {
            meshes.SetActive(true);
            target = questController.currentQuest.gameObject.transform;
            transform.LookAt(target.transform.position);
        }
        else
        {
            meshes.SetActive(false);
        }
    }
}
