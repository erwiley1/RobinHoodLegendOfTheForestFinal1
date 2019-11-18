using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    // Player transform slot
    private Transform player;
    // Player gameObject slot
    public GameObject playerObject;
    // Camera offset, values are input from the editor
    public Vector3 offset;

    // Camera slot
    public Camera mainCamera;

    void Start()
    {
        // Get the component Camera from gameObject
        mainCamera = gameObject.GetComponent<Camera>();
    }

    void Update () 
    {
        // If the player transform slot is null (which it always will be from the start), look for gameObjects tagged as "Player"
        // The if statement will forever search until a player is found.
        if (player == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            // If a player is found, get its transform in the player slot
            player = playerObject.GetComponent<Transform>();
        }

        // Get player's x and y position and add the camera's offset x and y point position
        // Camera follows the player with specified offset position
        transform.position = new Vector3 (player.position.x + offset.x, player.position.y + offset.y, offset.z); 
    }
}
