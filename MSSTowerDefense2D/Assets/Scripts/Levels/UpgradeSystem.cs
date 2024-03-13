using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Employee Settings")]
    public Transform EmployeeArea;
    public GameObject[] employeePrefabs;
    public float employeePositionOffset;
    private List<GameObject> addEmployeeUseList = new List<GameObject>();
    public float employeeSpeedBoost;
    [Space(10)]
    [Header("Shelf Settings")]
    public List<ShelfScript> Shelves = new List<ShelfScript>();

    public void AddEmployee()
    {
        Vector3 nextPos = EmployeeArea.position + new Vector3(addEmployeeUseList.Count * employeePositionOffset, 0, 0);
        GameObject emp = Instantiate(employeePrefabs[0], nextPos, Quaternion.identity);
        emp.transform.SetParent(EmployeeArea);
        addEmployeeUseList.Add(emp);
    }

    public void BoostEmployee()
    {
        
        FindObjectOfType<NormalEmployee>().aiPath.maxSpeed += employeeSpeedBoost;
    }
}
