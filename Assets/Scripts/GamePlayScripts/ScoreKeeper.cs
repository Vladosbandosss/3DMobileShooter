using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }

    private float lastEnemyKilledTime;
    private float streakExpTime = 1f;
    private int streakCount;

    private void Awake()
    {
        score = 0;
    }

    private void Start()
    {
        Enemy.onDeadStatic += OnEnemyKilled;
        FindObjectOfType<PlayerScripts>().onDead += OnPlayerDead;
    }

    private void OnEnemyKilled()
    {
        if (Time.time > lastEnemyKilledTime + streakExpTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }

        lastEnemyKilledTime = Time.time;

        score += 5 + Random.Range(2, 5) * streakCount;
    }

    private void OnPlayerDead()
    {
        Enemy.onDeadStatic -= OnEnemyKilled;
    }
}
