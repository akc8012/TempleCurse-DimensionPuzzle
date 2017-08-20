using UnityEngine;
using System.Collections;

public class BeginCamera : MonoBehaviour
{
	Camera mainCam;
	CamMovement camMovement;
	float startOffset = 25;

	float normalSpeed = 3;
	float quickSpeed = 75;

	void Start()
	{
		mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		transform.position = mainCam.transform.position - (Vector3.up * startOffset);
		transform.rotation = mainCam.transform.rotation;
		camMovement = mainCam.GetComponent<CamMovement>();
		if (camMovement) camMovement.MovementEnabled(false);

		StartCoroutine("GetToCam");
	}

	void Update()
	{
		if (Input.GetButtonDown("Submit"))
		{
			StopCoroutine("GetToCam");
			StartCoroutine("GetToCamQUICK");
		}
	}

	void OnDestroy()
	{
		StuffToDoWhenReachCam();
	}

	IEnumerator GetToCam()
	{
		yield return new WaitForEndOfFrame();	// wait just onnnneee frame so we can be destroyed before playing sound if we're retrying the level

		if (SoundManager.instance && !HealthManager.instance.LostAGuy)
			SoundManager.instance.PlaySound(SoundManager.instance.woosh);

		while (Vector3.Distance(transform.position, mainCam.transform.position) > 0.01f)
		{
			transform.position = Vector3.Lerp(transform.position, mainCam.transform.position, Time.deltaTime * normalSpeed);
			yield return null;
		}

		StuffToDoWhenReachCam();
	}

	IEnumerator GetToCamQUICK()
	{
		while (Vector3.Distance(transform.position, mainCam.transform.position) > 0.01f)
		{
			transform.position = Vector3.MoveTowards(transform.position, mainCam.transform.position, Time.deltaTime * quickSpeed);
			yield return null;
		}

		StuffToDoWhenReachCam();
	}

	void StuffToDoWhenReachCam()
	{
		if (mainCam)
			mainCam.enabled = true;

		if (camMovement) camMovement.MovementEnabled(true);
		Destroy(GameObject.Find("Old WorldSpace Canvas"));
		GetComponent<Camera>().enabled = false;
		enabled = false;

		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
		{
			player.GetComponent<CharacterMotor>().EnableJump(true);
		}
	}
}