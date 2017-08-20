using UnityEngine;
using System.Collections;

public class FadeTransition : Transition
{
	public override void StartOutThenDie()
	{
		// we need to do this gross stuff because after restarting the scene, the circle transition forgets about itself...
		// I dunno man, this just works. Don't judge me, okay?
		FadeTransition me = GameObject.Find("Fade Canvas(Clone)").GetComponent<FadeTransition>();

		if (me)
			me.StartCoroutine(me.OutThenDie());
	}

	IEnumerator OutThenDie()
	{
		anim.SetTrigger("Out");

		yield return new WaitForSeconds(0.8f);      // after this time, why don't you just KILL YOURSELF
		Destroy(gameObject);
		LevelManager.instance.Pause(false);
		LevelManager.instance.EnableRestart(true);
	}
}