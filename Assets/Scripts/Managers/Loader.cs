using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
	public GameObject levelManager;          //GameManager prefab to instantiate.
	public GameObject soundManager;
	public GameObject scoreManager;
	public GameObject healthManager;
	public GameObject leaderboardManager;

	void Awake()
	{
		//Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
		if (LevelManager.instance == null)
			Instantiate(levelManager);

		if (SoundManager.instance == null)
			Instantiate(soundManager);

		if (ScoreManager.instance == null)
			Instantiate(scoreManager);

		if (HealthManager.instance == null)
			Instantiate(healthManager);

		if (LeaderboardManager.instance == null)
			Instantiate(leaderboardManager);
	}
}