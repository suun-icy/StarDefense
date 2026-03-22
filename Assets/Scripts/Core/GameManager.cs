using UnityEngine;

/// <summary>
/// 游戏主控制器
/// </summary>
public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	[Header("UI")]
	public GameObject startPanel;
	[Header("系统")]
	public WaveSpawner waveSpawner;
	public enum GameState
	{
		Preparing,  // 准备阶段
		Playing,    // 游戏中
		GameOver    // 结束
	}

	public GameState State;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		State = GameState.Preparing;
	}

	public void StartGame()
	{
		Debug.Log("启动游戏");
		State = GameState.Playing;
		//WaveSpawner.Instance.StartWave();


		//  隐藏开始UI
		if (startPanel != null)
		{
			startPanel.SetActive(false);
		}

		//  开始刷怪
		if (waveSpawner != null)
		{
			waveSpawner.StartWave();
		}
	}
}