using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goldTouch : MonoBehaviour
{
    // Slot for sceneChanger script, gets obtained from sceneChangerObject.
    public sceneChanger my_sceneChanger_script;

    // Private gameObject slot for the scene-changing gameObject.
    private GameObject sceneChangerObject;

    // gameObject slot for the object that produces noise when the gold is collected.
    public GameObject coinNoiseObject;

    private int isTouched = 0; // 0: awaiting detection, 1: player detected, 2: stop detecting for player
    // Start is called before the first frame update
    void Start()
    {
        // Search the scene for object with "SceneChanger" tag and place it into sceneChangerObject.
        sceneChangerObject = GameObject.FindGameObjectWithTag("SceneChanger");
        // Get sceneChanger script component from sceneChangerObject.
        my_sceneChanger_script = sceneChangerObject.GetComponent<sceneChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player gets detected...
        if (isTouched == 1)
        {
            // Set int to 2 (stop detecting player).
            isTouched = 2;
            // Add to the gold amount in the scene-changing object script
            my_sceneChanger_script.goldAmount -= 1;
            // Instantiate the coin noise-making gameObject;
            Instantiate(coinNoiseObject);
            // Destroy the gameObject.
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If gameObject is being touched by an object tagged "Player", and int flag isTouched != 2 (stop detecting for player)...
        if (other.gameObject.CompareTag("Player") && isTouched != 2)
        {
            // set isTouched flag to 1 (player detected).
            isTouched = 1;
        }
    }
}
