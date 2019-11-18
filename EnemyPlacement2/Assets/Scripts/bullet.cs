using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // Movement speed of the bullet.
    public float speed = 1f;
    // Rigidbody2d of gameObject
    private Rigidbody2D _rb2D;
    // CircleCollider2d of the gameObject
    private CircleCollider2D arrowHitBox;
    //public Quaternion instantiatorRotation;

    void Awake()
    {
        // Get the Rigidbody2d from the gameObject and put it into _rb2D.
        _rb2D = gameObject.GetComponent<Rigidbody2D>();
        // Get the CircleCollider2d from the gameObject and put it into arrowHitBox.
        arrowHitBox = gameObject.GetComponent<CircleCollider2D>();
    }

    void LateUpdate()
    {
        // Move based on the given speed * deltaTime.
        transform.Translate(speed * Time.deltaTime, 0, 0);
   
        // Constantly subtract the speed by -0.07 as long as it's not equal to zero. This is to make the arrow fall as
        // it flies in the air.
        if (speed != 0)
        {
            speed -= 0.07f;
        }

        // When the arrow is no longer moving (speed <= 0), start the coroutine that despawns the arrow after 5 seconds.
        if (speed <= 0)
        {
            StartCoroutine(destroyMe());
        }  
        
        // If speed < 0, correct it back to 0.
        if (speed < 0)
        {
            speed = 0;
        }
    }


    IEnumerator destroyMe()
    {
        // Destroy the collider after landing so that the arrow doesn't do any more damage.
        Destroy(arrowHitBox);
        
        // Set the rigidbody2D type to static to also ensure the arrow doesn't move, as well as to make sure gravity
        // does not affect it.
        _rb2D.bodyType = RigidbodyType2D.Static;
        
        // Wait 5 seconds... 
        yield return new WaitForSeconds(5f);
        // ...before despawning arrow to save resources.
        Destroy(gameObject);
    }
    
    // Stops the arrows when they hit an object and sticks it in the air.
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Set speed to zero to make sure the arrow does not move.
        speed = 0;
    }
}
