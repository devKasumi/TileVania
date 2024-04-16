using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersist : MonoBehaviour
{
    ///*[SerializeField]*/ private GameObject[] coins;
    private GameObject[] coins;
    private GameObject exitLevel;
    //private List<GameObject> coins = new List<GameObject>();

    private int coinCount = 0;

    private void Start()
    {
        coins = GameObject.FindGameObjectsWithTag("Coin");
        //exitLevel = GameObject.FindGameObjectWithTag("Exit");
        coinCount = coins.Length;
        Debug.LogError("coin count:  " + coins.Length);
    }

    void Awake() // singleton
    {
        exitLevel = GameObject.FindGameObjectWithTag("Exit");
        int numScenePersists = FindObjectsOfType<ScenePersist>().Length;
        if (numScenePersists > 1)
        {
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void FixedUpdate()
    {
        coins = GameObject.FindGameObjectsWithTag("Coin");
        Debug.LogError("coin count:  " + coins.Length);
        if (coinCount == 0)
        {
            exitLevel.SetActive(true);
        }
        else
        {
            exitLevel.SetActive(false);
        }
    }

    private void Update()
    {
        if (coinCount == 0)
        {
            exitLevel.SetActive(true);
        }
        else
        {
            exitLevel.SetActive(false);
        }
    }

    public void ResetScenePersist()
    {
        Destroy(gameObject);
    }

    public void setCoinCount(int coin)
    {
        coinCount += coin;
    }
}
