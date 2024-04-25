using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;

public class UpgradeSystem : MonoBehaviour
{
    [Header("Employee Settings")]
    public Transform EmployeeArea;
    public Transform EmployeeArealocate;
    public GameObject[] employeePrefabs;
    public float employeePositionOffset;
    public List<GameObject> addEmployeeUseList = new List<GameObject>();
    public float employeeSpeedBoost;
    [Space(10)]
    [Header("Shelf Settings")]
    [SerializeField] List<ShelfScript> Shelves = new List<ShelfScript>();
    public float shelfVisibilityBoost;
    private SkeletonCardStackingUIScript skeletonCardScript;

    private void Start()
    {
        skeletonCardScript = GameObject.Find("Employee IDs").GetComponent<SkeletonCardStackingUIScript>();
        addEmployeeUseList.Add(FindObjectOfType<NormalEmployee>().gameObject);
    }

    private void Update()
    {
        AddShelfRangeInUpdate();
    }

    //upgrade functions: 1. employee load speed boost 2. add employee 3. add shelf attraction 4. add shelf load max 5.add shelf purchase power 6.reduce employee cost
    public void AddEmployee()
    {
        GameObject emp = Instantiate(employeePrefabs[0], Vector3.zero, Quaternion.identity);
        emp.GetComponent<NormalEmployee>().employeeArea = EmployeeArealocate;
        emp.transform.position = emp.GetComponent<NormalEmployee>().randomOffsetPos();
        emp.transform.SetParent(EmployeeArea.transform);
        
        addEmployeeUseList.Add(emp);
        skeletonCardScript.CreateNewCard(emp);
        ES3.Save("money", GameManager.instance.money);

        ES3.Save("Employees", GameManager.instance.employeeArea);

        ES3.Save("Shelves", GameManager.instance.shelfPlacementManager.shelfCollectionForReload);

        ES3.Save("Level", GameManager.instance.level);

    }

    public void BoostEmployee()
    {

        NormalEmployee[] Es = FindObjectsOfType<NormalEmployee>();
        foreach (var e in Es)
        {
            e.stayShelfDuration -= employeeSpeedBoost;
            e.stayShelfDuration = Mathf.Clamp(e.stayShelfDuration, 0.5f, 10f);
        }
        ES3.Save("money", GameManager.instance.money);

        ES3.Save("Employees", GameManager.instance.employeeArea);

        ES3.Save("Shelves", GameManager.instance.shelfPlacementManager.shelfCollectionForReload);

        ES3.Save("Level", GameManager.instance.level);
    }

    public GameObject ShelfPointerUI;

    void Awake()
    {
        DontDestroyOnLoad(ShelfPointerUI);
    }

    public void AddShelfRange()
    {
        GameManager.instance.upgradePanel.SetActive(false);
        Shelves.Clear();
        Shelves.AddRange(FindObjectsOfType<ShelfScript>().Where(shelf => shelf.visibility > 0));
        ShelfPointerUI.SetActive(true);

    }

    private void AddShelfRangeInUpdate()
    {
        if (ShelfPointerUI.activeSelf)
        {
            ShelfPointerUI.transform.position = GetMouseWorldPosition();
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = GetMouseWorldPosition();
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == 6)
                    {
                        Debug.Log("hit!");
                        ShelfScript shelf = hit.collider.GetComponent<ShelfScript>();
                        if (Shelves.Contains(shelf))
                        {
                            shelf.visibility += shelfVisibilityBoost;
                            ShelfPointerUI.SetActive(false);
                            ES3.Save("money", GameManager.instance.money);

                            ES3.Save("Employees", GameManager.instance.employeeArea);

                            ES3.Save("Shelves", GameManager.instance.shelfPlacementManager.shelfCollectionForReload);

                            ES3.Save("Level", GameManager.instance.level);
                            GameManager.instance.confirmUpgrade();
                        }
                    }
                }
            }
        }
    }

    public Vector2 GetMouseWorldPosition()
    {
        
        Vector3 mouseScreenPosition = Input.mousePosition;
        
        mouseScreenPosition.z = Camera.main.transform.position.z * -1;

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        return mouseWorldPosition;
    }
}
