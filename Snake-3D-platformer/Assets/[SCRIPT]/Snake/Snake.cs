using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Snake : MonoBehaviour 
{

#region DataClass

	[System.Serializable]
	public class MovingSettings
	{
		public Rigidbody rb;
		public CapsuleCollider capsCollider;
		public float currSpeed;		
		public bool isGraund;
		public bool isWater;
		public float delta;
		public float jumpForce;
		public bool isCanJump;
		public bool isDmage;
		public bool isAtack;
		public bool isJumping;
	}


	[System.Serializable]
	public class InputVar 
	{
		public float horizontal;
		public float vertical;
		public float moveAmount;
		public Vector3 moveDirection;
		public Vector3 flyDirection;
		public bool isJumpButton;	
		public bool isRunButton;			
		public bool isAttackButton;	
	}	


	[System.Serializable]
	public class Tail
	{
		public  Transform oroginTail;
		public List<TailPart> parts;
		public TailPart endTail;
		
	}

#endregion


#region  Events

	public event Action<float> PowerJumpEvent;
	public event Action<ItemType> AddItem;
	public event Action<ItemType> RemoveItem;

#endregion


#region Var
	

	public float JumpPower {get {return settings.jumpForce;}}
	public SnakeTongue SnakeTongue {get; private set;}//язык нашей змейки
	public int TailLength {get {return tail.parts.Count;}} //длина хвоста змейки
	private SnakeData data;
	public Transform mTransform;
	public InputVar inp;
	public GameObject FPCamera {get; set;}
	


	[SerializeField] private MovingSettings settings;		
	[SerializeField] private Tail tail;		
	[SerializeField] private GameObject dynamiContainer;		
	private Animator anim;
	private GameManager gameManager;
	private Color[] stableColors;
	private Color damageColor;
	private Renderer rendererMesh;
	
	private bool isInit;
	private bool isGameOver; //флаг - проиграл ли игрок

	

#endregion
	

#region Initialization

	public void Init(GameManager gameManager)
	{
		this.gameManager = gameManager;
		mTransform = transform;
        dynamiContainer = this.gameManager.DynamicConteiner; //контейнер для динамических объектов
							
		gameObject.tag = StaticPrm.TAG_SNAKE; //устанавливаем тех автоматичекси
		inp = new InputVar();
		data = DataManager.Instance.SnakeData;
			
		/* хвост */
		tail = new Tail();		
		FindTailsPart();

		/* находим язык змейки */
		FindSnakeTongue();			

		anim = GetComponent<Animator>();	
		gameObject.AddComponent<Rigidbody>();		
		settings = new MovingSettings();			
		settings.rb = GetComponent<Rigidbody>();
		settings.rb.constraints = RigidbodyConstraints.FreezeRotationX | 
						 		   RigidbodyConstraints.FreezeRotationY | 
						     	   RigidbodyConstraints.FreezeRotationZ; 
		settings.capsCollider = GetComponent<CapsuleCollider>();			

		/* подписываемся на события */
		EventSubscription();

		/* задаем цвет при уроне и нормальный */
		GetColorMaterial();

		/* находим камеру от первого лица на змейке */
		FPCamera = mTransform.Find("FPCamera").gameObject;
		FPCamera.SetActive(false); //отключаем

		isInit = true;
	}	


	/* подписка на события */
	void EventSubscription()
	{
		gameManager.SnakeInput.Event_UpdateTick += Tick;
		gameManager.SnakeInput.Event_UpdateFixedTick += FixedTick;
		gameManager.GameEnd += DisableMoving; //game over
	}


	/* получает текущие материалы на змейке */
	void GetColorMaterial()
	{
		damageColor =  new Color(1f, 0f, 0f, 0.001f);
		rendererMesh = mTransform.GetComponentInChildren<Renderer>();
		stableColors = new Color[rendererMesh.materials.Length];
		
		for (int i = 0; i < rendererMesh.materials.Length -1; i++)
		{
			stableColors[i] = rendererMesh.materials[i].color;			
		}		
	}	
	

#endregion


#region Update & FixedUpdate
	void Tick(float d)
	{
		if (isGameOver)
		{
			ResetMove();
			return;
		}

		if (!isInit)
			return;

		settings.delta = d;

		JumpControll();
		WaitInputAttack();
		Animations();
		TailMoveUpdate();
		MovingAudio(); //звук передвижения

	}


	void FixedTick(float d)
	{
		if (isGameOver)
		{
			ResetMove();
			return;
		}
			

		if (!isInit)
			return;

		settings.delta = d;
		settings.isGraund = OnGraund();

		switch(settings.isGraund)
		{
			case true:
				if (inp.isAttackButton) 
					return;

				MovingNormal();
				RotationNormal();
			break;
			case false:
				MovingInFly();
				MovingWater();
				RotationNormal();
			break;
		}
	}

#endregion
	

#region Move & Rotate graund

	/* сбрасываем свсе параметры ввода по умолчанию */
	void ResetMove()
	{
		inp.horizontal = 0f;
		inp.vertical = 0f;
		inp.moveAmount = 0f;
		inp.moveDirection = Vector3.zero;
		inp.flyDirection = Vector3.zero;
	}
	

	void Move(float speed)
	{
		Vector3 dir = Vector3.zero;

		/* если активирована камера от первого лица */
		if (FPCamera.activeSelf)
		{
			dir = mTransform.forward * (speed * inp.vertical);
		}
		else
		{
			dir = mTransform.forward * (speed * inp.moveAmount);	
		}		

		if (settings.isCanJump)
		{
			settings.rb.drag = 0f;
			inp.flyDirection = dir;	
			dir.y = settings.jumpForce;	
			inp.flyDirection = dir;	

			settings.jumpForce = 0f;
			settings.isCanJump = false;		
			JumpingAudio();					
		}

		JumpSliderUpdate();	
		settings.rb.velocity = dir;	
	}

	
	void MovingNormal()
	{		
		settings.rb.drag = (inp.moveAmount > 0.05f) ? 0f : 4f;			
		SpeedController(); 
		Move(settings.currSpeed);		
	}


	void RotationNormal()
	{		
		/* если активирована камера от первого лица */
		if (FPCamera.activeSelf)
		{
			RotationFPCamera();			
		}
		else //иначе делаем поворот по умолчанию
		{
			Vector3 targetDir = (inp.moveDirection);
			targetDir.y = 0f;

			if (targetDir == Vector3.zero)
				targetDir = mTransform.forward;

			Quaternion lookDir = Quaternion.LookRotation(targetDir);
			Quaternion targetRot = Quaternion.Slerp(mTransform.rotation, lookDir, data.rotationSpeed * settings.delta);
			mTransform.rotation = targetRot;
		}		
	}


	void RotationFPCamera()
	{
		var yRot = inp.horizontal * data.rotationSpeed;
		mTransform.Rotate(0f, yRot, 0f);
	}


	void MovingOnAir()
	{

	}


	/* плавно увеличивает, или уменьшает скорость */
	private void SpeedController()
	{
		if (inp.isRunButton)
		{
			if (settings.currSpeed < data.speedFastRun)
				settings.currSpeed += data.runAcceleration;
			else
				settings.currSpeed = data.speedFastRun;				
		}
		else
		{
			if (settings.currSpeed > data.speedRun)
				settings.currSpeed -= data.runAcceleration / 2f;
			else
				settings.currSpeed = data.speedRun;
		}
	}

#endregion


#region  WaterMove

	void MovingWater()
	{
		if (OnWater())
		{			
			
			settings.rb.drag = 10f;	

			float speed = data.speedRun / 2f;

			if (inp.isRunButton)
				speed = speed + data.speedFastRun / 3f;

			Move(speed);
		}			
	}

	
	bool OnWater()
	{		
		return CheckForLayer(data.waterLayer);
	}

#endregion


#region  FlyMoving

	void MovingInFly()
	{
		settings.rb.drag = 0f;	
	}

#endregion


#region Jump

	void JumpControll()
	{
		if (inp.isJumpButton && (settings.isGraund || OnWater()))
		{
			AddJumpForce();																				
		}
		else
		{
			if (settings.jumpForce > 0f && !settings.isCanJump)
			{
				settings.isCanJump = true; 					
			}				
		}
	}


	/* постепенно добавляет к силе прыжка силу пока не достигнет максимум */
	void AddJumpForce()
	{
		settings.jumpForce += data.jumpAcceleration;	

		if (settings.jumpForce >= data.hightJump)
		{
			settings.jumpForce  = data.hightJump;
			settings.isCanJump = true; 
			return;
		}		
			
	}


	void JumpSliderUpdate()
	{
		/* рассылаем сообщение */
		if (PowerJumpEvent != null)
		{
			var jForce = (1 / data.hightJump) * settings.jumpForce;
			PowerJumpEvent(jForce);
		}
	}

#endregion


#region Tail

	/* находит компоненты для хвоста в сцене */
	void FindTailsPart()
	{
		//создаем хвост
		var tailPref = Resources.Load("Snake/Prefabs/Tail_snake");
		var goTail = Instantiate(tailPref, mTransform.position - mTransform.forward * 2f, mTransform.rotation) as GameObject;
		goTail.transform.parent = dynamiContainer.transform;

		//заполняем переменные класса хвоста
		tail.parts = new List<TailPart>();
		tail.oroginTail = mTransform.GetChild(1).transform;
		tail.endTail = goTail.GetComponent<TailPart>();
		tail.endTail.Init();
		tail.endTail.SetOrigin(tail.oroginTail);
	}

	void TailMoveUpdate()
	{
		var speedTail = (inp.isRunButton) ? data.speedFastRun : data.speedRun; 

		if (tail.parts.Count > 0)
		{
			foreach(var part in tail.parts)
				part.Tick(settings.delta, speedTail);
		}

		tail.endTail.Tick(settings.delta, speedTail);
	}


	void AddPartTail()
	{
		int count = tail.parts.Count;

		/* ограничиваем максимальную длину хвоста */
		if (count >= data.maxLenthTail)
			return;

		Vector3 createPoint;		
		Transform tempOriginTail;

		if (count > 0)
		{
			createPoint = tail.parts[count-1].mTransform.position;
			tempOriginTail = tail.parts[count-1].mTransform;
		}
		else
		{
			createPoint = tail.oroginTail.position; 
			tempOriginTail = tail.oroginTail;
		}

		/* создаем новый кусок хвоста */
		var go = Resources.Load("Snake/Prefabs/Part") as GameObject;		
		var newTailPart = Instantiate(go, createPoint, Quaternion.identity).GetComponent<TailPart>();
		
		
		/* инициализируем переменные у нового кусочка,
		задаем ему origin */
		newTailPart.Init();
		newTailPart.SetOrigin(tempOriginTail);

		/* помещаем его в контейнер с 
		динамическими элементами игры */
		newTailPart.mTransform.parent = dynamiContainer.transform;
		newTailPart.mTransform.localPosition = createPoint;

		/* помещаем его в контейнер с 
		динамическими элементами игры */
		newTailPart.mTransform.parent = dynamiContainer.transform;
		newTailPart.mTransform.localPosition = createPoint;

		/* добавляем кусочек в список */
		tail.parts.Add(newTailPart);	

		/*меняем у наконечника хвоста origin,
		чтоб он следовал за последним элементом в списке частей хвоста */	
		tail.endTail.SetOrigin(newTailPart.mTransform);
	}


	void RemovePartTail()
	{
		if (tail.parts.Count > 0)
		{
			int lastPartID = tail.parts.Count - 1;
			TailPart deletePart = tail.parts[lastPartID];
			Transform tempOrigin;

			if (tail.parts.Count < 2)
			{
				tempOrigin = tail.oroginTail;				
			}
			else
			{
				tempOrigin = tail.parts[lastPartID-1].mTransform;				
			}
			
			/* перецепляем хвост на предпоследний элемент,
			и удаляем последний */
			tail.endTail.SetOrigin(tempOrigin);
			tail.parts.RemoveAt(lastPartID);
			Destroy(deletePart.gameObject);

		}
	}

#endregion


#region SnakeTongue

	/* находит язык */
	void FindSnakeTongue()
	{
		SnakeTongue = mTransform.Find("SnakeTongue").GetComponent<SnakeTongue>();
		SnakeTongue.OnFood += OnFood; //подписываемся на события
		SnakeTongue.gameObject.tag = StaticPrm.TAG_SNAKE_TONGUE;
		SnakeTongue.gameObject.SetActive(false);
	}

	/* ожидает команды для атаки и запускает по нажатию кнопки */
	void WaitInputAttack()
	{
		if (inp.isAttackButton && !settings.isAtack && !settings.isDmage)
		{
			SnakeAttack();		
			AttackAudio();	
		}
	}

	/* активирует язык */
	void SnakeAttack()
	{
		SnakeTongue.gameObject.SetActive(true);
		settings.isAtack = true;
		StartCoroutine(EndAttack());
	}

	/* завершает атаку */
	IEnumerator EndAttack()
	{		
		yield return new WaitForSeconds(0.8f);
		SnakeTongue.gameObject.SetActive(false);
		settings.isAtack = false;
	}	

#endregion


#region Other


	/* отключает возможность двигаться */
	private void DisableMoving()
	{
		isGameOver = true;
	}


	private void OnFood()
	{				
		if (AddItem != null) AddItem(ItemType.FOOD);
		AddPartTail();
		EatingAudio();
		
	}

	private void AddLive()
	{
		if (AddItem != null) AddItem(ItemType.LIVE);
		LiveAudio();
	}


	private void AddTime()
	{		
		if (AddItem != null) AddItem(ItemType.TIME);
		TimeAudio();
	}


	public void Damage()
	{
		//если сейчас змейка  атакована - возвращаемся
		if (settings.isDmage)
			return;

		/* если игрок проиграл - возвращаемся */
		if (isGameOver)
			return;

		if (tail.parts.Count > 0)
		{
			RemovePartTail();
		}
		else
		{
			if (RemoveItem != null) RemoveItem(ItemType.LIVE);
		}
		
		SetDamageColor();
		StartCoroutine(NoDamage());	
		DamageAudio();	
	}


	private IEnumerator NoDamage()
	{
		settings.isDmage = true;

		yield return new WaitForSeconds(data.timeDamage);

		settings.isDmage = false;
		SetNormalColor();
	}


	private void SetDamageColor()
	{
		for (int i = 0; i < rendererMesh.materials.Length -1; i++)
		{
			rendererMesh.materials[i].color = damageColor;	
		}		
	}

	private void SetNormalColor()
	{
		for (int i = 0; i < rendererMesh.materials.Length -1; i++)
		{
			rendererMesh.materials[i].color = stableColors[i];
		}
	}


#endregion


#region Collision

	bool OnGraund()
	{	
		return CheckForLayer(data.graundLayer);
	}


	/* проверяет есть ли данный слой под ногами */
	bool CheckForLayer(LayerMask layer)
	{
		List<Vector3> origins =  new List<Vector3>();
		var forward = mTransform.forward;
		var right = mTransform.right;
		var mPos = mTransform.position;
		float maxDist = data.graundDistance;

		/* числа, унажая на которые задаем нужную длину векторов */		
		var coffRight = 0.35f;
		var coffDist = 0.8f;

		/* centr ray */
		Vector3 origin_0 = mPos + (Vector3.up * maxDist * coffDist);
		Vector3 origin_0_right = mPos + right * coffRight + (Vector3.up * maxDist * coffDist);
		Vector3 origin_0_left = mPos + right * -coffRight + (Vector3.up * maxDist * coffDist);
		/* forward ray */
		Vector3 origin_forward = mPos + forward + (Vector3.up * maxDist * coffDist);
		Vector3 origin_forward_left = mPos + forward + right * -coffRight + (Vector3.up * maxDist * coffDist);
		Vector3 origin_forward_right = mPos + forward + right * coffRight + (Vector3.up * maxDist * coffDist);
		/* back ray */
		Vector3 origin_back = mPos + forward * -1f + (Vector3.up * maxDist * coffDist);
		Vector3 origin_back_left = mPos + forward * -1f + right * -coffRight + (Vector3.up * maxDist * coffDist);
		Vector3 origin_back_right = mPos + forward * -1f + right * coffRight + (Vector3.up * maxDist * coffDist);
		
		origins.Add(origin_0);
		origins.Add(origin_0_right);
		origins.Add(origin_0_left);
		origins.Add(origin_forward);
		origins.Add(origin_forward_right);
		origins.Add(origin_forward_left);
		origins.Add(origin_back);
		origins.Add(origin_back_right);
		origins.Add(origin_back_left);

		Vector3 dir = -Vector3.up;		
		RaycastHit hit;
		int countCollicionPoint = 0;
		
		foreach(Vector3 o in origins)
		{
			Debug.DrawLine(o, o + dir * maxDist, Color.green);
			if (Physics.Raycast(o, dir, out hit, maxDist, layer))
			{
				countCollicionPoint++;
				Vector3 tp = hit.point;

				if (countCollicionPoint > 3)
					mTransform.position = tp;

				return true;
			}	
		}	

		return false;
	}


	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == StaticPrm.TAG_SNAKE_LIVE)
		{
			Destroy(other.gameObject);
			AddLive();			
		}

		if (other.gameObject.tag == StaticPrm.TAG_CLOCK)
		{
			Destroy(other.gameObject);
			AddTime();			
		}		
	}


	/* проверка на столкновение с движущимися платформамми */
	private void OnCollisionStay(Collision other) 
	{
		/* если я на платформе */
		var movePlatform = other.gameObject.GetComponentInParent<PlatformController>();
		if (movePlatform)
		{
			mTransform.parent = movePlatform.transform;			
		}		
	}

	
	private void OnCollisionExit(Collision other) 
	{		
		/* если я спрыгнул с платформы */
		var movePlatform = other.gameObject.GetComponentInParent<PlatformController>();
		if (movePlatform)
		{
			mTransform.parent = dynamiContainer.transform;;			
		}		
	}

#endregion


#region Animation
	void Animations()
	{
		anim.SetBool(StaticPrm.animGraund, settings.isGraund);
		anim.SetBool(StaticPrm.animJump, inp.isJumpButton); //jump
		anim.SetBool(StaticPrm.animGrab, inp.isAttackButton); // grub
		anim.SetFloat(StaticPrm.animHor, inp.horizontal);		
		/* если зажата клавиша бега, то ускоряемся */
		var verticalValue = (inp.isRunButton) ? inp.moveAmount * 2f : inp.moveAmount;
		anim.SetFloat(StaticPrm.animVer, verticalValue);
	}

#endregion


#region Sounds	

	void AttackAudio()
	{
		AudioManager.Instance.Play(StaticPrm.AUDIO_SNAKE_ATTACK);
	}

	void MovingAudio()
	{
		var x_Vel = Mathf.Abs(settings.rb.velocity.x);
		var z_Vel = Mathf.Abs(settings.rb.velocity.z);

		/* передвижение по суше */
		if ((x_Vel > 0.1f || z_Vel > 0.1f) && OnGraund())
		{
			if (inp.isRunButton)
			{
				AudioManager.Instance.Play(StaticPrm.AUDIO_SNAKE_RUN);
				AudioManager.Instance.Stop(StaticPrm.AUDIO_SNAKE_WALK);
			}				
			else
			{
				AudioManager.Instance.Play(StaticPrm.AUDIO_SNAKE_WALK);
				AudioManager.Instance.Stop(StaticPrm.AUDIO_SNAKE_RUN);
			}				
		}
		else
		{
			AudioManager.Instance.Stop(StaticPrm.AUDIO_SNAKE_WALK);
			AudioManager.Instance.Stop(StaticPrm.AUDIO_SNAKE_RUN);			
		}

		/* передвижение по воде */
		if (!settings.isWater && OnWater())
		{
			settings.isWater = OnWater();
			AudioManager.Instance.Play(StaticPrm.AUDIO_SNAKE_ENTER_WATER);	
		}
		else
			settings.isWater = OnWater();


		if ((x_Vel > 0.1f || z_Vel > 0.1f) && OnWater())		
			AudioManager.Instance.Play(StaticPrm.AUDIO_SNAKE_MOVE_WATER);		
		else
			AudioManager.Instance.Stop(StaticPrm.AUDIO_SNAKE_MOVE_WATER);
		
			

	}

	void JumpingAudio()
	{
		AudioManager.Instance.Play(StaticPrm.AUDIO_SNAKE_JUMP);		
	}

	void DamageAudio()
	{
		AudioManager.Instance.Play(StaticPrm.AUDIO_SNAKE_DAMAGE);		
	}

	void EatingAudio()
	{
		AudioManager.Instance.Play(StaticPrm.AUDIO_SNAKE_EATING);	
	}

	void TimeAudio()
	{
		AudioManager.Instance.Play(StaticPrm.AUDIO_TIME_ADD);	
	}

	void LiveAudio()
	{
		AudioManager.Instance.Play(StaticPrm.AUDIO_TIME_ADD);	
	}


#endregion


}


public enum ItemType
{
	LIVE, TIME, FOOD
}