using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum employeeStage
{
    standBy = 0,
    isSelected = 1,
    actionSelected = 2,
}

public enum employeeAction
{
    noAction = 0,
    reload = 1,
    moveShelf = 2,
}
public class NormalEmployee : Bot
{
    //the shelf being selected
    public ShelfScript NeededShelf;
    public bool isCarrying = false;
    public int carryMax = 3;
    public int carryCount = 0;
    protected bool startShelfSelect;

    //check if the shelf is selected
    public ShelfPlacementManager shelfPlacementManager;
    //stage of the employee
    public employeeStage eStage = employeeStage.standBy;
    //action of the employee
    public employeeAction eAction = employeeAction.noAction;

    /////////////////////////////////
    //UI when the employee is selected
    public Canvas employeeUICanvas;
    public GameObject employeeActionPanelPrefab;

    protected GameObject actionPanel;
    protected List<Button> actionButtons;

    public override void init()
    {
        base.init();
        shelfPlacementManager = ShelfPlacementManager.instance;
        employeeUICanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
    private void Start()
    {
        init();
    }

    protected override void Update()
    {
        base.Update();

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D mouseHit = Physics2D.Raycast(mousePosition, Vector2.zero);
        //if the employee can be selected
        if (canSelectEmployee())
        {
            switch (eStage)
            {
                case employeeStage.standBy:
                    chooseEmployee(mousePosition, mouseHit);
                    break;
                case employeeStage.isSelected:
                    chooseActions(mousePosition, mouseHit);
                    break;
                case employeeStage.actionSelected:
                    switch (eAction)
                    {
                        case employeeAction.reload:
                            break;
                        case employeeAction.moveShelf:
                            break;
                    }
                    break;
            }

        }
        else
        {
            if(actionPanel != null)
            {
                Destroy(actionPanel);
            }
            eStage = employeeStage.standBy;
            eAction = employeeAction.noAction;
        }

    }

    public virtual bool canSelectEmployee()
    {
        if (shelfPlacementManager.shelfBeingRepositioned != null)
        {
            return false;
        }
        else
        {
            return true;
        }
        
    }

    //First step, click the employee
    public virtual void chooseEmployee(Vector2 mousePosition, RaycastHit2D hit)
    {
        //If mouse is clicked
        if (Input.GetMouseButtonDown(0))
        {
            //if not clicked on empty space
            if (hit.collider != null)
            {
                //if clicked on the employee
                if (hit.collider.gameObject == this.gameObject)
                {
                    eStage = employeeStage.isSelected;
                    actionPanel = Instantiate(employeeActionPanelPrefab, employeeUICanvas.transform);
                    actionPanel.GetComponent<EmployeeActionPanel>().target = this.transform;
                    //add listener to the buttons
                    actionButtons = new List<Button>();
                    actionButtons.AddRange(actionPanel.GetComponentsInChildren<Button>());
                    foreach (Button button in actionButtons)
                    {
                        int buttonIndex = actionButtons.IndexOf(button);
                        button.onClick.AddListener(() => OnButtonClick(buttonIndex));
                    }
                }
            }   
        }
    }

    //second step, click the action
    //different situations for different buttons
    public void OnButtonClick(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                eStage = employeeStage.actionSelected;
                eAction = employeeAction.reload;
                break;
            case 1:
                eStage = employeeStage.actionSelected;
                eAction = employeeAction.moveShelf;
                break;
        }
    }

    public virtual void chooseActions(Vector2 mousePosition, RaycastHit2D hit)
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIObject())
            {

            }
            else if(hit.collider != null && hit.collider.gameObject == this.gameObject)
            {

            }
            //if clicked on empty space£¬close the action panel, we can have the close animation later
            else
            {
                eStage = employeeStage.standBy;
                Destroy(actionPanel);
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult r in results)
        {
            if (r.gameObject == actionPanel || r.gameObject.transform.IsChildOf(actionPanel.transform))
            {
                return true;
            }
        }
        return false;
    }

    public virtual void chooseShelf(Vector2 mousePosition, RaycastHit2D hit)
    {
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

                }
            }
            else
            { }
        }
    }

    //public virtual void chooseShelf()
    //{
    //    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

    //    if(Input.GetMouseButtonDown(0))
    //    {
    //        if (hit.collider != null)
    //        {
    //            if (hit.collider.gameObject.GetComponent<ShelfScript>() != null)
    //            {
    //                NeededShelf = hit.collider.gameObject.GetComponent<ShelfScript>();
    //                destinationSetter.target = NeededShelf.transform;
    //                aiPath.canMove = true;
    //                startShelfSelect = false;

    //            }
    //        }
    //        else
    //        {


    //        }
    //    }

    //}


}
