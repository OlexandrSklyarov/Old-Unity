using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameConfig", fileName = "GameConfig")]
public class GameConfig : ScriptableObject 
{

	[Range(0f, 600f)] public float raundTime; //время рауда в секундах
	[Range(1f, 5f)] public float timeToRestart; //время до рестарта игры

	public int hiScore; //лучший рузультат

}
