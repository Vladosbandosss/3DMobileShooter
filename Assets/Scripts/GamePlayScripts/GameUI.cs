using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField]private Image fadePlane;
    [SerializeField]private float fadeTime = 1f;

    [SerializeField]private GameObject gameOverUI;
    [SerializeField]private RectTransform waveBanner;

    [SerializeField]private Text waveTittle;
    [SerializeField]private Text waveEnemyCount;

    [SerializeField]private float bannerSpeed;
    
    string [] wavesNumbers = {"One","Two","Three","Four","Five"};

    [SerializeField]private Text scoreUI;
    [SerializeField]private Text gameOverScoreUI, gameOverHighScoreUI;

    [SerializeField]private RectTransform healthBar;
    private float healthPercent;

    private EnemySpawner spawner;
    private PlayerScripts player;

    private string ConvertName = "D6";

    private void Awake()
    {
        spawner = FindObjectOfType<EnemySpawner>();
        spawner.onNewWave += onNewWave;

        player = FindObjectOfType<PlayerScripts>();
        player.onDead += OnGameOver;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString(ConvertName);

        SetPlayerHealth();
    }
    
    private IEnumerator AnimateWaveBunner()
    {
        float percent = 0;
        float delayTime = 1f;
        float endDelayTime = Time.time + 1 / bannerSpeed + delayTime;
        int dir = 1;

        while (percent>=0)
        {
            percent += Time.deltaTime * bannerSpeed * dir;

            if (percent >= 1)
            {
                percent = 1;

                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            waveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-420, -200, percent);

            yield return null;
        }
    }

    private void onNewWave(int waveNum)
    {
        waveTittle.text = "Wave" + wavesNumbers[waveNum - 1];

        string enemyCount = spawner.waves[waveNum - 1].infinite
            ? "Infinite"
            : spawner.waves[waveNum - 1].enemyCount.ToString();
        
       StopCoroutine(nameof(AnimateWaveBunner));
       StartCoroutine(nameof(AnimateWaveBunner));
    }

    private IEnumerator Fade(Color flor, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent<0)
        {
            percent += Time.deltaTime * speed;

            fadePlane.color = Color.Lerp(flor, to, percent);

            yield return null;
        }
    }

    private void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear,new Color(0,0,0,0.95f),fadeTime));

        gameOverScoreUI.text = "Score" + scoreUI.text;
        
        SetHighScore();
    }

    private void SetHighScore()
    {
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        
        gameOverUI.SetActive(true);

        Cursor.visible = true;

        int currScore = int.Parse(scoreUI.text);
        int hightScore = DataManager.instance.GetScore();

        if (currScore > hightScore)
        {
            DataManager.instance.SetScore(currScore);
        }

        gameOverHighScoreUI.text = "HighScore:" + DataManager.instance.GetScore().ToString(ConvertName);
    }
    
    private void SetPlayerHealth()
    {
        healthPercent = 0;

        if (player != null)
        {
            healthPercent = player.health / player.initialHealth;///!!!!!!!!!!!!!
        }

        healthBar.localScale = new Vector3(healthPercent, 1f, 1f);
    }

    public void PlayNewGame()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("GamePlay");
    }
}
