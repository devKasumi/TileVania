using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurtleEnemy : MonoBehaviour
{
    Animator animator;
    BoxCollider2D turtleHead;
    PolygonCollider2D turtleBody;
    private float timer = 13f;
    private float tmpTimer;
    PlayerMovement player;
    private bool canBeKilledWithPlayerBullet;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        turtleHead = GetComponent<BoxCollider2D>();
        turtleBody = GetComponent<PolygonCollider2D>();
        player = GameObject.FindObjectOfType<PlayerMovement>();
        tmpTimer = timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Round(timer) == /*7f*/(tmpTimer - 6f))
        {
            //Debug.Log("timer 15f");
            animator.SetBool("spikesIn", true);
            animator.SetBool("spikesIdle", false);
            turtleHead.enabled = true;
            turtleBody.enabled = false;
            canBeKilledWithPlayerBullet = true;
        }
        if (Mathf.Round(timer) == /*6f*/(tmpTimer - 7f))
        {
            //Debug.Log("timer 10f");
            animator.SetBool("noSpikesIdle", true);
            animator.SetBool("spikesIn", false); 
        }
        if (Mathf.Round(timer) == /*2f*/(tmpTimer - 11f))
        {
            //Debug.Log("timer 5f");
            animator.SetBool("spikesOut", true);
            animator.SetBool("noSpikesIdle", false);
            turtleHead.enabled = false;
            turtleBody.enabled = true;
            canBeKilledWithPlayerBullet = false;
        }
        if (Mathf.Round(timer) == /*0f*/(tmpTimer - 13f))
        {
            //Debug.Log("timer 0f");
            animator.SetBool("spikesIdle", true);
            animator.SetBool("spikesOut", false);
            timer = 20f;
        }
        timer -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (turtleHead.IsTouchingLayers(LayerMask.GetMask("Player")))
        { 
            animator.SetBool("isDeath", true);
            StartCoroutine(TurtleEnemyGotKill());
        }
        if (turtleBody.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            player.GetComponent<Animator>().SetBool("isRunning", false);
            player.GetComponent<Animator>().SetTrigger("Death");
            player.GetComponent<Animator>().SetTrigger("Dying");
            player.PlayerGotPushedBack();
            StartCoroutine(DeathTimer());
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (canBeKilledWithPlayerBullet)
            {
                animator.SetBool("isDeath", true);
                StartCoroutine(TurtleEnemyGotKill());
            }
        }
    }

    IEnumerator TurtleEnemyGotKill()
    {
        turtleHead.enabled = false;
        turtleBody.enabled = false;
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    IEnumerator DeathTimer()
    {
        player.GetComponent<BoxCollider2D>().enabled = false;
        player.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(player.gameObject);
    }
}
