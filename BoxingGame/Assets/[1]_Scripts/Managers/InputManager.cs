using UnityEngine;
using System;


public class InputManager : MonoBehaviour
{

#region VAR

	public static InputManager Instance {get; private set;}

	Vector3 moveDirection;
	Vector3 startPos;
	bool isSwipe;
	

#endregion


#region  Event

	public event Action<Vector3> SwipeEvent;

#endregion


#region Init

	void Awake()
	{
		if (Instance == null)
			Instance = this;
				
		moveDirection = Vector3.zero;	
	}

#endregion


#region Update

	void Update()
	{
		SetDirection();
	}


	void SetDirection()
	{
		if (Input.GetMouseButtonDown(0))
		{
			startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);	
			isSwipe = false;				
		}
		else if (Input.GetMouseButton(0) && !isSwipe)
		{
			var curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var dir = new Vector3(curMousePos.x, curMousePos.y, 0f) - new Vector3(startPos.x, startPos.y, 0f);
			
			if (dir.magnitude > 1f)
			{
				moveDirection = dir.normalized;

				if (SwipeEvent != null)
					SwipeEvent(moveDirection);

				isSwipe = true;
			}						
		}	
	}

#endregion


}
