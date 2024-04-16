using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryPigMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] bool isFlip;
    [SerializeField] Transform detectedZoneCheck;
    [SerializeField] LayerMask detectedZoneLayer;
    [SerializeField] bool playerDetected;

    Rigidbody2D enemyRb;
    CapsuleCollider2D monsterHead;
    GameObject player;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        monsterHead = GetComponent<CapsuleCollider2D>();    
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyRb.velocity = new Vector2(isFlip ? -moveSpeed : moveSpeed, 0f);
        if (isPlayerDetected() && player && player.GetComponent<PlayerMovement>().isTouchingGround)
        {
            if (moveSpeed > 0f) moveSpeed = 7f;
            else moveSpeed = -7f;
            animator.SetBool("detectedPlayer", true);
        }
        else
        {
            if (moveSpeed > 0f) moveSpeed = 2f;
            else moveSpeed = -2f;
            animator.SetBool("isWalking", true);
            animator.SetBool("detectedPlayer", false);
        }
        EnemyDie();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AngryPigHitWall());
                moveSpeed = -moveSpeed;
                FlipEnemyFacing();
            }
        }
    }

    void FlipEnemyFacing()
    {
        float flipRotation = (isFlip) ? -Mathf.Sign(enemyRb.velocity.x) : Mathf.Sign(enemyRb.velocity.x);
        transform.localScale = new Vector2(flipRotation, 1f);
    }

    bool isPlayerDetected()
    {
        playerDetected = Physics2D.OverlapCircle(detectedZoneCheck.position, 0.02f, detectedZoneLayer);
        if (Physics2D.OverlapCircle(detectedZoneCheck.position, 0.02f, detectedZoneLayer) && player) player.GetComponent<PlayerMovement>().enterLadder = 0;
        return player ? Physics2D.OverlapCircle(detectedZoneCheck.position, 0.02f, detectedZoneLayer) : false;
    }

    void AngryPigEnemyMove()
    {
        animator.SetBool("detectedPlayer", true);
        enemyRb.velocity = new Vector2(moveSpeed, 0f);
    }

    void EnemyDie()
    {
        if (monsterHead.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            enemyRb.velocity = new Vector2(0f, 0f);
            animator.SetBool("isDeath", true);
            monsterHead.enabled = false;
            gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(GotKilledByPlayer());
        }
    }

    IEnumerator GotKilledByPlayer()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
    }

    IEnumerator AngryPigHitWall()
    {
        animator.SetBool("isHitWall", true);
        yield return new WaitForSecondsRealtime(0.25f);
        animator.SetBool("isHitWall", false);
    }
}
