using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeyIdiotBox : MonoBehaviour
{
	public Image heyIdiotImg;

	bool canStart = true;
	bool timerStarted = false;
	float fadeInSpeed = 3;

	void Update()
	{
		if (Input.GetAxis("Spin Cube") != 0)
		{
			if (!canStart) return;
			canStart = false;

			if (timerStarted)
			{
				StopCoroutine("WaitForDummy");
			}
			if (heyIdiotImg.gameObject.activeInHierarchy)
			{
				StartCoroutine("FadeOut");
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && !timerStarted && canStart)
		{
			StartCoroutine("WaitForDummy");
		}
	}

	IEnumerator WaitForDummy()
	{
		timerStarted = true;
		yield return new WaitForSeconds(3);
		StartCoroutine("FadeIn");
	}

	IEnumerator FadeIn()
	{
		heyIdiotImg.gameObject.SetActive(true);
		heyIdiotImg.color = new Color(1, 1, 1, 0);
		float incAlpha = 0;

		while(heyIdiotImg.color.a < 0.9f)
		{
			heyIdiotImg.color = new Color(1, 1, 1, incAlpha);
			incAlpha += fadeInSpeed * Time.deltaTime;
			yield return null;
		}

		heyIdiotImg.color = new Color(1, 1, 1, 1);
	}

	IEnumerator FadeOut()
	{
		heyIdiotImg.gameObject.SetActive(true);
		heyIdiotImg.color = new Color(1, 1, 1, 1);
		float decAlpha = 1;

		while (heyIdiotImg.color.a > 0.1f)
		{
			heyIdiotImg.color = new Color(1, 1, 1, decAlpha);
			decAlpha -= fadeInSpeed * 2 * Time.deltaTime;
			yield return null;
		}

		heyIdiotImg.color = new Color(1, 1, 1, 0);
	}
}