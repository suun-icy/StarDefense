using UnityEngine;

public class PowerConnector : MonoBehaviour
{
	public float connectRadius = 5f;
	private PowerNode node;

	void Start()
	{
		node = GetComponent<PowerNode>();
		Invoke(nameof(Connect), 0.1f);
	}

	void Connect()
	{
		Collider[] cols = Physics.OverlapSphere(transform.position, connectRadius);

		foreach (var col in cols)
		{
			var other = col.GetComponent<PowerNode>();
			if (other != null && other != node)
			{
				if (!node.connections.Contains(other))
					node.connections.Add(other);

				if (!other.connections.Contains(node))
					other.connections.Add(node);
			}
		}
	}
}