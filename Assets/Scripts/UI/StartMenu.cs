using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour
{
	public Button startButt;
	public Button leaderboardButt;
	public RectTransform startButtTarget;
	public Button backButt;
	public Button nextButt0;
	public Button nextButt1;
	public Button nextButt2;
	public Button nextButt3;
	public IntroBoxSpin introBoxSpin;

	public float buttFlySpeed;

	void Start()
	{
		startButt.onClick.AddListener(StartButtonClick);
		leaderboardButt.onClick.AddListener(LeaderboardButtonClick);
		backButt.onClick.AddListener(BackButtonClick);
		nextButt0.onClick.AddListener(NextButtClick);
		nextButt1.onClick.AddListener(ArcadeYesClick);
		nextButt2.onClick.AddListener(ArcadeNoClick);
		nextButt3.onClick.AddListener(NextButtClick);
	}

	void StartButtonClick()
	{
		GameObject.Find("Box").GetComponent<IntroBoxSpin>().StartCoroutine("GoToFrontSide");
		StartCoroutine("StartButtFlyAway");
		startButt.interactable = false;
		leaderboardButt.interactable = false;
		SoundManager.instance.PlayClickThenMute();
	}

	void LeaderboardButtonClick()
	{
		GameObject.FindWithTag("MainCamera").GetComponent<IntroCamMove>().StartCoroutine("GoToLeaderboard");
		startButt.gameObject.SetActive(false);
		leaderboardButt.gameObject.SetActive(false);
		SoundManager.instance.PlayClickThenMute();
	}

	void BackButtonClick()
	{
		GameObject.FindWithTag("MainCamera").GetComponent<IntroCamMove>().StartCoroutine("BackToStart");
		backButt.gameObject.SetActive(false);
		SoundManager.instance.PlayClickThenMute();
	}

	void NextButtClick()
	{
		introBoxSpin.NextButtClick();

		nextButt0.interactable = false;
		nextButt1.interactable = false;
		nextButt2.interactable = false;
		nextButt3.interactable = false;
	}

	void ArcadeYesClick()
	{
		LevelManager.instance.ToggleArcadeMode(true);
		NextButtClick();
	}

	void ArcadeNoClick()
	{
		LevelManager.instance.ToggleArcadeMode(false);
		NextButtClick();
	}

	IEnumerator StartButtFlyAway()
	{
		for (int i = 0; i < 100; i++)
		{
			startButt.transform.position += Vector3.right * buttFlySpeed;
			leaderboardButt.transform.position += Vector3.right * buttFlySpeed;
			yield return null;
		}
	}
}