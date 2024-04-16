using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBreaking : MonoBehaviour
{
    [SerializeField] GameObject coin;

    Animator boxAnimator;

    private void Start()
    {
        boxAnimator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            boxAnimator.SetBool("isBreaked", true);
            StartCoroutine(createCoinAfterBreakingBox());
        }
    }

    IEnumerator createCoinAfterBreakingBox()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Instantiate(coin, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f), gameObject.transform.rotation);
        FindObjectOfType<ScenePersist>().setCoinCount(1);
        gameObject.SetActive(false);
    }

}
