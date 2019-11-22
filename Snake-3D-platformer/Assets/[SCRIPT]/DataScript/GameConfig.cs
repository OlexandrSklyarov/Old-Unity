using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameConfig")]
public class GameConfig : ScriptableObject 
{

	public int gameTime; //стартовое время 
	public int playerLive; //количество жизней при старте игры
	public int minLengthSnakeTail; //минимальная длина хвоста для открытия портала
	public int amountAddTime; //количество времени добавляемого при подборе часов
	public float timeReloadLevel; //время до загрузки уровня по новой
	public float timeExplosionBomb; //время до взрыва бомбы
	public float timeDestroyItem; //время до исчезновения пердметов из коробок
	public float bombExplosionRadius; // радиус взрыва бомбы 
	public float playSoundRadius; // максимальная дистанци при которой слышно звуки от объектов


}
