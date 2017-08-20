using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircleTransition : Transition
{
	public RectTransform circleTransform;

	Transform player;
	Camera cam;
	Vector3 playerPos;
	bool isShrinking;

	void Start()
	{
		if (GameObject.FindWithTag("Player"))
		{
			isShrinking = true;
			player = GameObject.FindWithTag("Player").GetComponent<Transform>();
			cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

			playerPos = cam.WorldToScreenPoint(player.position);
			playerPos.z = 0;
		}

	}

	void Update()
	{
		if (isShrinking)
		{
			circleTransform.position = playerPos;
		}
	}

	public override void StartOutThenDie()
	{
		// we need to do this gross stuff because after restarting the scene, the circle transition forgets about itself...
		// I dunno man, this just works. Don't judge me, okay?
		CircleTransition me = GameObject.Find("Transition Canvas(Clone)").GetComponent<CircleTransition>();

		if (me)
		{
			me.isShrinking = false;
			me.StartCoroutine(me.OutThenDie());
			me.circleTransform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
		}
	}

	IEnumerator OutThenDie()
	{
		anim.SetTrigger("Grow");

		yield return new WaitForSeconds(1.5f);      // after this time, why don't you just KILL YOURSELF
		Destroy(gameObject);
		LevelManager.instance.Pause(false);
		LevelManager.instance.EnableRestart(true);
	}
}