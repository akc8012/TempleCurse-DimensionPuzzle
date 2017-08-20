using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CanvasKeep : MonoBehaviour
{
	int startingLevel;
	bool persisted = false;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		startingLevel = SceneManager.GetActiveScene().buildIndex;
	}
	
	void OnLevelWasLoaded(int level)
	{
		if (!GameObject.Find("Begin Camera"))    // if we can't find the begin camera, why even exist? this is to prevent persisting through to start screen
			Destroy(gameObject);
		else
		{
			if (level != startingLevel)     // are we loading a new level (not the level we woke up on)
				SetToOldCanvas();

			else if (persisted)             // this only happens AFTER the level is restarted
				Destroy(gameObject);
			else
				persisted = true;
		}
	}

	void SetToOldCanvas()
	{
		// Prevent old canvas's from being created twice
		string checkName = "";
		for (int i = 0; i < 3; i++)
			checkName += gameObject.name[i];

		if (checkName == "Old")
			Destroy(gameObject);

		gameObject.name = "Old " + gameObject.name;
		GetComponent<CanvasButtons>().SetSubmitButton(false);
		GetComponent<CanvasCalcOffset>().SwitchCamera();
	}
}