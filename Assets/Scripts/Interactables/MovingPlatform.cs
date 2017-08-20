using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
	public Transform targetA;
	public Transform targetB;
	public float speed = 1;
	public float waitTime = 1;

	Vector3 followTarget;
	Vector3 lastPos;
	Vector3 direction;
	char currentTarget;
	public Vector3 Direction { get { return direction; } }

	Quaternion lastRot;

	void Start()
	{
		followTarget = targetA.position;
		currentTarget = 'A';

		StartCoroutine("ConstantStupidity");
	}
	
	void Update()
	{
		if (transform.rotation != lastRot)
			ResetCurrentTarget();

		lastPos = transform.position;
		transform.position = Vector3.MoveTowards(transform.position, followTarget, Time.deltaTime * speed);

		direction = transform.position - lastPos;


		if (currentTarget == 'A' && transform.position == targetA.position)
		{
			StartCoroutine("PauseThenSwitch", targetB.position);
			currentTarget = 'B';
		}

		if (currentTarget == 'B' && transform.position == targetB.position)
		{
			StartCoroutine("PauseThenSwitch", targetA.position);
			currentTarget = 'A';
		}

		lastRot = transform.rotation;
	}

	IEnumerator PauseThenSwitch(Vector3 newTarget)
	{
		yield return new WaitForSeconds(waitTime);
		followTarget = newTarget;
	}

	void ResetCurrentTarget()
	{
		if (currentTarget == 'A')
			followTarget = targetA.position;
		else
			followTarget = targetB.position;
	}

	IEnumerator ConstantStupidity()
	{
		yield return new WaitForSeconds(10);
		ResetCurrentTarget();

		StartCoroutine("ConstantStupidity");
	}
}