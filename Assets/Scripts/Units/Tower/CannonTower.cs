using UnityEngine;

/// <summary>
/// 炮塔（范围伤害）
/// </summary>
public class CannonTower : BaseTower
{
    [Header("炮塔设置")]
    public float explosionRadius = 3f;
    public float explosionDamage = 50f;
    public GameObject explosionPrefab;
    public Transform turret; // 炮管旋转
    
    private Transform currentTarget;

    void Start()
    {
        range = 15f;
        fireRate = 2f;
    }

    void Update()
    {
        // 寻找目标
        FindTarget();
        
        // 旋转炮管朝向目标
        if (currentTarget != null && turret != null)
        {
            Vector3 direction = currentTarget.position - turret.position;
            direction.y = 0;
            turret.rotation = Quaternion.LookRotation(direction);
        }
        
        timer += Time.deltaTime;
        if (timer >= fireRate && currentTarget != null)
        {
            Attack();
            timer = 0;
        }
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        BaseEnemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        currentTarget = nearestEnemy?.transform;
    }

    void Attack()
    {
        if (currentTarget == null) return;

        // 创建爆炸效果
        CreateExplosion(currentTarget.position);
        
        // 范围伤害
        Collider[] hits = Physics.OverlapSphere(currentTarget.position, explosionRadius);
        
        foreach (var hit in hits)
        {
            BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }
    }

    void CreateExplosion(Vector3 position)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, position, Quaternion.identity);
        }
        else
        {
            // 简单可视化效果
            Debug.DrawLine(position, position + Vector3.up * 2f, Color.yellow, 0.5f);
        }
    }

    void OnDrawGizmosSelected()
    {
        // 攻击范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
        
        // 爆炸范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
