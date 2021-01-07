using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class JoystickHolde : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image hitBox;
    public OnScreenStick Joystick;
    public void OnDrag(PointerEventData eventData)
    {
        Joystick.OnDrag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
        Vector2 locPos;
        RectTransform r_trans = Joystick.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(hitBox.rectTransform, eventData.position, eventData.pressEventCamera, out locPos))
        {
            Joystick.gameObject.SetActive(true);
            r_trans.localPosition = locPos;
            Joystick.OnPointerDown(eventData);
        }
     }

    public void OnPointerUp(PointerEventData eventData)
    {
        Joystick.gameObject.SetActive(false);
        Joystick.OnPointerUp(eventData);
    }

   
}
