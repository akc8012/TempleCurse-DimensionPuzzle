using UnityEngine;
using System.Collections;

public class RotateBox : MonoBehaviour
{
	public Transform[] stuffToRotate;
	public GameObject[] enableAfterFirstSpin;
	public GameObject[] enableAfterSecondSpin;
	public float rotationSpeed = 60;

	CollectableMovement[] collectMovements;
	float rotationAmount = 0.0f;
	int spinCount = 0;
	CharacterMotor characterMotor;
	CamMovement camMovement;

	void Start()
	{
		characterMotor = GameObject.FindWithTag("Player").GetComponent<CharacterMotor>();
		camMovement = GameObject.FindWithTag("MainCamera").GetComponent<CamMovement>();

		GameObject[] stuff = GameObject.FindGameObjectsWithTag("Item");
		collectMovements = new CollectableMovement[stuff.Length];
		for (int i = 0; i < stuff.Length; i ++)
			collectMovements[i] = stuff[i].GetComponent<CollectableMovement>();

		StartCoroutine("WaitThenDisable");
	}

	IEnumerator WaitThenDisable()
	{
		yield return new WaitForSeconds(0.25f);

		for (int i = 0; i < enableAfterFirstSpin.Length; i++)
			enableAfterFirstSpin[i].SetActive(false);

		for (int i = 0; i < enableAfterSecondSpin.Length; i++)
			enableAfterSecondSpin[i].SetActive(false);
	}
	
	public IEnumerator RotateForward()
	{
		characterMotor.EnableMovement(false);
		camMovement.MovementEnabled(false);
		PauseObjects(true);

		if (SoundManager.instance)
			SoundManager.instance.PlaySound(SoundManager.instance.crank);

		rotationAmount = 0.0f;
		while (rotationAmount < 89)
		{
			transform.RotateAround(new Vector3(0, 4.25f, 0), Vector3.forward, rotationSpeed * Time.deltaTime);
			foreach (Transform element in stuffToRotate)
			{
				element.RotateAround(new Vector3(0, 4.25f, 0), Vector3.forward, rotationSpeed * Time.deltaTime);
			}
			
			rotationAmount += rotationSpeed * Time.deltaTime;

			yield return null;
		}
		characterMotor.EnableMovement(true);
		camMovement.MovementEnabled(true);
		PauseObjects(false);

		if (spinCount == 0)
		{
			for (int i = 0; i < enableAfterFirstSpin.Length; i++)
				enableAfterFirstSpin[i].SetActive(true);
		}
		else if (spinCount == 1)
		{
			for (int i = 0; i < enableAfterSecondSpin.Length; i++)
				enableAfterSecondSpin[i].SetActive(true);
		}
		spinCount++;
	}

	void PauseObjects(bool enable)
	{
		for (int i = 0; i < collectMovements.Length; i++)
			collectMovements[i].PauseMovement(enable);
	}
}