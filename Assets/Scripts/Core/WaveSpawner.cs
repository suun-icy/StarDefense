using UnityEngine;

/// <summary>
/// 敌人波次生成器
/// </summary>
public class WaveSpawner : MonoBehaviour
{
	public static WaveSpawner Instance;

	public GameObject enemyPrefab;
	public Transform spawnPoint;

	private void Awake()
	{
		Instance = this;
	}

	public void StartWave()
	{
		InvokeRepeating(nameof(SpawnEnemy), 1f, 2f);
	}

	void SpawnEnemy()
	{
		if (enemyPrefab == null || spawnPoint == null)
		{
			Debug.LogError("没有设置敌人或出生点！");
			return;
		}

		Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
	}
}