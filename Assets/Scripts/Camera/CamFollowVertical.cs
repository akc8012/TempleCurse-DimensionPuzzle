using UnityEngine;
using System.Collections;

public class CamFollowVertical : MonoBehaviour
{
	public float distanceY = 0;

	CharacterMotor charMotor;
	Transform playerTrans;
	float highest = -10;        // highest the player has ever been
	float heightOffset;
	bool lastTouchingFloor = false;		// compare states
	Vector3 targetPos;
	public float speed = 4;
	bool movementEnabled = true;

	void Start()
	{
		charMotor = GameObject.FindWithTag("Player").GetComponent<CharacterMotor>();
		playerTrans = GameObject.FindWithTag("Player").GetComponent<Transform>();

		heightOffset = transform.position.y;        // need to change this when the look at target isn't at 0, 0, 0
		highest = playerTrans.position.y;
	}

	void LateUpdate()
	{
		if (!movementEnabled)
			return;
		
		// we just started touching the floor again (the last frame, we weren't)
		if (charMotor.TouchingFloor && charMotor.TouchingFloor != lastTouchingFloor)
		{
			highest = playerTrans.position.y;
		}
		lastTouchingFloor = charMotor.TouchingFloor;

		// we're touching the floor, and we're now higher than the camera (fixes slope issues)
		if (charMotor.TouchingFloor && playerTrans.position.y > highest)
		{
			highest = playerTrans.position.y;
		}

		targetPos = new Vector3(transform.position.x, highest + heightOffset + distanceY, transform.position.z);
		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
	}

	public void MovementEnabled(bool enable)
	{
		movementEnabled = enable;
	}
}