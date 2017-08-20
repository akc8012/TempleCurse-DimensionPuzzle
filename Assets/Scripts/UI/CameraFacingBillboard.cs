using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
	public float offsetAngle = 0;

	Transform cam;

	void Start()
	{
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
	}

	void Update()
	{
		transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
		transform.Rotate(0, offsetAngle, 0);
	}
}