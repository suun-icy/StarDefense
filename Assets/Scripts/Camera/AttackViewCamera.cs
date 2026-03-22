using UnityEngine;

/// <summary>
/// 攻击视角（后续用于导弹/坦克）
/// </summary>
public class AttackViewCamera : MonoBehaviour
{
	public Transform target;

	void LateUpdate()
	{
		if (target == null) return;

		transform.position = target.position - target.forward * 6 + Vector3.up * 3;
		transform.LookAt(target);
	}
}