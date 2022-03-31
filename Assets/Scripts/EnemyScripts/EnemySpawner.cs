using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public float minSpeed;
        public float maxSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;

        public Color skinColor;
    }

    public event Action<int> onNewWave;

    public Wave[] waves;
    public Enemy enemy;

    private LivingEntity playerEntity;
    private GameObject playerT;

    private Wave currentWave;
    private int currentWaveNumber;
    private int enemyRemainSpawn;
    private int enemiesRemAlive;
    private float nextSpawnTime;

    [SerializeField]private Transform[] spawnPoints;
    [SerializeField] private GameObject spawnIndicator;

    private float timeBetweenCampingChecs = 2f;
    private float campTresholdDistance = 1.5f;
    private float nextCampCheckTime;
    private Vector3 campPositionOld;
    private bool isCamping;

    private bool isDisabled;

    [SerializeField] private AudioClip levelCompleted;

    private void Start()
    {
        playerEntity = GameObject.FindWithTag(TagManager.PLAYERTAG).GetComponent<PlayerScripts>();
        playerT = playerEntity.gameObject;

        nextCampCheckTime = timeBetweenCampingChecs + Time.time;
        campPositionOld = playerT.transform.position;
        playerEntity.onDead += OnPlayerDead;
        
        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecs;

                isCamping = Vector3.Distance
                    (playerT.transform.position, campPositionOld) < campTresholdDistance;

                campPositionOld = playerT.transform.position;
            }

            if ((enemyRemainSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemyRemainSpawn--;

                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(nameof(SpawnEnemy));
            }
        }
    }

    private IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1f;
        float spawnTimer = 0f;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (isCamping)
        {
            spawnPoint = playerT.transform;
        }

        Vector3 temp = spawnPoint.position;
            temp.y = spawnIndicator.transform.position.y;
            spawnIndicator.transform.position = temp;

            while (spawnTimer < spawnDelay)
            {
                spawnIndicator.SetActive(!spawnIndicator.activeInHierarchy);
                spawnTimer += Time.deltaTime;

                yield return null;
            }
            
        spawnIndicator.SetActive(false);

        Enemy spawnedEnemy = Instantiate(enemy, spawnPoint.position + Vector3.up, Quaternion.identity);

        spawnedEnemy.onDead += OnEnemyDead;
        
        spawnedEnemy.SetCharacteristic
            (currentWave.moveSpeed,currentWave.hitsToKillPlayer,currentWave.enemyHealth,currentWave.skinColor);
    }
    
    private void OnPlayerDead()
    {
        StopCoroutine(nameof(SpawnEnemy));
        isDisabled = true;
    }

    private void OnEnemyDead()
    {
        enemiesRemAlive--;

        if (enemiesRemAlive == 0)
        {
            NextWave();
        }
    }
    
    private void NextWave()
    {
        if (currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound(levelCompleted,transform.position);
        }

        currentWaveNumber++;

        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemyRemainSpawn = currentWave.enemyCount;
            enemiesRemAlive = enemyRemainSpawn;

            if (onNewWave != null)
            {
                onNewWave(currentWaveNumber);
            }
        }
    }
}
