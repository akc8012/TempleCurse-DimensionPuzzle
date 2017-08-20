using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GemCollectable : Collectable
{
	float rotateSpeed = 30;
	float heightOffset = 1.1f;

	override protected void ItemStuff()
	{
		StartCoroutine("CollectMovement");
		LevelManager.instance.Pause(true);

		if (SoundManager.instance)
		{
			SoundManager.instance.StartCoroutine("FadeOutMusic");
			SoundManager.instance.PlaySound(SoundManager.instance.collectGemSound);
		}
	}

	IEnumerator CollectMovement()
	{
		GetComponent<CollectableMovement>().enabled = false;
		GetComponent<Collider>().enabled = false;

		Vector3 target = transform.position;
		target.y = GameObject.FindWithTag("Player").transform.position.y + heightOffset;

		while (Vector3.Distance(transform.position, target) > 0.1f)
		{
			transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
			rotateSpeed += 300 * Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2.5f);

			yield return null;
		}

		GameObject.FindWithTag("MainCamera").GetComponent<CamLevelTrans>().StartCoroutine("MoveUp");
	}
}