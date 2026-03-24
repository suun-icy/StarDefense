using UnityEngine;
using System.Collections;

/// <summary>
/// 敌人波次生成器（支持多波次、不同难度、倒计时）
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
    public int baseEnemiesPerWave = 5;
    public float spawnInterval = 2f;
    public float waveDelay = 5f; // 波次间隔
    public float preparationTime = 10f; // 准备时间（倒计时）
    
    [Header("难度曲线")]
    public float hpMultiplier = 1.2f; // 每波 HP 增长系数
    public float speedMultiplier = 1.05f; // 每波速度增长系数
    public int maxWaves = 10;
    
    private int currentWave = 0;
    private int enemiesSpawned = 0;
    private int enemiesToSpawn = 0;
    private bool isSpawning = false;
    private bool isPreparation = false;
    private Coroutine spawnCoroutine;
    private float preparationTimer = 0f;

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
        if (!isSpawning && !isPreparation)
        {
            currentWave = 0;
            StartPreparation();
        }
    }

    /// <summary>
    /// 停止刷怪
    /// </summary>
    public void StopWave()
    {
        isSpawning = false;
        isPreparation = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        CancelInvoke(nameof(SpawnEnemy));
    }

    /// <summary>
    /// 开始准备阶段（倒计时）
    /// </summary>
    public void StartPreparation()
    {
        isPreparation = true;
        preparationTimer = preparationTime;
        
        Debug.Log($"准备阶段开始！{preparationTime}秒后开始第 1 波");
        UIManager.Instance?.StartCountdown(preparationTime);
        
        StartCoroutine(PreparationCountdown());
    }

    /// <summary>
    /// 准备阶段倒计时协程
    /// </summary>
    private IEnumerator PreparationCountdown()
    {
        while (preparationTimer > 0)
        {
            preparationTimer -= Time.deltaTime;
            yield return null;
        }
        
        isPreparation = false;
        StartNextWave();
    }

    /// <summary>
    /// 开始下一波
    /// </summary>
    public void StartNextWave()
    {
        currentWave++;
        enemiesSpawned = 0;
        isSpawning = true;
        
        // 计算本波敌人数量和属性
        enemiesToSpawn = baseEnemiesPerWave + currentWave * 2;
        float interval = Mathf.Max(0.5f, spawnInterval - currentWave * 0.1f);
        
        Debug.Log($"第 {currentWave} 波开始！敌人数量：{enemiesToSpawn}");
        UIManager.Instance?.StartCountdown(0); // 清除倒计时
        
        spawnCoroutine = StartCoroutine(SpawnWave(enemiesToSpawn, interval));
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
        yield return new WaitUntil(() => GetAllEnemies().Length == 0);
        
        yield return new WaitForSeconds(waveDelay);
        
        if (isSpawning)
        {
            if (currentWave >= maxWaves)
            {
                GameManager.Instance?.Victory();
            }
            else
            {
                GameManager.Instance?.OnWaveComplete();
                
                // 开始准备下一阶段
                if (currentWave < maxWaves)
                {
                    isPreparation = true;
                    preparationTimer = preparationTime;
                    UIManager.Instance?.StartCountdown(preparationTime);
                    StartCoroutine(PreparationCountdown());
                }
            }
        }
    }

    /// <summary>
    /// 获取所有存活的敌人
    /// </summary>
    BaseEnemy[] GetAllEnemies()
    {
        return FindObjectsOfType<BaseEnemy>();
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
		Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

		Instantiate(enemyPrefab, point.position, Quaternion.identity);

		GameObject prefabToSpawn = GetEnemyPrefab();
        GameObject enemyObj = Instantiate(prefabToSpawn, spawnPos.position, Quaternion.identity);
        
        // 应用波次难度加成
        ApplyWaveBonus(enemyObj.GetComponent<BaseEnemy>());
    }

    /// <summary>
    /// 应用波次难度加成
    /// </summary>
    void ApplyWaveBonus(BaseEnemy enemy)
    {
        if (enemy == null) return;
        
        // 增加 HP
        enemy.maxHp *= Mathf.Pow(hpMultiplier, currentWave - 1);
        enemy.hp = enemy.maxHp;
        
        // 增加速度
        enemy.speed *= Mathf.Pow(speedMultiplier, currentWave - 1);
        
        // 后期波次有概率出现特殊敌人
        if (currentWave >= 3 && Random.value < 0.2f)
        {
            enemy.prioritizeEnergy = true;
            Debug.Log("生成了优先攻击能源的敌人！");
        }
        
        if (currentWave >= 5 && Random.value < 0.15f)
        {
            enemy.canDestroyTerrain = true;
            Debug.Log("生成了可以破坏地形的敌人！");
        }
    }

    /// <summary>
    /// 获取出生点
    /// </summary>
    Transform GetSpawnPoint()
    {
        // 优先使用 GameManager 中设置的出生点
        if (GameManager.Instance != null && GameManager.Instance.spawnPoint != null)
        {
            return GameManager.Instance.spawnPoint;
        }
        
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
    
    /// <summary>
    /// 手动触发测试波次（用于调试）
    /// </summary>
    public void TriggerTestWave()
    {
        if (!isSpawning && !isPreparation)
        {
            Debug.Log("测试波次触发！");
            StartNextWave();
        }
    }
}
