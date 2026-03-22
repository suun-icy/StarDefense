using UnityEngine;

/// <summary>
/// ·ÀÓùËþ»ù´¡£¨¼ò»¯°æ£©
/// </summary>
public class BaseTower : MonoBehaviour
{
	public float range = 10f;
	public float fireRate = 1f;

	float timer;

	void Update()
	{
		timer += Time.deltaTime;

		if (timer >= fireRate)
		{
			Attack();
			timer = 0;
		}
	}

	void Attack()
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, range);

		foreach (var hit in hits)
		{
			BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
			if (enemy != null)
			{
				enemy.TakeDamage(10);
				break;
			}
		}
	}
}