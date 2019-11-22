using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour 
{
	[SerializeField] private GameManager managerGame;	
	private DataManager dataManager;


#region Events

	public event Action<ActionUI> EventUI;

#endregion

#region  UI Game INTERFACE

	[Header("Game INTERFACE")]

	[SerializeField] private GameObject inputButtonsUI;
	[SerializeField] private GameObject uiComponets;	
	[SerializeField] private Slider powerJumpSlider;	
	[SerializeField] private GameObject livePanel;	
	[SerializeField] private GameObject timePanel;	

#endregion

#region MenuSettings

	[Header("Menu Settings")]

	[SerializeField] private GameObject menuSettingsComponent;

	//музыка
	[SerializeField] private Slider musicVolumeSlider;
	[SerializeField] private Text vm_num_text;

	//звуки
	[SerializeField] private Slider soundVolumeSlider;
	[SerializeField] private Text sound_num_text;	

	//переключатель камеры
	[SerializeField] private GameObject fpCameraTogle;
	[SerializeField] private GameObject fpc_togle_ON;
	private bool isTogleFPC_On; //переключатель
	

#endregion

#region GameMessagePanel

	[Header("Message Panel")]

	[SerializeField] private GameObject gameMessagePanel;
	[SerializeField] private Text messageText;	
	private readonly float timeActiveMessage = 2f;	

#endregion
	
	

	public void Init () 
	{
		managerGame = GameManager.Instance;
		dataManager = DataManager.Instance;	

		SetUiComponents();		
		SetMenuSettings();
		SetMessagePanel();

		SetValueInit(); //устанавливаем слайдеры настроек в нужное положение	
		EventSubscription();
		HideGameMenu();			
		UpdateLivesPanel();		
	}


	/* находим нужные компоненты UI */
	void SetUiComponents()
	{		
		uiComponets = GameObject.Find("UIComponets").gameObject;
		powerJumpSlider = uiComponets.transform.GetChild(0).gameObject.GetComponent<Slider>();
		powerJumpSlider.gameObject.SetActive(false);
		livePanel = uiComponets.transform.GetChild(2).gameObject;
		timePanel = uiComponets.transform.GetChild(3).gameObject;	

		inputButtonsUI =  GameObject.Find("InputButtonsUI").gameObject;	

		ActiveUIElement();
	}


	/* находит и устанавливает компоненты меню настроек */
	private void SetMenuSettings()
	{
		menuSettingsComponent = GameObject.Find("Menu_settings").gameObject;
		var menuPanel = menuSettingsComponent.transform.GetChild(0).gameObject;

		musicVolumeSlider = menuPanel.transform.Find("VOLUME_MUSIC_SLIDER").GetComponent<Slider>();
		vm_num_text = menuPanel.transform.Find("VM_NUM_VALUE").GetComponent<Text>();

		soundVolumeSlider = menuPanel.transform.Find("SOUND_SLIDER").GetComponent<Slider>();
		sound_num_text = menuPanel.transform.Find("SFX_NUM_VALUE").GetComponent<Text>();
			
		fpCameraTogle = menuPanel.transform.Find("FP_CAMERA_TOGLE").gameObject;
		fpc_togle_ON = fpCameraTogle.transform.Find("ON_TOGLE").gameObject;
		isTogleFPC_On = false;
		fpc_togle_ON.SetActive(isTogleFPC_On);
		
	}


	/* настраивает панель сообщений из игры */
	private void SetMessagePanel()
	{
		gameMessagePanel = GameObject.Find("GameMessagePanel").gameObject;
		messageText = gameMessagePanel.transform.GetChild(0).GetComponent<Text>();
		messageText.color = new Color(225, 227, 0, 255); //жёлтый
		gameMessagePanel.SetActive(false);		
	}


	/* задает параметры настройкам игры в меню */
	void SetValueInit()
	{
		musicVolumeSlider.value = AudioManager.Instance.GetMusicValue(); //музыка
		vm_num_text.text = System.Math.Ceiling(musicVolumeSlider.value * 100f).ToString();

		soundVolumeSlider.value = AudioManager.Instance.GetSoundValue(); //звуки
		sound_num_text.text = System.Math.Ceiling(soundVolumeSlider.value * 100f).ToString();
	}



	/* подписка на события  */
	void EventSubscription()
	{
		managerGame.GameEnd += HideUIElements; // Game over
		managerGame.Snake.PowerJumpEvent += SnakePowerJumpVisualization; // snake jump value
		managerGame.SendGameMessage += DisplayingMessage;  // game manager message
		managerGame.Scales.OnScalesMessage += DisplayingMessage;  // ScalesController massage
	}


	/* скрывает панель меню */
	void HideGameMenu()
	{
		menuSettingsComponent.SetActive(false);
	}


	/* скрывает кнопки мобыльного управления и UI элементы с экрана */
	void HideUIElements()
	{
		uiComponets.SetActive(false);
		inputButtonsUI.SetActive(false);		
	}


	/* показывает UI элементы (управление и инфопанель) */
	void ActiveUIElement()
	{
		uiComponets.SetActive(true);
		inputButtonsUI.SetActive(true);
	}
	
	
	/* отображает визуально силу прыжка змейки */
	public void SnakePowerJumpVisualization(float power) 
	{
		if (power > 0.1f)
		{
			powerJumpSlider.gameObject.SetActive(true);
			powerJumpSlider.value = power;
		}
		else
		{
			powerJumpSlider.gameObject.SetActive(false);
		}
	}


#region Changing values void

	
	/* показывает всплывающее сообщение из игры на экран */
	private void DisplayingMessage(string message, Color color)
	{
		gameMessagePanel.SetActive(true);
		messageText.text = message;
		messageText.color = color;
		StartCoroutine(HideMessage());

	}


	/*  */
	private IEnumerator HideMessage()
	{
		yield return new WaitForSeconds(timeActiveMessage);
		gameMessagePanel.SetActive(false);
	}


	/* устанавливаем громкость МУЗЫКИ слайдером */
	public void SetMusic()
	{
		AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value); //музыка	
		vm_num_text.text = System.Math.Ceiling(musicVolumeSlider.value * 100f).ToString();
	}


	/* устанавливаем громкость ЗВУКОВ слайдером */
	public void SetSound()
	{
		AudioManager.Instance.SetSoundVolume(soundVolumeSlider.value); //звуки
		sound_num_text.text = System.Math.Ceiling(soundVolumeSlider.value * 100f).ToString();
	}


	/* обновляет панель инвормации (жихзни) */
	public void UpdateLivesPanel()
	{
		var textLive = livePanel.transform.GetChild(1).gameObject.GetComponent<Text>();
		textLive.text = " x " + dataManager.DataGame.playerLive;
	}


	/* обновляет панель инвормации (время) */
	public void UpdateTimePanel(string min, string sec)
	{
		var textTime = timePanel.transform.GetChild(0).gameObject.GetComponent<Text>();				
		textTime.text = String.Format("{0} : {1}", min, sec);
	}

#endregion


#region Button controll void

	/* сбрасывает игру на начало */
	public void GameRestart()
	{
		if (EventUI != null)
			EventUI(ActionUI.RESTERT_LEVEL);
	}


	/* выход из игры полностью */
	public void QuitGame()
	{
		if (EventUI != null)
			EventUI(ActionUI.QUIT_GAME);
	}


	/* выходиз меню настроек в игру */
	public void ReturnToGame()
	{
		if (EventUI != null)
			EventUI(ActionUI.RETURN_TO_GAME);

		menuSettingsComponent.SetActive(false);
	}


	/* вход в меню настроек */
	public void EnterMenuSettings()
	{
		if (EventUI != null)
			EventUI(ActionUI.MENU_SETTINGS);

		menuSettingsComponent.SetActive(true);
	}


	/* обрабатывает нажатие на кнопку включения FPCamera */
	public void TogleFPCamera()
	{
		//инвертируем флаг на противоположный
		isTogleFPC_On = !isTogleFPC_On;

		if (isTogleFPC_On)
		{
			if (EventUI != null) EventUI(ActionUI.FP_CAMERA_ON);			
		}			
		else
		{
			if (EventUI != null) EventUI(ActionUI.FP_CAMERA_OFF);			
		}
			
		fpc_togle_ON.SetActive(isTogleFPC_On);
	}

#endregion

}

public enum ActionUI
{
	QUIT_GAME, 
	RETURN_TO_GAME, 
	MENU_SETTINGS,
	FP_CAMERA_ON,
	FP_CAMERA_OFF,
	RESTERT_LEVEL
}
