using UnityEngine;
using System.Collections;

/// <summary>
/// 敌人波次生成器（支持多波次、不同难度）
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    [Header("敌人预制体")]
    public GameObject enemyPrefab;
    public GameObject[] enemyPrefabs; // 多种敌人类型
    
    [Header("出生点")]
    public Transform spawnPoint;
    public Transform[] spawnPoints; // 多个出生点
    
    [Header("波次设置")]
    public int enemiesPerWave = 5;
    public float spawnInterval = 2f;
    public float waveDelay = 5f; // 波次间隔
    
    private int currentWave = 0;
    private int enemiesSpawned = 0;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 开始刷怪
    /// </summary>
    public void StartWave()
    {
        if (!isSpawning)
        {
            currentWave = 0;
            StartNextWave();
        }
    }

    /// <summary>
    /// 停止刷怪
    /// </summary>
    public void StopWave()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        CancelInvoke(nameof(SpawnEnemy));
    }

    /// <summary>
    /// 开始下一波
    /// </summary>
    public void StartNextWave()
    {
        currentWave++;
        enemiesSpawned = 0;
        isSpawning = true;
        
        Debug.Log($"第 {currentWave} 波开始！");
        
        // 增加难度
        int enemyCount = enemiesPerWave + currentWave * 2;
        float interval = Mathf.Max(0.5f, spawnInterval - currentWave * 0.1f);
        
        spawnCoroutine = StartCoroutine(SpawnWave(enemyCount, interval));
    }

    /// <summary>
    /// 刷怪协程
    /// </summary>
    private IEnumerator SpawnWave(int enemyCount, float interval)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (!isSpawning) yield break;
            
            SpawnEnemy();
            enemiesSpawned++;
            
            yield return new WaitForSeconds(interval);
        }
        
        // 等待所有敌人被消灭后再开始下一波
        yield return new WaitForSeconds(waveDelay);
        
        if (isSpawning)
        {
            GameManager.Instance?.OnWaveComplete();
            StartNextWave();
        }
    }

    /// <summary>
    /// 生成单个敌人
    /// </summary>
    void SpawnEnemy()
    {
        if (enemyPrefab == null && (enemyPrefabs == null || enemyPrefabs.Length == 0))
        {
            Debug.LogError("没有设置敌人预制体！");
            return;
        }
        
        Transform spawnPos = GetSpawnPoint();
        if (spawnPos == null)
        {
            Debug.LogError("没有设置出生点！");
            return;
        }

        GameObject prefabToSpawn = GetEnemyPrefab();
        Instantiate(prefabToSpawn, spawnPos.position, Quaternion.identity);
    }

    /// <summary>
    /// 获取出生点
    /// </summary>
    Transform GetSpawnPoint()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        return spawnPoint;
    }

    /// <summary>
    /// 获取敌人预制体
    /// </summary>
    GameObject GetEnemyPrefab()
    {
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            // 根据波次选择更强的敌人
            int index = Mathf.Min(currentWave / 3, enemyPrefabs.Length - 1);
            return enemyPrefabs[index];
        }
        return enemyPrefab;
    }
}
