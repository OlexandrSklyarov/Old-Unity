using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKA
{
	public class PazzleElement : MonoBehaviour 
	{
		 Vector3 m_startPosition;//позиция при создании
		public int ID {get; private set;} //идентификатор позиции
		public int CurNumber {get; private set;} //текущи номер ячейки

		
		public void Init(int _id)
		{			
			m_startPosition = new Vector3(transform.position.x, transform.position.y, 0f);
			ID = _id;
			CurNumber = _id;
		}


		//метод задает новую позицию пазлу
		public void SetPosition(Vector3 pos, int newNumber)
		{
			pos.z = 0f;
			transform.position = pos;
			CurNumber = newNumber;
		}


		public Vector3 GetStartPosition()
		{
			return m_startPosition;
		}


		//true, если пазл на своем месте
		public bool PazzleInPosition()
		{
			return m_startPosition == new Vector3(transform.position.x, transform.position.y, 0f);
		}



	}
}