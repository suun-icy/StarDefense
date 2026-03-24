using UnityEngine;

[RequireComponent(typeof(PowerNode))]
public class PowerPlant : MonoBehaviour
{
	void Start()
	{
		GetComponent<PowerNode>().isSource = true;
	}
}