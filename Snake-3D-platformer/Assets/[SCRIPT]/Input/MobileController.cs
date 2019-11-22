using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileController : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

	private Image joystickBG;
	[SerializeField] private Image joystick;	
	

	public Vector2 InputVector {get; private set;}
	public Vector2 SwipeVector {get; private set;}
	public bool InputJump {get; private set;}
	public bool InputGrab {get; private set;}
	public bool InputRun {get; private set;}


	public void Init()
	{
		joystickBG = transform.GetChild(0).GetComponent<Image>();
		joystick = joystickBG.transform.GetChild(0).GetComponent<Image>();
		
		ResetButtonInput();	
	}    
	
#region Joystick void

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);		
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        InputVector = Vector2.zero;
		joystick.rectTransform.anchoredPosition = Vector2.zero; //возврат джойстика в центр при отпускании

		SwipeVector = Vector2.zero;		
    }


	public void OnDrag(PointerEventData eventData)
    {		
        Vector2 pos;

		/* если нажали на джойстик в зоне его действия */
		if (Vector2.Distance(eventData.pressPosition, joystickBG.rectTransform.anchoredPosition) < 500f
			&& RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBG.rectTransform, 
																	   eventData.position,
																	   eventData.pressEventCamera,
																	   out pos))
		{
			Debug.Log("Touch joystick");

			/* получения координат позиции касания на джойстике */
			pos.x = (pos.x / joystickBG.rectTransform.sizeDelta.x);
			pos.y = (pos.y / joystickBG.rectTransform.sizeDelta.x);

			/* установка точных координат из касания */
			InputVector = new Vector2(pos.x * 2f - 1f, pos.y * 2f - 1f);
			InputVector = (InputVector.magnitude > 0.1f) ? InputVector.normalized : InputVector;

			/* двигаем сам джойстик */
			joystick.rectTransform.anchoredPosition = new Vector2(InputVector.x * (joystickBG.rectTransform.sizeDelta.x / 2f),
																  InputVector.y * (joystickBG.rectTransform.sizeDelta.y / 2f));

		}
		
    }

	public void OnBeginDrag(PointerEventData eventData)
    {
		
        Vector2 delta = eventData.delta;
		float x = 0f;
		float y = 0f;

		if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
		{
			x = delta.x;
		}
		else
		{
			y = delta.y;
		}

		SwipeVector = new Vector2(x, y).normalized;
		//Debug.Log("Swipe vector: " + SwipeVector);
    }
	


#endregion

#region Buttons void

	public void OnJump()
	{
		InputJump = true;		
	}


	public void OffJump()
	{
		InputJump = false;		
	}


	public void OnGrab()
	{
		InputGrab = true;				
	}


	public void OffGrab()
	{
		InputGrab = false;	
	}


	public void OnRun()
	{
		InputRun = true;		
	}


	public void OffRun()
	{
		InputRun = false;		
	}
	
	
	private void ResetButtonInput()
	{
		InputJump = InputGrab = InputRun = false;	
	}

    

    #endregion

}
