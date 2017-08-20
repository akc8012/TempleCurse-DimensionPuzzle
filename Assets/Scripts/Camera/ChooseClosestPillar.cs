using UnityEngine;
using System.Collections;

public class ChooseClosestPillar : MonoBehaviour
{
	public GameObject[] pillars;

	float[] diststances;			// contains pillars distances to camera, must always be updated, parallel to the pillars array
	int closestPillar = 0;          // tells us the index of the current closest pillar
	int pillarAmount;
	public int PillarAmount { get { return pillarAmount; } }

	void Start()
	{
		pillarAmount = pillars.Length;

		diststances = new float[] { 0.0f, 0.0f, 0.0f, 0.0f };
		ChangePillar(closestPillar);
	}

	void Update()
	{
		diststances[closestPillar] = Vector3.Distance(transform.position, pillars[closestPillar].transform.position);
		Debug.DrawRay(pillars[closestPillar].transform.position, Vector3.up * 100, Color.blue);

		for (int i = 0; i < pillars.Length; i++)
		{
			if (i == closestPillar && i != pillars.Length-1) i++;

			diststances[i] = Vector3.Distance(transform.position, pillars[i].transform.position);

			if (diststances[i] < diststances[closestPillar])
			{
				ChangePillar(i);
			}
		}
	}

	void ChangePillar(int newClosestPillar)
	{
		closestPillar = newClosestPillar;

		for (int i = 0; i < pillars.Length; i++)
		{
			if (i == newClosestPillar) pillars[i].GetComponent<PillarCompPlayer>().enabled = true;
			else pillars[i].GetComponent<PillarCompPlayer>().enabled = false;
		}
	}

	public void SwapRedYellow()
	{
		for (int i = 0; i < pillars.Length; i++)
		{
			pillars[i].GetComponent<PillarCompPlayer>().SwapRedYellow = true;
			pillars[i].GetComponent<PillarCompPlayer>().SwitchSides();
			pillars[i].GetComponent<PillarCompPlayer>().SwitchColor(1, false);
			pillars[i].GetComponent<PillarCompPlayer>().SwitchColor(2, false);
		}
	}
}
