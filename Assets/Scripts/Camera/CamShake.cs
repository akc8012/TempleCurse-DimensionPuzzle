using UnityEngine;
using System.Collections;

public class CamShake : MonoBehaviour
{
	CamMovement camMovement;
	Vector3 startPos;
	float intensity = 0.25f;

	void Start()
	{
		camMovement = GetComponent<CamMovement>();
	}

	public IEnumerator ShakeCam(float duration)
	{
		camMovement.MovementEnabled(false);
		startPos = transform.position;
		float startTime = Time.time;

		while ((Time.time - startTime) < duration)
		{
			transform.position = (Random.insideUnitSphere * intensity) + startPos;
			yield return null;
		}

		transform.position = startPos;

		if (!LevelManager.instance.BeingRestarted && !LevelManager.instance.Paused)
			camMovement.MovementEnabled(true);
	}
}