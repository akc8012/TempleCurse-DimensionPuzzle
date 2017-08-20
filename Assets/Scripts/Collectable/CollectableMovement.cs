using UnityEngine;
using System.Collections;

public class CollectableMovement : MonoBehaviour
{
	public float rotateSpeed = 30;
	public float moveSpeed = 0.1f;
	public float height = 1;

	Vector3 targetA;
	Vector3 targetB;
	bool enableMove = true;

	void Start()
	{
		targetA = transform.position;
		targetB = transform.position + (transform.up * height);
	}
	
	void Update()
	{
		if (!enableMove)
			return;

		transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

		float weight = Mathf.Cos(Time.time * moveSpeed * 2 * Mathf.PI) * 0.5f + 0.5f;
		transform.position = targetA * weight + targetB * (1 - weight);
	}

	public void PauseMovement(bool enable)
	{
		enableMove = !enable;

		if (enableMove)
		{
			targetA = transform.position;
			targetB = transform.position + (transform.up * height);
		}
	}
}