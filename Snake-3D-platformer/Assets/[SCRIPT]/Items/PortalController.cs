using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour 
{
	private const float TIME_OPEN_PORTAL = 20f; 
	private ParticleSystem particleOpen;
	private ParticleSystem particleClose;
	private GameObject portalTrigger;	
	private ScalesController scales;
	private GameManager gameManager;


	public event Action OnSnakePortal;

	
	public void Init () 
	{
		gameManager = GameManager.Instance;

		particleOpen = transform.Find("ParticleSystemOpen").GetComponent<ParticleSystem>();
		particleClose = transform.Find("ParticleSystemClose").GetComponent<ParticleSystem>();
		portalTrigger = transform.Find("PortalTrigger").gameObject;

		scales = gameManager.Scales;
		scales.OnNormalAmountFood += OpenPortal; //подписываемся на событие срабытывание весов

		ClosePortal();
	}

	
	public void OpenPortal()
	{		
		portalTrigger.SetActive(true);
		particleOpen.gameObject.SetActive(true);
		particleClose.gameObject.SetActive(false);

		StartCoroutine(TimeClosePortal());
	}

	public void ClosePortal()
	{
		portalTrigger.SetActive(false);
		particleOpen.gameObject.SetActive(false);
		particleClose.gameObject.SetActive(true);
	}


	IEnumerator TimeClosePortal()
	{
		yield return new WaitForSeconds(TIME_OPEN_PORTAL);
		ClosePortal();
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == StaticPrm.TAG_SNAKE)
		{
			//сообщем подписчикам, что змейка вошла в портал
			if (OnSnakePortal != null)
				OnSnakePortal();
		}
	}
	
	
}
