using TMPro;
using UnityEngine;

/// <summary>
/// 资源管理（能源 + 物资）
/// </summary>
public class ResourceManager : MonoBehaviour
{
	public static ResourceManager Instance;

	[Header("资源")]
	public float energy = 100;
	public float material = 200;

	[Header("UI")]
	public TextMeshProUGUI energyText;
	public TextMeshProUGUI materialText;

	private float flashTimer = 0;

	private void Awake()
	{
		Instance = this;
	}


	void Start()
	{
		materialText.text = "测试文字";
		materialText.color = Color.red;
		UpdateUI();
		materialText.text = "测试文字";
		materialText.color = Color.red;
	}

	void Update()
	{
		UpdateMaterialColor(); // 每帧更新颜色
	}
	/// <summary>
	/// 使用能源（后续用）
	/// </summary>
	public bool UseEnergy(int amount)
	{
		if (energy < amount)
		{
			Debug.Log("能源不足！");
			UpdateUI();
			return false;
		}

		energy -= amount;
		UpdateUI();
		return true;
	}


	/// <summary>
	/// 使用物资
	/// </summary>
	public bool UseMaterial(int amount)
	{
		if (material < amount)
		{
			Debug.Log("物资不足！");
			UpdateUI();
			return false;
		}

		material -= amount;
		UpdateUI();
		return true;
	}
	/// <summary>
	/// 增加资源
	/// </summary>
	public void AddMaterial(int amount)
	{
		material += amount;
		UpdateUI();
	}

	/// <summary>
	/// 更新UI
	/// </summary>
	void UpdateUI()
	{
		energyText.text = "能源：" + energy;
		materialText.text = "物资：" + material;
		if (material < 50)
		{
			flashTimer += Time.deltaTime * 5;
			materialText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(flashTimer, 1));
		}
	}
	/// <summary>
	/// 处理颜色变化（闪烁）
	/// </summary>
	void UpdateMaterialColor()
	{
		if (material < 50)
		{
			flashTimer += Time.deltaTime * 5;
			materialText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(flashTimer, 1));
		}
		else
		{
			materialText.color = Color.white;
		}
	}
}