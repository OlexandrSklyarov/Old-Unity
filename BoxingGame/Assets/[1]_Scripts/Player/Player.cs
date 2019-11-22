using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{

#region Var
	
	PlayerData data;	
	Vector3 moveDirection;	
	Vector3 startPosition;	
	bool isCanMove;
	bool isPunch;

#endregion


#region Init

	void Start()
	{
		InputManager.Instance.SwipeEvent += Move; //подписка

		data = Resources.Load<PlayerData>("Data/PlayerData");
		startPosition = transform.position;
		isCanMove = true;
	}

#endregion


#region Update

	
	void Move(Vector3 _dir)
	{		
		if (!GameManager.Instance.IsRaundOn) return;

		if (IsStartPosition() && !isPunch)
		{			
			moveDirection = _dir;	
			isPunch = true;
			
			AudioManager.Instance.Play(StaticString.SOUND_JAB);
			StartCoroutine(MovingForward());
		}
	}


	bool IsStartPosition()
	{		
		return Vector3.Distance(startPosition, transform.position) <= 0.01f;
	}


	IEnumerator MovingForward()
	{
		while(isCanMove)
		{				
			var dir = moveDirection;
			dir.z = 0f;
			transform.Translate(dir * data.moveSpeed * Time.deltaTime, Space.World);	

			Rotation();						

			yield return null;
		}	

		StartCoroutine(MovingBack());	
	}


	void Rotation()
	{
		var dir = transform.TransformDirection(moveDirection);
		var angle = Vector3.SignedAngle(transform.up, dir, transform.forward);
		var rot = Quaternion.AngleAxis(angle, transform.forward);
		transform.rotation = rot;		
	}


	IEnumerator MovingBack()
	{
		while(Vector3.Distance(startPosition, transform.position) > 0.01f)
		{
			var dir = startPosition - transform.position;
			dir.z = 0f;			
			transform.Translate(dir * data.moveSpeed  * Time.deltaTime, Space.World);	

			yield return null;		
		}

		isCanMove = true;
		isPunch = false;

	}

#endregion


#region Collision

	void OnTriggerEnter(Collider other)
	{
		var tag = other.gameObject.tag;

		if (tag == StaticString.TAG_WALL)
			isCanMove = false;

		if (tag == StaticString.TAG_ENEMY)
		{
			isCanMove = false;
			var smile = other.gameObject.GetComponent<Smile>();
			smile.Damage();
			AudioManager.Instance.Play(StaticString.SOUND_BEAT);
		}
	}

#endregion


}
