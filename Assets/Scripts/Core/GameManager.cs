using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏主控制器（优化版）
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
    public float preparationTime = 10f; // 准备时间
    
    [Header("场景设置")]
    public Transform spawnPoint; // 敌人生成点
    public Transform endTarget;  // 敌人目标点（终点）
    public Transform energyCore; // 能源核心（优先攻击目标）
    
    public enum GameState
    {
        Preparing,  // 准备阶段
        Playing,    // 游戏中
        GameOver    // 结束
    }

    public GameState State { get; private set; }
    public int currentWave = 0;
    public float CurrentPlayerHealth => playerHealth;
    public int TotalKills => BaseEnemy.totalKills;
    
    private bool isCountingDown = false;
    private float countdownTimer = 0f;

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
        
        // 自动查找场景中的关键点（如果未手动赋值）
        if (spawnPoint == null)
        {
            GameObject sp = GameObject.Find("SpawnPoint");
            if (sp != null) spawnPoint = sp.transform;
        }
        
        if (endTarget == null)
        {
            GameObject et = GameObject.Find("EndTarget");
            if (et != null) endTarget = et.transform;
        }
        
        if (energyCore == null)
        {
            GameObject ec = GameObject.Find("EnergyCore");
            if (ec != null) energyCore = ec.transform;
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // 显示开始界面
        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }
    }

    void Update()
    {
        // 更新倒计时
        if (isCountingDown && State == GameState.Preparing)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                isCountingDown = false;
                StartGame();
            }
            else
            {
                UIManager.Instance?.StartCountdown(countdownTimer);
            }
        }
        
        // 按 R 键重新开始
        if (State == GameState.GameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // 按 T 键触发测试波次（调试用）
        if (Input.GetKeyDown(KeyCode.T) && State == GameState.Playing)
        {
            Debug.Log("测试波次触发！");
            waveSpawner?.TriggerTestWave();
        }
    }
    
    /// <summary>
    /// 开始准备阶段（倒计时后开始游戏）
    /// </summary>
    public void StartPreparing()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }
        
        State = GameState.Preparing;
        isCountingDown = true;
        countdownTimer = preparationTime;
        
        Debug.Log($"准备阶段开始！{preparationTime}秒后开始第 1 波");
        UIManager.Instance?.StartCountdown(preparationTime);
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
        BaseEnemy.totalKills = 0; // 重置击杀数
        
        // 更新击杀 UI
        UIManager.Instance?.UpdateKillCount(0);

        // 开始刷怪
        if (waveSpawner != null)
        {
            waveSpawner.StartWave();
        }
    }
    
    /// <summary>
    /// 是否正在倒计时
    /// </summary>
    public bool IsCountingDown()
    {
        return isCountingDown;
    }
    
    /// <summary>
    /// 获取倒计时剩余时间
    /// </summary>
    public float GetCountdown()
    {
        return countdownTimer;
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
	public static GameManager Instance;

	[Header("UI")]
	public GameObject startPanel;
	public GameObject gameOverPanel;
	public GameObject victoryPanel;

	[Header("系统")]
	public WaveSpawner waveSpawner;

	[Header("游戏设置")]
	public int maxWaves = 10;
	public float playerHealth = 20;

	public enum GameState
	{
		Preparing,
		Playing,
		Paused,
		GameOver,
		Victory
	}

	public GameState State { get; private set; }
	public int currentWave = 0;

	public bool IsPaused => State == GameState.Paused;
	public float CurrentPlayerHealth => playerHealth;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		State = GameState.Preparing;
		Time.timeScale = 1f;

		if (gameOverPanel) gameOverPanel.SetActive(false);
		if (victoryPanel) victoryPanel.SetActive(false);
	}

	void Update()
	{
		// 暂停
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePause();
		}

		// 重开
		if ((State == GameState.GameOver || State == GameState.Victory)
			&& Input.GetKeyDown(KeyCode.R))
		{
			RestartGame();
		}
	}

	#region 游戏流程

	public void StartGame()
	{
		Debug.Log("开始游戏");

		State = GameState.Playing;
		currentWave = 0;
		playerHealth = 20;

		if (startPanel) startPanel.SetActive(false);

		StartNextWave();
	}

	public void StartNextWave()
	{
		if (waveSpawner != null)
		{
			waveSpawner.StartWave();
		}
	}

	public void OnWaveComplete()
	{
		currentWave++;

		Debug.Log($"波次 {currentWave} 完成");

		if (currentWave >= maxWaves)
		{
			Victory();
		}
		else
		{
			// 可以在这里进入“建造阶段”
			State = GameState.Preparing;

			Invoke(nameof(StartNextWave), 5f); // 5秒后下一波
		}
	}

	#endregion

	#region 玩家状态

	public void OnEnemyReachEnd(float damage = 1f)
	{
		playerHealth -= damage;

		Debug.Log($"生命：{playerHealth}");

		if (playerHealth <= 0)
		{
			GameOver();
		}
	}

	#endregion

	#region 状态控制

	public void TogglePause()
	{
		if (State == GameState.Playing)
		{
			State = GameState.Paused;
			Time.timeScale = 0f;
		}
		else if (State == GameState.Paused)
		{
			State = GameState.Playing;
			Time.timeScale = 1f;
		}
	}

	public void GameOver()
	{
		State = GameState.GameOver;

		Debug.Log("游戏失败");

		waveSpawner?.StopWave();
		gameOverPanel?.SetActive(true);

		Time.timeScale = 0f;
	}

	public void Victory()
	{
		State = GameState.Victory;

		Debug.Log("游戏胜利");

		waveSpawner?.StopWave();
		victoryPanel?.SetActive(true);

		Time.timeScale = 0f;
	}

	#endregion

	#region 重开

	public void RestartGame()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	#endregion
}
