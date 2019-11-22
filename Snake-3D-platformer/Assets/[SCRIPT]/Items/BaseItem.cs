using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour 
{

	protected Rigidbody rb;

	protected void Init()
	{
		rb = GetComponent<Rigidbody>();
		AddingUpForce();
		StartCoroutine(DestroyItem());
	}

	protected void RotateItem()
	{
		transform.Rotate(new Vector3(0f, 80f * Time.deltaTime, 0f));
	}


	protected IEnumerator DestroyItem()
	{
		yield return new WaitForSeconds(DataManager.Instance.Config.timeDestroyItem);
		Destroy(gameObject);
	}


	protected void AddingUpForce()
	{
		rb.velocity = new Vector3(1f, 5f, 2f);
	}
}
