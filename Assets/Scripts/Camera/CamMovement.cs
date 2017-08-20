using UnityEngine;
using System.Collections;

public class CamMovement : MonoBehaviour
{
	protected bool movementEnabled = true;
	public bool inverted = false;

	public void MovementEnabled(bool enable)
	{
		movementEnabled = enable;
	}

	public void InvertMovement(bool enable)
	{
		inverted = enable;
	}
}