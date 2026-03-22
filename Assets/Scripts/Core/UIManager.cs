using UnityEngine;

/// <summary>
/// UI 管理器（先做基础）
/// </summary>
public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void ShowEnergyWarning()
	{
		Debug.Log("能源不足！");
	}

	public void ShowMaterialWarning()
	{
		Debug.Log("物资不足！");
	}
}