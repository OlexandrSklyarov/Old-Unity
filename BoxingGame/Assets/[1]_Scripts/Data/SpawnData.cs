using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/SpawnData", fileName = "SpawnData")]
public class SpawnData : ScriptableObject 
{
	public GameObject smilePrefab;
	public int maxCountMovingSmiles;

}
