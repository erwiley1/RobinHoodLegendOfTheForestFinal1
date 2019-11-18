using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSight : MonoBehaviour
{
    // Archer gameObject slot
    public GameObject archerGameObject;

    // Get enemyArcher script from gameObject
    private enemyArcher my_enemyArcherScript;
    // Start is called before the first frame update
    void Start()
    {
        // Get enemy archer script from enemy archer gameObject
        my_enemyArcherScript = archerGameObject.GetComponent<enemyArcher>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Upon player entering the trigger area, enemy archer is allowed to fire at player.
        if (other.gameObject.CompareTag("Player"))
        {
            my_enemyArcherScript.playerIsSeen = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // Upon player exiting the trigger area, enemy archer is not allowed to fire at player.
        if (other.gameObject.CompareTag("Player"))
        {
            my_enemyArcherScript.playerIsSeen = false;
        }
    }
}
