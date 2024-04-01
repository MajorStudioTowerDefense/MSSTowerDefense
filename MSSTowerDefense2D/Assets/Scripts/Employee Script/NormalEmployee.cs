using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public enum employeeStage
{
    standBy = 0,
    isSelected = 1,
    running = 2,
    backToStandBy = 3,
}

public enum employeeAction
{
    noAction = 0,
    reload = 1,
    moveShelf = 2,
}
public class NormalEmployee : Bot
{
    public AudioClip Walking;

    
    //the shelf being selected
    public ShelfScript NeededShelf;
    public bool isCarrying = false;
    public int carryMax = 3;
    public int carryCount = 3;

    private float timeAtShelf = 0f;
    public float stayShelfDuration = 5f;


    //check if the shelf is selected
    public ShelfPlacementManager shelfPlacementManager;
    //stage of the employee
    public employeeStage eStage = employeeStage.standBy;
    //action of the employee
    public employeeAction eAction = employeeAction.noAction;

    /////////////////////////////////

    public Transform employeeArea;

    /////////////////////////////////
     


    /////////////////////////////////
    //UI when the employee is selected
    public Canvas employeeUICanvas;
    public GameObject employeeActionPanelPrefab;

    protected GameObject actionPanel;
    protected List<Button> actionButtons;

    /////////////////////////////////

    public override void init()
    {
        base.init();
        shelfPlacementManager = ShelfPlacementManager.instance;
        employeeUICanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
    private void Start()
    {
        init();
        AudioManager.instance.PlaySound(Walking);

    }

    protected override void Update()
    {
        Debug.Log("current stage = " + eStage);
        Debug.Log("current action = " + eAction);
        base.Update();

        if (GameManager.instance.currentState != GameStates.STORE)
        {
            return;
        }

        if (eStage == employeeStage.standBy)
        {
            if(sprite.color != Color.white)
            {
                sprite.color = Color.white;
            }
        }
        else if (eStage == employeeStage.isSelected)
        {
            showSelectedUI();
        }

    }

    void showSelectedUI()
    {
        if(sprite.color != Color.cyan)
        {
            sprite.color = Color.cyan;
        }

        
    }
 

    public virtual void reloadShelf(ShelfScript shelf, int index)
    {
        //货架需要什么类型的货物的代码暂时留空
        //////////////////////////////////////////
        destinationSetter.target = shelf.gameObject.transform;
        int actualIncrease = Mathf.Min(carryCount, NeededShelf.loadAmountMax - NeededShelf.loadAmount);


        if (destinationSetter.target!=null && aiPath.reachedDestination)
        {
            if (NeededShelf != null)
            {
                
                timeAtShelf += Time.deltaTime;
                if (timeAtShelf >= stayShelfDuration && NeededShelf.loadAmount < NeededShelf.loadAmountMax)
                {
                    NeededShelf.loadAmount += actualIncrease;
                    carryCount -= actualIncrease;
                    //eStage = employeeStage.actionFinished;
                    timeAtShelf = 0;
                }
                else if (NeededShelf.loadAmount == NeededShelf.loadAmountMax)
                {
                    Debug.Log("Load amount has reached its maximum.");
                    //eStage = employeeStage.actionFinished;
                }
            }
            else
            {
                Debug.Log("No shelf selected");
            }
        }
    }

    public virtual void returnToStandBy()
    {
        if(destinationSetter.target != null)
        {
            destinationSetter.target = employeeArea;
            eStage = employeeStage.backToStandBy;
        }

    }

    public virtual void onWayToBack()
    {
        if (aiPath.reachedDestination)
        {
            Debug.Log("Back to stand by");
            eStage = employeeStage.standBy;
            eAction = employeeAction.noAction;
            carryCount = carryMax;
            //aiPath.canMove = false;
            //destinationSetter.target = null;
            //NeededShelf = null;
        }
    }


}
