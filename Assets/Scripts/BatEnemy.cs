using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BatEnemy : MonoBehaviour
{
    [SerializeField] Transform batDetectedZoneCheck;
    [SerializeField] LayerMask batDetectedZoneLayer;
    [SerializeField] float moveSpeed = 0.5f;

    Rigidbody2D batRigidBody;
    Animator animator;
    private GameObject player;
    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        batRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerDetected())
        {
            animator.SetBool("detectedPlayer", true);
            animator.SetTrigger("attackPlayer");
            Vector3 direction = player.transform.position - transform.position /*- new Vector3(1.5f, 1.5f, 0f)*/;
            direction.Normalize();
            movement = direction;

        }
        else
        {
            animator.SetBool("detectedPlayer", false);
        }
    }

    private void FixedUpdate()
    {
        FlipBatEnemy();
        MoveCharacter(movement);
    }

    bool isPlayerDetected()
    {
        return (player) ? Physics2D.OverlapCircle(batDetectedZoneCheck.position, 0.02f, batDetectedZoneLayer) : false;
    }

    void MoveCharacter(Vector2 direction)
    {
        batRigidBody.MovePosition((Vector2)transform.position + (direction * moveSpeed * 0.5f * Time.deltaTime));
    }

    void FlipBatEnemy()
    {
        Vector2 localScale = transform.localScale;
        if (player)
        {
            if (player.transform.position.x < transform.position.x)
            {
                localScale.x = Mathf.Abs(localScale.x) * -1f;
            }
            else
            {
                localScale.x = Mathf.Abs(localScale.x);
            }
        }
        transform.localScale = localScale;
    }
}
