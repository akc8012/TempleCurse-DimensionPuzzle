using UnityEngine;
using System.Collections;

public class SwitchableObject : MonoBehaviour
{
	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void GetSwitched()
	{
		rb.isKinematic = false;
		StartCoroutine("WaitToTurnBackOn");
	}

	IEnumerator WaitToTurnBackOn()
	{
		yield return new WaitForSeconds(3);
		rb.isKinematic = true;
	}
}