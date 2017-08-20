using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour
{
	public float startTime;
	public bool stopTime = false;
	public bool noTimer = false;

	Text clockText;
	RectTransform clockTextRect;
	RectTransform clockBackRect;
	float timeRemaining;
	float waitingTime;
	float pauseTime = 0;
	float pauseSubtraction = 0;
	float zoomInSpeed = 0.075f;
	bool timeZoomingIn = false;
	bool dead = false;

	void Start()
	{
		timeRemaining = startTime;
		waitingTime = Time.time;
		clockText = GameObject.Find("HUD Canvas/ClockText").GetComponent<Text>();
		clockTextRect = GameObject.Find("HUD Canvas/ClockText").GetComponent<RectTransform>();
		clockBackRect = GameObject.Find("HUD Canvas/ClockBacking").GetComponent<RectTransform>();
		clockText.text = "";
	}

	void Update()
	{
		if (stopTime || dead)
			return;

		if (timeRemaining > 0)
		{
			timeRemaining = startTime - (Time.time - waitingTime - pauseSubtraction);
			clockText.text = timeRemaining.ToString("F0");
		}
		else
		{
			StartCoroutine("ShowTextThenDie", GameObject.Find("HUD Canvas/TimeUpText"));
			StartCoroutine("JustInCaseRoutine");
			dead = true;
		}

		if (timeRemaining <= 10 && !timeZoomingIn)
			StartCoroutine("TimeZoomIn");
	}

	public void AddTime(float moreTime)
	{
		waitingTime += moreTime;
		ScoreManager.instance.StartCoroutine(ScoreManager.instance.BounceText(clockText.transform));
	}

	public int GetRemainingTime()
	{
		return (int)timeRemaining;
	}

	// gets called with false, then true
	public void EnableTimer(bool enable, bool reset)
	{
		if (noTimer) return;

		if (!enable) pauseTime = Time.time;
		else pauseSubtraction = Time.time - pauseTime;

		stopTime = !enable;
		if (reset) waitingTime = Time.time;
	}

	IEnumerator TimeZoomIn()
	{
		timeZoomingIn = true;
		float scaleInc = 1;
		bool tintRed = false;
		Image clockBacking = GameObject.Find("HUD Canvas/ClockBacking").GetComponent<Image>();
		Color origColor = clockBacking.color;

		while (timeRemaining > 0.5f)
		{
			if (Time.frameCount % 20 == 0)
				tintRed = !tintRed;

			if (tintRed)
				clockBacking.color = new Color(1, 84/255, 84/255);
			else
				clockBacking.color = origColor;

			clockTextRect.localScale = new Vector3(scaleInc, scaleInc, 1);
			clockBackRect.localScale = new Vector3(scaleInc, scaleInc, 1);

			if (!LevelManager.instance.Paused)
				scaleInc += zoomInSpeed * Time.deltaTime;

			yield return null;
		}

		clockBacking.color = origColor;
	}

	IEnumerator ShowTextThenDie(GameObject timeUp)
	{
		LevelManager.instance.Pause(true, false, false);
		Vector3 centerPos = timeUp.transform.position;
		float growSpeed = 1.4f;
		float fallSpeed = 600;

		timeUp.transform.position = new Vector3(Screen.width/2, Screen.height - 10, 0);
		timeUp.transform.localScale = new Vector3(0.2f, 0.2f, 1);

		timeUp.GetComponent<Text>().enabled = true;

		while (Vector3.Distance(timeUp.transform.position, centerPos) > 20)
		{
			timeUp.transform.localScale += new Vector3(growSpeed, growSpeed, 0) * Time.deltaTime;
			timeUp.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);
		HealthManager.instance.DecreaseLives(true);
	}

	IEnumerator JustInCaseRoutine()
	{
		yield return new WaitForSeconds(6);
		HealthManager.instance.DecreaseLives(true);
	}
}