using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameSession gameSession = FindObjectOfType<GameSession>();
            if (gameSession.playerLives > 0 && gameSession.playerLives < 5)
            {
                gameSession.GainHeart();
                Destroy(gameObject);
            }
        }
    }
}
