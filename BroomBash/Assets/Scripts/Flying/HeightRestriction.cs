using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightRestriction : MonoBehaviour
{
    [Tooltip("The tag used by the 'ground' GameObject")]
    public string groundTag = "Ground";
    [Tooltip("How high the player can fly based on current height from ground")]
    public float maxHeightFromGround;
    [Tooltip("How low the player can fly based on current height from ground")]
    public float minHeightFromGround;

    private GameObject ground;
    private float minFlightHeight;
    private float maxFlightHeight;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        if (ground = GameObject.FindGameObjectWithTag(groundTag))
        {
            minFlightHeight = ground.transform.position.y + minHeightFromGround;
            maxFlightHeight = ground.transform.position.y + maxHeightFromGround;
            playerController = this.gameObject.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        if (ground != null)
        {
            // Check max flight height
            if (this.transform.position.y >= maxFlightHeight)
            {
                this.transform.position = new Vector3(this.transform.position.x, maxFlightHeight, this.transform.position.z);
                this.transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(this.transform.eulerAngles), Quaternion.Euler(0, this.transform.eulerAngles.y, this.transform.eulerAngles.z), Time.deltaTime * playerController.stoppedLevelingRotattionSpeed).eulerAngles;

            }
            if (this.transform.position.y <= minFlightHeight)
            {
                this.transform.position = new Vector3(this.transform.position.x, minFlightHeight, this.transform.position.z);
                this.transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(this.transform.eulerAngles), Quaternion.Euler(0, this.transform.eulerAngles.y, this.transform.eulerAngles.z), Time.deltaTime * playerController.stoppedLevelingRotattionSpeed).eulerAngles;

            }
        }
    }
}
