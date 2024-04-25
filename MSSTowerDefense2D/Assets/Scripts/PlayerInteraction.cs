using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour
{
    //Mouse
    private float mouseDownTime;
    private bool timerOn = false;
    public float holdMouseDuration = 1f;
    [SerializeField]private bool holdLongEnough = false;

    delegate void AssignTaskDelegate(GameObject chosen);
    AssignTaskDelegate assignedTask;

    interactionStage currentStage = interactionStage.primary;

    [Header("UI")]
    public Sprite[] mouseUIs;
    public Image mouseUIPrefab;

    [Header("clickedGameObject")]
    [SerializeField] private NormalEmployee EmployeeChosenInPrimaryTask;
    [SerializeField] private ShelfScript shelfChosenForReload;
    [SerializeField] private ShelfScript ShelfChosenInPrimaryTask;


    [Header("ShelfPlacementManager")]
    public ShelfPlacementManager shelfPlacementManager;
    public GameObject shadowShelfPrefab;
    public GameObject shadowShelf;

    [Header("Employee")]
    public UpgradeSystem upgradeSystem;

    [Header("Layermasks")]
    public LayerMask interactionLayer;
    [SerializeField]bool onCorrectLayer = false;

    enum interactionStage
    {
        primary = 0,
        findTarget
    }

    private void Start()
    {
        Cursor.visible = false;
        shelfPlacementManager = ShelfPlacementManager.instance;
    }
    private void Update()
    {
        MouseHoverOnInteractable();
        HandleMouseInput();
        if(GameManager.instance.currentState == GameStates.END)
        {
            //ResetAllAtEndStage();
            return;
        }
        if (currentStage == interactionStage.primary) { assignPrimaryTask(); }
        if(currentStage == interactionStage.findTarget) { assignTaskTarget(); }

        
        
        Debug.Log("current STage is "+currentStage);
    }
    
    void assignPrimaryTask()
    {
        if (GameManager.instance.currentState == GameStates.STORE)
        {
            //如果鼠标左键点击且点到了物体
            //If mose left click and clicked on interacted object
            if(Input.GetMouseButtonDown(0) && onCorrectLayer)
            {
                GameObject clicked = hoverGameObject;
                if (clicked != null)
                {
                    //如果点击的是员工
                    //if clicked is an employee
                    if(clicked.tag == "Employee")
                    {
                        EmployeeChosenInPrimaryTask = clicked.GetComponent<NormalEmployee>();
                        //如果员工处于standBy状态
                        //if the employee is in standBy state
                        if (EmployeeChosenInPrimaryTask.eStage == employeeStage.standBy)
                        {
                                
                            EmployeeChosenInPrimaryTask.eStage = employeeStage.isSelected;
                            if(mouseUIs.Length > 0)
                            {
                                changeMouseUI(1);
                            }
                            currentStage = interactionStage.findTarget;
                            assignedTask = assignTaskTargetForEmployee;
                        }
                            
                    }
                    ////如果点击的是货架
                    ////if clicked is a shelf
                    if (clicked.tag == "interactedShelf")
                    {
                        Debug.Log("shelf clicked");
                        //单次点击目前没有功能
                    }
                }
            }
            /**
            //如果鼠标长按并释放且点到了shelf
            else if(holdLongEnough && longHoldGameObject!=null  && longHoldGameObject.tag == "interactedShelf" && longHoldGameObject.GetComponent<ShelfScript>().loadAllowed)
            {
                GameObject clicked = longHoldGameObject;
                Debug.Log("hold long enough");
                ShelfChosenInPrimaryTask = clicked.GetComponent<ShelfScript>();
                if(ShelfChosenInPrimaryTask.loadAmount == ShelfChosenInPrimaryTask.loadAmountMax)
                {
                    Debug.Log("shelf full");
                    return;
                }
                ShelfChosenInPrimaryTask.loadAllowed = false;
                currentStage = interactionStage.findTarget;
                assignedTask = assignTaskTargetForShelf;
                shadowShelf = Instantiate(shadowShelfPrefab, PrintMousePosition(), Quaternion.identity);
                shadowShelf.GetComponent<SpriteRenderer>().sprite = ShelfChosenInPrimaryTask.GetComponent<SpriteRenderer>().sprite;
                ShelfChosenInPrimaryTask.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                ShelfChosenInPrimaryTask.gameObject.tag = "interactedShelf";
                shelfPlacementManager.SetCurrentShelfInstance(shadowShelf);
                if (mouseUIs.Length > 0)
                {
                    changeMouseUI(2);
                }
            }**/
        }
    }


    //该程序选择任务目标
    void assignTaskTarget()
    {

        //如果点击右键,则取消当前行为
        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log("cancel");
            currentStage = interactionStage.primary;
            resetEverythingToDefault();
            return;
        }
        //如果点击的是左键，则为任务选择目标
        if(Input.GetMouseButtonDown(0))
        {
            if(assignedTask!=null)
            {
                if (hoverGameObject != null)
                {
                    assignedTask(hoverGameObject);
                }
                else if(!IsClickOverButton())
                {
                    currentStage = interactionStage.primary;
                    resetEverythingToDefault();
                    return;
                }
                
                
            }
        }
        
        /*
        if (assignedTask == assignTaskTargetForShelf && shelfPlacementManager.GetCurrentShelfInstance()!=null)
        {
            
            shelfPlacementManager.SnapShelfToGridAvoidOverlap();
        }*/
        
    }

    [SerializeField] Button[] shelfStockButtons;
    //员工执行任务时所分配的目标
    void assignTaskTargetForEmployee(GameObject clicked)
    {
        if (clicked == null) { return; }
        
        //如果点击的是员工
        if (clicked.tag=="Employee")
        {
            //清除当前员工状态
            EmployeeChosenInPrimaryTask.eStage = employeeStage.standBy;
            EmployeeChosenInPrimaryTask.eAction = employeeAction.noAction;
            //切换至新点击员工并改变其状态
            EmployeeChosenInPrimaryTask = clicked.GetComponent<NormalEmployee>();
            EmployeeChosenInPrimaryTask.eStage = employeeStage.isSelected;
        }
        //如果点击的是货架 
        else if (!IsClickOverButton() && clicked.tag=="interactedShelf" && clicked.GetComponent<ShelfScript>().loadAllowed)
        {
            //如果当前没有货架被选中则选择当前货架
            if(shelfChosenForReload == null) { shelfChosenForReload = clicked.GetComponent<ShelfScript>(); }
            //如果点击的货架不是当前选中的货架，则关闭按钮后切换货架
            else if(shelfChosenForReload != clicked.GetComponent<ShelfScript>())
            {
                Canvas lastShelfCanvas = shelfChosenForReload.GetComponentInChildren<Canvas>();
                lastShelfCanvas.sortingOrder = 0;
                Debug.Log("switch shelf");
                foreach (Button button in shelfChosenForReload.GetComponentsInChildren<Button>(true))
                {
                    button.onClick.RemoveAllListeners();
                    button.gameObject.SetActive(false);
                }
                shelfChosenForReload = clicked.GetComponent<ShelfScript>();
            }
            
            
            shelfStockButtons = shelfChosenForReload.GetComponentsInChildren<Button>(true);
            Canvas ShelfCanvas = shelfChosenForReload.GetComponentInChildren<Canvas>();
            ShelfCanvas.sortingOrder = 1;
            //如果货架上没有物品则显示三个按钮
            if(shelfChosenForReload.loadAmount == 0)
            {
                for (int i = 0; i < shelfStockButtons.Length; i++)
                {
                    Button button = shelfStockButtons[i];
                    button.gameObject.SetActive(true);
                    button.onClick.RemoveAllListeners();
                    //根据不同货架类型显示不同的物品
                    shelvesType type = shelfChosenForReload.thisType;
                    foreach(Transform child in button.transform)
                    {
                        Image image = child.GetComponent<Image>();
                        switch (type)
                        {
                            case shelvesType.Shelf:
                                image.sprite = ItemManager.Instance.shelfItemSprites[i];
                                break;
                            case shelvesType.HighShelf:
                                image.sprite = ItemManager.Instance.shelfItemSprites[i + 3];
                                break;
                            case shelvesType.Table:
                                image.sprite = ItemManager.Instance.shelfItemSprites[i + 6];
                                break;
                            case shelvesType.Rack:
                                image.sprite = ItemManager.Instance.shelfItemSprites[i + 9];
                                break;
                        }
                    }
                    
                    button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80 + i*80f, button.GetComponent<RectTransform>().anchoredPosition.y);
                    goods product = shelfChosenForReload.itemsCanBeSold[i].GetItem();
                    button.onClick.AddListener(() => OnButtonClick(product));
                }
            }
            //如果货架上有物品则只显示一个按钮，读取当前货架物品
            else
            {
                for (int i = 0; i < shelfStockButtons.Length; i++)
                {
                    Button button = shelfStockButtons[i];
                    button.onClick.RemoveAllListeners();
                    button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, button.GetComponent<RectTransform>().anchoredPosition.y);
                    if (i == 0)
                    {
                        button.gameObject.SetActive(true);
                        foreach(Transform child in button.transform)
                        {
                            Image image = child.GetComponent<Image>();
                            image.sprite = ItemManager.Instance.shelfItemSprites[(int)shelfChosenForReload.sellingItem.GetItem()];
                        }
                        
                    }else if (i != 0)
                    {
                        button.gameObject.SetActive(false);
                    }
                    
                    button.onClick.AddListener(() => OnButtonClick(shelfChosenForReload.sellingItem.GetItem()));
                }
            }
            
        }
        
    }

    public void OnButtonClick(goods product)
    {
        EmployeeChosenInPrimaryTask.eAction = employeeAction.reload;
        EmployeeChosenInPrimaryTask.reloadShelf(shelfChosenForReload,product);
        
        resetSomethingToDefault();
    }

    /*
    public TextMeshProUGUI warningText;
    //货架放置时所分配的目标
    void assignTaskTargetForShelf(Collider2D clickedCol) 
    {
        shelfPlacementManager.SetCurrentShelfInstanceToNull();
        shadowShelf.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
        foreach (GameObject emp in upgradeSystem.addEmployeeUseList)
        {
            NormalEmployee employee = emp.GetComponent<NormalEmployee>();
            if(employee.eStage == employeeStage.standBy)
            {
                employee.eAction = employeeAction.moveShelf;
                employee.eStage = employeeStage.isSelected;
                employee.moveShelf(ShelfChosenInPrimaryTask,shadowShelf);
                currentStage = interactionStage.primary;
                resetSomethingToDefault();
                return;
            }
            
        }
        Debug.Log("no employee available");
        warningText.gameObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y,0);
        warningText.gameObject.SetActive(true);
        currentStage = interactionStage.primary;
        resetEverythingToDefault();

    }
    */

    //重置某些状态至默认,选择完target后使用
    void resetSomethingToDefault()
    {
        if(assignedTask == assignTaskTargetForEmployee)
        {
            
            if (shelfStockButtons != null)
            {
                foreach(Button button in shelfStockButtons)
                {
                    button.onClick.RemoveAllListeners();
                    button.gameObject.SetActive(false);
                }
                shelfStockButtons = null;
            }
            if(EmployeeChosenInPrimaryTask != null)
            {
                EmployeeChosenInPrimaryTask = null;
                
            }
            if(shelfChosenForReload != null)
            {
                Canvas ShelfCanvas = shelfChosenForReload.GetComponentInChildren<Canvas>();
                ShelfCanvas.sortingOrder = 0;
                shelfChosenForReload = null;
            }
            changeMouseUI(0);
            currentStage = interactionStage.primary;
        }
        /*
        if (assignedTask == assignTaskTargetForShelf)
        {
            if (ShelfChosenInPrimaryTask != null)
            {
                ShelfChosenInPrimaryTask = null;
            }
            if (shadowShelf != null)
            {
                shadowShelf = null;
            }
            changeMouseUI(0);
            assignedTask = null;
            shelfPlacementManager.SetCurrentShelfInstance(null);
        }*/
    }

    //重置所有状态至默认,取消选项时使用
    void resetEverythingToDefault()
    {
        if(assignedTask == assignTaskTargetForEmployee)
        {
            if(EmployeeChosenInPrimaryTask != null)
            {
                EmployeeChosenInPrimaryTask.eStage = employeeStage.standBy;
                EmployeeChosenInPrimaryTask.eAction = employeeAction.noAction;
                EmployeeChosenInPrimaryTask = null;
                changeMouseUI(0);
            }
            if(shelfStockButtons != null)
            {
                foreach(Button button in shelfStockButtons)
                {
                    button.onClick.RemoveAllListeners();
                    button.gameObject.SetActive(false);
                }
                shelfStockButtons = null;
            }
            if(shelfChosenForReload != null)
            {
                Canvas ShelfCanvas = shelfChosenForReload.GetComponentInChildren<Canvas>();
                ShelfCanvas.sortingOrder = 0;
                shelfChosenForReload = null;
            }
            assignedTask = null;
        }
        /*
        if(assignedTask == assignTaskTargetForShelf)
        {
            if(ShelfChosenInPrimaryTask != null)
            {
                ShelfChosenInPrimaryTask.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                ShelfChosenInPrimaryTask.loadAllowed = true;
                ShelfChosenInPrimaryTask = null;
            }
            if(shadowShelf!=null)
            {
                Destroy(shadowShelf);
            }
            changeMouseUI(0);
            assignedTask = null;
            shelfPlacementManager.SetCurrentShelfInstance(null);
        }*/
    }

    private void ResetAllAtEndStage()
    {
        if (EmployeeChosenInPrimaryTask != null)
        {
            EmployeeChosenInPrimaryTask.eStage = employeeStage.standBy;
            EmployeeChosenInPrimaryTask.eAction = employeeAction.noAction;
            EmployeeChosenInPrimaryTask = null;
            changeMouseUI(0);
        }
        if (shelfStockButtons != null)
        {
            foreach (Button button in shelfStockButtons)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
            shelfStockButtons = null;
        }
        if (shelfChosenForReload != null)
        {
            Canvas ShelfCanvas = shelfChosenForReload.GetComponentInChildren<Canvas>();
            ShelfCanvas.sortingOrder = 0;
            shelfChosenForReload = null;
        }
        if (ShelfChosenInPrimaryTask != null)
        {
            ShelfChosenInPrimaryTask.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            ShelfChosenInPrimaryTask.loadAllowed = true;
            ShelfChosenInPrimaryTask = null;
        }
        if (shadowShelf != null)
        {
            Destroy(shadowShelf);
            shadowShelf = null;
        }
        if(GameObject.Find("ShadowShelf(Clone)") != null)
        {
            Destroy(GameObject.Find("ShadowShelf(Clone)"));
        }
        changeMouseUI(0);
        assignedTask = null;
        shelfPlacementManager.SetCurrentShelfInstance(null);
    }
    

    

    void changeMouseUI(int index)
    {
        mouseUIPrefab.sprite = mouseUIs[index];
    }

    //on correct layer bool
    #region newCode
    private void HandleMouseInput()
    {
        if(currentStage == interactionStage.primary)
        {
            if (onCorrectLayer)
            {
                changeMouseUI(4);
                if(hoverGameObject.tag == "interactedShelf")
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartMouseDownTimer();
                    }
                }

            }
            if (Input.GetMouseButton(0) && timerOn)
            {
                CheckHoldDuration();
            }
            if (Input.GetMouseButtonUp(0))
            {
                ResetMouseDownTimer();
            }
            if (!timerOn && !onCorrectLayer)
            {
                changeMouseUI(0);
            }
        }
        
    }

    private void StartMouseDownTimer()
    {
        mouseDownTime = Time.time;
        timerOn = true;
        longHoldGameObject = hoverGameObject;
    }

    private void CheckHoldDuration()
    {
        float duration = Time.time - mouseDownTime;
        UpdateMouseUI(duration);
        if (duration >= holdMouseDuration)
        {
            holdLongEnough = true;
            
        }
    }

    private void UpdateMouseUI(float duration)
    {
        if (duration >= 0.3f && duration < holdMouseDuration)
        {
            if (mouseUIPrefab.sprite == mouseUIs[4]) { changeMouseUI(3); }

            mouseUIPrefab.fillAmount = duration / holdMouseDuration;
        }
        else if (duration >= holdMouseDuration)
        {
            mouseUIPrefab.fillAmount = 1;
        }
    }

    private void ResetMouseDownTimer()
    {
        if (mouseUIPrefab.sprite == mouseUIs[3]) { changeMouseUI(0); }

        mouseUIPrefab.fillAmount = 1;
        timerOn = false;
        holdLongEnough = false;
        longHoldGameObject = null;
    }

    [SerializeField] GameObject hoverGameObject;
    public GameObject longHoldGameObject;
    void MouseHoverOnInteractable()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(worldPoint.x, worldPoint.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero,Mathf.Infinity,interactionLayer);

        if(hit.collider!=null)
        {
            onCorrectLayer = true;
            hoverGameObject = hit.collider.transform.parent.gameObject;
        }
        else
        {
            onCorrectLayer = false;
            hoverGameObject = null;
        }


    }

    public bool getHoldLongEnough()
    {
        return holdLongEnough;
    }

    bool IsClickOverButton()
    {
        // 创建一个新的 PointerEventData 实例
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };

        // 创建一个用于接收结果的 RaycastResult 列表
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // 检查结果，查找带有 Button 组件的游戏对象
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true; // 发现一个实际的 Button 元素
            }
        }
        return false; // 没有发现任何 Button 元素
    }

    #endregion

    Vector3 PrintMousePosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mousePosition.x, mousePosition.y,0);
    }

}
