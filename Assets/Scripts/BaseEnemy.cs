using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敌人基类（NavMesh + 状态机）
/// </summary>
public class BaseEnemy : MonoBehaviour
{
	public enum EnemyState
	{
		FollowPath,
		AttackEnergy,
		Dead
	}

	[Header("基础属性")]
	public float hp = 100;
	public float maxHp = 100;
	public float speed = 3f;
	public float damage = 1f;
	public bool canDestroyTerrain = false;

	[Header("AI")]
	public bool prioritizeEnergy = false;
	public Transform energyTarget;

	[Header("路径")]
	public Transform[] waypoints;
	private int currentWaypointIndex = 0;

	[Header("掉落")] 
	public int goldReward = 10;
	public int energyReward = 5;

	private NavMeshAgent agent;
	private EnemyState currentState = EnemyState.FollowPath;
	private bool isDead = false;

	public static int totalKills = 0;

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();

		if (agent == null) return;

		NavMeshHit hit;

		if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
		{
			agent.Warp(hit.position);
		}
	}

	void Update()
	{
		if (isDead || GameManager.Instance.IsPaused) return;

		// 状态切换
		if (prioritizeEnergy && energyTarget != null)
		{
			currentState = EnemyState.AttackEnergy;
		}

		switch (currentState)
		{
			case EnemyState.FollowPath:
				UpdateFollowPath();
				break;

			case EnemyState.AttackEnergy:
				UpdateAttackEnergy();
				break;
		}
	}

	#region 路径移动

	void UpdateFollowPath()
	{
		if (agent == null) return;
		// 判断
        if (!agent.isOnNavMesh) return;

		if (agent.pathPending) return;

		float dist = agent.remainingDistance;
		if (dist <= agent.stoppingDistance)
		{
			currentWaypointIndex++;

			if (currentWaypointIndex >= waypoints.Length)
			{
				ReachEnd();
				return;
			}

			MoveToNextWaypoint();
		}
	}

	void MoveToNextWaypoint()
	{
		if (waypoints == null || waypoints.Length == 0) return;

		agent.SetDestination(waypoints[currentWaypointIndex].position);
	}

	#endregion

	#region 能源攻击

	void UpdateAttackEnergy()
	{
		if (energyTarget == null)
		{
			currentState = EnemyState.FollowPath;
			return;
		}

		agent.SetDestination(energyTarget.position);

		float distance = Vector3.Distance(transform.position, energyTarget.position);

		if (distance < 2f)
		{
			AttackEnergyTarget();
		}
	}

	void AttackEnergyTarget()
	{
		Debug.Log("攻击能源建筑！");
		// TODO：调用能源系统
	}

	#endregion

	#region 受伤与死亡

	public void TakeDamage(float damage)
	{
		if (isDead) return;

		hp -= damage;

		if (hp <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		isDead = true;

		totalKills++;

		ResourceManager.Instance?.AddMaterial(goldReward);
		ResourceManager.Instance?.AddEnergy(energyReward);

		Destroy(gameObject);
	}

	#endregion

	void ReachEnd()
	{
		GameManager.Instance?.OnEnemyReachEnd(damage);
		Destroy(gameObject);
	}
}