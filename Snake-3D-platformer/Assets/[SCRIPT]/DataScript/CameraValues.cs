using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Controller/Camera values")]
public class CameraValues : ScriptableObject 
{

	public float turnSmooth = 0.1f;
	public float movespeed = 9f;
	public float x_rotate_speed = 8f;
	public float y_rotate_speed = 8f;
	public float minAngle = -35f;
	public float maxAngle = 35f;
	public float normalX;
	public float normalY;
	public float normalZ = -35f;
	public float adaptSpeed = 9f;

	public LayerMask obstacle; //какие слои считать за препятствия
}
