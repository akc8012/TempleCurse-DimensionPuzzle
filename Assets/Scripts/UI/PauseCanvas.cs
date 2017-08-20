using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseCanvas : MonoBehaviour
{
	public Toggle muteSfx;
	public Toggle muteMusic;
	public Toggle invertCube;
	public Button resumeButt;
	public Button exitButt;
	public float distFromCam;

	GameObject mainCanvas;		// only for start screen, do this so that we can find it after we turn it off

	void Awake()
	{
		muteSfx.onValueChanged.AddListener(MuteSfxToggle);
		muteMusic.onValueChanged.AddListener(MuteMusicToggle);
		invertCube.onValueChanged.AddListener(CubeInvertToggle);
		resumeButt.onClick.AddListener(ResumeButtClicked);
		exitButt.onClick.AddListener(ExitButtClicked);
	}

	void Start()
	{
		muteSfx.Select();
		muteSfx.isOn = SoundManager.instance.SfxMuted;
		muteMusic.isOn = SoundManager.instance.MusicMuted;
		invertCube.isOn = LevelManager.instance.CamInverted;

		StartCoroutine("WaitABit");
	}

	IEnumerator WaitABit()
	{
		yield return new WaitForSeconds(0.05f);

		Transform cam = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
		transform.position = cam.position + (cam.forward * distFromCam);

		mainCanvas = GameObject.Find("Main Canvas");
		if (mainCanvas)		// this means we're on the start screen
		{
			mainCanvas.SetActive(false);
			invertCube.gameObject.SetActive(false);		// we don't even wanna see this option!

			if (GameObject.Find("Leaderboard Canvas"))                      // turn this off, don't worry about
				GameObject.Find("Leaderboard Canvas").SetActive(false);		// setting it back on, because pressing
        }																	// its button will do that for us
	}

	void Update()
	{
		if (Input.GetButtonUp("Pause"))
		{
			Unpause();
		}

		muteSfx.isOn = SoundManager.instance.SfxMuted;
		muteMusic.isOn = SoundManager.instance.MusicMuted;
	}
	
	void MuteSfxToggle(bool enable)
	{
		if (SoundManager.instance)
			SoundManager.instance.MuteSound(enable);
	}

	void MuteMusicToggle(bool enable)
	{
		if (SoundManager.instance)
			SoundManager.instance.MuteMusic(enable);
	}

	void CubeInvertToggle(bool enable)
	{
		if (LevelManager.instance)
			LevelManager.instance.InvertCamMovement(enable);
	}

	void ResumeButtClicked()
	{
		Unpause();
	}

	void ExitButtClicked()
	{
		Application.Quit();
	}

	void Unpause()
	{
		LevelManager.instance.Pause(false);

		if (mainCanvas)
		{
			mainCanvas.SetActive(true);
			mainCanvas.GetComponent<StartMenu>().startButt.Select();
		}

		GameObject player = GameObject.FindWithTag("Player");
		if (player && player.GetComponent<CharacterMotor>())
		{
			player.GetComponent<CharacterMotor>().EnableJump(true);		// do this to say jump key is not up yet
		}

		Destroy(gameObject);
	}
}