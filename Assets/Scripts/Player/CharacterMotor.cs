using UnityEngine;
using System.Collections;

public class CharacterMotor : MonoBehaviour
{
	public bool xAxisOnly;

	CharacterController controller;
	Transform cam;
	Animator anim;

	Vector3 velocity = Vector3.zero;
	float moveSpeed = 10;            // what to increment velocity by
	float maxVel = 5;       // maximum velocity in any direction
	float rotSmooth = 20;    // smoothing on the lerp to rotate towards stick direction
	float gravity = 35;
	float jumpSpeed = 11;
	bool isGrounded = false;
	bool jumpKeyUp = true;
	bool touchingFloor = false;		// used for vertical camera
	public bool TouchingFloor { get { return touchingFloor; } }     // read-only accessor

	bool onLadder = false;
	Vector3 ladderForward = Vector3.forward;
	bool enableMovement = true;
	bool playedLandingSfx = false;
	bool enableJump = false;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		cam = GameObject.FindWithTag("MainCamera").transform;
		anim = GetComponentInChildren<Animator>();
	}

	void Update()
	{
		if (!enableMovement || !controller.enabled) return;

		if (Input.GetKeyDown(KeyCode.P))
		{
			enableJump = !enableJump;
		}

		float speed = 0.0f;	
		Vector3 moveDir = GetInput(ref speed);
		Rotate(moveDir, speed);
		speed *= moveSpeed;

		float lastVelY = velocity.y;
		velocity = transform.forward * speed;
		velocity = Vector3.ClampMagnitude(velocity, maxVel);
		velocity.y = lastVelY;

		isGrounded = controller.isGrounded;

		if (Input.GetButton("Jump") && isGrounded && jumpKeyUp && enableJump)
		{
			velocity.y = jumpSpeed;
			anim.SetTrigger("Jumping");
			if (SoundManager.instance) SoundManager.instance.PlaySound(SoundManager.instance.jumpSound);
			jumpKeyUp = false;
		}
		if (Input.GetButtonUp("Jump")) jumpKeyUp = true;

		if (!isGrounded) velocity.y -= gravity * Time.deltaTime;
		else velocity.y = (velocity.y < -1) ? -0.05f : velocity.y;  // settting vel.y to -0.05f to make sure negative vel stops building
																	// up once player touches the ground (only happens once upon touchdown)
																	// while still ensuring negative vel is great enough to satisfy isGrounded
		if (onLadder) velocity.y += gravity * Time.deltaTime;       // stops "sinking" on ladder

		controller.Move(velocity * Time.deltaTime);
		DownRay();
	}

	void DownRay()
	{
		touchingFloor = false;

		Vector3 center = new Vector3(controller.bounds.center.x, controller.bounds.center.y - controller.bounds.extents.y, controller.bounds.center.z);

		Ray downRay = new Ray(center, Vector3.down);
		RaycastHit hitInfo;

		if (Physics.Raycast(downRay, out hitInfo, controller.skinWidth * 1.3f))
		{
			touchingFloor = true;
			
			if (hitInfo.collider.gameObject.tag == "MovingPlatform")
				RideAlongMovePlat(hitInfo.collider.gameObject.GetComponent<MovingPlatform>());

			if (hitInfo.collider.gameObject.tag == "Boulder")
				playedLandingSfx = true;	// don't play landing sfx when jump over boulder
		}

		anim.SetBool("TouchingFloor", touchingFloor);

		if (touchingFloor)
		{
			if (!playedLandingSfx && velocity.y <= 0)
			{
				if (SoundManager.instance)
					SoundManager.instance.PlaySound(SoundManager.instance.landing);
				playedLandingSfx = true;
			}
		}
		else
			playedLandingSfx = false;

		Debug.DrawRay(center, Vector3.down * controller.skinWidth * 1.3f, Color.magenta);
	}

	Vector3 GetInput(ref float speed)
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		Vector3 stickDir = new Vector3(horizontal, 0, (xAxisOnly) ? 0 : vertical);        // if we're xAxisOnly, set z to 0

		if (onLadder)
		{
			speed = vertical;
			speed *= moveSpeed * 0.3f;
			velocity = transform.up * speed;

			stickDir.z = 0;
		}

		speed = Mathf.Clamp(Vector3.Magnitude(stickDir), 0, 1);     // make sure we can't exceed 1 (diagonals)

		if (!onLadder)
		{
			anim.SetFloat("Ladder", 0);
			anim.SetFloat("Walking", speed);
		}
		else
		{
			anim.SetFloat("Walking", 0);
			anim.SetFloat("Ladder", Mathf.Abs(vertical));

			if (SoundManager.instance)
			{
				if (Mathf.Abs(vertical) > 0)
					SoundManager.instance.PlayLadderSound();
				else
					SoundManager.instance.StopLadderSound();
			}
		}

		// get camera rotation
		Vector3 cameraDir = cam.forward;
		cameraDir.y = 0.0f;                 // cameraDir is the camera's forward vector, with the y removed
		Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDir);    // creates a rotation that describes how to take forward (0,0,1), and rotate it so that it is facing the same direction as the camera
																								// we do this so that in the next line, no matter where the camera is, pushing up will always move the player forward
																								// convert joystick input in Worldspace coordinates

		Vector3 moveDir = (xAxisOnly) ? stickDir : referentialShift * stickDir;      // multiplying a quaternion by a vector applies that rotation to the vector
															// here, we end up rotating the stick direction by the rotation it takes from forward to reach the camera's rotation (referentialShift)
															// WHY?: This rotates the stickDir around so that moveDir will be relative to the camera, not to the world

		// fixes bug when the camera forward is exactly -forward (opposite to Vector3.forward) by flipping the x around
		if (Vector3.Dot(Vector3.forward, cameraDir.normalized) == -1) moveDir = new Vector3(-moveDir.x, moveDir.y, moveDir.z);

		Debug.DrawRay(transform.position, stickDir * 2, Color.blue);
		Debug.DrawRay(transform.position + Vector3.up, moveDir * 2, Color.green);
		
		return moveDir;
	}

	void Rotate(Vector3 moveDir, float speed)
	{
		if (onLadder && moveDir.x == 0)
		{
			Vector3 targetRotation = Vector3.Lerp(transform.forward, ladderForward, Time.deltaTime * rotSmooth);
			transform.rotation = Quaternion.LookRotation(targetRotation);
			return;
		}

		if (Vector3.Angle(moveDir, transform.forward) > 135)		// if the difference is above a certain angle,
			transform.forward = moveDir;							// we'll want to snap right to it, instead of lerping
		else {
			Vector3 targetRotation = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * rotSmooth);
			if (targetRotation != Vector3.zero) transform.rotation = Quaternion.LookRotation(targetRotation);
		}
	}

	void RideAlongMovePlat(MovingPlatform movePlat)
	{
		Vector3 dir = movePlat.Direction;
		dir.y = -0.05f;		// ensure that we stay grounded on the platform

		controller.Move(dir);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Ladder")
		{
			onLadder = true;
			ladderForward = other.gameObject.transform.forward * -1;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Ladder")
		{
			onLadder = false;

			if (SoundManager.instance)
				SoundManager.instance.StopLadderSound();
		}
	}

	void OnLadderSwitch(bool enabled)
	{
		onLadder = enabled;

		if (!enabled && SoundManager.instance)
			SoundManager.instance.StopLadderSound();
	}

	public void EnableMovement(bool enable)
	{
		enableMovement = enable;
		controller.enabled = enable;

		if (!enable)
		{
			anim.SetFloat("Walking", 0);    // HEY, STOP IT WITH THE WALKING
			anim.SetFloat("Ladder", 0);
		}
	}

	public void EnableJump(bool enable)
	{
		enableJump = enable;

		if (Input.GetButton("Jump"))
			jumpKeyUp = false;
	}

	public void CauseJump(float height)
	{
		velocity.y = height;
		anim.SetTrigger("Jumping");
	}

	public void LetsFly()
	{
		anim.SetTrigger("Flying");
	}
}
