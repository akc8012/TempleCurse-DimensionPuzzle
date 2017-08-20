using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour
{
	public float activateDistance = 2;
	public SwitchableObject switchableObject;

	GameObject off;
	GameObject on;
	Transform player;
	bool playerInRange;
	public bool PlayerInRange { get { return playerInRange; } }

	void Start()
	{
		off = transform.Find("Off").gameObject;
		on = transform.Find("On").gameObject;

		player = GameObject.FindWithTag("Player").transform;
	}
	
	void Update()
	{
		playerInRange = Vector3.Distance(transform.position, player.position) < activateDistance;
	}

	public void TurnOn()
	{
		off.SetActive(false);
		on.SetActive(true);
		switchableObject.GetSwitched();
	}
}