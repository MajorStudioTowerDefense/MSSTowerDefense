using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeTab : MonoBehaviour
{
    public bool MoveOut = false;
    public bool MoveIn = false;
    public GameObject employeeTab;
    public Vector2 ogPosition, newPosition, currentPosition;

    private void Start()
    {
        ogPosition = employeeTab.GetComponent<RectTransform>().position;
        newPosition = new Vector2(ogPosition.x - 250, ogPosition.y);
    }

     private void Update()
     {
        currentPosition = employeeTab.GetComponent<RectTransform>().position;
        MoveTab();
     }

    public void MoveTab()
    {
       if (MoveOut == true)
       {
            employeeTab.GetComponent<RectTransform>().position += new Vector3(150f * Time.deltaTime, 0, 0);
           if (employeeTab.GetComponent<RectTransform>().position.x >= ogPosition.x)
           {
                MoveOut = false;
           }
       }
       else if (MoveIn == true)
       {
            employeeTab.GetComponent<RectTransform>().position -= new Vector3(150f * Time.deltaTime, 0, 0);
            if (employeeTab.GetComponent<RectTransform>().position.x <= newPosition.x)
            {
                MoveIn = false;
            }
        }
    }

    public void ButtonClick()
    {
        if (currentPosition.x >= ogPosition.x)
        {
            MoveIn = true;
        }
        else if (currentPosition.x <= newPosition.x)
        {
            MoveOut = true;

        }
    }
}
