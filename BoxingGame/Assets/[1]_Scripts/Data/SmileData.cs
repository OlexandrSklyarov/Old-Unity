using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/SmileData", fileName = "SmileData")]
public class SmileData : ScriptableObject 
{

	public Sprite[] sprites;		

	[Range(0.1f, 2.5f)] public float maxForwardDistance;
	[Range(0.1f, 0.5f)] public float minMoveSpeed;
	[Range(0.6f, 1f)] public float maxMoveSpeed;

	[Range(0.1f, 0.9f)] public float timeDeleyMin;
	[Range(1f, 3f)] public float timeDeleyMax;

}
