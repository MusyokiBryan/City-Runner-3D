using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE = 5;
    public static GameManager Instance { set; get; }

    private bool isGameStarted = false;
    public bool IsDead { set; get; }
    private PlayerMotor motor;

    // UI and the UI fields
    public Animator gameCanvas, menuAnim, diamondAnim;
    public TextMeshProUGUI scoreText, coinText, modifierText, highScoreText;
    private float score, coinScore, modifierScore;
    private int lastScore;

    //Death Menu
    public Animator deathMenuAnim;
    public TextMeshProUGUI deadScoreText, deadCoinText;

    private void Awake()
    {
        Instance = this;
        modifierScore = 1;
        //UpdateScores();
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        modifierText.text = "x" + modifierScore.ToString("0.0");
        coinText.text = coinScore.ToString("0");
        scoreText.text = scoreText.text = score.ToString("0");
        highScoreText.text = PlayerPrefs.GetInt("Hiscore").ToString();
    }

    private void Update()
    {

        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            motor.StartRunning();
            FindObjectOfType<EnvironmentSpawner>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;
            gameCanvas.SetTrigger("Show");
            menuAnim.SetTrigger("Hide");
            
        }

        if (isGameStarted && !IsDead)
        {
            //Score increment
            score += (Time.deltaTime * modifierScore);
            if (lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
        }
    }

    public void GetCoin()
    {
        diamondAnim.SetTrigger("Collect");
        coinScore++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE;
        scoreText.text = scoreText.text = score.ToString("0");
    }
    public void UpdateModifier(float modifierAmmount)
    {
        modifierScore = 1.0f + modifierAmmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }
    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
    }
    public void OnDeath()
    {
        IsDead = true;
        FindObjectOfType<EnvironmentSpawner>().IsScrolling = false;
        deadScoreText.text = score.ToString("0");
        deadCoinText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        gameCanvas.SetTrigger("Hide");

        //check for highscore
        if(score > PlayerPrefs.GetInt("Hiscore"))
        {
            float s = score;
            if(s % 1 ==0)
            s +=1;
            PlayerPrefs.SetInt("Hiscore", (int)s);
        }
    }
}
