using UnityEngine;

/// <summary>
/// 敌人基类（支持路径点移动、AI 行为）
/// </summary>
public class BaseEnemy : MonoBehaviour
{
    [Header("基础属性")]
    public float hp = 100;
    public float maxHp = 100;
    public float speed = 3f;
    public float damage = 1f; // 对终点的伤害
    
    [Header("AI 行为")]
    public bool prioritizeEnergy = false; // 优先攻击能源
    public bool canDestroyTerrain = false; // 能否破坏地形
    public float terrainDamageRate = 10f; // 地形破坏速度
    
    [Header("路径")]
    public int currentWaypointIndex = 0;
    public Transform target; // 备用：直接追踪目标
    public Transform energyTarget; // 能源目标（优先攻击）
    
    [Header("掉落")]
    public int goldReward = 10;
    public int energyReward = 5;
    
    private bool isFollowingPath = true;
    private Transform currentTerrainObstacle; // 当前阻挡的地形
    private Vector3? detourTarget; // 绕路目标点
    private float terrainDamageTimer = 0f;
    
    // 静态击杀计数
    public static int totalKills = 0;

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        hp -= damage;
        
        if (hp <= 0)
        {
            Die();
        }
    }
    
    void Update()
    {
        // 优先攻击能源目标
        if (prioritizeEnergy && energyTarget != null)
        {
            MoveTowardsEnergyTarget();
            return;
        }
        
        // 尝试破坏地形
        if (canDestroyTerrain && currentTerrainObstacle != null)
        {
            DamageTerrain();
            return;
        }
        
        // 检查是否有绕路需求
        if (detourTarget.HasValue)
        {
            MoveToDetourTarget();
            return;
        }
        
        if (isFollowingPath && Waypoint.waypoints != null && Waypoint.waypoints.Length > 0)
        {
            FollowPath();
        }
        else if (target != null)
        {
            FollowTarget();
        }
    }
    
    /// <summary>
    /// 移动到能源目标
    /// </summary>
    void MoveTowardsEnergyTarget()
    {
        if (energyTarget == null)
        {
            isFollowingPath = true;
            return;
        }
        
        Vector3 dir = (energyTarget.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, energyTarget.position);
        
        if (distance < 1.5f)
        {
            // 攻击能源
            AttackEnergyTarget();
        }
        else
        {
            transform.position += dir * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
    
    /// <summary>
    /// 攻击能源目标
    /// </summary>
    void AttackEnergyTarget()
    {
        // 这里可以实现对能源建筑的伤害逻辑
        Debug.Log("正在攻击能源目标！");
    }
    
    /// <summary>
    /// 沿路径点移动
    /// </summary>
    void FollowPath()
    {
        // 优先使用 GameManager 中设置的终点目标
        if (GameManager.Instance != null && GameManager.Instance.endTarget != null)
        {
            target = GameManager.Instance.endTarget;
        }
        
        // 设置能源核心目标（用于优先攻击）
        if (GameManager.Instance != null && GameManager.Instance.energyCore != null)
        {
            energyTarget = GameManager.Instance.energyCore;
        }
        
        if (currentWaypointIndex >= Waypoint.waypoints.Length)
        {
            ReachEnd();
            return;
        }
        
        Vector3 targetPos = Waypoint.waypoints[currentWaypointIndex].transform.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPos);
        
        // 检测前方是否有地形阻挡
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 2f))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                currentTerrainObstacle = hit.transform;
                if (!canDestroyTerrain)
                {
                    FindDetourPath(hit.point);
                }
                return;
            }
        }
        
        if (distance < 0.5f)
        {
            currentWaypointIndex++;
        }
        else
        {
            transform.position += dir * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
    
    /// <summary>
    /// 寻找绕路路径
    /// </summary>
    void FindDetourPath(Vector3 obstaclePoint)
    {
        // 简单的绕路逻辑：向两侧寻找可通行点
        Vector3 rightDir = transform.right;
        Vector3 leftDir = -transform.right;
        
        float checkDistance = 5f;
        
        if (!Physics.Raycast(transform.position, rightDir, checkDistance))
        {
            detourTarget = transform.position + rightDir * checkDistance;
        }
        else if (!Physics.Raycast(transform.position, leftDir, checkDistance))
        {
            detourTarget = transform.position + leftDir * checkDistance;
        }
        else
        {
            // 无法绕路，等待地形被破坏或其他处理
            Debug.Log("无法绕路，等待...");
        }
    }
    
    /// <summary>
    /// 移动到绕路目标
    /// </summary>
    void MoveToDetourTarget()
    {
        if (!detourTarget.HasValue)
        {
            isFollowingPath = true;
            return;
        }
        
        Vector3 dir = (detourTarget.Value - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, detourTarget.Value);
        
        if (distance < 0.5f)
        {
            detourTarget = null;
            currentTerrainObstacle = null;
            isFollowingPath = true;
        }
        else
        {
            transform.position += dir * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
    
    /// <summary>
    /// 破坏地形
    /// </summary>
    void DamageTerrain()
    {
        if (currentTerrainObstacle == null)
        {
            isFollowingPath = true;
            return;
        }
        
        terrainDamageTimer += Time.deltaTime;
        
        if (terrainDamageTimer >= 1f / terrainDamageRate)
        {
            terrainDamageTimer = 0f;
            // 这里可以调用 TerrainModifier 来实际修改地形
            Debug.Log("正在破坏地形...");
            
            // 模拟地形破坏后移除障碍
            // 实际项目中应该调用 TerrainModifier.Instance.ModifyTerrain()
            currentTerrainObstacle = null;
            isFollowingPath = true;
        }
    }
    
    /// <summary>
    /// 追踪目标（备用模式）
    /// </summary>
    void FollowTarget()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    /// <summary>
    /// 到达终点
    /// </summary>
    void ReachEnd()
    {
        Debug.Log("敌人到达终点！");
        GameManager.Instance?.OnEnemyReachEnd(damage);
        Destroy(gameObject);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    void Die()
    {
        totalKills++;
        
        // 奖励资源
        ResourceManager.Instance?.AddMaterial(goldReward);
        ResourceManager.Instance?.AddEnergy(energyReward);
        
        // 更新 UI
        UIManager.Instance?.UpdateKillCount(totalKills);
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 设置是否优先攻击能源
    /// </summary>
    public void SetPrioritizeEnergy(bool value)
    {
        prioritizeEnergy = value;
    }
    
    /// <summary>
    /// 设置能源目标
    /// </summary>
    public void SetEnergyTarget(Transform newTarget)
    {
        energyTarget = newTarget;
    }
}
