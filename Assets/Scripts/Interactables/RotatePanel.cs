using UnityEngine;
using System.Collections;

public class RotatePanel : MonoBehaviour
{
	public bool swapRedYellow;
	public GameObject button;

	RotateBox box;
	bool active = false;

	void Start()
	{
		box = GameObject.Find("Box").GetComponent<RotateBox>();
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if (!active)
			{
				box.StartCoroutine("RotateForward");
				active = true;
				button.SetActive(false);
				SoundManager.instance.PlaySound(SoundManager.instance.panelStep);

				if (swapRedYellow)
				{
					GameObject.FindWithTag("MainCamera").GetComponent<ChooseClosestPillar>().SwapRedYellow();
					GameObject.Find("CloudsToy Mngr").GetComponent<CloudColor>().FlipRedYellow();
				}
			}
		}
	}
}