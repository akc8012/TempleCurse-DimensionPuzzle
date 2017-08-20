using UnityEngine;
using System.Collections;

public class TimepieceCollectable : Collectable
{
	public GameObject timeTextMesh;
	public GameObject particleBurst;
	TimeManager timeManager;

	void Start()
	{
		timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
	}

	override protected void ItemStuff()
	{
		Instantiate(timeTextMesh, transform.position + Vector3.up * 3, Quaternion.identity);
		Instantiate(particleBurst, transform.position + (Vector3.up * 2.5f), Quaternion.identity);
		timeManager.AddTime(10);

		if (SoundManager.instance)
			SoundManager.instance.PlaySound(SoundManager.instance.collectSound);
	}
}