using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonPop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector2(transform.localScale.x * 1.1f, transform.localScale.y * 1.1f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector2(transform.localScale.x / 1.1f, transform.localScale.y / 1.1f);

    }
}
