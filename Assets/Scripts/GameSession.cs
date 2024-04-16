using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    [SerializeField] int score = 0;
    public int playerLives = 5;
    public int melonScore = 0;
    public int pineappleScore = 0;

    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject[] hearts;
    [SerializeField] Sprite defaultHeart;
    [SerializeField] Sprite deathHeart;

    [SerializeField] GameObject infoButton;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject playButton;
    [SerializeField] Canvas menuCanvas;
    [SerializeField] Canvas scoreCanvas;
    [SerializeField] Canvas infoCanvas;
    [SerializeField] Canvas settingCanvas;
    [SerializeField] Canvas abilityPopup;
    [SerializeField] Canvas winingCanvas;
    [SerializeField] TextMeshProUGUI congratzPopupText;

    public bool isCheckpointChecked = false;
    public bool endPointChecked;

    //private bool isGameStarted = false;
    private bool isContinue;
    private bool isPlayingBGMusic = true;

    // Start is called before the first frame update
    void Awake() // singleton
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;  
        if (numGameSessions > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);    
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = score.ToString();
        isContinue = false;
    }

    private void Update()
    {
        if (isContinue)
        {
            continueButton.SetActive(true);
        }

        if (endPointChecked)
        {
            StartCoroutine(WiningPopupTimer());
            winingCanvas.gameObject.SetActive(true);
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();
    }

    public void AddToMelonScore(int melonPoints)
    {
        melonScore += melonPoints;
        if (melonScore == 15)
        {
            showPopup("Congratulations, you gain a new ability, now you can dash by double click A or D.");
        }
    }

    public void AddToPineappleScore(int pineapplePoints)
    {
        pineappleScore += pineapplePoints;
        if (pineappleScore == 10)
        {
            showPopup("Congratulations, you gain a new ability, now you can double jump by pressing space twice."); 
        }
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            hearts[playerLives - 1].GetComponent<UnityEngine.UI.Image>().sprite = deathHeart;
            StartCoroutine(TakeLife());
        }
        else
        {
            restoreHeart();
            StartCoroutine(ResetGameSession());
        }
    }

    IEnumerator TakeLife()
    {
        yield return new WaitForSecondsRealtime(1f);
        playerLives--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        livesText.text = playerLives.ToString();
    }

    IEnumerator ResetGameSession()
    {
        endPointChecked = false;
        yield return new WaitForSecondsRealtime(1f);
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    void restoreHeart()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<UnityEngine.UI.Image>().sprite = defaultHeart;
        }
    }

    public void GainHeart()
    {
        playerLives++;
        hearts[playerLives - 1].GetComponent<UnityEngine.UI.Image>().sprite = defaultHeart;
        livesText.text = playerLives.ToString();
    }

    public void OnPlayButtonClicked()
    {
        menuCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(true);
        infoCanvas.gameObject.SetActive(false);
        settingCanvas.gameObject.SetActive(false);
        isContinue = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnContinueButtonClicked()
    {
        menuCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(true);
        infoCanvas.gameObject.SetActive(false);
        settingCanvas.gameObject.SetActive(false);
    }

    public void OnInfoButtonClicked()
    {
        menuCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(false);
        infoCanvas.gameObject.SetActive(true);
        settingCanvas.gameObject.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        if (infoCanvas.gameObject.activeInHierarchy)
        {
            menuCanvas.gameObject.SetActive(true);
            scoreCanvas.gameObject.SetActive(false);
            infoCanvas.gameObject.SetActive(false);
            settingCanvas.gameObject.SetActive(false);
        }
        else
        {
            menuCanvas.gameObject.SetActive(false);
            scoreCanvas.gameObject.SetActive(true);
            infoCanvas.gameObject.SetActive(false);
            settingCanvas.gameObject.SetActive(false);
        }
    }

    public void OnResetLevelButtonClicked()
    {
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnSettingButtonClicked()
    {
        menuCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(false);
        infoCanvas.gameObject.SetActive(false);
        settingCanvas.gameObject.SetActive(true);
    }

    public void OnMenuButtonClicked()
    {
        menuCanvas.gameObject.SetActive(true);
        scoreCanvas.gameObject.SetActive(false);
        infoCanvas.gameObject.SetActive(false);
        settingCanvas.gameObject.SetActive(false);
    }

    public void OnVolumeButtonClicked()
    {
        if (isPlayingBGMusic)
        {
            gameObject.GetComponent<AudioSource>().mute = true;
            isPlayingBGMusic = false;
        }
        else
        {
            gameObject.GetComponent<AudioSource>().mute = false;
            isPlayingBGMusic = true;
        }
    }

    public void OnCloseButtonClicked()
    {
        menuCanvas.gameObject.SetActive(false);
        scoreCanvas.gameObject.SetActive(false);
        infoCanvas.gameObject.SetActive(false);
        settingCanvas.gameObject.SetActive(false);
        Application.Quit();
    }

    public void OnReturnPopupButtonClicked()
    {
        abilityPopup.gameObject.SetActive(false);
    }

    public void OnPlayAgainButtonClicked()
    {
        winingCanvas.gameObject.SetActive(false);
        StartCoroutine(ResetGameSession());
    }

    void showPopup(string data)
    {
        congratzPopupText.text = data;
        abilityPopup.gameObject.SetActive(true);
    }

    IEnumerator WiningPopupTimer()
    {
        yield return new WaitForSecondsRealtime(0.5f);
    }
}
