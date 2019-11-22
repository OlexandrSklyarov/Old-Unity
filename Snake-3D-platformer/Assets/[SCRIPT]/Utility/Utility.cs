using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility //: MonoBehaviour 
{

	//расчитывает угол вылета снаряда
	public static float AngleBalistic(float distance, float speedBullet)
    {
        //Находим велечину гравитации
        float gravity=Physics.gravity.magnitude;
       
        float discr = Mathf.Pow(speedBullet,4f) - 4f*(- gravity*gravity/4)*(-distance*distance);

        //Время полёта
        float t = ((-speedBullet*speedBullet)-Mathf.Sqrt(discr))/(-gravity*gravity/2f);
        t = Mathf.Sqrt(t);                     
        float th = gravity*t*t/8f;

        //Угол пушки
        float angle = 180f * ( Mathf.Atan(4f*th/distance) / Mathf.PI);
		
        //Возрощаем угол
        return(angle);
    }


}
