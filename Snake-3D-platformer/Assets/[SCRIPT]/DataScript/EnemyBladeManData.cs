using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/BladeManData")]
public class EnemyBladeManData : ScriptableObject 
{
	public float[] jumpPower;
	public float[] speed;	
	public float checkDistanceDown;	
	public float checkRadius;	
	public float timeToJump; //время за которое пройдет анимация старта прыжка
	public GameObject explosionEffect; //еффект при разрушении
	public LayerMask graundLayer;
	public LayerMask waterLayer;
	
	
}