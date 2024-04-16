using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    Rigidbody2D commonEnemyRb;
    CapsuleCollider2D monsterHead;
    Animator animator;
    private GameObject player;

    public bool isFacingRight;

    // Start is called before the first frame update
    void Start()
    {
        commonEnemyRb = GetComponent<Rigidbody2D>();
        monsterHead = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        commonEnemyRb.velocity = new Vector2(moveSpeed, 0f);
        EnemyDie();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !collision.CompareTag("Enemy"))
        {
            moveSpeed = -moveSpeed;
            FlipEnemyFacing();
        }
    }

    void FlipEnemyFacing()
    {
        float flipRotation = (isFacingRight) ? -Mathf.Sign(commonEnemyRb.velocity.x) : Mathf.Sign(commonEnemyRb.velocity.x);
        transform.localScale = new Vector2(flipRotation, 1f);
    }

    void EnemyDie()
    {
        if (monsterHead.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            commonEnemyRb.velocity = new Vector2(0f, 0f);
            animator.SetBool("isDeath", true);
            monsterHead.enabled = false;
            gameObject.GetComponent<PolygonCollider2D>().enabled = false;   
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(GotKilledByPlayer());
        }
    }

    IEnumerator GotKilledByPlayer()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Destroy(gameObject);
    }
}
