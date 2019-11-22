using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : BaseItem 
{

    void Start () 
	{		
		Init();
	}	
	
	void Update () 
	{
		RotateItem();
	}
	
}
