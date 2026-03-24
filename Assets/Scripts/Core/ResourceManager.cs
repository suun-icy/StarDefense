using TMPro;
using UnityEngine;

/// <summary>
/// 资源管理（能源 + 物资 + 自动恢复）
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("资源")]
    public float energy = 100;
    public float material = 200;
    
    [Header("资源上限")]
    public float maxEnergy = 500;
    public float maxMaterial = 1000;
    
    [Header("自动恢复")]
    public float energyRegenRate = 1f; // 每秒恢复能源
    public float materialRegenRate = 0.5f; // 每秒恢复物资
    
    [Header("UI")]
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI materialText;

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
        UpdateUI();
    }

    void Update()
    {
        // 自动恢复资源（仅在游戏进行中）
        if (GameManager.Instance != null && GameManager.Instance.State == GameManager.GameState.Playing)
        {
            RegenerateResources();
        }
        
        UpdateUI();
    }

    /// <summary>
    /// 恢复资源
    /// </summary>
    void RegenerateResources()
    {
        energy = Mathf.Min(maxEnergy, energy + energyRegenRate * Time.deltaTime);
        material = Mathf.Min(maxMaterial, material + materialRegenRate * Time.deltaTime);
    }

    /// <summary>
    /// 使用能源
    /// </summary>
    public bool UseEnergy(float amount)
    {
        if (energy < amount)
        {
            UIManager.Instance?.ShowEnergyWarning();
            return false;
        }

        energy -= amount;
        UpdateUI();
        return true;
    }

    /// <summary>
    /// 使用物资
    /// </summary>
    public bool UseMaterial(float amount)
    {
        if (material < amount)
        {
            UIManager.Instance?.ShowMaterialWarning();
            return false;
        }

        material -= amount;
        UpdateUI();
        return true;
    }
    
    /// <summary>
    /// 增加能源
    /// </summary>
    public void AddEnergy(float amount)
    {
        energy = Mathf.Min(maxEnergy, energy + amount);
        UpdateUI();
    }

    /// <summary>
    /// 增加物资
    /// </summary>
    public void AddMaterial(float amount)
    {
        material = Mathf.Min(maxMaterial, material + amount);
        UpdateUI();
    }

    /// <summary>
    /// 更新 UI
    /// </summary>
    void UpdateUI()
    {
        if (energyText != null)
        {
            energyText.text = $"能源：{energy:F0}/{maxEnergy:F0}";
        }
        
        if (materialText != null)
        {
            materialText.text = $"物资：{material:F0}/{maxMaterial:F0}";
            
            // 低资源警告颜色
            if (material < 50)
            {
                materialText.color = Color.Lerp(Color.white, Color.red, 
                    Mathf.PingPong(Time.time * 5, 1));
            }
            else
            {
                materialText.color = Color.white;
            }
        }
    }
    
    /// <summary>
    /// 设置资源（用于测试）
    /// </summary>
    public void SetResources(float newEnergy, float newMaterial)
    {
        energy = Mathf.Clamp(newEnergy, 0, maxEnergy);
        material = Mathf.Clamp(newMaterial, 0, maxMaterial);
        UpdateUI();
    }
}
