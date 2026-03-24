using UnityEngine;

/// <summary>
/// 激光塔（持续伤害）
/// </summary>
public class LaserTower : BaseTower
{
    [Header("激光设置")]
    public float damagePerSecond = 20f;
    public LineRenderer laserLine;
    public Color laserColor = Color.red;
    
    private new Transform currentTarget;
    private bool isFiring = false;

    void Start()
    {
        if (laserLine == null)
        {
            laserLine = GetComponent<LineRenderer>();
        }
        
        if (laserLine != null)
        {
            laserLine.enabled = false;
            laserLine.startWidth = 0.2f;
            laserLine.endWidth = 0.1f;
            laserLine.material = new Material(Shader.Find("Sprites/Default"));
            laserLine.startColor = laserColor;
            laserLine.endColor = laserColor;
        }
        
        fireRate = 0.1f; // 激光塔攻击频率高
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            Attack();
            timer = 0;
        }
        
        // 持续更新激光
        if (isFiring && currentTarget != null)
        {
            UpdateLaser();
            ApplyDamage();
        }
        else
        {
            DisableLaser();
        }
    }

	new void Attack()
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
            isFiring = true;
            
            if (laserLine != null)
            {
                laserLine.enabled = true;
            }
        }
        else
        {
            currentTarget = null;
            isFiring = false;
        }
    }

    void UpdateLaser()
    {
        if (laserLine != null && currentTarget != null)
        {
            laserLine.SetPosition(0, transform.position + Vector3.up * 0.5f);
            laserLine.SetPosition(1, currentTarget.position);
        }
    }

    void ApplyDamage()
    {
        if (currentTarget != null)
        {
            BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerSecond * Time.deltaTime);
            }
            else
            {
                currentTarget = null;
                isFiring = false;
            }
        }
    }

    void DisableLaser()
    {
        if (laserLine != null)
        {
            laserLine.enabled = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
