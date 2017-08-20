using UnityEngine;
using System.Collections;

public class PlayerFallOut : MonoBehaviour
{
	void Update()
	{
		if (transform.position.y < -4)
		{
			transform.position = Vector3.zero;
		}
	}
}