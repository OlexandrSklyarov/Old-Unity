using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Controller/Camera values collision")]
public class CameraValuesCollision : ScriptableObject 
{

	public float speedX;
	public float speedY;
	public float limitY;
	public float minDistance;	
	public LayerMask obstacles;

}
