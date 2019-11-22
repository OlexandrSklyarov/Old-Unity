using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalesController : MonoBehaviour 
{	
	[SerializeField] private GameObject scalepan; //чаша весов
	private bool isWeigher;
	private bool isDown;
	private bool isUp;
	private bool isPlaySound; //был ли проигран звук результата
	private DataManager dataManager;
	private Snake snake;
	private float maxY;
	private float minY;
	private float moveSpeed = 0.9f;
	private bool isInit;

	

	public event Action OnNormalAmountFood;
	public event Action<string, Color> OnScalesMessage;

	
	public void Init () 
	{
		var managerGame = GameManager.Instance;
		snake = managerGame.Snake;
		dataManager = DataManager.Instance;		
		scalepan = transform.Find("Scalepan").gameObject;
		isDown = false;
		isUp = true;
		isWeigher = false;
		maxY = scalepan.transform.localPosition.y;
		minY = maxY -0.4f;

		isInit = true;
		
	}
	
	
	void Update () 
	{
		if (!isInit)
			return;
			
		MovingScales();
	}

	void MovingScales()
	{
		var localPos = scalepan.transform.localPosition;

		if (isWeigher)
		{
			if(!isDown && localPos.y > minY)
			{
				localPos.y -= moveSpeed * Time.deltaTime;
				scalepan.transform.localPosition = localPos;
				isUp = false;
				MoveDownAudio();
			}							
			else if (localPos.y <= minY)
			{
				CheckedWeight();
				isDown = true;
			}							
		}
		else
		{
			if (!isUp && localPos.y < maxY)
			{
				localPos.y += moveSpeed * Time.deltaTime;
				scalepan.transform.localPosition = localPos;
				isDown = false;
			}
			else if (localPos.y >= maxY)
			{
				isUp = true;
			}
		}
	}


	/* проверка нужного веса (длины хвоста змейки) для открытия дверей */
	void CheckedWeight()
	{
		if (snake.TailLength >= dataManager.DataGame.minLengthSnakeTail)
		{
			/* оазсылаем сообщение, что вес в достатке */
			if (OnNormalAmountFood != null)
				OnNormalAmountFood();
			
			if (OnScalesMessage != null)
				OnScalesMessage("Portal is open", Color.blue);

			OpenPortalAudio(); //звук
			
		}
		else
		{
			if (OnScalesMessage != null)
				OnScalesMessage("Need great FOOD...", Color.red);

			NoOpenPortalAudio(); //звук
		}		
	}


	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == StaticPrm.TAG_SNAKE)
		{
			isWeigher = true;
		}		
	}


	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == StaticPrm.TAG_SNAKE)
		{
			isWeigher = false; // змейка не взвешивается
			isPlaySound = false; //отключаем флаг, что звук результата проигран
			MoveUpAudio();
		}
	}

#region AUDIO

	void MoveDownAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 
																dataManager.Config.playSoundRadius)
			return;

		AudioManager.Instance.Play(StaticPrm.AUDIO_SCALE_MOVE_DOWN); //play down
		AudioManager.Instance.Stop(StaticPrm.AUDIO_SCALE_MOVE_UP); // stop up
	}

	void MoveUpAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 
																dataManager.Config.playSoundRadius)
			return;

		AudioManager.Instance.Play(StaticPrm.AUDIO_SCALE_MOVE_UP); // play up
		AudioManager.Instance.Stop(StaticPrm.AUDIO_SCALE_MOVE_DOWN); // stop down
	}

	void OpenPortalAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 
																dataManager.Config.playSoundRadius)
			return;

		if (!isPlaySound)
		{
			AudioManager.Instance.Play(StaticPrm.AUDIO_SCALE_NORMAL_MASS);
			isPlaySound = true;
		}		
	}

	void NoOpenPortalAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 
																dataManager.Config.playSoundRadius)
			return;

		if (!isPlaySound)
		{
			AudioManager.Instance.Play(StaticPrm.AUDIO_SCALE_LOW_MASS);
			isPlaySound = true;
		}		
	}

#endregion


}
