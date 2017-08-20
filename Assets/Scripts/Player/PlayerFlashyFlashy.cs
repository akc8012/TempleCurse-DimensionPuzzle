using UnityEngine;
using System.Collections;

public class PlayerFlashyFlashy : MonoBehaviour
{
	public Renderer rend;
	Color origColor = new Color(0, 0, 0, 0);

	void Start()
	{
		// rend = GetComponent<Renderer>();
	}

	void OnDestroy()
	{
		if (origColor != new Color(0, 0, 0, 0))     // if color has been set to player's color
		{
			rend.sharedMaterial.SetColor("_Color", origColor);
		}
	}

	public IEnumerator FlashyFlashy()
	{
		int count = 7;
		origColor = rend.sharedMaterial.GetColor("_Color");
		Color targetColor;

		while (count > 0)
		{
			count--;
			if (count % 2 == 0)
				targetColor = Color.red;
			else
				targetColor = origColor;

			rend.sharedMaterial.SetColor("_Color", targetColor);
			yield return new WaitForSeconds(0.15f);
		}

		rend.sharedMaterial.SetColor("_Color", origColor);
	}
}