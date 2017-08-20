using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardCanvas : MonoBehaviour
{
	public InputField inputField;
	public Text scoreText;
	public Button deleteButt;
	public Button againButt;
	public float distFromCam;
	public float downOffset;
	public float riseSpeed;
	public bool onStartScreen = false;

	Vector3 targetPos;

	void Start()
	{
		inputField.onEndEdit.AddListener(EndEdit);
		deleteButt.onClick.AddListener(DeleteButtClick);
		againButt.onClick.AddListener(AgainButtClick);

		if (!onStartScreen)
		{
			GameObject cam = GameObject.FindWithTag("MainCamera");
			transform.position = cam.transform.position + (cam.transform.forward * distFromCam);
			targetPos = transform.position;

			if (cam.GetComponent<CamFollowVertical>())
				cam.GetComponent<CamFollowVertical>().enabled = false;

			transform.position += Vector3.down - new Vector3(0, downOffset, 0);
			StartCoroutine("RiseUp");
			LevelManager.instance.Pause(true);
		}

		UpdateList();
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape) && inputField.isActiveAndEnabled)
			inputField.ActivateInputField();
	}

	IEnumerator RiseUp()
	{
		while(Vector3.Distance(transform.position, targetPos) > 0.01f)
		{
			transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * riseSpeed);
			yield return null;
		}

		CheckForRoom();
	}

	void CheckForRoom()
	{
		if (LeaderboardManager.instance.CheckForRoom(ScoreManager.instance.Score))
		{
			inputField.gameObject.SetActive(true);
			inputField.interactable = true;
			inputField.Select();
			SoundManager.instance.SetCanUseKeys(false);
		}
		else
		{
			inputField.gameObject.SetActive(false);
			againButt.gameObject.SetActive(true);
			againButt.Select();
		}
	}

	void UpdateList()
	{
		scoreText.text = "";

		for (int i = 0; i < 10; i++)
		{
			scoreText.text += (i+1) + ") ";
			scoreText.text += LeaderboardManager.instance.PrintScore(i);
			scoreText.text += "\n";
		}
	}

	void EndEdit(string input)
	{
		if (input == "")	// we don't want empty strings now
			return;

		LeaderboardManager.instance.AddScore(input, ScoreManager.instance.Score);
		inputField.gameObject.SetActive(false);
		againButt.gameObject.SetActive(true);
		againButt.Select();
		UpdateList();
	}

	void DeleteButtClick()
	{
		LeaderboardManager.instance.DeleteEverything();
		UpdateList();
	}

	void AgainButtClick()
	{
		SoundManager.instance.SetCanUseKeys(true);
		LevelManager.instance.GoToLevel(0);
	}
}