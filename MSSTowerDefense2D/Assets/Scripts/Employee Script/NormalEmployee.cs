using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum employeeStage
{
    standBy = 0,
    isSelected = 1,
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
            }
            chooseEmployee(mousePosition,mouseHit);
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

    //different situations for different buttons
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

    public virtual void chooseActions(Vector2 mousePosition, RaycastHit2D hit)
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(hit.collider != null && hit.collider.gameObject == this.gameObject)
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
