using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class PlatformHolder : MonoBehaviour
{
	public GameObject[] platforms;

	[HideInInspector]
	public DetectColliderSwitch[][] allPlatforms;

	int playerIndex;

	void Start()
	{
		// create an array to hold all the collider arrays
		allPlatforms = new DetectColliderSwitch[platforms.Length][];

		for (int i = 0; i < platforms.Length; i++)
		{
			// magically grab all the child objects (the platforms) of the parent group as an array, then throw them into this 2D array
			allPlatforms[i] = platforms[i].GetComponentsInChildren<DetectColliderSwitch>();
		}
	}

	public bool CheckCollider(int area, string name)			// this function might never even be helpful, but whatever
	{
		foreach (DetectColliderSwitch plat in allPlatforms[area])
		{
			if (plat.gameObject.name == name)
			{
				return plat.ColOn;
			}
		}

		return false;
	}

	public void SetPlayerIndex(int newIndex)
	{
		playerIndex = newIndex;
	}

	public int CheckPlayerIndex()
	{
		return playerIndex;
	}

	public int GetDimensionIndex(GameObject obj)
	{
		if (!obj.transform.parent) return -1;						// doesn't have a parent... can't give any info

		string parentName = obj.transform.parent.gameObject.name;
		string numString = Regex.Replace(parentName, "[^0-9]", "");	// replace anything that isn't nums 0-9 with empty string ""

		int returnNum;
		int.TryParse(numString, out returnNum);						// attempt to parse the string for an int, set returnNum to it, if this works

		return returnNum;
	}
}
