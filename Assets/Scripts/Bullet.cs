using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 20f;
    private Rigidbody2D playerBulletRb;
    private PlayerMovement player;
    private GameObject enemyObject;
    private float xSpeed;
    private bool isPlayerFacingRight;

    // Start is called before the first frame update
    void Start()
    {
        playerBulletRb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * bulletSpeed;
        isPlayerFacingRight = player.transform.localScale.x > 0;
        transform.localScale = new Vector2(isPlayerFacingRight ? transform.localScale.x : -transform.localScale.x, transform.localScale.y);
    }

    // Update is called once per frame
    void Update()
    {
        playerBulletRb.velocity = new Vector2(xSpeed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("BlueBirdEnemy"))
        {
            ProcessKillEnemy(collision, "isDeath");
        }
        else if (collision.CompareTag("BatEnemy"))
        {
            ProcessKillEnemy(collision, "gotHitByPlayer");
        }
        //else if (collision.CompareTag("Platforms"))
        //{
        //    Destroy(gameObject);
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    void ProcessKillEnemy(Collider2D collision, string animationCondition)
    {
        collision.gameObject.GetComponent<Animator>().SetBool(animationCondition, true);
        enemyObject = collision.gameObject;
        StartCoroutine(KillEnemy());
    }

    IEnumerator KillEnemy()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Destroy(enemyObject);
        Destroy(gameObject);
    }
}
