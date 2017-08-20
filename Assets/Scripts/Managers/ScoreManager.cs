using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
	public static ScoreManager instance = null;

	TimeManager timeManager;
	Text scoreBox;

	int score = 0;          // current total score
	int scoreAtLevelStart = 0;
	int timeBonus = 0;
	int livesBonus = 0;

	public int Score { get { return score; } }

	enum FunWithNumbersState { NotRunning, Limbo, Running };
	FunWithNumbersState funWithNumbersState = FunWithNumbersState.NotRunning;

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
		StartCoroutine("Init");
	}

	void OnLevelWasLoaded(int level)
	{
		if (level == 0)     // only reset this is we're back on the main menu
		{
			score = 0;
			return;
		}

		timeBonus = 0;
		livesBonus = 0;
		scoreAtLevelStart = score;
		
		StartCoroutine("Init");
	}

	void Update()
	{
		if (funWithNumbersState == FunWithNumbersState.Running &&
			Input.GetButtonDown("Submit"))
		{
			StopCoroutine("FunWithNumbers");
			funWithNumbersState = FunWithNumbersState.Limbo;
			CalcScore(true, true);
			SoundManager.instance.StopSound(SoundManager.instance.scoreTally);

			StartCoroutine(BounceText(FindText("Time Bonus").transform, 0.01f));
			StartCoroutine(BounceText(FindText("Lives Bonus").transform, 0.01f));
			StartCoroutine(BounceText(FindText("Total Score").transform, 0.01f));
		}

		if (funWithNumbersState == FunWithNumbersState.Limbo && 
			!Input.GetButton("Submit"))
		{
			GameObject.Find("WorldSpace Canvas").GetComponent<CanvasButtons>().SetSubmitButton(true);
			funWithNumbersState = FunWithNumbersState.NotRunning;
		}

		if (funWithNumbersState == FunWithNumbersState.Running)
			SoundManager.instance.PlaySound(SoundManager.instance.scoreTally, true);
	}

	IEnumerator Init()
	{
		yield return new WaitForSeconds(0.1f);      // we need to wait a bit just to ENSURE that the old canvas has been renamed (to old canvas)

		if (GameObject.Find("TimeManager"))
			timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();

		if (GameObject.Find("HUD Canvas/ScoreText"))
		{
			scoreBox = GameObject.Find("HUD Canvas/ScoreText").GetComponent<Text>();
			CalcScore(false, true);
		}
	}

	public void CalcScore(bool drawNewScore, bool skipFun)
	{
		int scoreBeforeBonus = score;
		timeBonus = (drawNewScore) ? timeManager.GetRemainingTime() : 0;
		int possibleLivesBonus = 
			(HealthManager.instance.LostAGuy) ? (0) : (HealthManager.instance.Lives * 100);		// I'm just attempting to make this readable.
		livesBonus = (drawNewScore) ? possibleLivesBonus : 0;

		score = score + timeBonus + livesBonus;

		if (skipFun)
			DrawScore(scoreBeforeBonus);
		else
			StartCoroutine("FunWithNumbers", scoreBeforeBonus);
	}

	IEnumerator FunWithNumbers(int scoreBeforeBonus)
	{
		funWithNumbersState = FunWithNumbersState.Running;

		int totalFun = scoreBeforeBonus;
		int timeFun = 0;						// "fun" numbers are just for display, start at 0 and keep adding up
		while (timeFun < timeBonus)
		{
			timeFun += 2;
			totalFun += 2;
			DrawScore("Time Bonus", timeFun);
			DrawScore("Total Score", totalFun);
			yield return null;
		}
		DrawScore("Time Bonus", timeBonus);     // after the while loop, display the REAL number, in case the fun number messed up
		StartCoroutine(BounceText(FindText("Time Bonus").transform, 0.01f));

		int livesFun = 0;
		while (livesFun < livesBonus)
		{
			livesFun += 2;
			totalFun += 2;
			DrawScore("Lives Bonus", livesFun);
			DrawScore("Total Score", totalFun);
			yield return null;
		}
		DrawScore("Lives Bonus", livesBonus);
		StartCoroutine(BounceText(FindText("Lives Bonus").transform, 0.01f));

		DrawScore(scoreBeforeBonus);
		StartCoroutine(BounceText(FindText("Total Score").transform, 0.01f));

		funWithNumbersState = FunWithNumbersState.NotRunning;
		SoundManager.instance.StopSound(SoundManager.instance.scoreTally);
		GameObject.Find("WorldSpace Canvas").GetComponent<CanvasButtons>().SetSubmitButton(true);
	}

	void DrawScore(int scoreBeforeBonus)	// DON'T CALL ME, CALL CALC SCORE INSTEAD!!!!
	{
		DrawScore("Score", scoreBeforeBonus);
		DrawScore("Time Bonus", timeBonus);
		DrawScore("Lives Bonus", livesBonus);
		DrawScore("Total Score", score);

		scoreBox.text = score.ToString("0000000");
		SoundManager.instance.StartCoroutine("FadeInMusic");
	}

	void DrawScore(string drawText, int toDraw)		// only draws a single text component
	{
		FindText(drawText).GetComponent<Text>().text = toDraw + "";
	}

	GameObject FindText(string toFind)
	{
		if (GameObject.Find("WorldSpace Canvas"))
		{
			string findText = "WorldSpace Canvas/" + toFind + " Number";
			GameObject foundText = GameObject.Find(findText);
			return foundText;
		}

		return null;
	}

	public void IncreaseScore(int add)
	{
		score += add;
		scoreBox.text = score.ToString("0000000");
		DrawScore("Score", score);
		DrawScore("Total Score", score);
		StartCoroutine(BounceText(scoreBox.transform));
	}

	public IEnumerator BounceText(Transform pos, float heightMod = 1)
	{
		Vector3 startPos = pos.position;
		Vector3 gravity = new Vector3(0, -1.4f * heightMod, 0);
		Vector3 velocity = new Vector3(0, 7 * heightMod, 0);

		while (pos.position.y > startPos.y + (gravity.y * heightMod))
		{
			velocity += gravity;
			pos.position += velocity;

			yield return null;
		}

		pos.position = startPos;
	}

	public void SetScoreToLevelStart()
	{
		score = scoreAtLevelStart;
	}
}