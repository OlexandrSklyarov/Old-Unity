using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCamHandler : MonoBehaviour 
{
	[System.Serializable]
	public class CollisionProp
	{
		public bool isCollisionWall;
		public float x;
		public float y;
		public float z;
	}

	public CameraValues Values {get; set;}
	public Transform camTrans;
	public Transform target;
	public Transform pivot;
	public Transform mTransform;	

	float delta;

	float mouseX;
	float mouseY;
	float smoothX;
	float smoothY;
	float smoothXvelocity;
	float smoothYvelocity;
	float lookAngle;
	float tiltAngle;

	
	[SerializeField] CollisionProp camColSettings;
	[SerializeField] bool isCollisionCamera;

	bool isGameOver;
	

	
	Snake snake;


	public void Init()
	{		
		var gameManager = GameManager.Instance;
		mTransform = this.transform;
		snake = gameManager.Snake;
		target = snake.mTransform;	
		pivot = mTransform.Find("Pivot");
		camTrans = pivot.transform.Find("CamTrans");		
		Values = DataManager.Instance.CameraData;
		camColSettings = new CollisionProp();
		isGameOver = false;

		gameManager.GameEnd += GameEnd; //подписка на событие окончания игры
	}

	public void LateTick (float d)
	{
		if (isGameOver)
		{
			RotationOnCircle();
			return;
		}


		delta = d;

		if (target == null)
			return;

		HandlePosition();		
		HandleRotation();
				

		float speed = Values.movespeed;

		Vector3 targetPosition = Vector3.Lerp(mTransform.position, target.position, delta * speed);
		mTransform.position = targetPosition;

	}	


	void HandlePosition()
	{
		float targetX = Values.normalX;
		float targetY = Values.normalY;
		float targetZ = Values.normalZ;

		float t = delta * Values.adaptSpeed;
		
		if (isCollisionCamera) ObstacleReact(ref targetY, ref targetZ);		

		Vector3 newPivotPositon = pivot.localPosition;
		newPivotPositon.x = targetX;
		newPivotPositon.y = targetY;

		Vector3 newCamPosition = camTrans.localPosition;
		newCamPosition.z = targetZ;		
		
		pivot.localPosition = Vector3.Lerp(pivot.localPosition, newPivotPositon, t);
		camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, newCamPosition, t);		
	}


	void HandleRotation()
	{
		mouseX = snake.inp.horizontal; //Input.GetAxis(StaticPrm.MOUSE_X); 
		mouseY = 0f;//Input.GetAxis(StaticPrm.MOUSE_Y);

		if (Values.turnSmooth > 0f)
		{
			smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothXvelocity, Values.turnSmooth);
			smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothYvelocity, Values.turnSmooth);
		}
		else
		{
			smoothX = mouseX;
			smoothY = mouseY;
		}

		lookAngle += smoothX * Values.y_rotate_speed;
		Quaternion targetRot = Quaternion.Euler(0f, lookAngle, 0f);
		mTransform.rotation = targetRot;

		tiltAngle -= smoothY * Values.x_rotate_speed;
		tiltAngle = Mathf.Clamp(tiltAngle, Values.minAngle, Values.maxAngle);
		pivot.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
	}


	void  ObstacleReact(ref float y, ref float z)
	{			
				
		var dir = camTrans.position - target.transform.position;
		var dist = dir.magnitude;	
		var smoothSpeed = 1.5f;	
		var minValue = 0.01f;
		RaycastHit hit;	

		//Debug.DrawLine(targetPos, locPos, Color.blue);

		if (Physics.Raycast(pivot.transform.position, dir, out hit, dist, Values.obstacle))
		{
			camColSettings.z = -(hit.point - pivot.transform.position).magnitude;			
			camColSettings.y = (camColSettings.y > 2f) ?  
								Mathf.Lerp(camColSettings.y, Mathf.Abs(camColSettings.z), delta * smoothSpeed) : 2f;			
		}	
		else if (!Physics.Raycast(target.transform.position, dir, dist + 1f, Values.obstacle))	
		{
			if (camColSettings.y < Values.normalY)
			{
				camColSettings.y = Mathf.Lerp(camColSettings.y, Values.normalY, delta * smoothSpeed);				
			}

			if (camColSettings.z > Values.normalZ)
			{
				camColSettings.z = Mathf.Lerp(camColSettings.z, Values.normalZ, delta * smoothSpeed);				
			}
		}	
		else
		{
			if (Mathf.Abs(camColSettings.y - Values.normalY) > minValue)
			{
				camColSettings.y = Mathf.Lerp(camColSettings.y, Values.normalY, delta * smoothSpeed);				
			}
			else
				camColSettings.y = Values.normalY;

			if (Mathf.Abs(camColSettings.z - Values.normalZ) > minValue)
			{
				camColSettings.z = Mathf.Lerp(camColSettings.z, Values.normalZ, delta * smoothSpeed);				
			}
			else
			{
				camColSettings.z = Values.normalZ;
			}			
		}

		y = camColSettings.y;
		z = camColSettings.z;
			
	}


#region  GAMEOVER methods

	/* вращение по кругу */
	void RotationOnCircle()
	{
		mouseX = 1f; 
		mouseY = 0f;

		if (Values.turnSmooth > 0f)
		{
			smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothXvelocity, Values.turnSmooth);
			smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothYvelocity, Values.turnSmooth);
		}
		else
		{
			smoothX = mouseX;
			smoothY = mouseY;
		}

		lookAngle += smoothX * Values.y_rotate_speed;
		Quaternion targetRot = Quaternion.Euler(0f, lookAngle, 0f);
		mTransform.rotation = targetRot;

		tiltAngle -= smoothY * Values.x_rotate_speed;
		tiltAngle = Mathf.Clamp(tiltAngle, Values.minAngle, Values.maxAngle);
		pivot.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
	}

	void GameEnd()
	{
		isGameOver = true;
	}

#endregion


}