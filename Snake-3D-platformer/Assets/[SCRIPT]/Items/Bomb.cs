using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour 
{

	private float explosionPower = 1500f;
	private float explosionRadius = 5f;

	[SerializeField]
	private GameObject explosionEffect;


	private void Start()
	{
		StartCoroutine(Explosion());
	}
	
	

	/* доработать этот метод!!! */
	IEnumerator Explosion()
	{
		yield return new WaitForSeconds(DataManager.Instance.Config.timeExplosionBomb);

		var r = DataManager.Instance.Config.bombExplosionRadius; 

		Collider[] allParts = Physics.OverlapSphere(transform.position, r);

		VisualEffect();

		AudioManager.Instance.Play(StaticPrm.AUDIO_EXPLOSION_BOMB);

		for (int i = 0; i < allParts.Length; i++)
		{
			Rigidbody targetRigidbody = allParts[i].GetComponent<Rigidbody>();

			if (!targetRigidbody)
				continue;
			
			var dist = Vector3.Distance(allParts[i].transform.position, transform.position);
			var power = explosionPower / ((dist > 2f) ? dist : 2f);

			targetRigidbody.AddExplosionForce(power, transform.position, explosionRadius);
			
			if (allParts[i].tag == StaticPrm.TAG_SNAKE)
			{
				allParts[i].GetComponent<Snake>().Damage();				
			}
				
		}

		Destroy(gameObject);
	}


	void VisualEffect()
	{
		var effect = Instantiate(explosionEffect, transform.position, Quaternion.identity) as GameObject;
		Destroy(effect, 1f);
	}
}
