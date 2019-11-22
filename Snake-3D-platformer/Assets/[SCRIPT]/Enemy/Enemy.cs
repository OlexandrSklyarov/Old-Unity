using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{	
	private Transform mTransform;
	private Rigidbody rb;			
	private Vector3 moveDirection;
	private DataManager dataManager;	
	[SerializeField] private float speed;
	


	void Start()
	{
		dataManager = DataManager.Instance;
		gameObject.tag = StaticPrm.TAG_ENEMY;
		mTransform = transform;
		rb = GetComponent<Rigidbody>();			
		moveDirection = mTransform.forward;
	}


	void FixedUpdate()
	{		
		Move();
		Rotation();
	}


	void Move()
	{	
		StartCoroutine(GetDirection());	
		var moveDirNorm = moveDirection.normalized * speed * Time.deltaTime;		
		rb.velocity = moveDirNorm;
		MoveAudio();
	}


	void Rotation()
	{
		Vector3 targetDir = moveDirection;
		targetDir.y = 0f;

		if (targetDir == Vector3.zero)
			targetDir = mTransform.forward;

		Quaternion lookDir = Quaternion.LookRotation(targetDir);
		Quaternion targetRot = Quaternion.Slerp(mTransform.rotation, lookDir, 360f * Time.deltaTime);
		mTransform.rotation = targetRot;
	}


	IEnumerator GetDirection()
	{
		yield return new WaitForSeconds(1f);

		var mData = dataManager.EnemyDataKvak;
		var dist = 2f;	
		var origin = mTransform.position + Vector3.up * 0.5f;
		var origin2 = origin + mTransform.forward * dist;			

		/* проверяем на препятствие впереди, и на землю под ногами  */
		if (Physics.Raycast(origin, mTransform.forward, dist, mData.obstacle)
			|| !Physics.Raycast(origin2, Vector3.down, dist, mData.obstacle))
		{
			moveDirection = mTransform.forward * -1f;
		}			
		
	}


	private void OnCollisionEnter(Collision other) 
	{
		if (other.gameObject.tag == StaticPrm.TAG_SNAKE)
			other.gameObject.GetComponent<Snake>().Damage();

		
	}

	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == StaticPrm.TAG_SNAKE_TONGUE)
		{			
			var exp = dataManager.EnemyDataKvak.explosionEffect;
			var goExp = Instantiate(exp, mTransform.position, Quaternion.identity);
			DestroyAudio();
			Destroy(goExp, 0.2f);
			Destroy(gameObject);
		}
			
	}


	void MoveAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 
														DataManager.Instance.Config.playSoundRadius)
			return;
			
		AudioManager.Instance.Play(StaticPrm.AUDIO_ENEMY_KVAK_MOVE);
	}

	void DestroyAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 
														DataManager.Instance.Config.playSoundRadius)
			return;
			
		AudioManager.Instance.Play(StaticPrm.AUDIO_ENEMY_DESTROY);
	}

	

}
