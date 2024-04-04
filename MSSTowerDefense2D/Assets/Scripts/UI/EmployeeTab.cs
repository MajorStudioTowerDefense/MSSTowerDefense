using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeTab : MonoBehaviour
{
    public bool MoveOut = false;
    public bool MoveIn = false;
    public RectTransform rectTransform;
    public Vector2 ogPosition;
    public Vector2 newPosition;
    public Vector2 movePosition;
    public Vector2 currentPosition;

    private void Start()
    {
        ogPosition = GetComponent<RectTransform>().anchoredPosition;
        movePosition = new Vector2(0.2f, 0);
        newPosition = new Vector2(ogPosition.x - 100, ogPosition.y);

    }

     private void Update()
     {
        currentPosition = rectTransform.anchoredPosition;
    }

    public void MoveTab()
    {
       if (MoveOut == true)
       {
           rectTransform.anchoredPosition += movePosition;
           if (rectTransform.anchoredPosition.x < ogPosition.x)
           {
                MoveOut = false;
           }
       }
       else if (MoveIn == true)
       {
            rectTransform.anchoredPosition -= movePosition;
            if (rectTransform.anchoredPosition.x < newPosition.x)
            {
                MoveIn = false;
            }
        }
    }

    public void ButtonClick()
    {
        if (rectTransform.anchoredPosition.x > ogPosition.x)
        {
            MoveIn = true;
        }
        else if (rectTransform.anchoredPosition.x < newPosition.x)
        {
            MoveOut = true;
        }
    }
}
