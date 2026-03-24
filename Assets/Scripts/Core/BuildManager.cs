using UnityEngine;

/// <summary>
/// 建造管理系统（支持多种塔、预览、升级）
/// </summary>
public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("塔预制体")]
    public GameObject basicTowerPrefab;
    public GameObject laserTowerPrefab;
    public GameObject cannonTowerPrefab;
    public GameObject missileTowerPrefab; // 新增：导弹塔

    public TowerType currentTowerType = TowerType.Basic;
    
    [Header("建造成本 (物资/能源)")]
    public int basicTowerMaterialCost = 50;
    public int basicTowerEnergyCost = 0;
    
    public int laserTowerMaterialCost = 80;
    public int laserTowerEnergyCost = 30;
    
    public int cannonTowerMaterialCost = 120;
    public int cannonTowerEnergyCost = 20;
    
    public int missileTowerMaterialCost = 100;
    public int missileTowerEnergyCost = 40; // 新增：导弹塔成本

    [Header("预览")]
    public GameObject previewObject;
    public Material validMaterial;
    public Material invalidMaterial;
    
    private bool isBuildingMode = false;
    private GameObject currentPreview;
    private bool canBuildHere = false;

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
        CreatePreview();
    }

    void Update()
    {
        if (isBuildingMode)
        {
            UpdatePreviewPosition();
            CheckCanBuild();
            HandleBuildInput();
        }
        
        // 数字键切换塔类型
        HandleTowerSelection();
        
        // ESC 取消建造
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelBuild();
        }
    }

    /// <summary>
    /// 创建预览物体
    /// </summary>
    void CreatePreview()
    {
        previewObject = new GameObject("BuildPreview");
        Renderer renderer = previewObject.AddComponent<MeshRenderer>();
        MeshFilter filter = previewObject.AddComponent<MeshFilter>();
        
        // 创建简单的立方体作为预览
        filter.mesh = CreateCubeMesh();
        
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.green;
        mat.SetFloat("_Mode", 3); // Transparent mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        
        renderer.material = mat;
        renderer.enabled = false;
        
        Collider col = previewObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
        
        currentPreview = previewObject;
    }

    Mesh CreateCubeMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f)
        };
        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
        return mesh;
    }

    /// <summary>
    /// 选择塔类型
    /// </summary>
    public void SelectTower(int type)
    {
        currentTowerType = (TowerType)type;
        isBuildingMode = true;
        currentPreview.SetActive(true);
        
        // 更新 UI 显示
        UIManager.Instance?.UpdateSelectedTowerInfo(currentTowerType, GetCurrentTowerMaterialCost());
        
        Debug.Log("选择塔：" + currentTowerType);
    }

    /// <summary>
    /// 获取当前塔的预制体
    /// </summary>
    public GameObject GetCurrentTowerPrefab()
    {
        switch (currentTowerType)
        {
            case TowerType.Basic:
                return basicTowerPrefab;
            case TowerType.Laser:
                return laserTowerPrefab;
            case TowerType.Cannon:
                return cannonTowerPrefab;
            case TowerType.Missile:
                return missileTowerPrefab;
            default:
                return basicTowerPrefab;
        }
    }

    /// <summary>
    /// 获取当前塔的物资成本
    /// </summary>
    public int GetCurrentTowerMaterialCost()
    {
        switch (currentTowerType)
        {
            case TowerType.Basic:
                return basicTowerMaterialCost;
            case TowerType.Laser:
                return laserTowerMaterialCost;
            case TowerType.Cannon:
                return cannonTowerMaterialCost;
            case TowerType.Missile:
                return missileTowerMaterialCost;
            default:
                return basicTowerMaterialCost;
        }
    }
    
    /// <summary>
    /// 获取当前塔的能源成本
    /// </summary>
    public int GetCurrentTowerEnergyCost()
    {
        switch (currentTowerType)
        {
            case TowerType.Basic:
                return basicTowerEnergyCost;
            case TowerType.Laser:
                return laserTowerEnergyCost;
            case TowerType.Cannon:
                return cannonTowerEnergyCost;
            case TowerType.Missile:
                return missileTowerEnergyCost;
            default:
                return basicTowerEnergyCost;
        }
    }

    /// <summary>
    /// 更新预览位置
    /// </summary>
    void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                currentPreview.transform.position = hit.point + Vector3.up * 0.5f;
                currentPreview.SetActive(true);
                return;
            }
        }
        currentPreview.SetActive(false);
    }

    /// <summary>
    /// 检查是否可以建造
    /// </summary>
    void CheckCanBuild()
    {
        canBuildHere = false;

        // 检查资源
        int materialCost = GetCurrentTowerMaterialCost();
        int energyCost = GetCurrentTowerEnergyCost();
        
        if (ResourceManager.Instance.material < materialCost || 
            ResourceManager.Instance.energy < energyCost)
        {
            SetPreviewColor(Color.red);
            return;
        }

        // 检查是否有其他塔
        Collider[] hits = Physics.OverlapSphere(currentPreview.transform.position, 1f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Tower") || hit.GetComponent<BaseTower>() != null)
            {
                SetPreviewColor(Color.red);
                return;
            }
        }

        canBuildHere = true;
        SetPreviewColor(Color.green);
    }

    /// <summary>
    /// 设置预览颜色
    /// </summary>
    void SetPreviewColor(Color color)
    {
        Renderer renderer = currentPreview.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color c = color;
            c.a = 0.5f;
            renderer.material.color = c;
        }
    }

    /// <summary>
    /// 处理建造输入
    /// </summary>
    void HandleBuildInput()
    {
        if (Input.GetMouseButtonDown(0) && canBuildHere)
        {
            TryBuild(currentPreview.transform.position);
        }
    }

    /// <summary>
    /// 尝试建造
    /// </summary>
    void TryBuild(Vector3 pos)
    {
        int materialCost = GetCurrentTowerMaterialCost();
        int energyCost = GetCurrentTowerEnergyCost();
        
        // 使用双资源系统
        if (!ResourceManager.Instance.UseMaterial(materialCost))
        {
            return;
        }
        
        if (energyCost > 0 && !ResourceManager.Instance.UseEnergy(energyCost))
        {
            // 退还物资
            ResourceManager.Instance.AddMaterial(materialCost);
            return;
        }

        GameObject tower = Instantiate(GetCurrentTowerPrefab(), pos, Quaternion.identity);
        tower.tag = "Tower";
        
        Debug.Log($"建造 {currentTowerType} 塔，花费 {materialCost} 物资 + {energyCost} 能源");
    }

    /// <summary>
    /// 处理塔选择快捷键
    /// </summary>
    void HandleTowerSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectTower((int)TowerType.Basic);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectTower((int)TowerType.Laser);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectTower((int)TowerType.Cannon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) // 新增：导弹塔快捷键
        {
            SelectTower((int)TowerType.Missile);
        }
    }

    /// <summary>
    /// 取消建造
    /// </summary>
    public void CancelBuild()
    {
        isBuildingMode = false;
        currentPreview.SetActive(false);
    }
}
