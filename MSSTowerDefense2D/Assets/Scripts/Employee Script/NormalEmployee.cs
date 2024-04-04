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
    //the item needed to be carried
    private goods itemNeeded;


    /////////////////////////////////
    //UI for employee
    public SpriteRenderer carriedItemSprite;
    public SpriteRenderer carriedShelfSprite;
    public Canvas employeeCanvas;
    public Image employeeLoadingImage;
    

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
        employeeCanvas = GetComponentInChildren<Canvas>();
        employeeCanvas.worldCamera = Camera.main;
        employeeLoadingImage = employeeCanvas.GetComponentInChildren<Image>();

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
 

    public virtual void reloadShelf(ShelfScript shelf, goods product)
    {
        //获取货物枚举类型，附加到货架上
        itemNeeded = product;
        //////////////////////////////////////////
        aiPath.canMove = true;
        destinationSetter.targetPosition = shelf.gameObject.transform.position;
        NeededShelf = shelf;
        carriedItemSprite.sprite = ItemManager.Instance.shelfItemSprites[(int)product];
        eStage = employeeStage.running;

    }

    private ShelfScript moveShelfNeeded;
    private GameObject shadowNeeded;
    public virtual void moveShelf(ShelfScript shelf, GameObject shadow)
    {
        aiPath.canMove = true;
        moveShelfNeeded = shelf;
        Debug.Log("moveShelfname = "+moveShelfNeeded.gameObject.name);
        destinationSetter.targetPosition = shelf.transform.position;
        aiPath.destination = shelf.transform.position;
        eStage = employeeStage.running;
        
        shadowNeeded = shadow;
    }

    public virtual void onWayToGrabShelf()
    {
        if(destinationSetter.targetPosition == moveShelfNeeded.transform.position && aiPath.reachedDestination)
        {
            Debug.Log("Arrived at the shelf");
            carriedShelfSprite.sprite = moveShelfNeeded.GetComponent<SpriteRenderer>().sprite;
            moveShelfNeeded.gameObject.SetActive(false);
            destinationSetter.targetPosition = shadowNeeded.transform.position;
            eStage = employeeStage.finishing;
        }
        
    }

    public virtual void onWayToMoveShelf()
    {
        if (destinationSetter.targetPosition == shadowNeeded.transform.position && aiPath.reachedDestination)
        {
            Debug.Log("Arrived at the shadow");
            moveShelfNeeded.gameObject.SetActive(true);
            moveShelfNeeded.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            ShelfPlacementManager.instance.RepositionShelfMovedByEmployee(shadowNeeded.transform.position, moveShelfNeeded.gameObject);
            moveShelfNeeded.loadAllowed = true;
            carriedShelfSprite.sprite = null;
            Destroy(shadowNeeded);
            eStage = employeeStage.backToStandBy;
            destinationSetter.targetPosition = employeeArea.position;
            aiPath.destination = employeeArea.position;
        }
    }

    public virtual void successReload()
    {
        if(destinationSetter.targetPosition == NeededShelf.transform.position && aiPath.reachedDestination)
        {
            int actualIncrease = Mathf.Min(carryCount, NeededShelf.loadAmountMax - NeededShelf.loadAmount);

            
            timeAtShelf += Time.deltaTime;
            if (timeAtShelf <= stayShelfDuration)
            {
                float progress = timeAtShelf / stayShelfDuration;
                employeeLoadingImage.fillAmount = progress;
            }
            if (timeAtShelf >= stayShelfDuration && NeededShelf.loadAmount < NeededShelf.loadAmountMax)
            {
                foreach(Items items in NeededShelf.itemsCanBeSold)
                {
                    if(items.GetItem() == itemNeeded)
                    {
                        NeededShelf.loadAmount += actualIncrease;
                        carryCount -= actualIncrease;
                        eStage = employeeStage.backToStandBy;
                        destinationSetter.targetPosition = employeeArea.position;
                        carriedItemSprite.sprite = null;
                        timeAtShelf = 0;
                        NeededShelf.sellingItem = items;
                        break;
                    }
                }

            }
            else if (NeededShelf.loadAmount == NeededShelf.loadAmountMax)
            {
                carriedItemSprite.sprite = null;
                Debug.Log("Load amount has reached its maximum.");
                destinationSetter.targetPosition = employeeArea.position;
                eStage = employeeStage.backToStandBy;
            }
        }
    }

    public virtual void returnToStandBy()
    {
        if(destinationSetter.targetPosition == employeeArea.position && aiPath.reachedDestination)
        {
            eStage = employeeStage.standBy;
            eAction = employeeAction.noAction;
            carryCount = carryMax;
            NeededShelf = null;
            moveShelfNeeded = null;
        }

    }

}
