using UnityEngine;
using System.Collections;

public class ScoreCollectable : Collectable
{
	public GameObject scoreTextMesh;
	public GameObject particleBurst;

	override protected void ItemStuff()
	{
		if (ScoreManager.instance)
			ScoreManager.instance.IncreaseScore(100);

		Instantiate(scoreTextMesh, transform.position + Vector3.up * 0.6f, Quaternion.identity);
		Instantiate(particleBurst, transform.position, Quaternion.identity);

		if (SoundManager.instance)
			SoundManager.instance.PlaySound(SoundManager.instance.collectSound);
	}
}