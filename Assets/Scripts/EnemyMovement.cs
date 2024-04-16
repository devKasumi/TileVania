using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody2D enemyRb;
    [SerializeField] bool isFlip;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyRb.velocity = new Vector2(isFlip ? -moveSpeed : moveSpeed, 0f);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        moveSpeed = -moveSpeed;
        FlipEnemyFacing();  
    }

    void FlipEnemyFacing()
    {
        float flipRotation = -Mathf.Sign(enemyRb.velocity.x);
        transform.localScale = new Vector2(flipRotation, 1f);
    }
}
