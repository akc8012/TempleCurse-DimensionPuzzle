using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
	public static LeaderboardManager instance = null;

	public class BoardPlayer : IComparable<BoardPlayer>
	{
		public string name;
		public int score;

		public BoardPlayer(string newName, int newScore)
		{
			name = newName;
			score = newScore;
		}

		//This method is required by the IComparable interface. 
		public int CompareTo(BoardPlayer other)
		{
			if (other == null)
			{
				return 1;
			}

			//Return the difference in score.
			return score - other.score;
		}
	}

	List<BoardPlayer> boardPlayers = new List<BoardPlayer>();
	int maxLength = 10;

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
		RepopulateLocalList();
	}

	void RepopulateLocalList()
	{
		int i = 0;
		while (i < 30)
		{
			string nameString = "Name" + i;
			string scoreString = "Score" + i;

			if (PlayerPrefs.HasKey(nameString) && PlayerPrefs.HasKey(scoreString))
			{
				BoardPlayer newPlayer = new BoardPlayer(PlayerPrefs.GetString(nameString), PlayerPrefs.GetInt(scoreString));
				boardPlayers.Add(newPlayer);

				i++;
			}
			else
				break;
		}

		boardPlayers.Sort();
		boardPlayers.Reverse();
	}

	public void AddScore(string name, int score)
	{
		BoardPlayer newPlayer = new BoardPlayer(name, score);

		boardPlayers.Add(newPlayer);
		boardPlayers.Sort();
		boardPlayers.Reverse();

		ResetPrefsList();
	}

	public bool CheckForRoom(int score)
	{
		if (boardPlayers.Count >= maxLength)
		{
			if (score <= FindLowestScore())
				return false;
			else
				return true;
		}
		else
			return true;
	}

	int FindLowestScore()
	{
		int lowest = int.MaxValue;

		foreach (BoardPlayer element in boardPlayers)
		{
			if (element.score < lowest)
				lowest = element.score;
		}

		return lowest;
	}

	void ResetPrefsList()
	{
		PlayerPrefs.DeleteAll();

		int i = 0;
		foreach (BoardPlayer element in boardPlayers)
		{
			string nameString = "Name" + i;
			string scoreString = "Score" + i;

			PlayerPrefs.SetString(nameString, element.name);
			PlayerPrefs.SetInt(scoreString, element.score);
			PlayerPrefs.Save();
			i++;
		}
	}

	public string PrintScore(int i)
	{
		string nameString = "Name" + i;
		string scoreString = "Score" + i;

		if (PlayerPrefs.HasKey(nameString) && PlayerPrefs.HasKey(scoreString))
			return PlayerPrefs.GetString(nameString) + ": " + PlayerPrefs.GetInt(scoreString);
		else
			return "";
	}

	int GetNumOfPlayersOnBoard()
	{
		int i = 0;
		while (i < 30)
		{
			string nameString = "Name" + i;
			string scoreString = "Score" + i;

			if (PlayerPrefs.HasKey(nameString) && PlayerPrefs.HasKey(scoreString))
				i++;
			else
				break;
		}

		return i;
	}

	public void DeleteEverything()
	{
		boardPlayers.Clear();
		PlayerPrefs.DeleteAll();
	}
}
