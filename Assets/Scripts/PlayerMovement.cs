using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 16f;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] float jumpingPower = 1f;
    [SerializeField] Vector2 deathKick;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] GameObject checkpoint;
    [SerializeField] CompositeCollider2D tileMapCollider; 

    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Rigidbody2D playerRb;
    private Animator myAnimator;
    private CapsuleCollider2D myBodyCollider;
    private BoxCollider2D myFeetCollider;
    private Color myColor;
    private TrailRenderer trailRenderer;
    private GameSession gameSession;
    private Vector2 checkpointPos;

    private float horizontal;

    private float gravityScaleAtStart;
    private bool isAlive = true;
    private bool doubleJump;

    private bool isClimbing;

    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower = 20f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    private float doubleClick = 0.2f;
    private float lastClickTime;

    public int enterLadder = 0;

    public bool isTouchingGround;

    //private bool isDeathByTouchingEnemy = false;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = playerRb.gravityScale;
        myColor = gameObject.GetComponent<Renderer>().material.color;
        trailRenderer = GetComponent<TrailRenderer>();
        gameSession = FindObjectOfType<GameSession>();
        checkpointPos = checkpoint.transform.position;
    }

    void Awake()
    {
        RespawnPlayerAtCheckpoint();
        playerControls = new PlayerControls();
        playerControls.Player.Fire.canceled += ctx => StopFiring();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive || isDashing) return;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
        enterWater();
        if (!isClimbing)
        {
            Dashing();
        }
    }

    void OnMove(InputValue value)
    {
        if (!isAlive || isDashing) return;
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) return;

        if (isGrounded() && !value.isPressed)
        {
            doubleJump = false;
        }

        if (value.isPressed)
        {
            //myRigidbody.velocity += new Vector2(0f, jumpSpeed);
            if (isGrounded() || doubleJump)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, jumpSpeed);
                if (FindObjectOfType<GameSession>().pineappleScore >= 10)
                {
                    doubleJump = !doubleJump;
                }
            }
        }

        if (value.isPressed && playerRb.velocity.y > 0f)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y * jumpingPower);
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive || isDashing) return;
        //Debug.LogError(value.isPressed.ToString());
        myAnimator.SetBool("isShooting", true);
        Instantiate(bullet, gun.position, transform.rotation);
    }

    void StopFiring()
    {
        myAnimator.SetBool("isShooting", false);
    }

    bool isGrounded()
    {
        return myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    void Run()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        playerRb.velocity = new Vector2(horizontal * runSpeed, playerRb.velocity.y);
        bool playerHasHorizontalSpeed = Mathf.Abs(playerRb.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(playerRb.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRb.velocity.x), 1f);
        }

    }

    void ClimbLadder()
    {
        //bool isTouchingLadder = myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            isClimbing = false;
            tileMapCollider.isTrigger = false;
            playerRb.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            isTouchingGround = true;
            return;
        }

        isClimbing = true;
        Vector2 climbVelocity = new Vector2(playerRb.velocity.x, moveInput.y * climbSpeed);
        playerRb.velocity = climbVelocity;
        playerRb.gravityScale = 0f;
        bool playerHasVerticalSpeed = Mathf.Abs(playerRb.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
        tileMapCollider.isTrigger = true;
        isTouchingGround = false;
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            playerRb.velocity = deathKick;
            myBodyCollider.enabled = false;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
        //RespawnPlayerAtCheckpoint();
    }

    void enterWater()
    {
        gameObject.GetComponent<Renderer>().material.color = (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Water"))) ? new Color(0f,0f,0f,0.5f) : myColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platforms") && !isAlive)
        {
            myAnimator.SetTrigger("Death");
            playerRb.velocity = new Vector2(0f, 0f);
        }
        if (collision.gameObject.CompareTag("SpikeHead"))
        {
            isAlive = false;
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetTrigger("Death");
            StartCoroutine(DeathTimer());
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
        if (collision.gameObject.CompareTag("Ladder"))
        {
            isTouchingGround = false;
            enterLadder++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            Debug.Log("enemy bullettt !!!!!!!!!");
            isAlive = false;
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetTrigger("Death");
            StartCoroutine(DeathTimer());
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
        if (collision.CompareTag("Checkpoint"))
        {
            Debug.Log("checkedddd point!!!!");
            checkpointPos = checkpoint.transform.position;
            gameSession.isCheckpointChecked = true;
        }
        if (collision.CompareTag("Ladder"))
        {
            isTouchingGround = false;
            enterLadder++;
        }
        if (collision.CompareTag("FireTrap"))
        {
            isAlive = false;
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetTrigger("Death");
            StartCoroutine(DeathTimer());
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
        if (collision.CompareTag("EndPoint"))
        {
            FindObjectOfType<GameSession>().endPointChecked = true;
        }
    }

    void Dashing()
    {
        if (FindObjectOfType<GameSession>().melonScore >= 15)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) && canDash)
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                if (timeSinceLastClick <= doubleClick)
                {
                    StartCoroutine(Dash());
                }

                lastClickTime = Time.time;
            }
        } 
    }

    public void PlayerGotPushedBack()
    {
        playerRb.velocity = deathKick;
    }

    IEnumerator Dash()
    {
        canDash = true;
        isDashing = true;
        float originalGravity = playerRb.gravityScale;
        playerRb.gravityScale = 0f;
        playerRb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        playerRb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Destroy(gameObject);
    }

    void RespawnPlayerAtCheckpoint()
    {
        checkpointPos = checkpoint.transform.position;
        gameSession = FindObjectOfType<GameSession>();
        if (gameSession.isCheckpointChecked)
        {
            transform.position = checkpointPos;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
