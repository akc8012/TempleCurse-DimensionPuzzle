using UnityEngine;
using System.Collections;

public class CanvasCalcOffset : MonoBehaviour
{
	public float zOffset;
	public float offsetAngle = 0;

	Transform cam;
	bool switchOccured = false;
	float beginCamStartY;

	void Start()
	{
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
	}

	void Update()
	{
		transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
		transform.Rotate(0, offsetAngle, 0);

		Vector3 offset = cam.position + cam.forward * zOffset;
		if (!switchOccured)
			offset.y = transform.position.y;
		else
			offset.y = beginCamStartY;

		transform.position = offset;
	}

	public void SwitchCamera()
	{
		cam = GameObject.Find("Begin Camera").GetComponent<Transform>();
		beginCamStartY = cam.transform.position.y;
		switchOccured = true;
	}
}