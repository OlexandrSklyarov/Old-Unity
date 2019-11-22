using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Snake/Snake Data")]
public class SnakeData : ScriptableObject 
{

	public float timeDamage; // время безсмертия
	public float speedRun; //обычная скорость бега
	public float speedFastRun;	//ускореный бег
	public float runAcceleration; //ускорение  при беге
	public float hightJump;
	public float jumpAcceleration; // сила добавляемая к силе прыжка
	public float rotationSpeed;
	public float graundDistance;
	public int maxLenthTail; //макс. длина хвоста (ячеек)
	public LayerMask graundLayer;
	public LayerMask waterLayer;
	
	
}
