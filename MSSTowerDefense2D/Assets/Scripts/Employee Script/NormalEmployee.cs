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
    finishing = 3,
    backToStandBy = 4,
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
    //UI for employee
    public Sprite[] carriedItemSprites;
    public SpriteRenderer carriedItemSprite;
    public SpriteRenderer carriedShelfSprite;
    

    /////////////////////////////////

    public override void init()
    {
        base.init();
        shelfPlacementManager = ShelfPlacementManager.instance;
        
    }
    private void Start()
    {
        init();
        AudioManager.instance.PlaySound(Walking);

    }

    protected override void Update()
    {
        Debug.Log("reachedDestination? "+aiPath.reachedDestination);
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
        else if(eStage == employeeStage.running)
        {
            if (sprite.color != Color.white)
            {
                sprite.color = Color.white;
            }
            if (eAction == employeeAction.reload)
            {
                successReload();
            }
            else if(eAction == employeeAction.moveShelf)
            {
                onWayToGrabShelf();
            }
        }
        else if (eStage == employeeStage.finishing)
        {
            if(eAction == employeeAction.moveShelf)
            onWayToMoveShelf();
        }
        else if (eStage == employeeStage.backToStandBy)
        {
            returnToStandBy();
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
        ///获取货物的名字，目前只有一种类型货物所以只有一个枚举，之后添加更多货物类型
        ///There is only one type of goods so far, so only one enum is available. More goods types will be added later.
        goods goodsName = shelf.sellingItems[index].GetItem();
        carriedItemSprite.sprite = carriedItemSprites[(int)goodsName];
        //////////////////////////////////////////
        aiPath.canMove = true;
        destinationSetter.target = shelf.gameObject.transform;
        NeededShelf = shelf;
        eStage = employeeStage.running;

    }

    private ShelfScript moveShelfNeeded;
    private GameObject shadowNeeded;
    public virtual void moveShelf(ShelfScript shelf, GameObject shadow)
    {
        aiPath.canMove = true;
        moveShelfNeeded = shelf;
        Debug.Log("moveShelfname = "+moveShelfNeeded.gameObject.name);
        destinationSetter.target = shelf.transform;
        aiPath.destination = shelf.transform.position;
        eStage = employeeStage.running;
        
        shadowNeeded = shadow;
    }

    public virtual void onWayToGrabShelf()
    {
        if(destinationSetter.target == moveShelfNeeded.transform && aiPath.reachedDestination)
        {
            Debug.Log("Arrived at the shelf");
            carriedShelfSprite.sprite = moveShelfNeeded.GetComponent<SpriteRenderer>().sprite;
            moveShelfNeeded.gameObject.SetActive(false);
            destinationSetter.target = shadowNeeded.transform;
            eStage = employeeStage.finishing;
        }
        
    }

    public virtual void onWayToMoveShelf()
    {
        if (destinationSetter.target == shadowNeeded.transform && aiPath.reachedDestination)
        {
            Debug.Log("Arrived at the shadow");
            moveShelfNeeded.gameObject.SetActive(true);
            moveShelfNeeded.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            ShelfPlacementManager.instance.RepositionShelfMovedByEmployee(shadowNeeded.transform.position, moveShelfNeeded.gameObject);
            moveShelfNeeded.loadAllowed = true;
            carriedShelfSprite.sprite = null;
            Destroy(shadowNeeded);
            eStage = employeeStage.backToStandBy;
            destinationSetter.target = employeeArea;
        }
    }

    public virtual void successReload()
    {
        if(destinationSetter.target == NeededShelf.transform && aiPath.reachedDestination)
        {
            int actualIncrease = Mathf.Min(carryCount, NeededShelf.loadAmountMax - NeededShelf.loadAmount);

            timeAtShelf += Time.deltaTime;
            if (timeAtShelf >= stayShelfDuration && NeededShelf.loadAmount < NeededShelf.loadAmountMax)
            {
                NeededShelf.loadAmount += actualIncrease;
                carryCount -= actualIncrease;
                eStage = employeeStage.backToStandBy;
                destinationSetter.target = employeeArea;
                carriedItemSprite.sprite = null;
                timeAtShelf = 0;
            }
            else if (NeededShelf.loadAmount == NeededShelf.loadAmountMax)
            {
                carriedItemSprite.sprite = null;
                Debug.Log("Load amount has reached its maximum.");
                destinationSetter.target = employeeArea;
                eStage = employeeStage.backToStandBy;
            }
        }
    }

    public virtual void returnToStandBy()
    {
        if(destinationSetter.target == employeeArea && aiPath.reachedDestination)
        {
            eStage = employeeStage.standBy;
            eAction = employeeAction.noAction;
            carryCount = carryMax;
            NeededShelf = null;
            moveShelfNeeded = null;
        }

    }

}
