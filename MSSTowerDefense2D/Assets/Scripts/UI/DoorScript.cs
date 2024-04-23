using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoorScript : MonoBehaviour, IPointerEnterHandler
{
    public RectTransform rectTransform;
    float startScaleX;
    Vector2 startScale;
    Vector2 endScale;
    bool inside = false;


    private void Start()
    {
        startScaleX = rectTransform.localScale.x;
        endScale = new Vector2(0.05f, startScale.y);
        startScale = rectTransform.localScale;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inside = true;
        StartCoroutine(closingTime());
    }

    IEnumerator closingTime()
    {
        yield return new WaitForSeconds(1.5f);
        inside = false;
    }

    private void Update()
    {

        if (inside == true)
        {
            if (rectTransform.localScale.x <= 0.1f)
            {
                rectTransform.localScale = endScale;
            }
            else
            {
                rectTransform.localScale = new Vector2(rectTransform.localScale.x - 1.1f*Time.deltaTime, startScale.y);
            }
        }
        else
        {
            if (rectTransform.localScale.x >= startScaleX)
            {
                rectTransform.localScale = startScale;
            }
            else
            {
                rectTransform.localScale = new Vector2(rectTransform.localScale.x + 0.2f * Time.deltaTime, startScale.y);

            }
        }
    }
}
