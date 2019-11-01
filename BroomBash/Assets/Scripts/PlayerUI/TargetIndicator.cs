﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{

    public Transform target;
    public GameObject meshes;
    public float hideDistance;
    public bool tutorial = false;

    private QuestController questController;

    // Start is called before the first frame update
    void Start()
    {
        // Get the quest controller
        questController = GameObject.FindObjectOfType<QuestController>();
        // Set the meshes active to false
        meshes.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(questController != null)
        {
            if (questController.countdownTimerIsActive)
            {
                meshes.SetActive(true);
                target = questController.currentQuest.gameObject.transform;
                /*Vector3 targetPosition = target.transform.position;
                targetPosition.z = 0;
                */
                Vector3 targetPosition = new Vector3(target.position.x, meshes.transform.position.y, target.position.z);
                transform.LookAt(targetPosition);

            }
            else
            {
                meshes.SetActive(false);
            }
        }

        if(tutorial && target != null){
            meshes.SetActive(true);
            //target = questController.currentQuest.gameObject.transform;
            /*Vector3 targetPosition = target.transform.position;
            targetPosition.z = 0;
            */
            Vector3 targetPosition = new Vector3(target.position.x, meshes.transform.position.y, target.position.z);
            transform.LookAt(targetPosition);
        } else if(tutorial && target == null)
        {
            meshes.SetActive(false);
        }
    }
}
