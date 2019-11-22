using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeInput : MonoBehaviour 
{

#region Events

	public event Action<float> Event_UpdateTick;
	public event Action<float> Event_UpdateFixedTick;

#endregion

	
#region Var

	public Snake Snake {get; private set;}
	public float horizontal;
	public float vertical;
	public bool sprintInput;
	public bool jumpInput;
	public bool grabInput;
	public float delta;	
	

	public PhaseGame phase;	
	private GameManager gameManager;
	[SerializeField] private MobileController mobControll;
	[SerializeField] private bool isMobileInput;
	private bool isInit;

#endregion
	


	public void Init()
	{		
		gameManager = GameManager.Instance;	
			
		phase = gameManager.Phase;	

		Snake = gameManager.Snake;		
		Snake.Init(gameManager);		
		
		mobControll = gameManager.MobControll;	
		mobControll.Init();

		isInit = true;
	}
	
	
	public void Tick(float d)
	{
		if (!isInit)
			return;

		delta = d;

		switch(phase)
		{
			case PhaseGame.IN_GAME:
				PhaseInGameUpdate();
			break;
			case PhaseGame.IN_MENU:
				//логика в меню игры
			break;
		}		
	}


	public void FixedTick(float d) 
	{
		if (!isInit)
			return;

		delta = d;

		switch(phase)
		{
			case PhaseGame.IN_GAME:
				PhaseInGameFixedUpdate();
			break;
			case PhaseGame.IN_MENU:
				//логика в меню игры
			break;
		}		
	}


#region  LogicInGame

	void PhaseInGameUpdate()
	{		
		GetInput_Update();
		InGame_UpdateInput_Update();
		
		if (Event_UpdateTick != null)
			Event_UpdateTick(delta);
		//Snake.Tick(delta);		
	}



	void PhaseInGameFixedUpdate()
	{	
		GetInput_FixedUpdate();
		InGame_UpdateInput_FixedUpdate();
		
		if (Event_UpdateFixedTick != null)
			Event_UpdateFixedTick(delta);

		//Snake.FixedTick(delta);
		//CameraHandlerSnake.FixedTick(delta);
	}

#endregion


#region Input
	void GetInput_Update()
	{		
		/* захват добычи */
		grabInput = (isMobileInput)? mobControll.InputGrab : Input.GetButton(StaticPrm.Grab);
		/* бег */
		sprintInput = (isMobileInput)? mobControll.InputRun : Input.GetButton(StaticPrm.Run);
		/* прижок */
		jumpInput =  (isMobileInput)? mobControll.InputJump : Input.GetButtonDown(StaticPrm.Jump);		
	}


	void GetInput_FixedUpdate()
	{				
		vertical = (mobControll.InputVector.y != 0f) ? mobControll.InputVector.y : Input.GetAxis(StaticPrm.Vertical);		
		horizontal = (mobControll.InputVector.x != 0f) ? mobControll.InputVector.x : Input.GetAxis(StaticPrm.Horizontal);
	}

#endregion

#region CalculateInput
	void InGame_UpdateInput_Update()
	{
		Snake.inp.isAttackButton = grabInput;	
		Snake.inp.isRunButton = sprintInput;

		/* если не нажат прыжок */
		Snake.inp.isJumpButton = jumpInput;			
	}


	void InGame_UpdateInput_FixedUpdate()
	{		
		Snake.inp.horizontal = horizontal;
		Snake.inp.vertical = vertical;
		Snake.inp.moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
		
		Vector3 moveDir = Vector3.zero;

		/* если основная камера активна (от третьего лица) */
		if (gameManager.CamController.gameObject.activeSelf)
		{
			moveDir = gameManager.CamController.mTransform.forward * vertical;
			moveDir += gameManager.CamController.mTransform.right * horizontal;	
		}
		else
		{
			moveDir = Snake.mTransform.forward * vertical;
			//moveDir += Snake.mTransform.right * horizontal;
		}
		

		moveDir.Normalize();
		Snake.inp.moveDirection = moveDir;
	}

#endregion
	
}
