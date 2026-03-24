using UnityEngine;

/// <summary>
/// 地形修改系统（支持玩家挖掘/抬高、敌人破坏）
/// </summary>
public class TerrainModifier : MonoBehaviour
{
    public static TerrainModifier Instance { get; private set; }
    
    [Header("地形引用")]
    public Terrain terrain;
    public TerrainData terrainData;
    
    [Header("挖掘设置")]
    public float modifyRadius = 2f;
    public float modifyStrength = 1f;
    public LayerMask groundLayer;
    

    public enum ToolMode
    {
        Dig,      // 挖掘（降低）
        Raise,    // 抬高
        Flatten,  // 平整
        Smooth    // 平滑
    }

	[Header("工具模式")]
	public ToolMode currentMode = ToolMode.Dig;
    public float targetHeight = 0f; // 平整目标高度
    
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
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }
        
        if (terrain != null && terrain.terrainData != null)
        {
            terrainData = terrain.terrainData;
        }
    }
    
    void Update()
    {
        // 鼠标左键：使用当前工具
        if (Input.GetMouseButton(0))
        {
            ModifyTerrainAtMouse();
        }
        
        // 数字键切换工具模式
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentMode = ToolMode.Dig;
            Debug.Log("切换到挖掘模式");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentMode = ToolMode.Raise;
            Debug.Log("切换到抬高模式");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentMode = ToolMode.Flatten;
            Debug.Log("切换到平整模式");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            currentMode = ToolMode.Smooth;
            Debug.Log("切换到平滑模式");
        }
    }
    
    /// <summary>
    /// 在鼠标位置修改地形
    /// </summary>
    void ModifyTerrainAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            Vector3 worldPos = hit.point;
            ModifyTerrain(worldPos, currentMode);
        }
    }
    
    /// <summary>
    /// 修改地形
    /// </summary>
    public void ModifyTerrain(Vector3 worldPos, ToolMode mode)
    {
        if (terrainData == null)
        {
            Debug.LogWarning("没有设置地形数据！");
            return;
        }
        
        // 转换为地形空间坐标
        Vector3 terrainPos = worldPos - terrain.transform.position;
        float normalizedX = terrainPos.x / terrainData.size.x;
        float normalizedY = terrainPos.z / terrainData.size.y;
        
        // 获取高度图分辨率
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        
        int centerX = Mathf.FloorToInt(normalizedX * heightmapWidth);
        int centerY = Mathf.FloorToInt(normalizedY * heightmapHeight);
        int radius = Mathf.CeilToInt(modifyRadius * heightmapWidth / terrainData.size.x);
        
        // 获取当前高度图数据
        float[,] heights = terrainData.GetHeights(centerX - radius, centerY - radius, radius * 2, radius * 2);
        
        // 修改高度
        for (int y = 0; y < heights.GetLength(0); y++)
        {
            for (int x = 0; x < heights.GetLength(1); x++)
            {
                float dist = Mathf.Sqrt(x * x + y * y) / radius;
                
                if (dist <= 1f)
                {
                    float falloff = Mathf.Cos(dist * Mathf.PI * 0.5f); // 余弦衰减
                    float delta = modifyStrength * falloff * Time.deltaTime;
                    
                    switch (mode)
                    {
                        case ToolMode.Dig:
                            heights[y, x] -= delta;
                            break;
                            
                        case ToolMode.Raise:
                            heights[y, x] += delta;
                            break;
                            
                        case ToolMode.Flatten:
                            heights[y, x] = Mathf.Lerp(heights[y, x], targetHeight, delta);
                            break;
                            
                        case ToolMode.Smooth:
                            heights[y, x] = GetSmoothHeight(heights, x, y);
                            break;
                    }
                    
                    // 限制高度范围
                    heights[y, x] = Mathf.Clamp01(heights[y, x]);
                }
            }
        }
        
        // 应用高度图修改
        terrainData.SetHeights(centerX - radius, centerY - radius, heights);
    }
    
    /// <summary>
    /// 获取平滑后的高度
    /// </summary>
    float GetSmoothHeight(float[,] heights, int x, int y)
    {
        float sum = 0f;
        int count = 0;
        
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = x + dx;
                int ny = y + dy;
                
                if (nx >= 0 && ny >= 0 && nx < heights.GetLength(1) && ny < heights.GetLength(0))
                {
                    sum += heights[ny, nx];
                    count++;
                }
            }
        }
        
        return count > 0 ? sum / count : heights[y, x];
    }
    
    /// <summary>
    /// 敌人破坏地形（在指定位置降低地形）
    /// </summary>
    public void EnemyDamageTerrain(Vector3 worldPos, float damageAmount)
    {
        float oldStrength = modifyStrength;
        modifyStrength = damageAmount;
        ModifyTerrain(worldPos, ToolMode.Dig);
        modifyStrength = oldStrength;
    }
    
    /// <summary>
    /// 设置目标高度（用于平整工具）
    /// </summary>
    public void SetTargetHeight(float height)
    {
        targetHeight = height;
    }
}
