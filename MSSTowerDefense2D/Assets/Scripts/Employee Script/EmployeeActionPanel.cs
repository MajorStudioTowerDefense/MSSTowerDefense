using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeActionPanel : MonoBehaviour
{
    public Transform target; // targetObject
    public Vector3 offset; // UI offset

    void Update()
    {
        if (target != null)
        {
            
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(target.position + offset);
            
            transform.position = screenPosition;
        }
    }
}
