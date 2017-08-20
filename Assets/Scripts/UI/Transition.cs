using UnityEngine;
using System.Collections;

public class Transition : MonoBehaviour
{
	public Animator anim;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public virtual void StartOutThenDie()
	{
		//
	}
}