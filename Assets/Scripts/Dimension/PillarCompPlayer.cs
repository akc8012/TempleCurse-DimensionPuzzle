using UnityEngine;
using System.Collections;

public class PillarCompPlayer : MonoBehaviour
{
	public int pillarNum;

	Transform player;
	Camera cam;
	PlatformHolder platformHolder;
	bool onLeft;
	bool lastOn;
	int pillarAmount;
	bool swapRedYellow = false;
	public bool SwapRedYellow { set { swapRedYellow = value; } }

	void Start()
	{
		player = GameObject.FindWithTag("Player").transform;
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		platformHolder = GameObject.Find("PlatformHolder").GetComponent<PlatformHolder>();
		pillarAmount = GameObject.FindWithTag("MainCamera").GetComponent<ChooseClosestPillar>().PillarAmount;
	}
	
	void Update()
	{
		onLeft = (cam.WorldToScreenPoint(player.position).x < cam.WorldToScreenPoint(transform.position).x);	// on = is player.x < pillar.x?

		if (onLeft != lastOn)	// do this so that we're not doing all these loops (expensive) every frame, only when the player switches pillar side
		{
			SwitchSides();
		}

		lastOn = onLeft;
	}

	public void SwitchSides()
	{
		for (int i = pillarNum; i < pillarNum + 2; i++)     // only loops through twice, starting at pillarNum (same num of platform group), and pillarNum + 1 (always the next platform group we care about)
		{
			int ndx;
			if (pillarAmount == 4) ndx = (i > 3) ? (0) : (i);       // the last pillar (3) needs to loop through 3 and 0, so once the loop exceeds 3, set the index to 0
			else ndx = i;                                           // only want this loop around behavior to occur when pillars == 4

			bool enabledBool = onLeft ? (ndx == pillarNum) : (ndx != pillarNum);    // this makes sure whichever side the player is on, the correct platform groups will turn on or off

			if (swapRedYellow)      // special case for the rotating panel level
			{
				if (ndx == 1) ndx = 3;
				else if (ndx == 3) ndx = 1;
			}

			foreach (DetectColliderSwitch plat in platformHolder.allPlatforms[ndx])
			{
				if (plat.gameObject.activeInHierarchy)
					plat.SendMessage("SetCollider", enabledBool);
			}

			if (enabledBool)        // if enabled is true, that must mean the index we're on is the side the player is on
				platformHolder.SetPlayerIndex(ndx);
		}
	}

	public void SwitchColor(int ndx, bool enabledBool)
	{
		foreach (DetectColliderSwitch plat in platformHolder.allPlatforms[ndx])
		{
			if (plat.gameObject.activeInHierarchy)
				plat.SendMessage("SetCollider", enabledBool);
		}
	}
}
