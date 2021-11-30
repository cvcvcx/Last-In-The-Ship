using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMemoryPool : MonoBehaviour
{
    private readonly List<GameObject> enemies = new List<GameObject>();
    [SerializeField]
    private Transform target;
    [SerializeField]
    private GameObject enemySpawnPointPrefab;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float enemySpawnTime = 1;
    [SerializeField]
    private float enemySpawnLatency = 1;
    
    
    private MemoryPool spawnPointMemoryPool;
    private MemoryPool enemyMemoryPool;

    private int numberOfEnemiesSpawnedAtOnce = 1;
    private int wave=0;
    public Transform[] spawnPoints;
    
    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        
    }
    private void Update()
    {


        if (enemies.Count <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        wave++;

        var spawnCount = Mathf.RoundToInt(wave * 5);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject item = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        
        item.GetComponent<EnemyFSM>().Setup(target.transform,this);
        enemies.Add(item);

        
    }
    public void DestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
        enemies.Remove(enemy);
    }
   
}
