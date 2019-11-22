using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour 
{
	private Transform mTransform;
	private Vector3 StartPosition;
	public Vector3 MoveDirection {get; private set;}
	[SerializeField] private float maxDistance;
	[SerializeField] private float speedMove;
	[SerializeField] MoveAxis moveAxis;
	private bool isCanRevers;	
	

	
	void Start () 
	{
		mTransform = transform;
		StartPosition = mTransform.position;
		isCanRevers = true;
	}
	
	
	void Update () 
	{
		MovingPlatform();
	}

	void MovingPlatform()
	{
		/* если я дальше максимума от стартовой позициии и разрешено изменять напрвление - меняем */
		if (Vector3.Distance(StartPosition, mTransform.position) > maxDistance & isCanRevers)
		{
			speedMove *= -1f;
			StartCoroutine(OnRevers());
		}
			

		switch(moveAxis)
		{
			case MoveAxis.X_AXIS:
				MoveDirection = new Vector3(1f ,0f, 0f);
			break;
			case MoveAxis.Y_AXIS:
				MoveDirection = new Vector3(0f ,1f, 0f);
			break;
			case MoveAxis.Z_AXIS:
				MoveDirection = new Vector3(0f ,0f, 1f);
			break;
		}

		MoveDirection *= speedMove * Time.deltaTime;

		mTransform.Translate(MoveDirection);

	}

	/* чразрешаем менять напрвление по таймеру */
	IEnumerator OnRevers()
	{
		isCanRevers = false;
		yield return new WaitForSeconds(1f);		
		isCanRevers = true;
	}
}

public enum MoveAxis
{
	X_AXIS, Y_AXIS, Z_AXIS
}
