using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{	

	public static GameManager Instance;


#region  Events

	public event Action<string, Color> SendGameMessage;
	public event Action GameEnd;

#endregion


#region Var

	public PhaseGame Phase {get; private set;}
	public Snake Snake {get; private set;}
	public SnakeInput SnakeInput {get; private set;}
	public MobileController MobControll {get; private set;}
	public SnakeCamHandler CamController {get; private set;}	
	
	public PortalController Portal {get; private set;}
	public ScalesController Scales {get; private set;}	
	public GameObject DynamicConteiner {get; private set;}	
	public DataManager ManagerData {get; private set;}
	
	
	private GameObject start;
	private ManagerUI managerUI;	
	private float delta;	
	private bool isUpdateTime;

#endregion


	void Awake()
	{
		if (Instance == null)
			Instance = this;
		
	}


	void Start () 
	{		
		Init();					
		EventSubscription();	
		Play();	
	}


	private void Init()
	{
		Phase = PhaseGame.IN_GAME;
		ManagerData = DataManager.Instance;	
		DynamicConteiner = GameObject.Find("Dynamic").gameObject;
		start = GameObject.Find("StartPortal").gameObject;

		CreateSnake();

		MobControll = GameObject.Find("InputButtonsUI").GetComponent<MobileController>();

		SnakeInput = GameObject.Find("InputPlayer").GetComponent<SnakeInput>();
		SnakeInput.Init();	

		CamController = GameObject.Find("CameraHolderSnake").GetComponent<SnakeCamHandler>();
		CamController.Init();	

		Scales = GameObject.FindObjectOfType<ScalesController>(); //весы
		Scales.Init();		

		Portal = GameObject.FindObjectOfType<PortalController>(); //портал
		Portal.Init();

		managerUI = GameObject.Find("UI_Manager").GetComponent<ManagerUI>();
		managerUI.Init();			

		isUpdateTime = true;

		StartMessage();
	}
	

	private void CreateSnake()
	{
		var snakePref = Resources.Load("Snake/Prefabs/Snake");
		var go = Instantiate(snakePref, start.transform.position + Vector3.up, start.transform.rotation) as GameObject;
		Snake = go.GetComponent<Snake>();
		Snake.transform.parent = DynamicConteiner.transform;		
	}
	

	/* подписка на события */
	void EventSubscription()
	{
		managerUI.EventUI += EventHandlerUI;
		Snake.AddItem += AddGameData;
		Snake.RemoveItem += RemoveGameData;
		Portal.OnSnakePortal += WinGame;
	}
	

	void Update () 
	{
		delta = Time.deltaTime;
		SnakeInput.Tick(delta);	

		if (isUpdateTime)
			StartCoroutine(GameTimerUpdate());	
	}


	void FixedUpdate () 
	{
		delta = Time.fixedDeltaTime;
		SnakeInput.FixedTick(delta);
	}


	void LateUpdate()
	{
		delta = Time.deltaTime;
		CamController.LateTick(delta);
	}


	/* реагирует на дуйчтвия нажатия кнопок на UI панели */
	void EventHandlerUI(ActionUI act)
	{
		switch(act)
		{
			case ActionUI.QUIT_GAME:
				AudioManager.Instance.Play(StaticPrm.AUDIO_QUIT_GAME);
				Application.Quit();
			break;
			case ActionUI.RETURN_TO_GAME:				
				Play();
				AudioManager.Instance.Play(StaticPrm.AUDIO_EXIT_SUB_MENU); // звук выхода из меню
				AudioManager.Instance.Play(StaticPrm.AUDIO_MUSIC_LEVEL); //запуск флновой муз
				AudioManager.Instance.Stop(StaticPrm.AUDIO_MUSIC_SUB_MENU); //стоп муз меню	
			break;
			case ActionUI.MENU_SETTINGS:							
				Stop();
				AudioManager.Instance.Play(StaticPrm.AUDIO_ENTER_SUB_MENU); //звук входа в меню
				AudioManager.Instance.Play(StaticPrm.AUDIO_MUSIC_SUB_MENU); //запуск муз в меню
				AudioManager.Instance.Stop(StaticPrm.AUDIO_MUSIC_LEVEL); //стоп фоновой муз
			break;
			case ActionUI.FP_CAMERA_ON:
				AudioManager.Instance.Play(StaticPrm.AUDIO_SELECT_IN_MENU);
				Set_ON_FPCamera();
			break;
			case ActionUI.FP_CAMERA_OFF:
				AudioManager.Instance.Play(StaticPrm.AUDIO_SELECT_IN_MENU);
				Set_OFF_FPCamera();
			break;
			case ActionUI.RESTERT_LEVEL:
				RestartGame(); //перезагружаем уровень без задержки (мгновенно)
			break;			
		}
	}


	void StartMessage()
	{
		if (SendGameMessage != null)
			SendGameMessage("GO!", Color.yellow);

		AudioManager.Instance.Play(StaticPrm.AUDIO_START_LEVEL); //звук начала уровня
		AudioManager.Instance.Play(StaticPrm.AUDIO_MUSIC_LEVEL); //запуск флновой муз
	}


	void Play()
	{
		Phase = PhaseGame.IN_GAME;
		Time.timeScale = 1f;
	}		


	void Stop()
	{
		Phase = PhaseGame.PAUSE;
		Time.timeScale = 0f;		
	}


	/* ВКЛЮЧАЕТ камеру от первого лица*/
	void Set_ON_FPCamera()
	{
		Snake.FPCamera.SetActive(true);
		CamController.gameObject.SetActive(false);	
	}


	/* ОТКЛЮЧАЕТ камеру от первого лица*/
	void Set_OFF_FPCamera()
	{
		Snake.FPCamera.SetActive(false);
		CamController.gameObject.SetActive(true);	
	}	


	void AddGameData(ItemType it)
	{
		switch(it)
		{
			case ItemType.FOOD:
				ManagerData.DataGame.foodCount++;
				LookMessage("Food + 1");				
			break;
			case ItemType.LIVE:
				ManagerData.DataGame.playerLive++;
				managerUI.UpdateLivesPanel();
				LookMessage("Live + 1");	
			break;
			case ItemType.TIME:
				ManagerData.DataGame.countGameTime += ManagerData.Config.amountAddTime;
				ChangeTime();
				LookMessage("Time + " + ManagerData.Config.amountAddTime);	
			break;
		}
	}


	void RemoveGameData(ItemType it)
	{
		switch(it)
		{
			case ItemType.FOOD:
				//нечего отнимать
			break;
			case ItemType.LIVE:
				if (ManagerData.DataGame.playerLive > 0)
				{
					ManagerData.DataGame.playerLive--;
					managerUI.UpdateLivesPanel();

					if (ManagerData.DataGame.playerLive <= 0)
						GameOver();
				}
			break;
			case ItemType.TIME:
				//нечего отнимать
			break;
		}
	}


	IEnumerator GameTimerUpdate()
	{
		isUpdateTime = false;
		yield return new WaitForSeconds(1f);

		if (ManagerData.DataGame.countGameTime > 0)
		{
			ManagerData.DataGame.countGameTime -= 1;
			ChangeTime();			
			isUpdateTime = true;
		}
	}


	void ChangeTime()
	{
		if (ManagerData.DataGame.countGameTime <= 0)
			GameOver();

		int m = ManagerData.DataGame.countGameTime / 60;
		int s = ManagerData.DataGame.countGameTime % 60;

		string min = (m >= 10) ? m.ToString() : "0" + m.ToString();
		string sec = (s >= 10) ? s.ToString() : "0" + s.ToString();

		managerUI.UpdateTimePanel(min, sec);
	}


	private void LookMessage(string str)
	{
		if (SendGameMessage != null)
			SendGameMessage(str, Color.yellow);
	}


	private void GameOver()
	{		
		if (SendGameMessage != null) SendGameMessage("GAME OVER", Color.red);	
		if (GameEnd != null) GameEnd();	

		AudioManager.Instance.Stop(StaticPrm.AUDIO_MUSIC_LEVEL); //стоп фоновой муз
		AudioManager.Instance.Play(StaticPrm.AUDIO_GAME_OVER);
		StartCoroutine(ReloadLevel());
	}


	private void WinGame()
	{
		if (SendGameMessage != null)
			SendGameMessage("YOU WIN !!!!", Color.green);

		Stop();
		AudioManager.Instance.Stop(StaticPrm.AUDIO_MUSIC_LEVEL); //стоп фоновой муз
		AudioManager.Instance.Play(StaticPrm.AUDIO_GAME_WIN);
	}


	//Перезагрузка уровня
	private IEnumerator ReloadLevel()
	{
		yield return new WaitForSeconds(ManagerData.Config.timeReloadLevel);
		SceneManager.LoadScene(ManagerData.DataGame.currentLevelName);
	}


	/* мгновенная перезагрузка уровня */
	private void RestartGame()
	{
		SceneManager.LoadScene(ManagerData.DataGame.currentLevelName);
	}
		
}

public enum PhaseGame
{
	IN_GAME, IN_MENU, PAUSE
}

