using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject hoverPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(this.transform.parent.parent.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverPanel.SetActive(true);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverPanel.SetActive(false);
    }

    private void Start()
    {
        hoverPanel.SetActive(false);

    }
}

