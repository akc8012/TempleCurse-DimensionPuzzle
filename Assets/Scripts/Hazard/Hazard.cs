using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (HealthManager.instance)
				HealthManager.instance.DecreaseLives(false);
		}
	}
}