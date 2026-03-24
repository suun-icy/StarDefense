using UnityEngine;

/// <summary>
/// 敌人基类（支持路径点移动）
/// </summary>
public class BaseEnemy : MonoBehaviour
{
    public float hp = 100;
    public float speed = 3f;
    
    [Header("路径")]
    public int currentWaypointIndex = 0;
    public Transform target; // 备用：直接追踪目标
    
    private bool isFollowingPath = true;

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log(name + " 受到伤害：" + damage);

        if (hp <= 0)
        {
            Die();
        }
    }
    
    void Update()
    {
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
    /// 沿路径点移动
    /// </summary>
    void FollowPath()
    {
        if (currentWaypointIndex >= Waypoint.waypoints.Length)
        {
            // 到达终点，扣除玩家生命值（后续实现）
            ReachEnd();
            return;
        }
        
        Vector3 targetPos = Waypoint.waypoints[currentWaypointIndex].transform.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        
        if (dir.magnitude < 0.5f)
        {
            // 到达当前路径点，前往下一个
            currentWaypointIndex++;
        }
        else
        {
            transform.position += dir * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
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
        GameManager.Instance?.OnEnemyReachEnd();
        Destroy(gameObject);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    void Die()
    {
        ResourceManager.Instance.AddMaterial(20);
        Destroy(gameObject);
    }
}
