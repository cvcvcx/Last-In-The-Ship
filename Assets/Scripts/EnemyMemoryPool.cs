using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMemoryPool : MonoBehaviour
{
    
    public static readonly List<GameObject> enemies = new List<GameObject>();
    [SerializeField]
    private Transform target;
    [SerializeField]
    private GameObject enemySpawnPointPrefab;
    [SerializeField]
    private GameObject enemyPrefab;

    
    
    private MemoryPool spawnPointMemoryPool;
    private MemoryPool enemyMemoryPool;        
    static private int wave=0;
    public Transform[] spawnPoints;
    
    static public int Wave { get { return wave; }   set{ wave = value; } }
    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        
    }
    private void Update()
    {


        UpdateUI();
        if (enemies.Count <= 0)
        {
            PlayerHUD.Instance.SetActiveEnterStartWaveUI(true);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SpawnWave();
            }
        }
    }
    private void UpdateUI()
    {
        PlayerHUD.Instance.UpdateWaveText(wave, enemies.Count);
    }

    

    private void SpawnWave()
    {
        wave++;

        var spawnCount = Mathf.RoundToInt(wave * 5);

        for (int i = 0; i < spawnCount; i++)
        {
            StartCoroutine("SpawnEnemy");
        }
    }
    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        PlayerHUD.Instance.SetActiveEnterStartWaveUI(false);
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject item = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        
        item.GetComponent<EnemyFSM>().Setup(target.transform,this);
        enemies.Add(item);

        
    }

  
    public void DestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
        enemies.Remove(enemy);        
        GameManager.Instance.AddScore(100);
    }



    static public void DestroyAllEnemy()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);

        }            
            enemies.Clear();
    }

 

    private bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;

        var direction = target.position - transform.position;

        direction.y = transform.forward.y;

        if (Vector3.Angle(direction, transform.forward) > 50.0f * 0.5f)
        {
            return false;
        }

        if (Physics.Raycast(transform.position, direction, out hit))
        {
            if (hit.transform == target) return true;
        }

        return false;
    }

}
