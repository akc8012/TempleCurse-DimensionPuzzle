using UnityEngine;
using System.Collections;

public class OneWayPlatform : MonoBehaviour
{
	public float playerHeight = 0.5f;

	Collider col;
	Transform player;
	DetectColliderSwitch detectColSwitch;
	float platformTop;
	bool playerHigher;

	public bool Higher { get { return playerHigher; } }

	void Start()
	{
		col = GetComponent<Collider>();
		player = GameObject.FindWithTag("Player").GetComponent<Transform>();
		detectColSwitch = GetComponent<DetectColliderSwitch>();

		platformTop = transform.position.y + (transform.localScale.y / 2);
	}
	
	void Update()
	{
		playerHigher = (player.position.y - playerHeight > platformTop);

		if (detectColSwitch == null)        // for platforms that aren't changed by dimension
			col.enabled = playerHigher;

		else if (playerHigher && detectColSwitch.ColOn || !playerHigher)      // only turn ON the platform if the collider should really be on, turn off whenever
			detectColSwitch.ActuallySetCol(playerHigher);	// need to use this function, because it handles shadow layer switching
			//col.enabled = playerHigher;
	}
}