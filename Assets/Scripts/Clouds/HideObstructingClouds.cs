using UnityEngine;
using System.Collections;

public class HideObstructingClouds : MonoBehaviour
{
	public Vector3 size = new Vector3(1, 1, 1);
	public float yOffset = 0;

	Transform[] clouds;

	void Start()
	{
		Invoke("Stuff", 0.25f);		// wait until, you know, the clouds show up
	}

	void Stuff()
	{
		clouds = GetComponentsInChildren<Transform>();

		for (int i = 0; i < clouds.Length; i++)
		{
			if (clouds[i].name != gameObject.name)		// GetComponentsInChildren includes the root parent,
			{                                           // so make sure we don't check that
				if (isInsideBox(clouds[i].position))
				{
					clouds[i].gameObject.SetActive(false);
				}
			}
		}
	}
	
	bool isInsideBox(Vector3 pos)
	{
		return (pos.x > -(size.x/2) &&
				pos.x < (size.x/2) &&

				pos.y > -(size.y/2) + yOffset &&
				pos.y < (size.y/2) + yOffset &&

				pos.z > -(size.z/2) &&
				pos.z < (size.z/2));
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5F);
		Gizmos.DrawCube(transform.position + Vector3.up * yOffset, size);
	}
}