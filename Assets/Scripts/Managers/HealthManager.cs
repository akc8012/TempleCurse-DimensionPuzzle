using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthManager : MonoBehaviour
{
	public static HealthManager instance = null;

	public GameObject leaderboardCanvas;

	Image[] liveSprites = new Image[3];
	Text guyText;
	CamShake camShake;
	PlayerFlashyFlashy flashyFlashy;
	int lives = 3;
	int guys = 8;
	bool canGetHit = true;
	int currentLevel = 0;
	bool lostAGuy = false;

	public int Lives { get { return lives; } }
	public int Guys { get { return guys; } }
	public bool LostAGuy { get { return lostAGuy; } }

	void Awake()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		Init();
	}

	void OnLevelWasLoaded(int level)
	{
		Init();

		if (level == 0)
		{
			guys = 8;
			return;
		}

		if (level != currentLevel)
		{
			ResetLostAGuy();
		}
		currentLevel = level;
	}

	void Init()
	{
		lives = 3;
		camShake = GameObject.FindWithTag("MainCamera").GetComponent<CamShake>();

		if (GameObject.FindWithTag("Player"))
		{
			flashyFlashy = GameObject.FindWithTag("Player").GetComponent<PlayerFlashyFlashy>();
		}

		if (GameObject.Find("HUD Canvas"))
		{
			liveSprites[0] = GameObject.Find("HUD Canvas/Live (0)").GetComponent<Image>();
			liveSprites[1] = GameObject.Find("HUD Canvas/Live (1)").GetComponent<Image>();
			liveSprites[2] = GameObject.Find("HUD Canvas/Live (2)").GetComponent<Image>();
			guyText = GameObject.Find("HUD Canvas/GuyText").GetComponent<Text>();

			guyText.text = guys+"";
		}

		if (!LevelManager.instance.ArcadeMode)
		{
			guyText.color = new Color(1, 1, 1, 0);
		}
	}

	public void DecreaseLives(bool instantKill)
	{
		if (!canGetHit) return;
		lives--;
		SoundManager.instance.PlaySound(SoundManager.instance.hurtSound);

		if (lives <= 0 || instantKill)
		{
			if (guys > 0 || !LevelManager.instance.ArcadeMode)						// either we have enough guys, or we're NOT in arcade mode, then we want to do hit effects then restart
				StartHitEffects(true);
			else
				Instantiate(leaderboardCanvas, Vector3.zero, Quaternion.identity);	// either we have NO guys, and we ARE in arcade mode, then that's a game-over
		}
		else
			StartHitEffects(false);
	}

	void StartHitEffects(bool restartAfterThis)
	{
		if (restartAfterThis)
		{
			RemoveAllLives();

			flashyFlashy.StartCoroutine("FlashyFlashy");
			camShake.StartCoroutine("ShakeCam", 0.15f);

			if (LevelManager.instance.ArcadeMode)
				LevelManager.instance.RestartLevel(true, true);
			else
				LevelManager.instance.RestartLevel(true, true, false);
		}
		else
		{
			RemoveLiveSprite(lives);

			flashyFlashy.StartCoroutine("FlashyFlashy");
			StartCoroutine("WaitThenEnableHit");
			camShake.StartCoroutine("ShakeCam", 0.15f);
		}
	}

	IEnumerator WaitThenEnableHit()
	{
		canGetHit = false;
		yield return new WaitForSeconds(1.2f);
		canGetHit = true;
	}

	public void LoseAGuy(bool setLostAGuy = true)
	{
		lives = 3;
		if (setLostAGuy)
			lostAGuy = true;

		SoundManager.instance.PlaySound(SoundManager.instance.loseGuy);

		if (LevelManager.instance.ArcadeMode)
		{
			guys--;
			guyText.text = guys + "";
			ScoreManager.instance.StartCoroutine(ScoreManager.instance.BounceText(guyText.transform));
		}
	}

	public void ResetLostAGuy()
	{
		lostAGuy = false;
	}

	void RemoveLiveSprite(int index)
	{
		Vector3 startPos = liveSprites[index].transform.position;
		float moveDist = 100;

		StartCoroutine(RemoveAnim(startPos, index, moveDist));
	}

	void RemoveAllLives()
	{
		for (int i = 2; i >= 0; i--)
		{
			Vector3 startPos = liveSprites[i].transform.position;
			float moveDist = 100;

			StartCoroutine(RemoveAnim(startPos, i, moveDist));
		}
	}

	IEnumerator RemoveAnim(Vector3 startPos, int index, float moveDist)
	{
		while (Vector3.Distance(startPos, liveSprites[index].transform.position) < moveDist)
		{
			Vector3 currPos = liveSprites[index].transform.position;
			liveSprites[index].transform.position = Vector3.MoveTowards(currPos, currPos + (Vector3.down * moveDist), Time.deltaTime * 400);
			liveSprites[index].transform.Rotate(Vector3.forward * 4);
			liveSprites[index].CrossFadeAlpha(0.0f, Time.deltaTime * 8, false);

			yield return null;
		}
	}
}