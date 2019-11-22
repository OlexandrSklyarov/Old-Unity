using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailPart : MonoBehaviour 
{

	Transform originTarget;	
	protected float offset = 0.3f;
	protected float rotSpeed = 10f;
	protected float speed;
	protected float distToOrigin;
	protected float delta;


	public Transform mTransform;



	public void Init()
	{
		mTransform = transform;			
	}


	public void SetOrigin(Transform originTarget)
	{
		this.originTarget = originTarget;
	}


	public void Tick(float d, float s)
	{
		delta = d;
		speed = s;
		distToOrigin = Vector3.Distance(mTransform.position, originTarget.position); 
		Moving();
		Rotation();
	}


	protected void Moving()
	{
		if (distToOrigin > offset)
		{
			mTransform.position = Vector3.Lerp(mTransform.position, originTarget.position, (speed + 1f) * delta);
		}	

		if (distToOrigin < offset)
		{
			var  pos = originTarget.position + mTransform.forward * -1 * offset;
			mTransform.position = Vector3.Lerp(mTransform.position, pos, (speed + 1f) * delta);
		}	

		if (Mathf.Abs(originTarget.position.y - mTransform.position.y) > 0.01f)
		{
			var mPos = mTransform.position;
			mPos.y = Mathf.Lerp(mTransform.position.y , originTarget.position.y, speed * delta);
			mTransform.position = mPos;
		}			
		
	}


	protected void Rotation()
	{
		Vector3 targetDir = originTarget.position - mTransform.position;		

		if (targetDir == Vector3.zero)
			targetDir = mTransform.forward;

		Quaternion lookDir = Quaternion.LookRotation(targetDir);
		Quaternion targetRot = Quaternion.Slerp(mTransform.rotation, lookDir, rotSpeed * delta);
		mTransform.rotation = targetRot;
		
	}
}
