using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDCanvasAdjustArcade : MonoBehaviour
{
	public Image[] stuffToSlide;

	void Start()
	{
		if (LevelManager.instance && !LevelManager.instance.ArcadeMode)
		{
			for (int i = 0; i < stuffToSlide.Length; i++)
			{
				stuffToSlide[i].rectTransform.position += Vector3.left*60;
			}
		}
	}
}