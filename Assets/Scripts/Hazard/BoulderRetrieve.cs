using UnityEngine;
using System.Collections;

public class BoulderRetrieve : MonoBehaviour
{
	public Transform spawnPoint;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Boulder")
		{
			Boulder boulder = other.gameObject.GetComponent<Boulder>();
			boulder.SendToSpawn(spawnPoint);
		}
	}
}