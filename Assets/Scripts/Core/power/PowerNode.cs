using System.Collections.Generic;
using UnityEngine;

public class PowerNode : MonoBehaviour
{
	public List<PowerNode> connections = new List<PowerNode>();

	[Header("是否为电源（发电站）")]
	public bool isSource = false;

	[HideInInspector]
	public bool isPowered = false;

	private IPowerUser powerUser;

	void Awake()
	{
		powerUser = GetComponent<IPowerUser>();
		PowerManager.Instance.RegisterNode(this);
	}

	public void SetPowered(bool powered)
	{
		if (isPowered == powered) return;

		isPowered = powered;

		// 通知用电设备
		if (powerUser != null)
		{
			if (isPowered)
				powerUser.OnPowerOn();
			else
				powerUser.OnPowerOff();
		}
	}

	private void OnDestroy()
	{
		if (PowerManager.Instance != null)
			PowerManager.Instance.UnregisterNode(this);
	}
}