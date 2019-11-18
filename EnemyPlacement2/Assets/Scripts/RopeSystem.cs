//SOURCE: https://www.raywenderlich.com/348-make-a-2d-grappling-hook-game-in-unity-part-1
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RopeSystem : MonoBehaviour
{
    private Dictionary<Vector2, int> wrapPointsLookup = new Dictionary<Vector2, int>();
    public GameObject ropeHingeAnchor;
    public DistanceJoint2D ropeJoint;
    // Transform slot for crosshair
    public Transform crosshair;
    // SpriteRenderer slot for the crosshair
    public SpriteRenderer crosshairSprite;
    // PlayerMovement script
    public PlayerMovement playerMovement;
    private bool ropeAttached;
    // Vector2 position of player
    private Vector2 playerPosition;
    // gameObject RigidBody2D
    private Rigidbody2D ropeHingeAnchorRb;
    // Sprite that renders at the hinge anchor points
    private SpriteRenderer ropeHingeAnchorSprite;
    // Line renderer slot
    public LineRenderer ropeRenderer;
    // Layer mask slot
    public LayerMask ropeLayerMask;
    // Maximum distance that the raycast can fire.
    private float ropeMaxCastDistance = 20f;
    // List that tracks the rope's wrapping points.
    private List<Vector2> ropePositions = new List<Vector2>();
    private bool distanceSet;
    // Float that sets the speed that the player can move up and down on the rope
    public float climbSpeed = 3f;
    // Boolean flag that determines whether the rope's distance joint and distance property can be increased or decreased.
    private bool isColliding;
    private bool onAndOff;
    
    void Awake()
    {
        // Disable the distanceJoint2D component
        ropeJoint.enabled = false;
        // Set playerPosition to the transform position of the player
        playerPosition = transform.position;
        // Get Rigidbody2D from gameObject
        ropeHingeAnchorRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        // Get SpriteRenderer from gameObject
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // "...capture the world position of the mouse cursor using the camera's ScreenToWorldPoint method. You then
        // calculate the facing direction by subtracting the player's position from the mouse position in the world."
        var worldMousePosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        //"...calculate the facing direction by subtracting the player's position from the mouse position in the world."
        var facingDirection = worldMousePosition - transform.position;
        // "...use this to create aimAngle, which is a representation of the aiming angle of the mouse cursor. "
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        // aimAnlge is kept positive in if statement.
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        // Make Quaternion that gets the aimAngle and converts it from a radian angle to an angle in degrees.
        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        // Get the gameObject position and put it into playerPosition.
        playerPosition = transform.position;

        // If rope not attached to anchor point
        if (!ropeAttached)
        {
            
            playerMovement.isSwinging = false;
            // Pass aimAngle into SetCrosshairPosition function, the explanation of what it does is above the
            // implementation of the function.
            SetCrosshairPosition(aimAngle);
        }
        // If rope attached to anchor point
        else 
        {
            // set isSwinging variable in playerMovement script to true to determine that the player is currently swinging.
            playerMovement.isSwinging = true;
            // Get last anchor rope position
            playerMovement.ropeHook = ropePositions.Last();
            crosshairSprite.enabled = false;
            // If ropePositions list is more than 0...
            if (ropePositions.Count > 0)
            {
                // "Fire a raycast out from the player's position, in the direction of the player looking at the last
                // rope position in the list — the pivot point where the grappling hook is hooked into the rock — with a
                // raycast distance set to the distance between the player and rope pivot position."
                var lastRopePoint = ropePositions.Last();
                var playerToCurrentNextHit = Physics2D.Raycast(playerPosition, (lastRopePoint - playerPosition).normalized, Vector2.Distance(playerPosition, lastRopePoint) - 0.1f, ropeLayerMask);
    
                // "If the raycast hits something, then that hit object's collider is safe cast to a PolygonCollider2D.
                // As long as it's a real PolygonCollider2D, then the closest vertex position on that collider is
                // returned as a Vector2, using that handy-dandy method you wrote earlier."
                if (playerToCurrentNextHit)
                {
                    var colliderWithVertices = playerToCurrentNextHit.collider as PolygonCollider2D;
                    if (colliderWithVertices != null)
                    {
                        var closestPointToHit = GetClosestColliderPointFromRaycastHit(playerToCurrentNextHit, colliderWithVertices);

                        // wrapPointsLookup gets checked to make sure the same position is not getting wrapped again.
                        // If it is being wrapped, the rope will be restarted and cut which will drop the player.
                        if (wrapPointsLookup.ContainsKey(closestPointToHit))
                        {
                            ResetRope();
                            return;
                        }

                        // ropePositions adds rope position that the rope should wrap around.
                        ropePositions.Add(closestPointToHit);
                        // wrapPointsLookup also gets updated with
                        wrapPointsLookup.Add(closestPointToHit, 0);
                        // distanceSet flag gets disabled which allows UpdateRopePositions() to re-configure the rope's
                        // distances to take into account the new rope length and segments.
                        distanceSet = false;
                    }
                }
            }
        }
        HandleInput(aimDirection);
        UpdateRopePositions();
        HandleRopeLength();
        HandleRopeUnwrap();
    }
    // Position the crosshair based on the aimAngle passed value calculated in Update() so that it circles around
    // the player in a radius on 1 unit based on sin cos.
    private void SetCrosshairPosition(float aimAngle)
    {
        // Makes sure that crosshair sprite is enabled.
        if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        var x = transform.position.x + 1f * Mathf.Cos(aimAngle);
        var y = transform.position.y + 1f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }

    // Gets called from Update()
    private void HandleInput(Vector2 aimDirection)
    {
        // Switches onAndOff boolean from true to false, and vice versa when scroll wheel clicked.
        if (Input.GetMouseButtonDown(2))
        {
            onAndOff = !onAndOff;
        }
    
        // If the onAndOff boolean is set to true...
        if (onAndOff)
        {
            // Rope line renderer is enabled and a 2D raycast is fired out from the player position in the aiming
            // direction.
            if (ropeAttached) return;
            ropeRenderer.enabled = true;
            /*
             * A maximum distance is specified so that the grappling hook can't be fired in infinite distance, and a
             * custom mask is applied so that you can specify which physics layers the raycast is able to hit.
             */
            var hit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);
        
            // If a raycast is found...
            if (hit.collider != null)
            {
                // Set ropeAttached to true
                ropeAttached = true;
                // Check is made on list of rope vertex positions to make sure the point hit isn't in there already.
                if (!ropePositions.Contains(hit.point))
                {
                    // 4
                    // Jump slightly to distance the player a little from the ground after grappling to something.
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    ropePositions.Add(hit.point);
                    // Set distance equal to distance between the player and the raycast point
                    ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    // ropeJoint gets enabled
                    ropeJoint.enabled = true;
                    // Anchor sprite enabled
                    ropeHingeAnchorSprite.enabled = true;
                }
            }
            // If a raycast is not found...
            else
            {
                // Disable rope renderer
                ropeRenderer.enabled = false;
                // Set ropeAttached flag to false
                ropeAttached = false;
                // Disable rope joint
                ropeJoint.enabled = false;
            }
        }

        if (onAndOff == false)
        {
            ResetRope();
        }
    }

    // Gets called when onAndOff == false
    private void ResetRope()
    {
        // clears wrapPointsLookup dictionary each time the player disconnects the rope
        wrapPointsLookup.Clear();
        ropeJoint.enabled = false;
        ropeAttached = false;
        // tell the isSwinging variable in playerMovement that the player is not swinging
        playerMovement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, transform.position);
        ropePositions.Clear();
        ropeHingeAnchorSprite.enabled = false;
    }
    
    private void UpdateRopePositions()
    {
        // Return out of this method if the rope isn't attached.
        if (!ropeAttached)
        {
            return;
        }

        // Set vertex count of rope line to number of positions in ropePosition, plus 1 for player position
        ropeRenderer.positionCount = ropePositions.Count + 1;

        // Loop through ropePositions list backwards. For every position, with the exception of the last position, set
        // the vertex position of the line renderer to the Vector2
        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != ropeRenderer.positionCount - 1) // if not the Last point of line renderer
            {
                ropeRenderer.SetPosition(i, ropePositions[i]);
                
                /*
                 * Set the rope anchor to the second-to-last rope position where the current hinge/anchor should be, or
                 * if there is only one rope position, then set that one to be the anchor point. This configures the
                 * ropeJoint distance to the distance between the player and the current rope position being looped over.
                 */
                if (i == ropePositions.Count - 1 || ropePositions.Count == 1)
                {
                    var ropePosition = ropePositions[ropePositions.Count - 1];
                    if (ropePositions.Count == 1)
                    {
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                    else
                    {
                        ropeHingeAnchorRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                }
                // "This if-statement handles the case where the rope position being looped over is the second-to-last
                // one; that is, the point at which the rope connects to an object, a.k.a. the current hinge/anchor point."
                else if (i - 1 == ropePositions.IndexOf(ropePositions.Last()))
                {
                    var ropePosition = ropePositions.Last();
                    ropeHingeAnchorRb.transform.position = ropePosition;
                    if (!distanceSet)
                    {
                        ropeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        distanceSet = true;
                    }
                }
            }
            //sets last vertex position of rope to player position.
            else
            {
                ropeRenderer.SetPosition(i, transform.position);
            }
        }
    }
    // Vector2 method that takes two parameter, RaycastHit2D hit, and PolygonCollider2D polyCollider
    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        /*
         * "This converts the polygon collider's collection of points, into a dictionary of Vector2 positions (the value
         * of each dictionary entry is the position itself), and the key of each entry, is set to the distance that this
         * point is to the player's position (float value). Something else happens here: the resulting position is
         * transformed into world space (by default a collider's vertex positions are stored in local space - i.e. local
         * to the object the collider sits on, and we want the world space positions)".
         */
        var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)), 
            position => polyCollider.transform.TransformPoint(position));

        /*
         * The dictionary is ordered by key. In other words, the distance closest to the player's current position, and
         * the closest one is returned, meaning that whichever point is returned from this method, is the point on the
         * collider between the player and the current hinge point on the rope".
         */
        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }
    
    private void HandleRopeLength()
    {
        // Get vertical axis input, "and depending on the ropeAttached iscColliding flags will either increase or
        // decrease the ropeJoint distance, having the effect of lengthening or shortening the rope."
        // Looks for vertical axis input, either s or down arrow
        if (Input.GetAxis("Vertical") >= 1f && ropeAttached && !isColliding)
        {
            ropeJoint.distance -= Time.deltaTime * climbSpeed;
        }
        // Looks for vertical axis input, either w or up arrow
        else if (Input.GetAxis("Vertical") < 0f && ropeAttached)
        {
            ropeJoint.distance += Time.deltaTime * climbSpeed;
        }
    }
    
    // If a collider is currently touching a collider2D, set isColliding boolean to true.
    void OnTriggerStay2D(Collider2D colliderStay)
    {
        isColliding = true;
    }
    
    // If a collider is not touching a collider2D, set isColliding boolean to false.
    private void OnTriggerExit2D(Collider2D colliderOnExit)
    {
        isColliding = false;
    }
    private void HandleRopeUnwrap()
    {
        if (ropePositions.Count <= 1)
        {
            return;
        }
        // Hinge = next point up from the player position
        // Anchor = next point up from the Hinge
        // Hinge Angle = Angle between anchor and hinge
        // Player Angle = Angle between anchor and player

        // "The index in the ropePositions collection two positions from the end of the collection".
        var anchorIndex = ropePositions.Count - 2;
        // "The index in the collection where the current hinge point is stored".
        var hingeIndex = ropePositions.Count - 1;
        // "calculated by referencing the anchorIndex location in the ropePositions collection, and is simply a Vector2
        // value of that position".
        var anchorPosition = ropePositions[anchorIndex];
        // "HingePosition is calculated by referencing the hingeIndex location in the ropePositions collection, and is
        // simply a Vector2 value of that position".
        var hingePosition = ropePositions[hingeIndex];
        // "hingeDir a vector that points from the anchorPosition to the hingePosition. It is used in the next variable
        // to work out an angle".
        var hingeDir = hingePosition - anchorPosition;
        // "hingeAngle is where the ever useful Vector2.Angle() helper function is used to calculate the angle between
        // anchorPosition and the hinge point."
        var hingeAngle = Vector2.Angle(anchorPosition, hingeDir);
        // "playerDir is the vector that points from anchorPosition to the current position of the player (playerPosition)".
        var playerDir = playerPosition - anchorPosition;
        // "playerAngle is then calculated by getting the angle between the anchor point and the player".
        var playerAngle = Vector2.Angle(anchorPosition, playerDir);
        
        if (!wrapPointsLookup.ContainsKey(hingePosition))
        {
            Debug.LogError("We were not tracking hingePosition (" + hingePosition + ") in the look up dictionary.");
            return;
        }
        
        if (playerAngle < hingeAngle)
        {
            // "If the current closest wrap point to the player has a value of 1 at the point where playerAngle < hingeAngle
            // then unwrap that point, and return so that the rest of the method is not handled".
            if (wrapPointsLookup[hingePosition] == 1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            // "Otherwise, if the wrap point was not last marked with a value of 1, but playerAngle is less than the
            // hingeAngle, the value is set to -1 instead".
            wrapPointsLookup[hingePosition] = -1;
        }
        else
        {
            // "If the current closest wrap point to the player has a value of -1 at the point where playerAngle > hingeAngle,
            // unwrap the point and return".
            if (wrapPointsLookup[hingePosition] == -1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            // "Otherwise, set the wrap point dictionary entry value at the hinge position to 1".
            wrapPointsLookup[hingePosition] = 1;
        }
    }
    private void UnwrapRopePosition(int anchorIndex, int hingeIndex)
    {
        // "The current anchor index becomes the new hinge position and the old hinge position is removed".
        var newAnchorPosition = ropePositions[anchorIndex];
        wrapPointsLookup.Remove(ropePositions[hingeIndex]);
        ropePositions.RemoveAt(hingeIndex);

        // "The rope hinge RigidBody2D has its position changed here to the new anchor position."
        ropeHingeAnchorRb.transform.position = newAnchorPosition;
        distanceSet = false;

        // Set new rope distance joint distance for anchor position if not yet set.
        if (distanceSet)
        {
            return;
        }
        ropeJoint.distance = Vector2.Distance(transform.position, newAnchorPosition);
        distanceSet = true;
    }
}
