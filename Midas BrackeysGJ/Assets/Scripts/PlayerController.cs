using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region PARAM
    private PlayerStatus playerStatus;
    //private GameManager gameManager;

    public bool drawDebugRaycasts = true;

    public RaycastHit2D pickableObject;

    [Header("Environment Check Properties")]
    [SerializeField] private Transform CheckTrap;
    public float footOffset = .4f;          //X Offset of feet raycast
    public float armOffset = .4f;
    public float eyeHeight = 1.5f;          //Height of wall checks
    public float reachOffset = .7f;         //X offset for wall grabbing
    public float grabOffset = 1f;
    public float headClearance = .5f;       //Space needed above the player's head
    public float groundDistance = .2f;      //Distance player is considered to be on the ground
    public float grabDistance = .4f;        //The reach distance for wall grabs
    public LayerMask groundLayer;           //Layer of the ground
    public LayerMask trapLayer;             // Layer of the trap
    public LayerMask jumpInteractifLayer;   // Layer of the enemy

    [Header("Status Flags")]
    public bool isOnGround;                 //Is the player on the ground?
    public bool isJumping;                  //Is player jumping?
    public bool isHanging;                  //Is player hanging?
    public bool isCrouching;                //Is player crouching?
    public bool isHeadBlocked;
    public bool canPick;
    public bool canThrow;

    PlayerInput input;
    BoxCollider2D bodyCollider;             //The collider component
    Rigidbody2D rigidBody;                  //The rigidbody component

    float jumpTime;                         //Variable to hold jump duration
    float coyoteTime;                       //Variable to hold coyote duration
    float playerHeight;                     //Height of the player

    float originalXScale;                   //Original scale on X axis
    int direction = 1;                      //Direction player is facing

    Vector2 colliderStandSize;              //Size of the standing collider
    Vector2 colliderStandOffset;            //Offset of the standing collider
    Vector2 colliderCrouchSize;             //Size of the crouching collider
    Vector2 colliderCrouchOffset;           //Offset of the crouching collider

    const float smallAmount = .05f;         //A small amount used for hanging position
    #endregion

    #region START | AWAKE
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        input = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        //gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        originalXScale = transform.localScale.x;
        playerHeight = bodyCollider.size.y;

        colliderStandSize = bodyCollider.size;
        colliderStandOffset = bodyCollider.offset;

        colliderCrouchSize = new Vector2(bodyCollider.size.x, bodyCollider.size.y / 2);
        colliderCrouchOffset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y / 2);
    }
    #endregion

    #region UPDATE | FIXED UPDATE| LATE UPDATE
    // Update is called once per frame
    void FixedUpdate()
    {
        //Check the environment to determine status
        PhysicsCheck();

        //Process ground and air movements
        GroundMovement();
        MidAirMovement();

        if (playerStatus.Power == PlayerStatus.PlayerPower.Poisoned && playerStatus.Health > 0)
        {
            if (playerStatus.PoisonDuration > 0)
            {
                playerStatus.PoisonDuration -= Time.deltaTime;
            }
            else
            {
                playerStatus.Health = 0;
            }
        }

        if (!isOnGround)
            JumpAttack();
    }
    #endregion

    #region METHODE
    private void JumpAttack()
    {
        Collider2D interactObjectJump = Physics2D.OverlapCircle(CheckTrap.position, playerStatus.JumpImpact, jumpInteractifLayer);
        if (interactObjectJump != null && interactObjectJump.CompareTag("Enemy"))
        {
            Debug.Log(" You've touched a enemy, you kill it");
            Destroy(interactObjectJump.gameObject);
        } else if(interactObjectJump != null && interactObjectJump.CompareTag("Player"))
        {
            switch (interactObjectJump.gameObject.GetComponent<PlayerStatus>().Power)
            {
                case PlayerStatus.PlayerPower.Normal:
                    Debug.Log("Normal");
                    break;

                case PlayerStatus.PlayerPower.Poisoned:
                    Debug.Log("Poisoned");
                    isJumping = true;
                    rigidBody.AddForce(new Vector2(0.0f, interactObjectJump.gameObject.GetComponent<PlayerStatus>().Bounciness), ForceMode2D.Impulse);

                    break;
                case PlayerStatus.PlayerPower.Midas:
                    Debug.Log("Steels");
                    break;
                default:
                    break;
            }
        }
    }

    void FlipCharacterDirection()
    {
        direction *= -1;

        Vector3 scale = Vector3.one;

        scale.x = 1 * direction;

        //transform.localScale = scale;
    }
    #endregion

    #region MOUVEMENT
    private void GroundMovement()
    {
        if (isHanging)
            return;
        /*if(gameManager.CanCrounch)
            if (input.crouchHeld && !isCrouching && !isJumping)
            Crouch();*/
        if (!input.crouchHeld && isCrouching)
            StandUp();
        else if (!isOnGround && isCrouching)
            StandUp();

        float xVelocity = playerStatus.Speed * input.horizontal;

        if (xVelocity * direction < 0.0f)
            FlipCharacterDirection();

        if (isCrouching)
            xVelocity /= playerStatus.CrouchSpeedDivisor;

        rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);

        if (isOnGround)
            coyoteTime = Time.time + playerStatus.CoyoteDuration;
    }

    private void MidAirMovement()
    {
        if (isHanging)
        {
            if (input.crouchPressed)
            {
                isHanging = false;
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                return;
            }

            if (input.jumpPressed)
            {
                isHanging = false;
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                rigidBody.AddForce(new Vector2(0.0f, playerStatus.HangingJumpForce), ForceMode2D.Impulse);
                return;
            }
        }

        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time))
        {

            if (isCrouching && !isHeadBlocked)
            {
                StandUp();
                rigidBody.AddForce(new Vector2(0.0f, playerStatus.CrouchJumpBoost), ForceMode2D.Impulse);
            }

            isJumping = true;
            isOnGround = false;

            jumpTime = playerStatus.JumpHoldDuration + Time.time;

            rigidBody.AddForce(new Vector2(0.0f, playerStatus.JumpForce), ForceMode2D.Impulse);
        }
        else if (isJumping)
        {
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0.0f, playerStatus.JumpHoldForce), ForceMode2D.Impulse);

            if (jumpTime <= Time.time)
                isJumping = false;
        }

        if (rigidBody.velocity.y < playerStatus.MaxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, playerStatus.MaxFallSpeed);

    }
    #endregion

    #region STANDUP CROUNCH
    void Crouch()
    {
        isCrouching = true;

        bodyCollider.size = colliderCrouchSize;
        bodyCollider.offset = colliderCrouchOffset;
        grabOffset /= 2;
    }

    void StandUp()
    {
        if (isHeadBlocked)
            return;

        isCrouching = false;

        bodyCollider.size = colliderStandSize;
        bodyCollider.offset = colliderStandOffset;
        grabOffset *= 2;

    }
    #endregion

    #region PHYSICS CHECK
    private void PhysicsCheck()
    {
        isOnGround = false;
        isHeadBlocked = false;
        canPick = false;
        canThrow = false;

        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0), Vector2.down, groundDistance);

        if (leftCheck || rightCheck)
            isOnGround = true;


        RaycastHit2D headCheck = Raycast(new Vector2(0.0f, bodyCollider.size.y), Vector2.up, headClearance);

        if (headCheck)
            isHeadBlocked = true;

        Vector2 grabDir = new Vector2(direction, 0f);

        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance);
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance);

        if (pickableObject != false && pickableObject.collider.gameObject.transform.parent == transform)
        {
            canThrow = true;
        }

        if (!isOnGround && !isHanging && rigidBody.velocity.y < 0f && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;
            pos.x += (wallCheck.distance - smallAmount) * direction;
            pos.y -= ledgeCheck.distance;
            transform.position = pos;
            rigidBody.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }
    }
    #endregion

    #region RAYCAST
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
    {
        //Call the overloaded Raycast() method using the ground layermask and return 
        //the results
        return Raycast(offset, rayDirection, length, groundLayer);
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    {
        //Record the player's position
        Vector2 pos = transform.position;

        //Send out the desired raycasr and record the result
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

        //If we want to show debug raycasts in the scene...
        if (drawDebugRaycasts)
        {
            //...determine the color based on if the raycast hit...
            Color color = hit ? Color.red : Color.green;
            //...and draw the ray in the scene view
            Debug.DrawRay(pos + offset, rayDirection * length, color);
        }

        //Return the results of the raycast
        return hit;
    }
    #endregion

    #region TRIGGER
    private void OnTriggerEnter2D(Collider2D other)
    {
        if( other.CompareTag("Trap") || other.CompareTag("Enemy"))
        {
            Debug.Log("Something touch you");
            playerStatus.Health = 0;
        }
        if (other.CompareTag("Power_Up"))
        {
            Debug.Log(other.gameObject.GetComponent<PowerUpBehaviours>().Status);
            switch (other.gameObject.GetComponent<PowerUpBehaviours>().Status)
            {
                case PowerUpBehaviours.PowerUpStatus.Coins:
                    break;

                case PowerUpBehaviours.PowerUpStatus.Poison:
                    playerStatus.Power = PlayerStatus.PlayerPower.Poisoned;
                    break;
                case PowerUpBehaviours.PowerUpStatus.Midas:
                    playerStatus.Power = PlayerStatus.PlayerPower.Midas;
                    break;
                default:
                    break;
            }
            
            Destroy(other.gameObject);
        }

    }
    #endregion

    #region COLLISION
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") && collision.gameObject.GetComponent<MovingPlatformBehaviours>() != null)
        {
             transform.parent = collision.gameObject.transform;
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
            transform.parent = null;
    }
    #endregion

    #region GIZMO
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(CheckTrap.position, 0.35f);
    }
    #endregion


}
