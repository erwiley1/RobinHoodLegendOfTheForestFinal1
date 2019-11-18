using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//CREDIT: https://answers.unity.com/questions/607100/how-to-make-an-ai-to-follow-the-player-in-2d-c.html
public class enemyArrow : MonoBehaviour
{
    // player gameobject slot
    private GameObject PlayerGameObject;
    // target transform slot
    private Transform target;//set target from inspector instead of looking in Update
    // speed float
    public float speed = 3f;
    // gameObject rigidbody2D slot
    private Rigidbody2D _rb2D;
    // Checks to make sure the arrow has turned only once
    private bool haveITurned;
    // arrow circle collider
    public CircleCollider2D _collider2D;
    // boolean that determines if the object should stop moving or not
    public bool stopMoving;
    
    void Start () 
    {
        // put any object tagged player into PlayerGameObject slot
        PlayerGameObject = GameObject.FindWithTag("Player");
        // target transform slot is equal to PlayerGameObject's transform
        target = PlayerGameObject.transform;
        // get circleCollider2D from gameObject
        _collider2D = gameObject.GetComponent<CircleCollider2D>();
    }
 
    void Update(){
         
        //rotate to look at the player
        if (haveITurned == false)
        {
            // Immediately set haveITurned boolean to true so if statement does not repeat
            haveITurned = true;
            // Rotate the arrow transform towards the target 
            transform.LookAt(target.position);
            // correcting the original rotation by rotating -90 degrees on y axis
            transform.Rotate(new Vector3(0,-90,0),Space.Self);
        }


        //move towards the player based speed * deltaTime
        transform.Translate(new Vector3(speed* Time.deltaTime,0,0) );

        //if stopMoving == true and the 2d collider is present...
        if (stopMoving && _collider2D != null)
        {
            // Set speed to 0 so the object stops moving
            speed = 0;
            // Set rigidBody2D to static to not only ensure the gameObject doesn't move, but so it does not get
            // affected by physics.
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            // Destroy the collider so it won't touch anything and to prevent the if statement from repeating.
            Destroy(_collider2D);
            // Start the coroutine that despawns the bullet after 4 seconds.
            StartCoroutine(Despawner());
        }

            

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Destroy the arrow when it touches an object tagged "Player" so the arrow does not float in the player position.
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        // Set the boolean stopMoving to true
        stopMoving = true;
    }

    IEnumerator Despawner()
    {
        // Wait 4 seconds...
        yield return new WaitForSeconds(4f);
        // ...then destroy the gameObject.
        Destroy(gameObject);
    }
}
