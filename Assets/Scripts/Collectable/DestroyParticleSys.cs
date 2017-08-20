using UnityEngine;
using System.Collections;

public class DestroyParticleSys : MonoBehaviour
{
	float lifeTime;

	void Start()
	{
		lifeTime = GetComponent<ParticleSystem>().startLifetime;
		StartCoroutine("Countdown");
	}
	
	IEnumerator Countdown()
	{
		yield return new WaitForSeconds(lifeTime);
		Destroy(gameObject);
	}
}