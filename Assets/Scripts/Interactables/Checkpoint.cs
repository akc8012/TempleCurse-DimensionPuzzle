using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
	public Transform point;
	public GameObject[] stuffToEnable;
	public GameObject disableMesh;

	bool alreadyTouched = false;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !alreadyTouched && LevelManager.instance)
		{
			LevelManager.instance.SetCP(point);

			if (!HealthManager.instance.LostAGuy)
				SoundManager.instance.PlaySound(SoundManager.instance.collectCPSound);
			alreadyTouched = true;

			for (int i = 0; i < stuffToEnable.Length; i++)
			{
				stuffToEnable[i].SetActive(true);
			}
			disableMesh.SetActive(false);
		}
	}
}