using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour 
{

#region VAR

	public static GameManager Instance {get; private set;}
	public int HiScore {get {return config.hiScore;} }
	public int ScoreBeaten {get; private set;}
	public bool IsRaundOn {get; private set;}
	
	GameConfig config;
	float currentTime;	

#endregion


#region  Event

	public event Action StartRaundEvent;
	public event Action StopRaundEvent;
	public event Action<int> ChangeScoreEvent;	
	public event Action<string, string> ChangeTimeEvent;

#endregion


#region Init

	void Awake()
	{
		if (Instance == null)
			Instance = this;
						
		config = Resources.Load<GameConfig>("Data/GameConfig");
		ResetValueDefault();
	}


	void ResetValueDefault()
	{
		currentTime = config.raundTime;
		ScoreBeaten = 0;
	}


#endregion


#region Update

	void Update()
	{
		StartRaund();						
	}


	//запуск раунда
	void StartRaund()
	{
		if (!IsRaundOn && currentTime > 0)
		{
			if (Input.GetMouseButton(0))
			{
				StartGame();				
			}				
		}
	}
	
	
	//изменяет время
	IEnumerator Timer()
	{
		while (currentTime > 0)
		{
			currentTime -= Time.deltaTime;			
			UpdateTimeUI();

			yield return null;
		}

		if (currentTime <= 0f)
		{
			currentTime = 0f;
			GameEnd();
		}				
	}


	//обновляет значение времени в UI
	void UpdateTimeUI()
	{
		int m = (int)(currentTime / 60f);
		int s = (int)(currentTime % 60f);

		string min = (m >= 10) ? m.ToString() : "0" + m.ToString();
		string sec = (s >= 10) ? s.ToString() : "0" + s.ToString();

		if (ChangeTimeEvent != null)
			ChangeTimeEvent(min, sec);
	}


	void StartGame()
	{
		IsRaundOn = true;

		StartCoroutine( Timer() );

		if (StartRaundEvent != null)
			StartRaundEvent();

		AudioManager.Instance.Play(StaticString.SOUND_GONG);
		AudioManager.Instance.Play(StaticString.BACKGRAUND_MUSIC);
	}


	void GameEnd()
	{
		if (ScoreBeaten > config.hiScore)
			config.hiScore = ScoreBeaten;

		IsRaundOn = false;

		if (StopRaundEvent != null)
			StopRaundEvent();

		AudioManager.Instance.Play(StaticString.SOUND_GONG);
		AudioManager.Instance.Stop(StaticString.BACKGRAUND_MUSIC);

		StartCoroutine( RestartGame() );
	}


	IEnumerator RestartGame()
	{
		yield return new WaitForSeconds(config.timeToRestart); 

		ResetValueDefault();
		StartGame();
	}


	//добавляет очки
	public void SetScore()
	{
		ScoreBeaten++;
		AudioManager.Instance.Play(StaticString.SOUND_ADD_POINT);

		if (ChangeScoreEvent != null)
			ChangeScoreEvent(ScoreBeaten);
	}

#endregion


}
