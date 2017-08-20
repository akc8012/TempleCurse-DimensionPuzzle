using UnityEngine;
using System.Collections;

public class Trampoline : MonoBehaviour
{
	public Animator animator;
	public GameObject wall;

	CharacterMotor charMotor;
	bool sentMsg = false;

	void Start()
	{
		charMotor = GameObject.FindWithTag("Player").GetComponent<CharacterMotor>();
	}
	
	void Update()
	{
		
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player" && !sentMsg)
		{
			if (Input.GetButton("Jump"))
			{
				charMotor.CauseJump(20);
				if (SoundManager.instance) SoundManager.instance.PlaySound(SoundManager.instance.highBounce);
			}
			else
			{
				charMotor.CauseJump(14);
				if (SoundManager.instance) SoundManager.instance.PlaySound(SoundManager.instance.lowBounce);
			}

			animator.SetTrigger("Bounce");
			wall.SetActive(false);
			StartCoroutine(WallBackOnTimer());
			sentMsg = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player" && sentMsg)
		{
			sentMsg = false;
		}
	}

	IEnumerator WallBackOnTimer()
	{
		yield return new WaitForSeconds(0.25f);
		wall.SetActive(true);
	}
}