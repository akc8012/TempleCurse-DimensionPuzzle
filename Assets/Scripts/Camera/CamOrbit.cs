using UnityEngine;
using System.Collections;

public class CamOrbit : CamMovement
{
	public float speed = 50;
	public float snapSpeed = 0.25f;
	public bool rotationLevel = false;

	Transform target;

	public GameObject camTargetPrefab;
	GameObject camTargetPointR;
	GameObject camTargetPointL;
	Vector3 startPos;
	Quaternion startRot;
	bool snapping = false;
	int lastClosestIndex = 12;  // make sure the points move to position at least once at start
	enum SnapStates { NoPress, PressOnce, PressCrazy };
	SnapStates snapState = SnapStates.PressOnce;
	float snapWaitTime = 0.6f;

	void Start()
	{
		if (GameObject.Find("Box"))
			target = GameObject.Find("Box").transform;
		else
			print("The camera needs a Box object as a target!");

		camTargetPointR = Instantiate(camTargetPrefab);
		camTargetPointR.transform.position = transform.position;
		camTargetPointR.transform.rotation = transform.rotation;
		camTargetPointR.name = "camTargetPointR";
		startPos = camTargetPointR.transform.position;
		startRot = camTargetPointR.transform.rotation;

		camTargetPointL = Instantiate(camTargetPrefab);
		camTargetPointL.transform.position = transform.position;
		camTargetPointL.transform.rotation = transform.rotation;
		camTargetPointL.name = "camTargetPointL";
	}

	void Update()
	{
		if (!movementEnabled)
		{
			if (SoundManager.instance)
				SoundManager.instance.StartCoroutine("FadeOutRumble");

			if (snapping)
			{
				StopCoroutine("SnapCamera");
				snapping = false;
			}

			return;
		}

		SetPointPosition();
		if (snapping)
			return;

		float horizontal = inverted ? -Input.GetAxis("Spin Cube") : Input.GetAxis("Spin Cube");

		if (SoundManager.instance)
		{
			if (horizontal != 0)
				SoundManager.instance.StartCoroutine("FadeInRumble");
			else
				SoundManager.instance.StartCoroutine("FadeOutRumble");
		}

		if (snapState == SnapStates.PressOnce || snapState == SnapStates.PressCrazy)
		{
			if (Input.GetAxisRaw("Snap Cube") == 1)
			{
				StartCoroutine("SnapCamera", inverted ? camTargetPointL.transform.rotation : camTargetPointR.transform.rotation);
				StartCoroutine("SnapAgainTimer");

				if (SoundManager.instance)
					SoundManager.instance.PlaySound(SoundManager.instance.snapWoosh);
			}
			if (Input.GetAxisRaw("Snap Cube") == -1)
			{
				StartCoroutine("SnapCamera", inverted ? camTargetPointR.transform.rotation : camTargetPointL.transform.rotation);
				StartCoroutine("SnapAgainTimer");

				if (SoundManager.instance)
					SoundManager.instance.PlaySound(SoundManager.instance.snapWoosh);
			}
		}
		
		if (Input.GetAxisRaw("Snap Cube") == 0)
		{
			StopCoroutine("SnapAgainTimer");
			snapState = SnapStates.PressOnce;
		}

		transform.LookAt(transform);
		transform.RotateAround(rotationLevel ? Vector3.zero : target.position, Vector3.up, speed * horizontal * Time.deltaTime);
	}

	IEnumerator SnapAgainTimer()
	{
		if (snapState == SnapStates.PressCrazy)
			yield break;

		snapState = SnapStates.NoPress;
		yield return new WaitForSeconds(snapWaitTime);
		snapState = SnapStates.PressCrazy;
	}

	// http://answers.unity3d.com/questions/29110/easing-a-rotation-of-rotate-around.html
	IEnumerator SnapCamera(Quaternion targetPoint)
	{
		if (snapping)
			yield break;

		snapping = true;

		float rotateAmount = Quaternion.Angle(transform.rotation, targetPoint);
		float step = 0.0f;				//non-smoothed
		float rate = 1.0f / snapSpeed;	//amount to increase non-smooth step by
		float smoothStep = 0.0f;		//smooth step this time
		float lastStep = 0.0f;          //smooth step last time
		float direction = targetPoint == camTargetPointR.transform.rotation ? -1 : 1;

		while (step < 1.0f)
		{
			step += Time.deltaTime * rate;						//increase the step
			smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step);    //get the smooth step
			transform.RotateAround(rotationLevel ? Vector3.zero : target.position, Vector3.up, direction * rotateAmount * (smoothStep - lastStep));
			lastStep = smoothStep;                              //store the smooth step
			yield return null;
		}

		//finish any left-over
		if (step > 1.0) transform.RotateAround(target.position, Vector3.up, direction * rotateAmount * (1.0f - lastStep));
		snapping = false;
	}

	// Set the point positions on a case-by-case basis,
	// based on the camera's euler y
	void SetPointPosition()
	{
		//if (snapping) return;

		int[] compareAngles = new int[] { 0, 90, 180, 270 };
		float closestAngle = 10000;
		int closestIndex = 0;
		for (int i = 0; i < compareAngles.Length; i++)
		{
			float angle = Quaternion.Angle(Quaternion.Euler(0, transform.eulerAngles.y, 0), Quaternion.Euler(0, compareAngles[i], 0));
			
			if (angle < closestAngle)
			{
				closestAngle = angle;
				closestIndex = i;
			}
		}

		if (closestIndex == lastClosestIndex)
			return;

		camTargetPointR.transform.position = startPos;
		camTargetPointR.transform.rotation = startRot;
		camTargetPointL.transform.position = startPos;
		camTargetPointL.transform.rotation = startRot;

		if (compareAngles[closestIndex] == 0)
		{
			camTargetPointR.transform.RotateAround(target.position, Vector3.up, 270);
			camTargetPointL.transform.RotateAround(target.position, Vector3.up, 90);
		}
		else if (compareAngles[closestIndex] == 90)
		{
			// keep R at 0
			camTargetPointL.transform.RotateAround(target.position, Vector3.up, 180);
		}
		else if (compareAngles[closestIndex] == 180)
		{
			camTargetPointR.transform.RotateAround(target.position, Vector3.up, 90);
			camTargetPointL.transform.RotateAround(target.position, Vector3.up, 270);
		}
		else if (compareAngles[closestIndex] == 270)
		{
			camTargetPointR.transform.RotateAround(target.position, Vector3.up, 180);
			// keep L at 0
		}

		lastClosestIndex = closestIndex;
	}

	public IEnumerator EndingSpin()
	{
		while (true)
		{
			transform.LookAt(transform);
			transform.RotateAround(target.position, Vector3.up, -(speed/1.5f) * Time.deltaTime);

			yield return null;
		}
	}
}
