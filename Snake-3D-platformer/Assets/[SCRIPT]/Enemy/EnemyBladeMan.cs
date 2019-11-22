using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBladeMan : MonoBehaviour 
{
#region VAR
	
	private Transform mTransform;		
	private Rigidbody rb;
	private Animator anim;
	private EnemyBladeManData data;	
	private int myIndex;
	[SerializeField] protected Vector3 moveDirection;	
	[SerializeField] private bool isJump;
	[SerializeField] private bool isGraund;
	private const int MAX_INDEX = 2;

	
#endregion

	private void Start()
	{
		Init(Random.Range(0, MAX_INDEX + 1)); //на один больше, чтоб захватывало весь диапазон
	}
	

	private void Init (int index) 
	{
		myIndex = index;
		mTransform = transform;
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		data = DataManager.Instance.EnemyBladeManData;		
	}	


	private void Update()
	{
		isGraund = OnGraund();	
		Animation();			
	}
	

	private void FixedUpdate() 
	{
		if (isGraund && !isJump)
		{			
			Moving();						
		}
	}

	
	private void Animation()
	{
		anim.SetBool(StaticPrm.animJump, isJump);
		anim.SetBool(StaticPrm.animGraund, isGraund);
	}


	private void Moving()
	{
		isJump = true;					
		StartCoroutine(Jump());
			
	}


	private IEnumerator Jump()
	{
		yield return new WaitForSeconds(data.timeToJump);	
		moveDirection = GetDirection();
		var dir = Vector3.zero;
		dir = moveDirection * data.speed[myIndex];		 		
		dir.y = data.jumpPower[myIndex];
		rb.velocity = dir;	

		JumpAudio();

		StartCoroutine(CanJump());
	}

	private IEnumerator CanJump()
	{
		yield return new WaitForSeconds(0.2f);	
		isJump = false;
	}
	

	private Vector3 GetDirection()
	{
		var forward = Vector3.forward;
		var back = Vector3.forward * -1f;
		var right = Vector3.right;
		var left = Vector3.right * -1f;
		var mPos = mTransform.position;
			
		var dir = -Vector3.up;
		var offset = data.checkDistanceDown / 2f;
		var centr = mPos + Vector3.up * offset;

		Vector3 origin_forward = centr + forward * data.checkRadius;
		Vector3 origin_back = centr + back * data.checkRadius;
		Vector3 origin_right = centr + right * data.checkRadius;
		Vector3 origin_left = centr + left * data.checkRadius;

		Vector3 [] originArray = new Vector3 [4];

		originArray[0] = origin_forward;
		originArray[1] = origin_back;
		originArray[2] = origin_right;
		originArray[3] = origin_left;

		List<Vector3> listDirection = new List<Vector3>();

		foreach(Vector3 origin in originArray)
		{
			RaycastHit hit;
			Debug.DrawLine(origin, origin + dir * data.checkDistanceDown, Color.green);

		if (Physics.Raycast(origin, dir, out hit, data.checkDistanceDown, data.graundLayer))
			{
				var direction = (hit.point - mTransform.position).normalized;							
				listDirection.Add(direction);
			}				
		}
		
		int randomIndex = Random.Range(0, listDirection.Count);
		
		return (listDirection.Count > 1) ? listDirection[randomIndex] : Vector3.zero;
	}
	

	private bool OnGraund()
	{		
		var mPos = mTransform.position;
		var dir = -Vector3.up;
		var dist = data.checkDistanceDown;
		var layer = data.graundLayer;		
		var offset = dist / 2f;
		
		var origin = mPos + Vector3.up * offset;
		RaycastHit hit;
		Debug.DrawLine(origin, origin + dir * dist, Color.red);

		if (Physics.Raycast(origin, dir, out hit, dist, layer))
		{
			mTransform.position = hit.point;
			return true;
		}

		return false;
	}


	
	private void OnCollisionEnter(Collision other)
	{
		/* если касается воды, то удалить объект через 1сек. */
		if (other.gameObject.tag == StaticPrm.TAG_WATER)
			Destroy(gameObject, 1f);	

		/* если в воздухе и касаемся змейки, наносив ей урон */
		if (other.gameObject.tag == StaticPrm.TAG_SNAKE)
		{
			other.gameObject.GetComponent<Snake>().Damage();	
		}
		
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == StaticPrm.TAG_SNAKE_TONGUE)
		{				
			var goExp = Instantiate(data.explosionEffect, mTransform.position, Quaternion.identity);
			DestroyAudio();
			Destroy(goExp, 0.2f);
			Destroy(gameObject);
		}
	}


	void JumpAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 
														DataManager.Instance.Config.playSoundRadius)
			return;
			
		AudioManager.Instance.Play(StaticPrm.AUDIO_ENEMY_BLADE_MAN_ROTATE);
	}

	void DestroyAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 
														DataManager.Instance.Config.playSoundRadius)
			return;
			
		AudioManager.Instance.Play(StaticPrm.AUDIO_ENEMY_DESTROY);
	}


}
