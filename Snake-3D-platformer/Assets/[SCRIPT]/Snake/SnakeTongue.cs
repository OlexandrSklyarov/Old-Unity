using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeTongue : MonoBehaviour 
{

	public event Action OnFood;


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == StaticPrm.TAG_FOOD)
		{
			Destroy(other.gameObject);
			SendMessageOnFood();
		}
	}


	void SendMessageOnFood()
	{
		if (OnFood != null)
			OnFood();
	}
	
}
