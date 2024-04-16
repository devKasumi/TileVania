using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    [SerializeField] int pointForFruitPickup = 1;

    Animator fruitAnimator;

    bool wasCollected = false;

    private void Start()
    {
        fruitAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !wasCollected)
        {
            wasCollected = true;
            if (gameObject.CompareTag("Melon"))
            {
                FindObjectOfType<GameSession>().AddToMelonScore(pointForFruitPickup);
                fruitAnimator.SetBool("isCollected", true);
                StartCoroutine(CollectFruit());
            }
            else if (gameObject.CompareTag("Pineapple"))
            {
                FindObjectOfType<GameSession>().AddToPineappleScore(pointForFruitPickup);
                fruitAnimator.SetBool("isCollected", true);
                StartCoroutine(CollectFruit());
            }
        }
    }

    IEnumerator CollectFruit()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
    }
}
