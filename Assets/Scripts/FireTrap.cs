using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    Animator animator;
    [SerializeField] float fireTimer = 10f;
    GameObject player;
    CapsuleCollider2D fireTop;
    private float originTimer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        fireTop = GetComponent<CapsuleCollider2D>();
        originTimer = fireTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Round(fireTimer) == (originTimer - 4f))
        {
            animator.SetBool("isFireGotHit", false);
            animator.SetBool("isFireOn", true);
            fireTop.enabled = true;
            gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        }
        else if (Mathf.Round(fireTimer) == 0f)
        {
            animator.SetBool("isFireOn", false);
            fireTop.enabled = false;
            gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            fireTimer = originTimer;
        }
        fireTimer -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("isFireGotHit", true);
        }
    }
}
