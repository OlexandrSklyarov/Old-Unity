using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour 
{

#region Var

	GameObject[] enemySpawnPoints;
	List<Smile> smiles;
	SpawnData data;
	int countMovingSmiles;

#endregion


#region Init
	
	void Awake() 
	{
		data = Resources.Load<SpawnData>("Data/SpawnData");	

		FindAllSpawnPoint();
		CreateSmile();			
	}


	void Start()
	{
		GameManager.Instance.StartRaundEvent += Restart;
	}
	

	//находит все точки для спавна врагов, и записывает в массив
	void FindAllSpawnPoint()
	{
		int countSpawnPoint = transform.childCount;
		enemySpawnPoints = new GameObject [countSpawnPoint];

		for (int i = 0; i < countSpawnPoint; i++)
		{
			enemySpawnPoints[i] = transform.GetChild(i).gameObject;
		}
	}


	void CreateSmile()
	{
		smiles = new List<Smile>();

		foreach(var point in enemySpawnPoints)
		{
			
			var go = Instantiate(data.smilePrefab) as GameObject;

			go.transform.parent = point.transform;
			go.transform.localPosition = Vector3.zero;

			var locRot = go.transform.localRotation;
			locRot = Quaternion.Euler(0f, 0f, 0f);			
			go.transform.localRotation = locRot;

			var smile = go.GetComponent<Smile>();
			smile.Init(this);
			smiles.Add(smile);
		}
	}
	
	
#endregion
	

#region Update

	void Update() 
	{
		if (!GameManager.Instance.IsRaundOn) return;

		StartSmile();
		FindMovingSmile();
	}


	//запуск смайлов
	void StartSmile()
	{
		while(countMovingSmiles < data.maxCountMovingSmiles)
		{
			var listFreeSmiles = FindFreeSmile();			
			var randomIndex = Random.Range(0, listFreeSmiles.Count);
			listFreeSmiles[randomIndex].MoveForward();
			countMovingSmiles++;			
		}
	}

	
	//находит смайлы, которые стоят
	List<Smile> FindFreeSmile()
	{
		var listFreeSmiles = new List<Smile>();

		foreach(var s in smiles)
		{
			if (s.IsStartPosition) 
				listFreeSmiles.Add(s);
		}

		return listFreeSmiles;
	}


	//считает смайлы, которые в движении
	void FindMovingSmile()
	{
		int count = 0;

		foreach(var s in smiles)
		{
			if (!s.IsStartPosition) 
				count++;
		}

		countMovingSmiles =  count;
	}


	//устанавливаем все смайли на стартовые позиции
	void Restart()
	{
		foreach(var s in smiles)
			s.Restart();
	}

#endregion


}
