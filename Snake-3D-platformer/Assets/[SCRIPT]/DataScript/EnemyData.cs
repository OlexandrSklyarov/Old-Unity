using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject 
{

	public LayerMask obstacle;
	public GameObject explosionEffect;
	
}
