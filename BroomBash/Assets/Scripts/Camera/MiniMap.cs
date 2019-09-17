using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform player;
    public bool rotateWithPlayer = false;
    public float heightAbovePlayer = 10f;

    private void LateUpdate()
    {
        Vector3 newPosition = new Vector3(player.position.x, player.position.y + heightAbovePlayer, player.position.z);
        this.transform.position = newPosition;
        // Rotate the mini map with the player
        if (rotateWithPlayer)
        {
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}
