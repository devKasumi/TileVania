using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBird : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] int startingPoint;
    [SerializeField] bool isFlip;
    [SerializeField] Transform[] points;

    Animator animator;

    private int count = 0;

    //private Rigidbody2D blueBirdRb;

    private int i;
    // Start is called before the first frame update
    void Start()
    {
        //blueBirdRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        BlueBirdMoving();
    }

    void BlueBirdMoving()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            if (i == 0 && count > 0)
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
            i++;
            if (i == points.Length)
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                i = 0; 
            }
            count++;
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("isDeath", true);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(GotKilledByPlayer());
        }
    }

    IEnumerator GotKilledByPlayer()
    {
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
    }
}
