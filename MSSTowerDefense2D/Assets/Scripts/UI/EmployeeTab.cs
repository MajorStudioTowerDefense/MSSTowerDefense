using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeTab : MonoBehaviour
{
    public bool MoveOut = false;
    public bool MoveIn = false;
    public RectTransform rectTransform;
    public Vector2 ogPosition, newPosition, currentPosition;

    private void Start()
    {
        ogPosition = rectTransform.GetComponent<RectTransform>().position;
        newPosition = new Vector2(ogPosition.x - 100, ogPosition.y);
    }

     private void Update()
     {
        currentPosition = rectTransform.position;
     }

    public void MoveTab()
    {
       if (MoveOut == true)
       {
           rectTransform.position += new Vector3(0.2f*Time.deltaTime, 0, 0);
           if (rectTransform.position.x > ogPosition.x)
           {
                MoveOut = false;
                Debug.Log("hi");
           }
       }
       else if (MoveIn == true)
       {
            rectTransform.position -= new Vector3(0.2f * Time.deltaTime, 0, 0);
            if (rectTransform.position.x < newPosition.x)
            {
                MoveIn = false;
                Debug.Log("die");

            }
        }
    }

    public void ButtonClick()
    {
        Debug.Log("enter");

        if (currentPosition.x >= ogPosition.x)
        {
            MoveIn = true;
            Debug.Log("didthing1");
        }
        else if (currentPosition.x <= newPosition.x)
        {
            MoveOut = true;
            Debug.Log("didthing2");

        }
    }
}
