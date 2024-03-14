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

    private void Start()
    {
        addEmployeeUseList.Add(FindObjectOfType<NormalEmployee>().gameObject);
    }
    public void AddEmployee()
    {
        Vector3 nextPos = EmployeeArea.position + new Vector3(addEmployeeUseList.Count * employeePositionOffset, 0, 0);
        GameObject employeeAreaNext = new GameObject("EmployeeArea(Clone)");
        employeeAreaNext.transform.position = nextPos;
        GameObject emp = Instantiate(employeePrefabs[0], employeeAreaNext.transform.position, Quaternion.identity);
        emp.transform.SetParent(employeeAreaNext.transform);
        emp.GetComponent<NormalEmployee>().employeeArea = employeeAreaNext.transform;
        addEmployeeUseList.Add(emp);
    }

    public void BoostEmployee()
    {

        NormalEmployee[] Es = FindObjectsOfType<NormalEmployee>();
        foreach (var e in Es)
        {
            e.aiPath.maxSpeed += employeeSpeedBoost;
        }
    }
}
