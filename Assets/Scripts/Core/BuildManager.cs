using UnityEngine;

/// <summary>
/// 建造管理系统
/// </summary>
public class BuildManager : MonoBehaviour
{
	public static BuildManager Instance;

	public GameObject towerPrefab;

	public TowerType currentTowerType = TowerType.Basic;

	private void Awake()
	{
		Instance = this;
	}

	void Update()
	{
		HandleBuildInput();
	}

	public void SelectTower(int type)
	{
		currentTowerType = (TowerType)type;
		Debug.Log("选择塔: " + currentTowerType);
	}
	public GameObject GetCurrentTowerPrefab()
	{
		switch (currentTowerType)
		{
			case TowerType.Basic:
				return towerPrefab;
		}

		return towerPrefab;
	}


	/// <summary>
	/// 鼠标点击建造
	/// </summary>
	void HandleBuildInput()
	{
		// 左键点击
		if (Input.GetMouseButtonDown(0))
		{
			// UI点击不建造（后续可加）
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 1000))
			{
				// 只允许在地面建造
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
				{
					TryBuild(hit.point);
				}
			}
		}
	}

	void TryBuild(Vector3 pos)
	{
		// 扣物资
		if (!ResourceManager.Instance.UseMaterial(50))
		{
			return;
		}

		// 创建塔
		Instantiate(Instance.GetCurrentTowerPrefab(), pos, Quaternion.identity);
	}
}