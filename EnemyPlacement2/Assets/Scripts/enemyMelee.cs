using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemyMelee : MonoBehaviour
{
    // Boolean that determines if the enemy is done charging or not
    private bool wasDoneCharging = true;
    // Vector2 of the gameObject
    private Vector2 myObjectLocation;

    // Boolean that detects if player is seen or not.
    public bool playerIsSeen;

    // Player gameObject.
    private GameObject player;

    // Sight range of the enemy.
    public float sightRange = 20f;
    
    // Movement speed of the enemy.
    private float speed = 5;
    
    // Boolean that determines if the enemy is allowed to move or not.
    private bool isMoving = true;
    
    // Enemy heatlh
    private int health = 3;

    // Get gameObject's original position.
    private Vector2 originalPosition;

    // Input new position for gameObject to ping to and pong from.
    public Vector2 newPosition;

    // Ping pong value
    public float pingPongVal= 0;
    
    // Value to add/subtract from pingPongVal.
    public float moveIncrementCurrent = 0.05f;
    
    // Float that remembers moveIncrementCurrent's value.
    private float moveIncrementCurrentOriginal;
    
    // Value to multiply the increment add by for when the enemy charges.
    public float moveIncrementMult;

    // Bool that tells when to flip sprite when set to true.
    private bool flipSprite;

    // Time will ping and pong from 0 to 1.
    public float time;
    
    // Every time after 1 iteration, the prev time will get the current time.
    private float prevTime;
    
    // Switches on and off to determine if prev time should get current time.
    private bool timeSwitch;
    
    // Get the gameObject's spriteRenderer.
    private SpriteRenderer mySpriteRenderer;

    // Slot for the damaging hitbox.
    public GameObject attackBoxObject;

    // Slot for gameObject that plays damage sound effect.
    public GameObject damageSoundObject;
    
    // Get the audiosource from gameObject.
    private AudioSource enemyAudio;
    
    // Vector2 points for time's Ping and Pong.
    Vector2 pointA;
    Vector2 pointB;
    
    // Get the distance between the two points that the knight will move back and forth from, and then divide the speed
    // by that length. This is to keep the speed consistant no matter how far or short the movement distance is. Math
    // is finally useful! Yay!
    float DistanceFormula(float x2, float x1, float y2, float y1)
    {
        return Mathf.Sqrt(Mathf.Pow(x2 - x1, 2) + Mathf.Pow(y2 - y1, 2f));
    }
    // Start is called before the first frame update
    void Start()
    {
        // Get the transform position of the gameObject and place it into originalPosition vector.
        originalPosition = gameObject.transform.position;
        // moveIncrementCurrentOriginal gets moveIncrementCurrent's value so moveIncrementCurrent can get the original value back
        // after it's value is changed. Also the original movement speed of the enemy
        moveIncrementCurrentOriginal = moveIncrementCurrent;
        // moveIncrementMult is equal to moveIncrementCurrent * 5, also is the charging speed for this enemy.
        moveIncrementMult = moveIncrementCurrent * 5;
        // Player gameObject slot is equal to first object found tagged with "Player".
        player = GameObject.FindWithTag("Player");
        // pointA Vector2 is equal to the originalPosition value, which is the position enemy was originally placed.
        pointA = new Vector2(originalPosition.x, originalPosition.y);
        // pointB Vector2 has its x value equal to the input position made in editor, y position remains same as original.
        pointB = new Vector2(newPosition.x, originalPosition.y);
        // speed is divided by the distance formula float, the way it works is explained where it is initialized.
        speed = speed / DistanceFormula(pointB.x, pointA.x, pointB.y, pointA.y);
        // Get the sprite renderer component from gameObject, place it into mySpriteRenderer.
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // Set the attackBoxObject to inactive; it only needs to be active when the enemy is attacking.
        attackBoxObject.gameObject.SetActive(false);
        // Get the AudioSource component from gameObject, place it into enemyAudio.
        enemyAudio = gameObject.GetComponent<AudioSource>();
    }


    private void FixedUpdate()
    {
        // Destroy the gameObject when health = 0.
        if (health <= 0)
        {
            Destroy(gameObject);
        }    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Get the distance from the player transform to the gameObject transform
        float playerDistanceFromCharacter = Vector2.Distance(player.transform.position, transform.position);
        // Draw a line from the player's transform position to gameObject's position
        Debug.DrawLine(player.transform.position, transform.position);
        
        // if the distance from player <= 20, playerIsSeen = true, meaning player is detected in range
        if (playerDistanceFromCharacter < sightRange)
        {
            playerIsSeen = true;
            Debug.Log("playerIsSeen = " + playerIsSeen);
            // If the boolean that detects player in range is true and the boolean that tells that enemy is done charging
            // is equal to true...
            if (wasDoneCharging && playerIsSeen)
            {
                //... Set the wasDoneCharging boolean to false to prevent if statement from repeating...
                wasDoneCharging = false;
                //... and start the coroutine that makes the enemy charge back and forth.
                StartCoroutine(Charge());
            }
        }
        // If player is not in sight range...
        else
        {
            //... set the playerIsSeen boolean to false to indicate that the player is not in sight
            playerIsSeen = false;
            //Debug.Log("playerIsSeen = " + playerIsSeen); // Test to make sure playerIsSeen works
        }
     
        // PingPong between 0 and 1, and increment based on the pingPongVal * speed.
        time = Mathf.PingPong(pingPongVal* speed, 1);
        
        // A boolean switch will turn on and off every iteration, when it's on, prevTime gets the time value,
        // when off, prevTime does not get the time value. This is to determine whether the time value is
        // currently adding or subtracting.
        if (timeSwitch == false)
            prevTime = time;
        timeSwitch = !timeSwitch;
        
        // Transform the position of the gameObject between the two Vector2 points
        transform.position = Vector2.Lerp(pointA, pointB, time);
        // Increase PingPongVal by the current increment
        if (isMoving)
        {
            pingPongVal+= moveIncrementCurrent;
        }

        
        // Flips sprite based on the time value adding or subtracting.
        // When the time value is subtracting, it will always be less than previous value. (flipX = true)
        if (prevTime > time)
        {
            mySpriteRenderer.flipX = true;
        }
        // When the time value is adding, it will always be more than previous value. (flipX = false)
        if (prevTime < time)
        {
            mySpriteRenderer.flipX = false;
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        // If the gameObject is hit by another gameObject tagged with "PlayerDamage"...
        if (other.gameObject.CompareTag("PlayerDamage"))
        {
            // ...decrement health by 1 and instantiate the damageSoundObject
            health -= 1;
            Instantiate(damageSoundObject);
        }
        // If the gameObject is hit by another gameObject tagged with "PlayerArrow"...
        if (other.gameObject.CompareTag("PlayerArrow"))
        {
            // ...decrement health by 1, instantiate the damageSoundObject, and destroy the other gameObject (arrow)
            Destroy(other.gameObject);
            health -= 1;
            Instantiate(damageSoundObject);
        }
    }

    IEnumerator Charge()
    {
        // Set isMoving to false to have the gameObject momentarily stop so that it looks like the melee enemy is
        // anticipating to charge for a moment, and to give the player a warning that the enemy is about to charge.
        isMoving = false; 
        // Wait 0.5 seconds
        yield return new WaitForSeconds(0.5f);
        // Set the attacking hitbox to true.
        attackBoxObject.gameObject.SetActive(true);
        // Set the current increment value to the multiplied value so the enemy charges.
        moveIncrementCurrent = moveIncrementMult;
        // Play the WOOSH sound effect.
        enemyAudio.Play();
        // Set the isMoving to true to have the gameObject move again.
        isMoving = true;
        // Wait 0.4 seconds.
        yield return new WaitForSeconds(0.4f);
        // Set the attacking hitbox to false.
        attackBoxObject.gameObject.SetActive(false);
        // Set the current increment value back to the original value so the enemy just walks.
        moveIncrementCurrent = moveIncrementCurrentOriginal;           
        // Wait randomly between 0.5-3 seconds before setting the wasDoneCharging boolean to true, which signals that
        // the gameObject has permission to charge again.
        yield return new WaitForSeconds(Random.Range(0.5f, 3f));
        wasDoneCharging = true;
        // tell the if statement in void Update() that the enemy has finished charging and can now charge again
    }
}
