//CREDIT FOR MOVEMENT SYSTEM: https://www.raywenderlich.com/348-make-a-2d-grappling-hook-game-in-unity-part-1

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{
    // Normal speed of player.
    public float speed = 10f;
    
    // Sprint speed of player.
    public float sprintSpeed = 20;
    
    // Memorizes the speed's original value.
    private float _originalSpeed;
    
    // Get Rigidbody2D from gameObject.
    private Rigidbody2D rBody;
    
    // Boolean flag that determines if player is allowed to jump or not
    public bool canJump;
    
    // Memorize the gravity scale of the Rigidbody2D
    private float _originalGravity;
    
    // Float used as force to increase velocity of Rigidbody2D, either vertically or horizontally.
    public float jumpForce = 10f;
    
    // Boolean that determines if player is on a left or right wall.
    private bool leftOrRight; // left = false, right = true
    
    // Bloolean that allows/doesn't allow movement from player.
    private bool _movementAvailable = true;
    
    // Set the gravity only once so if statement doesn't repeat
    private bool _setGravityOnce;
    
    // Get SpriteRenderer from gameObject
    private SpriteRenderer playerSprite;

    // Horizontal input float value
    private float horizontalInput;
    
    // Determines if player is touching left wall
    private bool touchingLeftWall;
    
    // Determines if player is touching right wall
    private bool touchingRightWall;
    
    // Determines if player is swinging
    public bool isSwinging;
    
    // Vector2 set to position rope grappling anchor is currently located.
    public Vector2 ropeHook;
    
    // Checks if player is grounded
    private bool groundCheck;
    
    // Checks if player is jumping
    private bool isJumping;
    
    // Float value to be used to add to swing motion.
    public float swingForce = 4f;
    
    // Float that changed vertical axis value.
    private float jumpInput;
    
    // Changes velocity of jumping.
    public float jumpSpeed = 3f;
    
    // Text slot for health text.
    public Text healthText;
    
    // Health of player.
    public int health = 4;
    
    // Get audioSource from gameObject.
    private AudioSource playerAudio;
    
    // Get SpriteRenderer from gameObject
    private SpriteRenderer spriteRenderer;
    
    // Scene slot for reloading
    Scene currentScene;
    
    // String to get the scene name.
    private string sceneName;
    
    // Float that determines what y position is considered a falling death for the player, can be changed in editor.
    public float fallDeathNumber = -20f;
    
    // Constantly switch between being on and off to give sprite flashing effect
    private bool spriteOnOff = true;
    
    // Integer flag that signals the stages of taking damage.
    private int detectDamage = 1; // 0: dont detect damage, 1: awaiting damage, 2: damage detected.

    // Animator slot from gameObject.
    private Animator playerAnimator;
    
    // Make read-only integers for the Animator variables.
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");

    void Awake()
    {
        // Get SpriteRenderer from gameObject.
        playerSprite = GetComponent<SpriteRenderer>();
        
        // Get Rigidbody2D from gameObject.
        rBody = GetComponent<Rigidbody2D>();
        
        // Get Animator from gameObject.
        playerAnimator = gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        // Get the active scene in Scene slot.
        currentScene = SceneManager.GetActiveScene();
        
        // Get scene name from scene slot.
        sceneName = currentScene.name;
        
        // originalSpeed gets the regular speed value from the start
        _originalSpeed = speed;
        
        // Get Rigidbody2d component from gameObject.
        rBody = GetComponent<Rigidbody2D>();
        
        // Create variable that gets the Rigidbody2D's gravity scale.
        var gravityScale = rBody.gravityScale;
        
        // Set the gravity scale value to _originalGravity.
        _originalGravity = gravityScale;
        
        // Get AudioSource component from gameObject.
        playerAudio = gameObject.GetComponent<AudioSource>();
        
        // Get SpriteRenderer component from gameObject.
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        // Pause the audio player.
        playerAudio.Pause();
    }
    
    void Update()
    {
        // If health <= 0 or transform.position.y < fallDeathNumber...
        if (health <= 0 || transform.position.y < fallDeathNumber)        
            // Reload the scene with the string variable containing the scene name.
            SceneManager.LoadScene(sceneName);        

        // If detectDamage int flag == 2 (damage detected)...
        if (detectDamage == 2)
        {
            // Set the detectDamage to 0 (dont detect damage).
            detectDamage = 0;
            
            // Start coroutine that goes through process of the player taking damage.
            StartCoroutine(DamageInvulnerability());
        }

        // Have the health text display the health number with the text "Health: " on the left of it.
        healthText.text = "Health: " + health;
        
        // Set jumpAxis value to the default jump button being pressed or not.
        jumpInput = Input.GetAxis("Jump");
        
        // Set horizontalInput value to the default jump button being pressed or not.
        horizontalInput = Input.GetAxis("Horizontal");
        
        // Create variable that gets the bounds of the sprite by the y value
        var halfHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;
        
        // Send out raycast downward by -0.04 minus the half height of the sprite bounds. This determines if gameObject
        // is grounded.
        groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - halfHeight - 0.04f), Vector2.down, 0.025f);
        
        // Set the velocity of the Rigidbody2D to horizontalInput value * speed, maintain velocity in y value.
        rBody.velocity = new Vector2(horizontalInput * speed, rBody.velocity.y);

        // If leftOrRight == false (player is on a left wall) and _movementAvailable flag is set to false...
        if (_movementAvailable == false && leftOrRight == false)
        {
            rBody.velocity = new Vector2(jumpForce, jumpForce);
        }
        
        // If leftOrRight == true (player is on a right wall) and _movementAvailable flag is set to false...
        if (_movementAvailable == false && leftOrRight)
        {
            rBody.velocity = new Vector2(-jumpForce, jumpForce);
        }

        // Set the animator float MoveSpeed to speed. use Mathf.Abs to make sure speed is always positive.
        playerAnimator.SetFloat(MoveSpeed, Mathf.Abs(horizontalInput));

        // If left shift is held down, while canJump is true, and either of the horizontal inputs are pressed...
        if (Input.GetKey(KeyCode.LeftShift) && canJump  && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)))
            // ...set the animator boolean IsSprinting to true.
            playerAnimator.SetBool(IsSprinting, true);
        
        // If left shift is not held down, canJump is false, or none of the horizontal inputs are pressed...
        else
            // ...set the animator boolean IsSprinting to false.
            playerAnimator.SetBool(IsSprinting, false);



        
        
        // Speed value switches back and forth from sprintSpeed to Original speed depending if player is sprinting or not.
        if (Input.GetKey(KeyCode.LeftShift) && !isSwinging)
        {
            // If speed is not more than or equal to sprint speed, keep adding to the speed by 1.
            if (!(speed >= sprintSpeed))
                speed += 1f;

            //Debug.Log("Key is held down");
        }
        else
        {
            // If speed is not less than or equal to original speed, keep subtracting to the speed by 1.
            if (!(speed <= _originalSpeed))
                speed -= 1f;
            //Debug.Log("Key not held down");
        }

        // If space is held down and the canJump boolean is true (confirming player is on ground), add force upwards by jumpForce.
        if (canJump)
        {
            // If space is pressed...
            if (Input.GetKeyDown("space"))
            {
                // Set canJump boolean to false to prevent player from jumping midair.
                canJump = false;
                
                // Add force upward to propel gameObject upwards.
                rBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }
        
        // If boolean that determines player is touching left wall is active...
        if (touchingLeftWall)
        {
            // If _setGravityOnce == false (if statement has permission to be used)...
            if (_setGravityOnce == false)
            {
                // Set _setGravityOnce boolean to true so if statement does not repeat.
                _setGravityOnce = true;
                
                // Set the gravity of the Rigidbody2D to 0.1.
                rBody.gravityScale = 0.1f;
            }
            
            // If space if pressed while touching left wall...
            if (Input.GetKeyDown("space"))
            {
                //Debug.Log("Wall jump A");
                
                // Start coroutine determining action of player jumping from left wall.
                StartCoroutine(WallJumpLeft());             
            }
        }
        
        // If boolean that determines player is touching right wall is active...
        else if (touchingRightWall)
        {
            // If _setGravityOnce == false (if statement has permission to be used)...
            if (_setGravityOnce == false)
            {
                // Set _setGravityOnce boolean to true so if statement does not repeat.
                _setGravityOnce = true;
                
                // Set the gravity of the Rigidbody2D to 0.1.
                rBody.gravityScale = 0.1f;
            }

            // If space is pressed while touching right wall...
            if (Input.GetKeyDown("space"))
            {
                Debug.Log("Wall jump A");
                
                // Start coroutine determining action of player jumping from right wall.
                StartCoroutine(WallJumpRight());
            }
        }
        
        // If neither right or left walls being touched...
        else
        {
            // If the gravity scale is less than what it was as the original gravity...
            if (rBody.gravityScale < _originalGravity)
                
                // ...set it back to the original gravity value.
                rBody.gravityScale = _originalGravity;
            
            // _setGravityOnce flag reset back to false.
            _setGravityOnce = false;
        }
    }

void FixedUpdate()
{
    if (horizontalInput < 0f || horizontalInput > 0f)
    {
        // Flip the sprite based on the horizontal input
        playerSprite.flipX = horizontalInput < 0f;
        if (isSwinging)
        {
            // "Get a normalized direction vector from the player to the hook point".
            var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;

            // "Inverse the direction to get a perpendicular direction. Depending on whether the slug is swinging left
            // or right, a perpendicular direction is calculated using the playerToHookDirection."
            Vector2 perpendicularDirection;
            
            // If the horizontal input facing left...
            if (horizontalInput < 0)
                // Set perpendicular direction to negative y value
                perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);

            // If the horizontal input facing right...
            else
                perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);


            // Create variable force that gets perpendicularDirection * swingForce.
            var force = perpendicularDirection * swingForce;
            
            // Add force to the Rigidbody2D in the direction of the force variable.
            rBody.AddForce(force, ForceMode2D.Force);
        }
        else
        {
            // if groundCheck == true...
            if (groundCheck)
            {
                // Create variable ground force which gets speed * 2.
                var groundForce = speed * 2f;
                
                // Adds force in the direction of the horizontalInput * groundForce - rBody.velocity.x) * groundForce
                rBody.AddForce(new Vector2((horizontalInput * groundForce - rBody.velocity.x) * groundForce, 0));
            }
        }
    }

    // If isSwinging flag == false...
    if (!isSwinging)
    {
        // If groundCheck == false, return out of if statement.
        if (!groundCheck) return;

        // isJumping equal to true as long as jumpInput > 0.
        isJumping = jumpInput > 0f;
        
        // If isJumping == true and canJump == true...
        if (isJumping && canJump)
        {
            // ...Have the Rigidbody2D velocity = jumpSpeed
            rBody.velocity = new Vector2(rBody.velocity.x, jumpSpeed);
        }
    }

    // If canJump == false...
    if (canJump == false)
        // Set animator bool IsJumping to true to play jumping animation
        playerAnimator.SetBool(IsJumping, true);

    else
        // Set animator bool IsJumping to false to stop jumping animation
        playerAnimator.SetBool(IsJumping, false);
}

 void OnCollisionEnter2D(Collision2D other)
    {
        //Checks if player is touching object tagged "ground", if so, canJump = true.
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            playerAnimator.SetBool(IsJumping, false);
        }
        
        // If player touching "LeftWall" tagged object, set touchingLeftWall flag to true
        if (other.gameObject.CompareTag("LeftWall"))
        {
            Debug.Log("Touching Wall");
            touchingLeftWall = true;
        }
        
        // If player touching "RightWall" tagged object, set touchingLRightWall flag to true
        if (other.gameObject.CompareTag("RightWall"))
        {
            Debug.Log("Touching Wall");
            touchingRightWall = true;
        }
        
        // If player touches "Damage" tagged object and detectDamage flag set to 1, set detectDamage to 2.
        if (other.gameObject.CompareTag("Damage") && detectDamage == 1)
            detectDamage = 2;
    }
    void OnCollisionExit2D(Collision2D other)
    {
        //Checks if player is not touching object tagged "ground", if so, canJump = false.
        if (other.gameObject.CompareTag("Ground"))
        {
            canJump = false;
        }

        // If player leaves "LeftWall" tagged object, set touchingLeftWall flag to false
        if (other.gameObject.CompareTag("LeftWall"))
        {
            Debug.Log("Touching Wall");
            touchingLeftWall = false;
        }
        
        // If player leaves "RightWall" tagged object, set touchingLRightWall flag to false
        if (other.gameObject.CompareTag("RightWall"))
        {
            Debug.Log("Touching Wall");
            touchingRightWall = false;
        }
    }

    
    IEnumerator WallJumpLeft()
    {
        // Set movement available to false
        _movementAvailable = false;
        // Indicate player is facing left wall
        leftOrRight = false;

        // Set jumpforce towards left direction while moving up * 0.7
        rBody.velocity = new Vector2(-jumpForce, jumpForce*0.7f);
        // Wait 0.2 seconds before making _movementAvailable true which allows the player to move again.
        yield return new WaitForSeconds(0.2f);
        _movementAvailable = true;
    }

    IEnumerator WallJumpRight()
    {
        // Set movement available to false
        _movementAvailable = false;
        // Indicate player is facing right wall
        leftOrRight = true;
        // Set jumpforce towards right direction while moving up * 0.7
        rBody.velocity = new Vector2(jumpForce, jumpForce*0.7f);
        // Wait 0.2 seconds before making _movementAvailable true which allows the player to move again.
        yield return new WaitForSeconds(0.2f);
        _movementAvailable = true;
    }
    
    IEnumerator DamageInvulnerability()
    {
        // Play hurt sound
        playerAudio.Play();
        // Subtract 1 health
        health -= 1;
        
        // Use for loop that repeatedly flashes the sprite to give hurt invincibility effect for 0.2 seconds.
        for (float x = 0; x < 0.2f; x += 0.01f)
        {
            spriteOnOff = !spriteOnOff;
            spriteRenderer.enabled = spriteOnOff;
            yield return new WaitForSeconds(0.01f);
        }
        // Set detectDamage int flag back to 1.
        detectDamage = 1;
    }



}
