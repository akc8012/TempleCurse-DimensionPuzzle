using UnityEngine;
using System.Collections;

public class CamStraightPath : CamMovement
{
	public Transform targA;
	public Transform targB;
	public Transform lookAtTarget;
	public float speed = 10;
	public float snapSpeed = 0.25f;

	Vector3 targetA;
	Vector3 targetB;
	bool snapping = false;
	float distDenom;

	void Start()
	{
		targetA = targA.position;
		targetB = targB.position;
		distDenom = Vector3.Distance(transform.position, targetB);
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

		if (snapping)
			return;

		float input = inverted ? -Input.GetAxis("Spin Cube") : Input.GetAxis("Spin Cube");

		if (SoundManager.instance)
		{
			if (input != 0 && !SameLocation(targetA) && !SameLocation(targetB))
				SoundManager.instance.StartCoroutine("FadeInRumble");
			else
				SoundManager.instance.StartCoroutine("FadeOutRumble");
		}

		if (input == 0) { }
		else if (input < 0)
			transform.position = Vector3.MoveTowards(transform.position, targetB, Time.deltaTime * speed * Mathf.Abs(input));
		else if (input > 0)
			transform.position = Vector3.MoveTowards(transform.position, targetA, Time.deltaTime * speed * Mathf.Abs(input));

		transform.LookAt(lookAtTarget);


		if (Input.GetAxisRaw("Snap Cube") == 1)
		{
			Vector3 tempTarget = inverted ? targetA : targetB;
			if (SameLocation(tempTarget))
				return;

			StartCoroutine("SnapCamera", tempTarget);

			if (SoundManager.instance)
				SoundManager.instance.PlaySound(SoundManager.instance.snapWoosh);
		}
		if (Input.GetAxisRaw("Snap Cube") == -1)
		{
			Vector3 tempTarget = inverted ? targetB : targetA;
			if (SameLocation(tempTarget))
				return;

			StartCoroutine("SnapCamera", tempTarget);

			if (SoundManager.instance)
				SoundManager.instance.PlaySound(SoundManager.instance.snapWoosh);
		}

		GetPercentLoc();
	}

	IEnumerator SnapCamera(Vector3 targetPoint)
	{
		if (snapping)
			yield break;

		snapping = true;

		float moveAmount = Vector3.Distance(transform.position, targetPoint);
		float step = 0.0f;              //non-smoothed
		float rate = 1.0f / snapSpeed;  //amount to increase non-smooth step by
		float smoothStep = 0.0f;        //smooth step this time
		float lastStep = 0.0f;          //smooth step last time

		while (step < 1.0f)
		{
			step += Time.deltaTime * rate;
			smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step);
			transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveAmount * (smoothStep - lastStep));
			transform.LookAt(lookAtTarget);
			lastStep = smoothStep;
			yield return null;
		}

		//finish any left-over
		if (step > 1.0) transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveAmount * (1.0f - lastStep));
		snapping = false;
	}

	bool SameLocation(Vector3 compareTarget)
	{
		return (Vector3.Distance(transform.position, compareTarget) < 0.01f);
	}

	public float GetPercentLoc()
	{
		float currentDist = Vector3.Distance(transform.position, targetB);

		return (currentDist / distDenom);
	}
}