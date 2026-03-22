using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 敌人基础类
/// </summary>
public class BaseEnemy : MonoBehaviour
{
	public float hp = 100;

	public float speed = 3f;
	public Transform target;
	/// <summary>
	/// 受到伤害
	/// </summary>
	public void TakeDamage(float damage)
	{
		hp -= damage;

		Debug.Log(name + " 受到伤害: " + damage);

		if (hp <= 0)
		{
			Die();
		}
	}
	void Update()
	{
		if (target == null) return;

		Vector3 dir = (target.position - transform.position).normalized;
		transform.position += dir * speed * Time.deltaTime;

		transform.rotation = Quaternion.LookRotation(dir);
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