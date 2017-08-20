using UnityEngine;
using System.Collections;

public class DetectColliderSwitch : MonoBehaviour
{
	public bool isLadder;
	public bool isTrampoline;

	public GameObject[] shadowsToChange;
	public GameObject walls;				// only for trampoline

	Collider col;
	OneWayPlatform oneWayPlatform = null;
	int[] layers = {8, 0};
	bool oneWayShouldBe = false;

	public bool ColOn { get { return (oneWayPlatform) ? oneWayShouldBe : col.enabled; } }   // are we a oneWayPlatform? if so,
																							// we need to care about sending what we SHOULD be,
																							// without turning on or off. Otherwise, just return
																							// the actual collider condition

	void Start()
	{
		col = GetComponent<Collider>();
		if (GetComponent<OneWayPlatform>() && GetComponent<OneWayPlatform>().enabled)
			oneWayPlatform = GetComponent<OneWayPlatform>();
	}
	
	public void SetCollider(bool enabledBool)	// only accessed by PillarCompPlayer
	{
		if (oneWayPlatform == null)
			ActuallySetCol(enabledBool);
		else
		{
			oneWayShouldBe = enabledBool;	// even if we're not higher, we still gotta tell one way platform that we SHOULD be on due to dimension

			if (oneWayPlatform.Higher)
				ActuallySetCol(enabledBool);
		}

		if (isLadder && !enabledBool)
			GameObject.FindWithTag("Player").SendMessage("OnLadderSwitch", false);

		if (isTrampoline)
		{
			walls.SetActive(enabledBool);
			for (int i = 0; i < shadowsToChange.Length; i++)
			{
				shadowsToChange[i].layer = layers[System.Convert.ToInt32(enabledBool)];
			}
		}

	}

	public void ActuallySetCol(bool enabledBool)	// this script uses this, as well as OneWayPlatform
	{
		col.enabled = enabledBool;		// set both the real collider (col.enabled) and the public bool to send out (colOn) to enabledBool
		gameObject.layer = layers[System.Convert.ToInt32(enabledBool)];

		foreach (Transform child in transform)
		{
			child.gameObject.layer = layers[System.Convert.ToInt32(enabledBool)];
		}
	}
}