using UnityEngine;

/// <summary>
/// 建造预览系统
/// </summary>
public class BuildPreview : MonoBehaviour
{
	public static BuildPreview Instance;

	public GameObject previewPrefab;

	public Material validMat;
	public Material invalidMat;

	private GameObject previewObj;
	private bool canBuild = false;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		CreatePreview();
	}

	void Update()
	{
		UpdatePreviewPosition();
		CheckCanBuild();
		HandleBuild();
	}

	/// <summary>
	/// 创建预览物体
	/// </summary>
	void CreatePreview()
	{
		previewObj = Instantiate(previewPrefab);
		previewObj.name = "Preview";

		// 去掉碰撞（避免干扰）
		foreach (var col in previewObj.GetComponentsInChildren<Collider>())
		{
			col.enabled = false;
		}
	}

	/// <summary>
	/// 跟随鼠标
	/// </summary>
	void UpdatePreviewPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out RaycastHit hit, 1000))
		{
			if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
			{
				previewObj.transform.position = hit.point;
			}
		}
	}

	/// <summary>
	/// 检测是否可建
	/// </summary>
	void CheckCanBuild()
	{
		Collider[] hits = Physics.OverlapSphere(previewObj.transform.position, 1f);

		canBuild = true;

		foreach (var hit in hits)
		{
			if (hit.gameObject.layer == LayerMask.NameToLayer("Tower"))
			{
				canBuild = false;
				break;
			}
		}

		UpdateMaterial();
	}

	/// <summary>
	/// 切换材质
	/// </summary>
	void UpdateMaterial()
	{
		var renderers = previewObj.GetComponentsInChildren<Renderer>();

		foreach (var r in renderers)
		{
			r.material = canBuild ? validMat : invalidMat;
		}
	}

	/// <summary>
	/// 点击建造
	/// </summary>
	void HandleBuild()
	{
		if (Input.GetMouseButtonDown(0) && canBuild)
		{
			if (!ResourceManager.Instance.UseMaterial(50))
				return;

			Instantiate(BuildManager.Instance.GetCurrentTowerPrefab(), previewObj.transform.position, Quaternion.identity);
		}
	}
}