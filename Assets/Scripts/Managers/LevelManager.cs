using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance = null;

	public GameObject circleCanvas;
	public GameObject fadeCanvas;
	public GameObject pauseCanvas;
	public GameObject minusLivesAnim;
	public bool allowLevelSkip = false;

	Vector3 checkPos;
	Quaternion checkRot;
	bool arcadeMode = true;
	bool enableLevelSkip = false;
	bool collectedCPinLevel = false;
	bool paused = false;
	float circleSpeed = 1.2f;
	float fadeSpeed = 0.5f;
	bool beingRestarted = false;
	bool camInverted = false;
	bool oneInCorner = false;
	Color[] savedCloudColor;

	public bool ArcadeMode { get { return arcadeMode; } }
	public bool CamInverted { get { return camInverted; } }
	public bool BeingRestarted { get { return beingRestarted; } }
	public bool Paused { get { return paused; } }

	void Awake()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		Cursor.visible = false;
	}

	void Start()
	{
		SoundManager.instance.StartCoroutine("FadeInMusic");
	}

	void OnLevelWasLoaded(int level)
	{
		InvertCamMovement(camInverted);
	}

	void Update()
	{
		if (allowLevelSkip)
		{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.BackQuote))
			{
				enableLevelSkip = !enableLevelSkip;
				Cursor.visible = true;

				if (enableLevelSkip)
					SoundManager.instance.PlaySound(SoundManager.instance.click);
				else
					SoundManager.instance.PlaySound(SoundManager.instance.tick);
			}

			if (enableLevelSkip)
			{
				if (Input.GetKeyDown("0")) GoToLevel(0);
				if (Input.GetKeyDown("1")) GoToLevel(1);
				if (Input.GetKeyDown("2")) GoToLevel(2);
				if (Input.GetKeyDown("3")) GoToLevel(3);
				if (Input.GetKeyDown("4")) GoToLevel(4);
				if (Input.GetKeyDown("5")) GoToLevel(5);
				if (Input.GetKeyDown("6")) GoToLevel(6);
				if (Input.GetKeyDown("7")) GoToLevel(7);
				if (Input.GetKeyDown("8")) GoToLevel(8);
				if (Input.GetKeyDown("9")) GoToLevel(9);
				if (Input.GetKeyDown("-")) GoToLevel(10);
			}
		}

		if (Input.GetButtonUp("Pause") && CanPause())
		{
			Pause(true, false, false);
			Instantiate(pauseCanvas, Vector3.zero + (Vector3.up * 20), Quaternion.identity);
		}

		if (Input.GetButtonDown("Restart") && CanRestart())
		{
			if (arcadeMode)
				RestartLevel(true, false);
			else
				RestartLevel(true, false, false);
			SoundManager.instance.PlaySound(SoundManager.instance.hurtSound);
		}
	}

	public void ToggleArcadeMode(bool enable)
	{
		arcadeMode = enable;
	}

	public void NextLevel()
	{
		GoToLevel(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void GoToLevel(int level)
	{
		SceneManager.LoadScene(level);
		ResetCheckPoint();
		paused = false;
	}

	public void RestartLevel(bool sendPlayerToCP, bool doCircle, bool doMinusLives = true)
	{
		if (beingRestarted)
			return;

		beingRestarted = true;
		Pause(true, false, false);

		if (doMinusLives) Instantiate(minusLivesAnim);
		else
		{
			Pause(true, false, false);
			HealthManager.instance.LoseAGuy();
			if (doCircle)
				StartCoroutine(WaitForTransition(collectedCPinLevel, circleCanvas));
			else
				StartCoroutine(WaitForTransition(collectedCPinLevel, fadeCanvas));

			return;
		}

		if (doCircle)			// circle for death	
		{
			if (sendPlayerToCP)
				StartCoroutine(WaitForOne(collectedCPinLevel, circleCanvas));
			else
				StartCoroutine(WaitForOne(false, circleCanvas));
		}
		else					// fade in for restart
		{
			if (sendPlayerToCP)
				StartCoroutine(WaitForOne(collectedCPinLevel, fadeCanvas));
			else
				StartCoroutine(WaitForOne(false, fadeCanvas));
		}
	}

	// ONLY called from retry butt on worldspace canvas
	public void RestartLevel()
	{
		if (beingRestarted)
			return;

		beingRestarted = true;
		Pause(true, false, false);
		HealthManager.instance.LoseAGuy(false);		// send false, as in: don't set lost a guy
		HealthManager.instance.ResetLostAGuy();
		StartCoroutine(WaitForTransition(false, fadeCanvas));
	}

	public void OneReachedCorner()
	{
		oneInCorner = true;
	}

	IEnumerator WaitForOne(bool sendPlayerToCP, GameObject transition)
	{
		while (!oneInCorner)
		{
			yield return null;
		}

		oneInCorner = false;
		StartCoroutine(WaitForTransition(sendPlayerToCP, transition));
	}

	IEnumerator WaitForTransition(bool sendPlayerToCP, GameObject transition)
	{
		if (!sendPlayerToCP)
			ResetCheckPoint();

		yield return new WaitForSeconds(0.2f);                                  // hold on just a bit before that transition!
		Instantiate(transition);                                                // spawn transition animation

		float speed;
		if (transition.name == "Fade Canvas")
			speed = fadeSpeed;
		else
			speed = circleSpeed;

		yield return new WaitForSeconds(speed);                                 // wait for transition to be over

		StartCoroutine("WaitForLevelReload", sendPlayerToCP);                   // load the level, then come back
		transition.GetComponent<Transition>().StartOutThenDie();                // tell the circle to grow back

		ScoreManager.instance.SetScoreToLevelStart();
	}

	IEnumerator WaitForLevelReload(bool sendPlayerToCP)
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);       // load scene

		yield return null;                                                      // go to next frame, ensuring scene is loaded
		Pause(true);

		if (GameObject.Find("Begin Camera"))
			Destroy(GameObject.Find("Begin Camera"));

		if (GameObject.FindWithTag("MainCamera"))
			GameObject.FindWithTag("MainCamera").GetComponent<Camera>().enabled = true;

		if (sendPlayerToCP)
			SendToCP();
	}

	void SendToCP()
	{
		if (GameObject.FindWithTag("Player"))
		{
			GameObject player = GameObject.FindWithTag("Player");
			player.transform.position = checkPos;
			player.transform.rotation = checkRot;
		}
	}

	public void SetCP(Transform newCP)
	{
		checkPos = newCP.position;
		checkRot = newCP.rotation;

		collectedCPinLevel = true;
	}

	void ResetCheckPoint()
	{
		checkPos = Vector3.zero;
		checkRot = Quaternion.identity;

		collectedCPinLevel = false;
	}

	public int CurrentLevel()
	{
		return SceneManager.GetActiveScene().buildIndex;
	}

	public void Pause(bool enable, bool resetTimer = false, bool hideHud = true)
	{
		paused = enable;
		enable = !enable;

		if (GameObject.FindWithTag("Player"))
			GameObject.FindWithTag("Player").GetComponent<CharacterMotor>().EnableMovement(enable);

		GameObject cam = GameObject.FindWithTag("MainCamera");
		if (cam)
		{
			if (cam.GetComponent<CamMovement>())
				cam.GetComponent<CamMovement>().MovementEnabled(enable);

			if (cam.GetComponent<CamFollowVertical>())
				cam.GetComponent<CamFollowVertical>().MovementEnabled(enable);
		}

		if (GameObject.Find("TimeManager"))
			GameObject.Find("TimeManager").GetComponent<TimeManager>().EnableTimer(enable, resetTimer);

		if (hideHud)
			HideHud(enable);
	}

	bool CanPause()
	{
		if (paused)
			return false;

		GameObject mainCanvas = GameObject.Find("Main Canvas");
		if (mainCanvas != null)
		{
			return (mainCanvas.GetComponent<StartMenu>().startButt.interactable &&
					mainCanvas.GetComponent<StartMenu>().startButt.IsActive());
		}

		GameObject beginCamera = GameObject.Find("Begin Camera");
		if (beginCamera != null)
		{
			return (!beginCamera.GetComponent<Camera>().enabled);
		}

		return true;
	}

	bool CanRestart()
	{
		return (!paused && HealthManager.instance.Guys > 0 && SceneManager.GetActiveScene().buildIndex != 0);
	}

	public void HideHud(bool enable)
	{
		if (GameObject.Find("HUD Canvas"))
			GameObject.Find("HUD Canvas").GetComponent<Canvas>().enabled = enable;
	}

	public void EnableRestart(bool enable)
	{
		beingRestarted = !enable;
	}

	public void InvertCamMovement(bool enable)
	{
		if (!GameObject.FindWithTag("MainCamera").GetComponent<CamMovement>())
			return;

		GameObject.FindWithTag("MainCamera").GetComponent<CamMovement>().InvertMovement(enable);
		camInverted = enable;
	}

	public void SaveCloudColor(Color light, Color dark)
	{
		savedCloudColor = new Color[] { light, dark };
	}

	public Color[] LoadCloudColor()
	{
		if (savedCloudColor != null)
			return new Color[] { savedCloudColor[0], savedCloudColor[1] };
		else
			return new Color[] {new Color(123.0f / 255, 238.0f / 255, 255.0f / 255),
								new Color(13.0f / 255, 73.0f / 255, 82.0f / 255)};
	}
}