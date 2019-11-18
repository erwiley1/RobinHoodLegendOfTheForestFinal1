using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletInstantiator : MonoBehaviour
{
    // Bullet to instantiate
    public GameObject prefabSphere;
    // Sound-maker object to instantiate
    public GameObject pewSoundObject;
    // Get the script from the pewSound gameObject;
    public pewSound my_pewSound_script;
    // Access the bullet script from bullet gameObject
    private bullet my_bullet_script;
    // Rotate the bullet based on the gameObject's transform rotation (UNUSED)
    //private Quaternion objectRotation;
    
    // Boolean that detects if the coroutine is done playing to prevent if staatement from repeating
    private bool hasCouroutinePlayed;
    // Get the location of the gameObject, the bullet will fire from this position
    private Vector3 myObjectLocation;

    // Float that determines how long the mouse is held down for, which also determines the velocity of the firing arrow.
    public float timeHeldDown = 0f;
    
    
    void Start()
    {
        // Get the pewsound script from the pewSound gameObject
        my_pewSound_script = pewSoundObject.GetComponent<pewSound>();
        // Get the bullet script from the bullet gameObject
        my_bullet_script = prefabSphere.GetComponent<bullet>();
    }

    // Update is called once per frame
    void Update()
    {
        // Wait for mouse button to be let go
        if (Input.GetMouseButtonUp(1))
        {          
            // The objectLocation vector gets set to the gameObject (player) location
            myObjectLocation = gameObject.transform.position;
            // An if statement with boolean hasCouroutinePlayed checks to see if the coroutine has played or not
            if (hasCouroutinePlayed == false)
            {
                // If the timeHeldDown goes beyond 25...
                if (timeHeldDown > 25)
                    // ...correct it back to 25.
                    timeHeldDown = 25;
                // Set the bullet speed to the timeHeldDown
                my_bullet_script.speed = timeHeldDown;
                // Boolean hasCouroutinePlayed = true, meaning the if statement can start again.
                hasCouroutinePlayed = true;
                // Start the couroutine that goes through the process of firing an arrow.
                StartCoroutine(FireArrow());
            }
        }

        // Constantly add +1 to timeHeldDown the as it gets held down
        if (Input.GetMouseButton(1))
        {
            timeHeldDown += 1f;
        }
        // When the mouse button is no longer being input, reset timeHeldDown back to zero
        else if (!(Input.GetMouseButton(1)))
        {
            timeHeldDown = 0;
        }
    }

    //instantiate the sphere every 0.1 seconds at the location of the gameObject
    IEnumerator FireArrow()
    {
        // Get the rotation of the gameObject and set it to Quaternion objectRotation (UNUSED).
        //objectRotation = transform.rotation;
        
        // Set rotation of bullet to objectRotation (UNUSED).
        //my_bullet_script.instantiatorRotation = objectRotation;

        // Instantiate the bullet at the gameObject's transform location, make sure rotation is set to zero on XYZ axis.
        Instantiate(prefabSphere, new Vector3(myObjectLocation.x, myObjectLocation.y, myObjectLocation.z), transform.rotation * Quaternion.Euler (0f, 0f, 0f));
        // Set the pewsound script to choose the first sound in the switch int variable, chooseSound.
        my_pewSound_script.chooseSound = 1;
        // Instantiate the pewSoundObject
        Instantiate(pewSoundObject);
        // wait 0.1f seconds before allowing the coroutine to start again
        yield return new WaitForSeconds(0.1f);
        hasCouroutinePlayed = false;
        //my_bullet_script.speed = 1f;
    }


}
