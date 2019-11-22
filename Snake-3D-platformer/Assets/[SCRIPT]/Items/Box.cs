using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour 
{

	private GameObject boxDestroyPrefab;
	private GameObject baseModel;
	private ItemData data;
	private float timeDelete = 1f;
	private bool isDestroy;
	private GameObject conteiner;
	

	void Start () 
	{
		boxDestroyPrefab = Resources.Load<GameObject>("Item/Box_destroy");		
		baseModel = transform.GetChild(0).gameObject;
		isDestroy = false;
		gameObject.layer = 9; //boxCell
		data = Resources.Load<ItemData>("DataResources/ItemsData");	
		conteiner = GameObject.Find("Dynamic").gameObject;
		
	}
	

	void OnTriggerEnter(Collider other)
	{
		if (isDestroy)
			return;

		if (other.gameObject.name == StaticPrm.NAME_SNAKE_TONGUE)
		{			
			DestroyBox();			
		}

	}
	
	public void DestroyBox()
	{
		isDestroy = true;
		var newBoxDesroy = Instantiate(boxDestroyPrefab, transform.position, Quaternion.identity) as GameObject;
		newBoxDesroy.transform.parent = transform;	
		PlayDestroyAudio();//звук	
		CreateItem();
		Destroy(baseModel);
		Destroy(gameObject, timeDelete);
	}


	private void CreateItem()
	{
		var pos = transform.position + Vector3.up + Vector3.right * Random.Range(-1f, 1f);
		int num = Random.Range(0, data.Items.Length-1);
		var item = Instantiate(data.Items[num], pos, Quaternion.identity) as GameObject;
		item.transform.parent = conteiner.transform;
		
	}
	

	void PlayDestroyAudio()
	{
		if (Vector3.Distance(transform.position, GameManager.Instance.Snake.mTransform.position) > 10f)
			return;
			
		AudioManager.Instance.Play(StaticPrm.AUDIO_BOX_DESTROY); 
	}
}
