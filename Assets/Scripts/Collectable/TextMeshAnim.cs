using UnityEngine;
using System.Collections;

public class TextMeshAnim : MonoBehaviour
{
	TextMesh textMesh;
	Vector3 target;
	float initialWaitTime = 0.15f;
	float height = 1.8f;
	float speed = 3.4f;
	float alphaFadeSpeed = 2;

	void Start()
	{
		target = transform.position + (Vector3.up * height);
		StartCoroutine("RiseUpAndFade");

		textMesh = GetComponentInChildren<TextMesh>();
	}
	
	IEnumerator RiseUpAndFade()
	{
		yield return new WaitForSeconds(initialWaitTime);
		float alphaFade = 1;
		
		while (Vector3.Distance(transform.position, target) > 0.01f)
		{
			transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
			textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alphaFade);
			alphaFade -= alphaFadeSpeed * Time.deltaTime;

			yield return null;
		}

		Destroy(gameObject);
	}
}