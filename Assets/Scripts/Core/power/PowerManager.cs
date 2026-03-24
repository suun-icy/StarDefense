using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
	public static PowerManager Instance;

	private List<PowerNode> allNodes = new List<PowerNode>();

	void Awake()
	{
		Instance = this;
	}

	public void RegisterNode(PowerNode node)
	{
		if (!allNodes.Contains(node))
			allNodes.Add(node);
	}

	public void UnregisterNode(PowerNode node)
	{
		allNodes.Remove(node);
	}

	void Update()
	{
		UpdatePowerGrid();
	}

	void UpdatePowerGrid()
	{
		// 1先全部断电
		foreach (var node in allNodes)
		{
			node.SetPowered(false);
		}

		// 2从所有电源开始 BFS
		Queue<PowerNode> queue = new Queue<PowerNode>();

		foreach (var node in allNodes)
		{
			if (node.isSource)
			{
				node.SetPowered(true);
				queue.Enqueue(node);
			}
		}

		// 2扩散供电
		while (queue.Count > 0)
		{
			var current = queue.Dequeue();

			foreach (var next in current.connections)
			{
				if (next == null) continue;

				if (!next.isPowered)
				{
					next.SetPowered(true);
					queue.Enqueue(next);
				}
			}
		}
	}
}