using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyArcher : MonoBehaviour
{
    public GameObject enemyArrows;

    private bool wasArrowShot;
    private Vector3 myObjectLocation;
    public float shootingSpeed = 1.3f;

    public bool playerIsSeen;

    public GameObject player;
    private Transform playerTransfrom;
    private Vector2 playerLocation;
    
    public float sightRange = 20f;

    private Animator archerAnimation;

    public GameObject damageSoundObject;
    
    public int health = 3;
    
    private SpriteRenderer  mySpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        archerAnimation = gameObject.GetComponent<Animator>();
        archerAnimation.SetBool("DoAnimation", false);
        playerTransfrom = player.GetComponent<Transform>();
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float playerDistanceFromCharacter = Vector3.Distance(player.transform.position, transform.position);
        Debug.DrawLine(player.transform.position, transform.position);
        
        // if the distance from player is less than input range
        if (playerDistanceFromCharacter < sightRange)
        {
            playerIsSeen = true;
            if (wasArrowShot == false && playerIsSeen)
            {
                wasArrowShot = true;
                StartCoroutine(Shoot());
            }
        }
        // Get the vector2 position of the player 
        playerLocation = playerTransfrom.position;

        // Get the vector2 position of gameObject (or enemy)
        myObjectLocation = gameObject.transform.position; 
        
        // Flip the sprite based on if the x position of the player is more than or less than the enemy location. This
        // is so the enemy faces towards the player.
        
        // Face left if player x location less than enemy location
        if (playerLocation.x < myObjectLocation.x)
        {
            mySpriteRenderer.flipX = false;
        }
        // Face right if player x location more than enemy location
        else
        {
            mySpriteRenderer.flipX = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("PlayerDamage"))
        {
            health -= 1;
            Instantiate(damageSoundObject);
        }
        if (other.gameObject.CompareTag("PlayerArrow"))
        {
            Destroy(other.gameObject);
            health -= 1;
            Instantiate(damageSoundObject);
        }
    }
    
    IEnumerator Shoot()
    {           
        yield return new WaitForSeconds(shootingSpeed);
        if (playerIsSeen)
        {
            Instantiate(enemyArrows, new Vector3(myObjectLocation.x, myObjectLocation.y, myObjectLocation.z), transform.rotation);
            archerAnimation.SetBool("DoAnimation",true);
            yield return new WaitForSeconds(0.01f);
            archerAnimation.SetBool("DoAnimation",false);
            wasArrowShot = false;
        }
    }
}
