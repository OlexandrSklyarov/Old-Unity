using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour 
{
	public class GameData
	{
		public int playerLive;
		public int countGameTime;
		public int foodCount;
		public int minLengthSnakeTail;
		public string currentLevelName;
	}	

	public static DataManager Instance;



	public GameData DataGame {get; set;}
	public CameraValuesCollision CameraData_2 {get; private set;} //настройки другой камеры, которая не используется
	public CameraValues CameraData {get; private set;}
	public SnakeData SnakeData {get; private set;}
	public EnemyData EnemyDataKvak {get; private set;}
	public GameConfig Config {get; private set;}
	public FoodData FoodData {get; private set;}
	public EnemyBladeManData EnemyBladeManData {get; private set;}
	


	void Awake()
	{
		if (Instance == null)
			Instance = this;
	
		Init();			
	}


	void Init()
	{
		SetGameData();
		CameraData = Resources.Load<CameraValues>("Snake/Data/CameraValuesSnake");	
		CameraData_2 = Resources.Load<CameraValuesCollision>("Snake/Data/CameraValuesCollision");	
		SnakeData = Resources.Load<SnakeData>("Snake/Data/SnakeData");	
		EnemyDataKvak  = Resources.Load<EnemyData>("DataResources/EnemyDataKvak");
		EnemyBladeManData = Resources.Load<EnemyBladeManData>("DataResources/EnemyBladeManData");
		FoodData = Resources.Load<FoodData>("DataResources/FoodData");	
	}


	private void SetGameData()
	{
		DataGame = new GameData();
		Config = Resources.Load<GameConfig>("DataResources/GameConfig");
		DataGame.countGameTime = Config.gameTime;
		DataGame.playerLive = Config.playerLive;
		DataGame.foodCount = 0;
		DataGame.minLengthSnakeTail = Config.minLengthSnakeTail;	
		DataGame.currentLevelName =  SceneManager.GetActiveScene().name;//Application.loadedLevel; //записываем текущий уровень
	}
}
