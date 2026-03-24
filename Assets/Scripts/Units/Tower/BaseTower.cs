using UnityEngine;

/// <summary>
/// 塔基类（所有塔的父类）
/// </summary>
public class BaseTower : MonoBehaviour
{
    [Header("基础属性")]
    public float range = 10f;
    public float fireRate = 1f;
    public float damage = 10f;
    
    [Header("目标")]
    public Transform currentTarget;
    
    protected float timer = 0f;
    protected bool hasTarget = false;

    void Update()
    {
        FindTarget();
        
        if (hasTarget && currentTarget != null)
        {
            timer += Time.deltaTime;
            
            if (timer >= fireRate)
            {
                Attack();
                timer = 0;
            }
        }
    }

    /// <summary>
    /// 寻找目标
    /// </summary>
    protected virtual void FindTarget()
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

        if (nearestEnemy != null)
        {
            currentTarget = nearestEnemy.transform;
            hasTarget = true;
        }
        else
        {
            currentTarget = null;
            hasTarget = false;
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    protected virtual void Attack()
    {
        if (currentTarget == null) return;

        BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        else
        {
            hasTarget = false;
        }
    }

    /// <summary>
    /// 绘制攻击范围（编辑器中可见）
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
