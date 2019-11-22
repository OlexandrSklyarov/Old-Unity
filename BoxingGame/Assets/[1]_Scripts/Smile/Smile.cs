using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smile : MonoBehaviour 
{

#region Var
	
	public bool IsStartPosition {get; private set;}

	EnemyController controller;
	SmileData data;
	SpriteRenderer spriteRenderer;
	int currentHealth;
	float startPosY;
	float endPosY;
	bool isDamage;

#endregion


#region Init

	public void Init (EnemyController _contriller) 
	{
		controller = _contriller;
		data = Resources.Load<SmileData>("Data/SmileData");
		spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();	

		startPosY = transform.localPosition.y;	
		endPosY = startPosY + data.maxForwardDistance;

		SetValuesDefault();

		IsStartPosition = true;
		
	}


	void SetValuesDefault()
	{
		currentHealth = data.sprites.Length-1; 
		spriteRenderer.sprite = data.sprites[currentHealth];
	}
	
#endregion
	

#region Update

	public void MoveForward()
	{
		StartCoroutine(Forward());
	}


	public void MoveBack()
	{		
		StartCoroutine(Back());		
	}

	
	//движение смайла вперед
	IEnumerator Forward()
	{	
		IsStartPosition = false;

		while (transform.localPosition.y < endPosY)
		{			
			var speed = Random.Range(data.minMoveSpeed, data.maxMoveSpeed);
			var dir = new Vector3 (0f, speed, 0f);
			transform.Translate(dir, Space.Self);			

			yield return null;
		}		

		SetPosEnd(endPosY);		
		yield return new WaitForSeconds( Random.Range(data.timeDeleyMin, data.timeDeleyMax));
		StartCoroutine(Back());		
	}


	//движение смайла назад
	IEnumerator Back()
	{
		while (transform.localPosition.y > startPosY)
		{			
			var speed = Random.Range(data.minMoveSpeed, data.maxMoveSpeed);
			var dir = new Vector3 (0f, -speed, 0f);
			transform.Translate(dir, Space.Self);			
			
			yield return null;
		}

		SetPosEnd(startPosY);	

		IsStartPosition = true;	
		if (isDamage) isDamage = false;

		//устанавливаем по стандарту жихни и спрайт смайла
		if (currentHealth <= 0)
			SetValuesDefault();
				
	}


	//устанавливает позицию по [y] в конце движения
	void SetPosEnd(float yPos)
	{
		var locPos = transform.localPosition;
		locPos.y = yPos;
		transform.localPosition = locPos;
	}


	public void Damage()
	{
		if (!isDamage)
		{
			isDamage = true;			

			if (currentHealth > 0)
			{				
				currentHealth--;			
				spriteRenderer.sprite = data.sprites[currentHealth];

				if (currentHealth <= 0)
					GameManager.Instance.SetScore();				
			}			

			//StopAllCoroutines();
			//StartCoroutine(Back());
		}
			
	}


	// устанавливает все значения по умолчанию, 
	// и возвращает смайл на стартовую позицию
	public void Restart()
	{
		StopAllCoroutines();
		SetPosEnd(startPosY);
		SetValuesDefault();
	}

#endregion



}
