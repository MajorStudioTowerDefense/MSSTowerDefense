using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalEmployee : Bot
{
    //the shelf being selected
    public ShelfScript NeededShelf;
    public bool isCarrying = false;
    public int carryMax = 3;
    public int carryCount = 0;
    protected bool startShelfSelect;

    
    //the employee being selected
    public bool employeeSelected = false;
    public ShelfPlacementManager shelfPlacementManager;

    /////////////////////////////////
    //UI when the employee is selected
    public GameObject employeeUICanvas;
    public GameObject employeeActionPanelPrefab;

    protected GameObject actionPanel;
    protected List<Button> actionButtons;

    public override void init()
    {
        base.init();
        shelfPlacementManager = ShelfPlacementManager.instance;
        employeeUICanvas = GameObject.Find("Canvas");
    }
    private void Start()
    {
        init();
    }

    protected override void Update()
    {
        base.Update();

        if(employeeSelected)
        {
            if(startShelfSelect)
            {
                chooseShelf();
            }
            
        }
        else if(employeeSelected == false && actionPanel!=null)
        {
            
            Destroy(actionPanel);
            actionButtons.Clear();
            startShelfSelect = false;
        }
        else if(employeeSelected == false && actionPanel == null)
        {
            chooseEmployee();
        }

    }

    
    public virtual void chooseEmployee()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (shelfPlacementManager.shelfBeingRepositioned == null && Input.GetMouseButtonDown(0))
        {
            //If not clicked on empty space
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    if (employeeSelected == false)
                    {
                        employeeSelected = true;
                        actionPanel = Instantiate(employeeActionPanelPrefab, employeeUICanvas.transform);
                        actionPanel.GetComponent<EmployeeActionPanel>().target = this.transform;
                        actionButtons = new List<Button>();
                        actionButtons.AddRange(actionPanel.GetComponentsInChildren<Button>());
                        foreach (Button button in actionButtons)
                        {
                            int buttonIndex = actionButtons.IndexOf(button);
                            button.onClick.AddListener(() => OnButtonClick(buttonIndex));
                        }
                    }

                }
                else if (startShelfSelect) { }
                else
                {
                    employeeSelected = false;
                }
            }
            //if clicked on empty space
            else
            {
                employeeSelected = false;
            }
            
        }
    }

    public void OnButtonClick(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                startShelfSelect = true;
                break;
            case 1:
                Debug.Log("Button 2");
                break;
        }
    }

    public virtual void chooseShelf()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if(Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<ShelfScript>() != null)
                {
                    NeededShelf = hit.collider.gameObject.GetComponent<ShelfScript>();
                    destinationSetter.target = NeededShelf.transform;
                    aiPath.canMove = true;
                    startShelfSelect = false;
                    employeeSelected = false;
                }
            }
            else
            {
                employeeSelected = false;

            }
        }

    }


}
