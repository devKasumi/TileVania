using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AIMonsterMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] Transform detectedZoneCheck;
    [SerializeField] LayerMask detectedZoneLayer;
    [SerializeField] bool isStaticEnemy;
    [SerializeField] bool isRinoEnemy;
    [SerializeField] bool isFatBirdEnemy;
    [SerializeField] bool isCannonEnemy;
    [SerializeField] bool playerDetected;

    Rigidbody2D AIMonsterRb;
    CapsuleCollider2D monsterHead;
    BoxCollider2D plantHead;
    Animator animator;
    private GameObject player;
    Vector2 fatBirdOriginalPosition;
    //BoxCollider2D enemyBoxCollider;

    public bool isFacingRight;
    private float facingDirection;

    private float timer;
    private bool isRinoHitWall = false;
    private int rinoMoveCount = 0;
    //private float fatBirdBackSpeed = 3f;

    private bool isFatBirdHitGround = false;
    private bool isFatBirdMoveBack = false;
    private int fatBirdMoveCount = 0;
    private bool isFatBirdBack = true;

    // Start is called before the first frame update
    void Start()
    {
        AIMonsterRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        facingDirection = (isFacingRight) ? 1f : -1f;
        player = GameObject.FindGameObjectWithTag("Player");
        if (isRinoEnemy || isFatBirdEnemy) monsterHead = GetComponent<CapsuleCollider2D>();
        if (isStaticEnemy) plantHead = GetComponent<BoxCollider2D>();
        if (isFatBirdEnemy)
        {
            AIMonsterRb.velocity = new Vector2(0f, 0f);
            fatBirdOriginalPosition = gameObject.transform.position;
            Debug.Log("origin position:  " + gameObject.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isStaticEnemy)
        {
            //AIMonsterRb.velocity = new Vector2(0f, 0f);
            if (isPlayerDetected())
            {
                animator.SetBool("attackingPlayer", true);
                timer += Time.deltaTime;
                if (timer > 0.5f)
                {
                    timer = 0;
                    EnemyShooting();
                }
            }
            else
            {
                animator.SetBool("attackingPlayer", false);
            }
            PlantEnemyDie();
        }
        else if (isRinoEnemy)
        {
            if (isPlayerDetected())
            {
                //Debug.Log("rino detected player!!!!!");
                //if (isRinoHitWall)
                //{
                //    AIMonsterRb.velocity = new Vector2(0f, 0f);
                //}
                //else RinoEnemyMove();
                RinoEnemyMove();
            }
            else
            {
                if (isRinoHitWall)
                {
                    //Debug.Log("rino hit wall!!!!");
                    AIMonsterRb.velocity = new Vector2(0f, 0f);
                    rinoMoveCount = 0;
                    animator.SetBool("isDetectedPlayer", false);
                }
                else
                {
                    //Debug.Log("rino do not hit walllll !!!!");
                    if (rinoMoveCount > 0) RinoEnemyMove();
                }
            }
            EnemyDie();
        }
        else if (isFatBirdEnemy)
        {
            if (isPlayerDetected() && isFatBirdBack)
            {
                //if (moveSpeed > 0F) moveSpeed = 7f;
                //else moveSpeed = -7f;
                //isFatBirdBack = false;
                //animator.SetBool("detectedPlayer", true);
                //if (isFatBirdHitGround)
                //{
                //    AIMonsterRb.velocity = new Vector2(0f, 0f);
                //}
                //else FatBirdEnemyAttackPlayer();
                FatBirdEnemyAttackPlayer();
            }
            else
            {

                if (isFatBirdHitGround)
                {
                    AIMonsterRb.velocity = new Vector2(0f, 0f);
                    animator.SetBool("hitGround", true);
                    fatBirdMoveCount = 0;
                    isFatBirdMoveBack = true;
                    isFatBirdHitGround = false;
                    isFatBirdBack = false;
                }
                else
                {
                    if (fatBirdMoveCount > 0 /*&& !isFatBirdHitGround*/)
                    {
                        FatBirdEnemyAttackPlayer();
                    }

                    if (isFatBirdMoveBack)
                    {
                        Debug.Log("fat bird move back!!!!!!dasdasd");
                        animator.SetBool("hitGround", false);
                        animator.SetBool("detectedPlayer", false);
                        AIMonsterRb.velocity = new Vector2(0f, moveSpeed);
                        //AIMonsterRb.velocity = new Vector2(0f, fatBirdBackSpeed);
                        if (Vector2.Distance(transform.position, fatBirdOriginalPosition) < 0.02f)
                        {
                            Debug.Log("stopppdpdp");
                            AIMonsterRb.velocity = new Vector2(0f, 0f);
                            isFatBirdMoveBack = false;
                            isFatBirdBack = true;
                            animator.SetBool("detectedPlayer", false);
                        }

                    }

                }               
            }
            EnemyDie();
        }
        else
        {
            Move();
            if (isPlayerDetected())
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", true);
                AIMonsterRb.velocity = new Vector2(0f, 0f);

                timer += Time.deltaTime * 0.5f;
                if (timer > 0.5f)
                {
                    timer = 0;
                    EnemyShooting();
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && !isFatBirdEnemy/*&& !collision.CompareTag("Bullet")*/)
        {
            if (gameObject.activeInHierarchy)
            {
                if (isRinoEnemy)
                {
                    StartCoroutine(RinoHitWall());
                }
            }
            if (isRinoEnemy)
            {
                if (player)
                {
                    if (player.GetComponent<PlayerMovement>().enterLadder == 0)
                    {
                        FlipEnemyFacing();
                        moveSpeed = -moveSpeed;
                        facingDirection = -facingDirection;
                    }
                }           
            }
            else
            {
                moveSpeed = -moveSpeed;
                FlipEnemyFacing();
                facingDirection = -facingDirection;
                //if (player.GetComponent<PlayerMovement>().enterLadder == 0)
                //{
                //    FlipEnemyFacing();
                //    moveSpeed = -moveSpeed;
                //    facingDirection = -facingDirection;
                //}
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFatBirdEnemy)
        {
            if (collision.CompareTag("Platforms"))
            {
                isFatBirdHitGround = true;
                animator.SetBool("hitGround", true);
            }
        }
        if (isCannonEnemy)
        {
            if (collision.CompareTag("Bullet"))
            {
                animator.SetBool("isDeath", true);
                AIMonsterRb.velocity = new Vector2(0f, 0f);
                gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                StartCoroutine(CannonEnemyGotKilled());
                Destroy(collision.gameObject);
            }
        }
    }

    void FlipEnemyFacing()
    {
        Debug.Log("Rino turn around !!!!!");
        float flipRotation = (isFacingRight) ? -Mathf.Sign(AIMonsterRb.velocity.x) : Mathf.Sign(AIMonsterRb.velocity.x);
        transform.localScale = new Vector2(flipRotation, 1f);
    }

    void EnemyShooting()
    {
        if (gameObject.activeInHierarchy)
        {
            Instantiate(bullet, gun.position, Quaternion.identity);
        }
    }

    void Move()
    {
        animator.SetBool("isRunning", true);
        animator.SetBool("isAttacking", false);
        AIMonsterRb.velocity = new Vector2(moveSpeed, 0f);
    }

    void RinoEnemyMove()
    {
        //isRinoHitWall = false;
        animator.SetBool("isDetectedPlayer", true);
        AIMonsterRb.velocity = new Vector2(moveSpeed, 0f);
        rinoMoveCount++;
    }

    void FatBirdEnemyAttackPlayer()
    {
        animator.SetBool("detectedPlayer", true);
        AIMonsterRb.velocity = new Vector2(0f, -moveSpeed);
        fatBirdMoveCount++;
    }

    bool isPlayerDetected()
    {
        if (Physics2D.OverlapCircle(detectedZoneCheck.position, 0.02f, detectedZoneLayer) && isRinoEnemy && player) player.GetComponent<PlayerMovement>().enterLadder = 0;
        return player ? Physics2D.OverlapCircle(detectedZoneCheck.position, 0.02f, detectedZoneLayer) : false;
    }

    void EnemyDie()
    {
        if (monsterHead.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            AIMonsterRb.velocity = new Vector2(0f, 0f);
            animator.SetBool("isDeath", true);
            StartCoroutine(GotKilledByPlayer());
        }
    }

    void PlantEnemyDie()
    {
        if (plantHead.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            animator.SetBool("isGotHitByPlayer", true);
            StartCoroutine(GotKilledByPlayer());
        }
    }

    IEnumerator GotKilledByPlayer()
    {
        if (isRinoEnemy || isFatBirdEnemy) gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        else gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);    
    }

    IEnumerator RinoHitWall()
    {
        isRinoHitWall = true;   
        animator.SetBool("isHitWall", true);
        yield return new WaitForSecondsRealtime(1f);
        isRinoHitWall = false;
        animator.SetBool("isHitWall", false);
    }

    IEnumerator CannonEnemyGotKilled()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
    }
}
