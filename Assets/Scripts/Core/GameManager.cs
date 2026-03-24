using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏主控制器
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("UI")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    
    [Header("系统")]
    public WaveSpawner waveSpawner;
    
    [Header("游戏设置")]
    public int maxWaves = 10;
    public float playerHealth = 20;
    
    public enum GameState
    {
        Preparing,  // 准备阶段
        Playing,    // 游戏中
        GameOver    // 结束
    }

    public GameState State { get; private set; }
    public int currentWave = 0;
    public float CurrentPlayerHealth => playerHealth;

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

    void Start()
    {
        State = GameState.Preparing;
        Time.timeScale = 1f;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 按 R 键重新开始
        if (State == GameState.GameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    /// <summary>
    /// 启动游戏
    /// </summary>
    public void StartGame()
    {
        Debug.Log("启动游戏");
        State = GameState.Playing;
        currentWave = 0;
        playerHealth = 20;

        // 隐藏开始 UI
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // 开始刷怪
        if (waveSpawner != null)
        {
            waveSpawner.StartWave();
        }
    }

    /// <summary>
    /// 敌人到达终点
    /// </summary>
    public void OnEnemyReachEnd(float damage = 1f)
    {
        playerHealth -= damage;
        Debug.Log($"玩家剩余生命：{playerHealth}");
        
        if (playerHealth <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// 波次完成
    /// </summary>
    public void OnWaveComplete()
    {
        currentWave++;
        Debug.Log($"波次 {currentWave} 完成");
        
        if (currentWave >= maxWaves)
        {
            Victory();
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        State = GameState.GameOver;
        Debug.Log("游戏结束！");
        
        if (waveSpawner != null)
        {
            waveSpawner.StopWave();
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 胜利
    /// </summary>
    public void Victory()
    {
        State = GameState.GameOver;
        Debug.Log("恭喜胜利！");
        
        if (waveSpawner != null)
        {
            waveSpawner.StopWave();
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 重新开始
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
