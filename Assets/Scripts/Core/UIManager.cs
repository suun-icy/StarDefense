using UnityEngine;
using TMPro;

/// <summary>
/// UI 管理系统（完整 UI 控制）
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("资源显示")]
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI materialText;
    
    [Header("游戏面板")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    
    [Header("建造 UI")]
    public GameObject buildPanel;
    public TextMeshProUGUI selectedTowerName;
    public TextMeshProUGUI selectedTowerCost;
    
    [Header("波次信息")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI healthText;
    
    [Header("统计信息")]
    public TextMeshProUGUI killCountText; // 击杀数量
    public TextMeshProUGUI countdownText; // 倒计时
    
    [Header("提示信息")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;

    private float waveCountdown = 0f;
    private bool isCountingDown = false;

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
        UpdateResourceUI();
        UpdateKillCount(0);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }

    void Update()
    {
        UpdateResourceUI();
        UpdateGameInfo();
        UpdateCountdown();
        
        // P 键暂停
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// 更新资源 UI
    /// </summary>
    public void UpdateResourceUI()
    {
        if (ResourceManager.Instance != null)
        {
            if (energyText != null)
                energyText.text = $"能源：{ResourceManager.Instance.energy:F0}";
            
            if (materialText != null)
            {
                materialText.text = $"物资：{ResourceManager.Instance.material:F0}";
                
                // 低资源警告
                if (ResourceManager.Instance.material < 50)
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
    }

    /// <summary>
    /// 更新游戏信息
    /// </summary>
    public void UpdateGameInfo()
    {
        if (GameManager.Instance != null)
        {
            if (waveText != null)
                waveText.text = $"波次：{GameManager.Instance.currentWave}/{GameManager.Instance.maxWaves}";
            
            if (healthText != null)
            {
                healthText.text = $"生命：{GameManager.Instance.CurrentPlayerHealth}";
                
                if (GameManager.Instance.CurrentPlayerHealth <= 5)
                {
                    healthText.color = Color.red;
                }
                else
                {
                    healthText.color = Color.white;
                }
            }
        }
    }

    /// <summary>
    /// 更新击杀计数
    /// </summary>
    public void UpdateKillCount(int kills)
    {
        if (killCountText != null)
        {
            killCountText.text = $"击杀：{kills}";
        }
    }

    /// <summary>
    /// 开始倒计时
    /// </summary>
    public void StartCountdown(float seconds)
    {
        waveCountdown = seconds;
        isCountingDown = true;
    }

    /// <summary>
    /// 更新倒计时显示
    /// </summary>
    void UpdateCountdown()
    {
        if (isCountingDown && countdownText != null)
        {
            if (waveCountdown > 0)
            {
                waveCountdown -= Time.deltaTime;
                int minutes = Mathf.FloorToInt(waveCountdown / 60);
                int seconds = Mathf.FloorToInt(waveCountdown % 60);
                countdownText.text = $"下波：{minutes:D2}:{seconds:D2}";
                countdownText.color = Color.white;
            }
            else
            {
                waveCountdown = 0;
                isCountingDown = false;
                countdownText.text = "波次进行中";
                countdownText.color = Color.green;
            }
        }
    }

    /// <summary>
    /// 显示开始界面
    /// </summary>
    public void ShowStartPanel()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏开始界面
    /// </summary>
    public void HideStartPanel()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 显示游戏结束界面
    /// </summary>
    public void ShowGameOverPanel(bool isVictory = false)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            TextMeshProUGUI title = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (title != null)
            {
                title.text = isVictory ? "胜利！" : "游戏结束";
            }
        }
    }

    /// <summary>
    /// 切换暂停
    /// </summary>
    public void TogglePause()
    {
        if (pausePanel != null)
        {
            bool isActive = pausePanel.activeSelf;
            pausePanel.SetActive(!isActive);
            Time.timeScale = isActive ? 1f : 0f;
        }
    }

    /// <summary>
    /// 显示警告
    /// </summary>
    public void ShowWarning(string message, float duration = 2f)
    {
        if (warningPanel != null && warningText != null)
        {
            warningText.text = message;
            warningPanel.SetActive(true);
            
            CancelInvoke(nameof(HideWarning));
            Invoke(nameof(HideWarning), duration);
        }
    }

    /// <summary>
    /// 隐藏警告
    /// </summary>
    public void HideWarning()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 显示能源不足警告
    /// </summary>
    public void ShowEnergyWarning()
    {
        ShowWarning("能源不足！", 1.5f);
    }

    /// <summary>
    /// 显示物资不足警告
    /// </summary>
    public void ShowMaterialWarning()
    {
        ShowWarning("物资不足！", 1.5f);
    }

    /// <summary>
    /// 更新选中塔的信息
    /// </summary>
    public void UpdateSelectedTowerInfo(TowerType type, int cost)
    {
        if (selectedTowerName != null)
            selectedTowerName.text = type.ToString() + "塔";
        
        if (selectedTowerCost != null)
            selectedTowerCost.text = $"成本：{cost}";
    }
}
