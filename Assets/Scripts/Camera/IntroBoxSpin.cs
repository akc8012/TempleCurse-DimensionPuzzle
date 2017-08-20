using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroBoxSpin : MonoBehaviour
{
	public Transform frontSideTarget;
	public Transform gameTitle;
	public GameObject blockParent;
	public GameObject[] titleStuff;     // everything except blue
	public GameObject enableBlue;
	public Transform[] stuffToSpin;
	public Button[] butts;

	public float speed;
	public float speedQUICK;

	Vector3 gameTitleTarget;
	int submitCount = 0;

	void Start()
	{
		gameTitleTarget = frontSideTarget.position + new Vector3(0, 4.35f, 0);
	}

	public void NextButtClick()
	{
		if (submitCount > 3)
			LevelManager.instance.NextLevel();
		else
		{
			StartCoroutine("SpinLeft");
			SoundManager.instance.PlayClickThenMute();
		}
	}

	public IEnumerator GoToFrontSide()
	{
		GetComponent<CollectableMovement>().enabled = false;

		Transform startPoint = transform;
		float dist = Vector3.Distance(transform.position, frontSideTarget.position);
		float duration = dist / speed;
		float t = 0;

		while (Vector3.Distance(transform.position, frontSideTarget.position) > 0.01f)
		{
			t += Time.deltaTime / duration;
			transform.position = Vector3.Lerp(startPoint.position, frontSideTarget.position, t);
			transform.rotation = Quaternion.Slerp(startPoint.rotation, frontSideTarget.rotation, t);

			gameTitle.position = Vector3.Lerp(gameTitle.position, gameTitleTarget, t);

			yield return null;
		}

		blockParent.SetActive(true);

		foreach (GameObject backing in titleStuff)
			backing.SetActive(false);
		enableBlue.SetActive(true);

		StartCoroutine("SpinLeft");
	}

	public IEnumerator SpinLeft()
	{
		Quaternion rotTarget = transform.rotation * Quaternion.Euler(0, 90, 0);

		while (Quaternion.Angle(transform.rotation, rotTarget) > 5)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, rotTarget, speed * Time.deltaTime);

			foreach (Transform thing in stuffToSpin)
				thing.rotation = Quaternion.Slerp(thing.rotation, rotTarget, speed * Time.deltaTime);

			yield return null;
		}

		StartCoroutine("SpinToExact", rotTarget);
	}

	IEnumerator SpinToExact(Quaternion rotTarget)
	{
		while (Quaternion.Angle(transform.rotation, rotTarget) > 0.1f)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTarget, speedQUICK * Time.deltaTime);

			foreach (Transform thing in stuffToSpin)
				thing.rotation = Quaternion.RotateTowards(thing.rotation, rotTarget, speedQUICK * Time.deltaTime);

			yield return null;
		}

		AfterSpinLeft();
	}

	void AfterSpinLeft()
	{
		butts[submitCount].interactable = true;
		butts[submitCount].Select();

		if (submitCount == 1)       // Do this for arcade no button (gross, I know)
		{
			butts[submitCount + 1].interactable = true;
			submitCount++;          // Because arcade mode no counts as another button, increment 1 additional to skip past it
		}

		submitCount++;
		SoundManager.instance.MuteSoundSecretly(false);
	}
}
