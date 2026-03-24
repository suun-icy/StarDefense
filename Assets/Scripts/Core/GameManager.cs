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