using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasButtons : MonoBehaviour
{
	public Button nextButt;
	public Button retryButt;
	public GameObject areYouSure;
	public Button no;
	public Button yes;

	void Start()
	{
		nextButt.onClick.AddListener(NextClicked);
		retryButt.onClick.AddListener(RestartClicked);
		no.onClick.AddListener(NoClicked);
		yes.onClick.AddListener(YesClicked);

		if (HealthManager.instance && HealthManager.instance.Guys <= 0)
			retryButt.gameObject.SetActive(false);
	}

	void NextClicked()
	{
		LevelManager.instance.NextLevel();
	}

	void RestartClicked()
	{
		areYouSure.SetActive(true);
		no.Select();
	}

	void NoClicked()
	{
		areYouSure.SetActive(false);
		retryButt.Select();
	}

	void YesClicked()
	{
		LevelManager.instance.RestartLevel();
	}

	public void SetSubmitButton(bool enabled)
	{
		if (enabled)
		{
			nextButt.interactable = true;
			retryButt.interactable = true;

			nextButt.Select();
		}
		else
		{
			nextButt.interactable = false;
			retryButt.interactable = false;
		}
	}
	
}