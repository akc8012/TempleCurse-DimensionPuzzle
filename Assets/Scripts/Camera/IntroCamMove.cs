using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroCamMove : MonoBehaviour
{
	public Transform zoomTarget;
	public Transform moveOverTarget;
	public Transform leaderboardTarget;
	public GameObject leaderboard;

	public float startingWait = 0.5f;
	public float zoomInSpeed = 4;
	public float moveOverSpeed = 4;

	Button startButt;
	Button leaderboardButt;
	Button backButt;
	RectTransform startButtTarget;

	void Start()
	{
		StartCoroutine("WaitThenZoomIn");

		StartMenu startMenu = GameObject.Find("Main Canvas").GetComponent<StartMenu>();

		startButt = startMenu.startButt;
		leaderboardButt = startMenu.leaderboardButt;
		backButt = startMenu.backButt;
		startButtTarget = startMenu.startButtTarget;
	}
	
	IEnumerator WaitThenZoomIn()
	{
		yield return new WaitForSeconds(startingWait);

		while (Vector3.Distance(transform.position, zoomTarget.position) > 0.2f)
		{
			transform.position = Vector3.Lerp(transform.position, zoomTarget.position, Time.deltaTime * zoomInSpeed);
			yield return null;
		}

		StartCoroutine("MoveToSide");
	}

	IEnumerator MoveToSide()
	{
		while (Vector2.Distance(startButt.transform.position, startButtTarget.position) > 10)
		{
			transform.position = Vector3.Lerp(transform.position, moveOverTarget.position, Time.deltaTime * moveOverSpeed);
			startButt.transform.position = Vector2.Lerp(startButt.transform.position, startButtTarget.position, Time.deltaTime * moveOverSpeed);
			leaderboardButt.transform.position = Vector2.Lerp(leaderboardButt.transform.position, new Vector2(startButtTarget.position.x, leaderboardButt.transform.position.y), Time.deltaTime * moveOverSpeed);

			yield return null;
		}

		startButt.interactable = true;
		leaderboardButt.interactable = true;
		startButt.Select();
	}

	public IEnumerator GoToLeaderboard()
	{
		leaderboard.SetActive(true);

		Transform startPoint = transform;
		float dist = Vector3.Distance(transform.position, leaderboardTarget.position);
		float duration = dist / moveOverSpeed;
		float t = 0;

		while (Vector3.Distance(transform.position, leaderboardTarget.position) > 0.01f)
		{
			t += Time.deltaTime / duration;
			transform.position = Vector3.Lerp(startPoint.position, leaderboardTarget.position, t);
			transform.rotation = Quaternion.Slerp(startPoint.rotation, leaderboardTarget.rotation, t);

			yield return null;
		}

		backButt.gameObject.SetActive(true);
		backButt.Select();
		SoundManager.instance.MuteSoundSecretly(false);
	}

	public IEnumerator BackToStart()
	{
		Transform startPoint = transform;
		float dist = Vector3.Distance(transform.position, moveOverTarget.position);
		float duration = dist / moveOverSpeed;
		float t = 0;

		while (Vector3.Distance(transform.position, moveOverTarget.position) > 0.01f)
		{
			t += Time.deltaTime / duration;
			transform.position = Vector3.Lerp(startPoint.position, moveOverTarget.position, t);
			transform.rotation = Quaternion.Slerp(startPoint.rotation, moveOverTarget.rotation, t);

			yield return null;
		}

		startButt.gameObject.SetActive(true);
		leaderboardButt.gameObject.SetActive(true);

		startButt.Select();
		SoundManager.instance.MuteSoundSecretly(false);
	}
}
