using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/FoodData")]
public class FoodData : ScriptableObject 
{
	public float[] jumpPower;
	public float[] speed;
	public Color[] colors;
	public float checkDistanceDown;	
	public float checkRadius;	
	public LayerMask graundLayer;
	public LayerMask waterLayer;
	
	
}
