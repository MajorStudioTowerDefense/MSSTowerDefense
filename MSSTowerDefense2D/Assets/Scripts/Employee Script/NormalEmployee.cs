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
    public AudioClip EmployeeWalking;

    
    //the shelf being selected
    public ShelfScript NeededShelf;
    public bool isCarrying = false;
    public int carryMax = 3;
    public int carryCount = 3;

    private float timeAtShelf = 0f;
    public float stayShelfDuration = 5f;
    public float reachTolerance = 1f;

    public float employeeAreaOffsetXMax = 2f;
    public float employeeAreaOffsetXMin = 0.5f;
    public float employeeAreaOffsetYMin = 0.4f;
    public float employeeAreaOffsetYMax = 1.2f;
    public Vector3 myEmployeeAreaPos = Vector3.zero;

    public int wage;

    private bool endStage = false;

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
    
    public Animator employeeAnimator;
    /////////////////////////////////

    public override void init()
    {
        base.init();
        shelfPlacementManager = ShelfPlacementManager.instance;
        
    }
    private void Start()
    {
        init();
        employeeAnimator = GetComponent<Animator>();
        employeeCanvas = GetComponentInChildren<Canvas>();
        employeeCanvas.worldCamera = Camera.main;
        employeeLoadingImage = employeeCanvas.GetComponentInChildren<Image>();

    }

    protected override void Update()
    {
        base.Update();
        if(GameManager.instance.currentState == GameStates.END)
        {
            if(endStage == false)
            {
                endStage = true;
                aiPath.canMove = true;
                myEmployeeAreaPos = randomOffsetPos();
                destinationSetter.targetPosition = myEmployeeAreaPos;
                carriedItemSprite.sprite = null;
                carriedShelfSprite.sprite = null;
                //progress
                employeeLoadingImage.fillAmount = 0;
                eStage = employeeStage.backToStandBy;
            }
            if(eStage == employeeStage.backToStandBy)
            {
                returnToStandBy();
            }
            
        }
        if (GameManager.instance.currentState != GameStates.STORE)
        {
            return;
        }
        endStage = false;
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
            
            if (eAction == employeeAction.moveShelf)
            AudioManager.instance.PlaySound(EmployeeWalking);

            onWayToMoveShelf();
        }
        else if (eStage == employeeStage.backToStandBy)
        {
            if (employeeAnimator.GetBool("isBuying"))
            {
                
                employeeAnimator.SetBool("isBuying", false);
                employeeAnimator.enabled = false;
            }
            
            returnToStandBy();
        }

    }

    public Vector3 randomOffsetPos()
    {
        float possibilityX = Random.Range(0f, 1f);
        float possibilityY = Random.Range(0f, 1f);
        float xSign = 1;
        float ySign = 1;
        if(possibilityX < 0.5f)
        {
            xSign = -1;
        }
        if(possibilityY < 0.5f)
        {
            ySign = -1;
        }
        float offsetX = xSign * Random.Range(employeeAreaOffsetXMin, employeeAreaOffsetXMax);
        float offsetY = ySign * Random.Range(employeeAreaOffsetYMin, employeeAreaOffsetYMax);
        Vector2 offset = new Vector2(offsetX, offsetY);
        Vector3 pos = employeeArea.position + (Vector3)offset;
        return pos;
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
        myEmployeeAreaPos = randomOffsetPos();
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
        myEmployeeAreaPos = randomOffsetPos();
        shadowNeeded = shadow;
    }

    public virtual void onWayToGrabShelf()
    {
        if(destinationSetter.targetPosition == moveShelfNeeded.transform.position && IsCloseToDestination())
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
        if (destinationSetter.targetPosition == shadowNeeded.transform.position && IsCloseToDestination())
        {
            Debug.Log("Arrived at the shadow");
            moveShelfNeeded.gameObject.SetActive(true);
            moveShelfNeeded.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            ShelfPlacementManager.instance.RepositionShelfMovedByEmployee(shadowNeeded.transform.position, moveShelfNeeded.gameObject);
            moveShelfNeeded.loadAllowed = true;
            carriedShelfSprite.sprite = null;
            Destroy(shadowNeeded);
            eStage = employeeStage.backToStandBy;
            destinationSetter.targetPosition = myEmployeeAreaPos;
            aiPath.destination = myEmployeeAreaPos;
        }
    }

    public virtual void successReload()
    {
        if(destinationSetter.targetPosition == NeededShelf.transform.position && IsCloseToDestination())
        {
            
            if (employeeAnimator.enabled!=true)
            {
                employeeAnimator.enabled = true;
                employeeAnimator.SetBool("isBuying", true);
            }
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
                        destinationSetter.targetPosition = myEmployeeAreaPos;
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
                destinationSetter.targetPosition = myEmployeeAreaPos;
                eStage = employeeStage.backToStandBy;
            }
        }
    }

    public virtual void returnToStandBy()
    {
        if(destinationSetter.targetPosition == myEmployeeAreaPos && IsCloseToDestination())
        {
            eStage = employeeStage.standBy;
            eAction = employeeAction.noAction;
            carryCount = carryMax;
            NeededShelf = null;
            moveShelfNeeded = null;
        }

    }

    private bool IsCloseToDestination()
    {
        float distance = Vector3.Distance(transform.position, destinationSetter.targetPosition);
        return distance <= reachTolerance;
    }

    void OnDrawGizmos()
    {
        if (destinationSetter != null)
        {
            // 设置Gizmos颜色为蓝色
            Gizmos.color = Color.blue;

            // 绘制一个圆圈表示接近目的地的区域
            Gizmos.DrawWireSphere(destinationSetter.targetPosition, reachTolerance);
        }
    }

}
