using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float force = 10f;
    Rigidbody2D enemyBulletRb;
    private GameObject player;
    //Collider2D playerCollier;
    //GameObject plantMonster;

    //private float timer;

    // Start is called before the first frame update
    void Start()
    {
        enemyBulletRb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;

        enemyBulletRb.velocity = new Vector2(direction.x, direction.y).normalized * force;

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
