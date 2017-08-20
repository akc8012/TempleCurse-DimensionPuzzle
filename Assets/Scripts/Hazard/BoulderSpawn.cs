using UnityEngine;
using System.Collections;

public class BoulderSpawn : MonoBehaviour
{
	public Boulder[] boulders;
	public float timeSeparation;
	public float waitTime = 0;

	void Start()
	{
		StartCoroutine("StartSpawning");
	}
	
	IEnumerator StartSpawning()
	{
		yield return new WaitForSeconds(waitTime);

		foreach (Boulder boulder in boulders)
		{
			boulder.SendToSpawn(transform);
			yield return new WaitForSeconds(timeSeparation);
		}
	}
}