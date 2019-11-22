using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour 
{

#region Var

	GameManager gameManager;
	GameObject infoPanel;
	GameObject resultPanel;
	Text scoreText;
	Text timeText;
	Text hiScoreText;
	


#endregion

	
#region  Init

	void Awake()
	{
		infoPanel = GameObject.Find("INFO_PANEL").gameObject;
		timeText = infoPanel.transform.Find("TIME_TEXT").GetComponent<Text>();
		scoreText = infoPanel.transform.Find("SCORE_TEXT").GetComponent<Text>();

		resultPanel = GameObject.Find("RESULT_PANEL").gameObject;
		hiScoreText = resultPanel.transform.Find("HI_SCORE_TEXT").GetComponent<Text>();

		HideInfoPanel();		
		HideResultPanel();
	}	


	void Start () 
	{
		gameManager = GameManager.Instance;
		Subscription();
	}


	void Subscription()
	{
		gameManager.StartRaundEvent += StartGame;
		gameManager.StopRaundEvent += StopGame;
		gameManager.ChangeScoreEvent += UpdateScoreText;
		gameManager.ChangeTimeEvent += UpdataTimeText;
	}

#endregion


#region Updata

	//обновление времени
	void UpdataTimeText(string min, string sec)
	{
		timeText.text = min + " : " + sec;
	}


	//обновление счёта
	void UpdateScoreText(int score)
	{
		scoreText.text = "SCORE: " + score;
	}


	void UpdateHiScore()
	{
		hiScoreText.text = "HI - SCORE: " + gameManager.HiScore;
	}


	//скрывает инфо панель
	void HideInfoPanel()
	{
		infoPanel.SetActive(false);
	}


	//показывает инфо панель
	void ActiveInfoPanel()
	{
		infoPanel.SetActive(true);
	}


	//показать панель результата
	void ActiveResultPanel()
	{
		resultPanel.SetActive(true);
		UpdateHiScore();
	}


	//скрыть панель результата
	void HideResultPanel()
	{
		resultPanel.SetActive(false);
	}

	
	void StartGame()
	{		
		UpdateScoreText(gameManager.ScoreBeaten);
		ActiveInfoPanel();
		HideResultPanel();		
	}
	

	void StopGame()
	{
		HideInfoPanel();
		ActiveResultPanel();
	}

#endregion
	
	
}
