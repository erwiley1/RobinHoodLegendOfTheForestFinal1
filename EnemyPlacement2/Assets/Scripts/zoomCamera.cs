using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// *** UNUSED SCRIPT ***


public class zoomCamera : MonoBehaviour
{
    // Get the camreaFollow script from the cameraObject
    private cameraFollow _my_cameraFollow_script;
    
    // gameObject slot desired for a camera w/ cameraFollow script attached.
    public GameObject cameraObject;

    // Zoom amount to be implemented in editor.
    public float zoomAmount = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get cameraFollows script from cameraObject.
        _my_cameraFollow_script = cameraObject.GetComponent<cameraFollow>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the touching object is tagged "Player"...
        if (other.gameObject.CompareTag("Player"))
        {
            // Change the camera's orthographic size to 10.
            _my_cameraFollow_script.mainCamera.orthographicSize = 10;

        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        // If the leaving touching object is tagged "Player"...
        if (other.gameObject.CompareTag("Player"))
        {
            // Change the camera's orthographic size to 5.
            _my_cameraFollow_script.mainCamera.orthographicSize = 5;
        }
    }
}
