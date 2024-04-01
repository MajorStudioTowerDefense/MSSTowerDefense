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
    //UI for employee
    public Sprite[] carriedItemSprites;
    public SpriteRenderer carriedItemSprite;
    

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
                
            }
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
        }

    }

}
