using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
	public Transform mTransform {get; private set;}

	
	private Transform target;	
	private Vector3 localPosition;
	private float currentYRotation;
	private float maxDistance;
	private float delta;	
	private CameraValuesCollision values;


	private Vector3 position
	{
		get {return mTransform.position;}
		set {mTransform.position = value;}
	}


	public void Init () 
	{
		mTransform = transform;		
		values = DataManager.Instance.CameraData_2;			
		maxDistance = (target.position - position).magnitude;
		localPosition = target.InverseTransformPoint(position);

	}
	
	
	public void LateTick (float d)
	{
		delta = d;
		position = target.TransformPoint(localPosition);
		CameraRotation();
		ObstacleReact();
		localPosition = target.InverseTransformPoint(position);
	}


    private void ObstacleReact()
    {
        Vector3 dir = position - target.position;
		float dist = dir.magnitude;
		RaycastHit hit;

		if (Physics.Raycast(target.position, dir, out hit, maxDistance, values.obstacles))
		{
			position = hit.point;
		}
		else if (dist < maxDistance &&  !Physics.Raycast(position, -mTransform.forward, -1f, values.obstacles))
		{
			position -= mTransform.forward * 0.5f;
		}
    }


    private void CameraRotation()
    {
        float mx = 0f;//gameManager.MobControll.InputVector.x;//Input.GetAxis(StaticPrm.MOUSE_X);
		float my = 0f;//gameManager.MobControll.SwipeVector.y;//Input.GetAxis(StaticPrm.MOUSE_Y);	

		/* поворот по вертикале вверх/вниз */
		if (my != 0f)
		{
			var tmp = Mathf.Clamp(currentYRotation + my * -values.speedY * delta, -values.limitY, values.limitY);

			if (tmp != currentYRotation)
			{
				var rot = tmp - currentYRotation;
				mTransform.RotateAround(target.position, mTransform.right, rot);
				currentYRotation = tmp;
			}
		}

		/* поворот по горизонтали вправо/влево */
		if (mx != 0f)
		{
			mTransform.RotateAround(target.position, Vector3.up, mx * values.speedX * delta);
		}

		mTransform.LookAt(target);
    }
}
