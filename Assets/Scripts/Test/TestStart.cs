using UnityEngine;

/// <summary>
/// 按空格启动游戏
/// </summary>
public class TestStart : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("启动游戏");
			GameManager.Instance.StartGame();
		}
	}
}