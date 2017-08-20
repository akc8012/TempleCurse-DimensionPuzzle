using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TheLastGem : Collectable
{
	public GameObject ceiling;

	float rotateSpeed = 30;
	float heightOffset = 1.1f;
	Transform player;
	bool doStick = false;

	void Start()
	{
		player = GameObject.FindWithTag("Player").GetComponent<Transform>();
	}

	void Update()
	{
		if (doStick)
		{
			Vector3 target = player.position;
			target.y = player.position.y + heightOffset;

			transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2);
		}
	}

	override protected void ItemStuff()
	{
		StartCoroutine("CollectMovement");
		LevelManager.instance.Pause(true, false, false);

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
		target.y = player.position.y + heightOffset;

		while (Vector3.Distance(transform.position, target) > 0.1f)
		{
			transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
			rotateSpeed += 300 * Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2.5f);

			yield return null;
		}

		transform.parent = null;
		doStick = true;
		ceiling.GetComponent<EndCeiling>().StartCoroutine("DoMoveOver");
	}
}