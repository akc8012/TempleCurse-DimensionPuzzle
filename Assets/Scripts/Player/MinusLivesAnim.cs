using UnityEngine;
using System.Collections;

public class MinusLivesAnim : MonoBehaviour
{
	Camera cam;
	TextMesh textMesh;
	float initialWaitTime = 0.8f;
	float speed = 40;
	float fadeAlpha = 1;
	float fadeSpeed = 0.2f;
	int screenCornerDist = 40;

	void Start()
	{
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		textMesh = GetComponentInChildren<TextMesh>();
		
		Transform player = GameObject.FindWithTag("Player").GetComponent<Transform>();
		transform.position = player.position + (Vector3.up * 1.2f);

		StartCoroutine("FlyAndFadeToCorner");
		StartCoroutine("Flash");
	}

	IEnumerator FlyAndFadeToCorner()
	{
		yield return new WaitForSeconds(initialWaitTime);

		Vector3 screenPoint = new Vector3(screenCornerDist, Screen.height - screenCornerDist, Vector3.Distance(transform.position, cam.transform.position));

		Vector3 target = cam.ScreenToWorldPoint(screenPoint);

		while (Vector3.Distance(transform.position, target) > 0.01f)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
			textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, fadeAlpha);
			fadeAlpha -= fadeSpeed * Time.deltaTime;
			yield return null;
		}

		HealthManager.instance.LoseAGuy();
		LevelManager.instance.OneReachedCorner();
		Destroy(gameObject);
	}

	IEnumerator Flash()
	{
		Color startColor = textMesh.color;
		bool set = false;

		while(true)
		{
			if (set)
				textMesh.color = new Color(1, 0, 0, fadeAlpha);
			else
				textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);

			set = !set;
			yield return new WaitForSeconds(0.1f);
		}
	}
}