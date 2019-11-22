using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PKA
{
	public class GameController : MonoBehaviour 
	{	
		[SerializeField]
		Slider volumeSlider; //слайдер громкости звука в игре

		static AudioSource audio;

		[SerializeField]
		AudioClip soundWinner; //мелодия при выиграше

		[SerializeField]
		AudioClip clickSFX; //звук клика

		[SerializeField]
		AudioClip clickButton; //звук клика на кнопку

		[SerializeField]
		GameObject menu; //панель меню

		[SerializeField]
		GameObject winBaner;//банер с табличкой о выиграше

		[SerializeField]
		Text eventText; //текст счетчика кликов

		[SerializeField]
		GameObject pazzlePrefab; //префаб пазла

		[SerializeField]
		GameObject gameField; // игровое поле

		float width_field, height_field; //ширина и высота поля		

		int countPazzles= 16; //колличество пазлов

		 List<PazzleElement> pazzleArray; //мяссив пазлов

		List<Vector3> randomPosition; //список случайных позиций пазлов

		static int countEvent; //счетчик шагов

		bool isPlayWinSound;
		

		void Start () 
		{			
			audio = GetComponent<AudioSource>();	
			
			NewGame();		
		}

		
		void LateUpdate()
		{		
			InputKey();

			UpdateScore();

			//проверка, не собрали ли мы пазл
			if (IsWin()) BanerOn();
		}	


		//метод ввода клавиш пользователем
		void InputKey()
		{
			if (Input.GetButtonDown("Cancel"))
			{	
				//вкл/выкл меню		
				menu.SetActive((menu.activeSelf == false) ? true : false);
			}

			audio.volume = volumeSlider.value/100f; //изменяем громкость звуков в игре положением слайдера
		}


		//метод подготовливает игру
		public void NewGame()
		{		
			
			winBaner.SetActive(false); //выключаем банер
			menu.SetActive(false); //выключаем панель меню
			ClearField(); //очищаем игровое поле
			CreatePazzle(); //создаем ячейки и помещаем их в игровое поле	
			SetRandomPosition(); //задаём случайную позицию пазлам
					
			eventText.text =   "Events: ";	    
			
		}


		//Метод выхода из игры
		public void ExitGame()
		{
			Application.Quit();
		}

		//метод проигрывает звук нажатия на кнопку
		public void PlayButtonSFX()
		{
			audio.PlayOneShot(clickButton);
		}
	

		//метод прибавляет к счетчику клик
		void AddEventCount()
		{
			//увеличиваем счетчик шагов
			countEvent++;

			//проиграть звук перемещения пазла			
			audio.PlayOneShot(clickSFX);
			
		}


		//метод очищает поле от старых объектов, если они есть на поле
		void ClearField()
		{
			if (gameField.transform.childCount > 0)
			{
				// удаление старых объектов, если они есть
				for(int i = 0; i < gameField.transform.childCount; i++)
				{
					Destroy(gameField.transform.GetChild(i).gameObject);
				}
			}

			//очищаем списки, если они не пустые
			if (randomPosition != null && randomPosition.Count > 0)randomPosition.Clear();
			if (pazzleArray != null && pazzleArray.Count > 0) pazzleArray.Clear();

			countEvent = 0;	
		}


		void UpdateScore()
		{			
			eventText.text = "Events: " + countEvent.ToString();			
		}

		//метод создания ячеек
		void CreatePazzle()
		{	
			//создаем список для элементов пазла
			pazzleArray =  new List<PazzleElement>();

			//создаем пустой список случайных позиций
			randomPosition = new List<Vector3>();			

			//узнаем ширину и высоту поля
			RectTransform gf_Transform = gameField.GetComponent<RectTransform>();
			width_field = gf_Transform.rect.width;
			height_field = gf_Transform.rect.height;

			//задаем начальную точку отчета координат на поле
			Vector3 start = new Vector3(-(width_field * 0.75f / 2), (height_field * 0.75f / 2), 0f);

			//временный объект для создания пазлов
			GameObject tempPazzle; 

			//счетчик номера пазла
			int num = 1;

			//создаем и размещаем на поле
			for (int i = 0; i < 4; i++)
			{		
				for (int j = 0; j < 4; j++)	
				{
					Vector3 createCor = new Vector3(start.x + j * (width_field / 4), 
													start.y - i * (height_field / 4), 
													0f);
					

					tempPazzle = (GameObject)Instantiate(pazzlePrefab, createCor, Quaternion.identity);				
					
					

					if (num != countPazzles)
					{
						tempPazzle.name = "Pazzle_"+ (num);	
						tempPazzle.GetComponent<RawImage>().texture = Resources.Load<Texture>("Image/numbers/" + (num));						
					}
					else
					{
						tempPazzle.name = "FreePazzle";	
						tempPazzle.GetComponent<RawImage>().enabled = false;
					}

					//назначаем родителем всех пазлов - gameField
					tempPazzle.transform.SetParent(gameField.transform, false);

					//записываем пазли в список
					pazzleArray.Add(tempPazzle.GetComponent<PazzleElement>() as PazzleElement);
					
					//вызываем метод у каждого пазла и передаем туда номер позиции
					pazzleArray[num-1].Init(num);										
											
					randomPosition.Add(pazzleArray[num-1].GetStartPosition()); //записываем эту позицию в список

					//увиличиваем счетчик
					num++;
					
				}				
			}		
		}


		//метод задает случайные позиции пазлам на поле
		void SetRandomPosition()
		{			
			do 
			{
				Shuffle(); //пермешиваем список позиций

				int index = 0;

				foreach(var el in pazzleArray)
				{
					//задаем текущему элементу одну из позиций в перемешаном списке позицый и текущий номер на поле
					el.SetPosition(randomPosition[index], index);
					index++;
				}
			}
			while(!canBeOrdered(pazzleArray));		
						
		}


		//метод проверяет на правильность перетасовки пазлов
		//можно ли собрать данную комбинацию
		//алгоритм взят с http://rsdn.org/forum/alg/169876.all
		bool canBeOrdered(List<PazzleElement> pazzleArr)
		{ 
			bool even = true;

			for(int i = 0 ; i<4; i++)
			{
				for(int j = i+1; j<4; j++)
				{
					if(pazzleArr[i].CurNumber > pazzleArr[j].CurNumber && pazzleArr[j].CurNumber != 16)
					{
						even=!even;
					}						
				}
			}			
			
			return even;
		}



		//метод перемешивания позиций
		void Shuffle()
		{				
			for (int i = randomPosition.Count-1 ; i >= 1; i--)
			{
				int j = Random.Range(0, i);
				// обменять значения data[j] и data[i]
				var temp = randomPosition[j];
				randomPosition[j] = randomPosition[i];
				randomPosition[i] = temp;
			}
		}


		//метод нажатия на пазл
		public void OnClick(PazzleElement el)
		{
			if (el.ID == 16) return;
			
			RaycastHit2D [] left =  Physics2D.RaycastAll(el.transform.position, -el.transform.right, 2f);
			RaycastHit2D [] right =  Physics2D.RaycastAll(el.transform.position, el.transform.right, 2f);
			RaycastHit2D [] up =  Physics2D.RaycastAll(el.transform.position, el.transform.up, 2f);
			RaycastHit2D [] down =  Physics2D.RaycastAll(el.transform.position, -el.transform.up, 2f);
			
			//проверяем, нет ли рядом пустой ячейки, если есть, то меняемся с ней местами
			if (left.Length > 0) CheckForEmptyPuzzle(left, el);						
			if (right.Length > 0) CheckForEmptyPuzzle(right, el);			
			if (up.Length > 0) CheckForEmptyPuzzle(up, el);
			if (down.Length > 0) CheckForEmptyPuzzle(down, el);
		}


		//метод проверяет рядом с нажатым пазлом свободной ячейки
		void CheckForEmptyPuzzle(RaycastHit2D [] hit2D, PazzleElement checkPazzle)
		{				
			//делаем проверку, и исключаем себя из найденых объектов
			foreach(var hitElement in hit2D)
			{
				PazzleElement tempPazzle = hitElement.collider.GetComponent<PazzleElement>();

				//если попалась пустой пазл, то меняемся с ним местами
				if (tempPazzle != this && tempPazzle.ID == 16) 
				{
					ChangePazzle(tempPazzle, checkPazzle);					
					return;					

				}
			}
		}


		//метод смены ячеек местами
		void ChangePazzle(PazzleElement tempPazzle, PazzleElement checkPazzle)
		{					
			//записываем позицию и ID найденного пазла во временные переменные
			Vector3 tempPos = tempPazzle.transform.position; 
			int tempID = tempPazzle.ID;
			//переставляем пазлам позиции и ID друг друга
			tempPazzle.SetPosition(checkPazzle.transform.position, checkPazzle.ID);
			checkPazzle.SetPosition(tempPos, tempID);
					
			 AddEventCount();						
		}
	

		//проверка на выиграш
		bool IsWin()
		{
			//если хоть один элемент не на своей позиции, то возвращаем false
			foreach(var el in pazzleArray)
			{
				if (!el.PazzleInPosition())
					return false;
			}

			return true;			
		}


		void BanerOn()
		{
			//включаем банер
			winBaner.SetActive(true);

			if (!isPlayWinSound)
			{
				//включаем мелодию победы
				audio.PlayOneShot(soundWinner);
				isPlayWinSound = true; //отмечаем что мелодия выиграша уже запущена
			}
			
			
		}
	}
}