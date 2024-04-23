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
    private bool holdLongEnough = false;

    delegate void AssignTaskDelegate(Collider2D clickedCol);
    AssignTaskDelegate assignedTask;

    interactionStage currentStage = interactionStage.primary;

    [Header("UI")]
    public Sprite[] mouseUIs;
    public Image mouseUIPrefab;

    [Header("clickedGameObject")]
    [SerializeField] private NormalEmployee clickedEmployeeForEmployee;
    [SerializeField] private ShelfScript clickedShelfForEmployee;
    [SerializeField] private ShelfScript clickedShelfForShelf;


    [Header("ShelfPlacementManager")]
    public ShelfPlacementManager shelfPlacementManager;
    public GameObject shadowShelfPrefab;
    public GameObject shadowShelf;

    [Header("Employee")]
    public UpgradeSystem upgradeSystem;

    [Header("Layermasks")]
    public LayerMask interactionLayer;
    bool onCorrectLayer = false;

    enum interactionStage
    {
        primary,
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
        if(GameManager.instance.currentState == GameStates.END)
        {
            ResetAllAtEndStage();
            return;
        }
        if (currentStage == interactionStage.primary) { assignPrimaryTask(); }
        if(currentStage == interactionStage.findTarget) { assignTaskTarget(); }

        
        // 检测鼠标按下事件
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownTime = Time.time; // 记录按下时刻
            timerOn = true;
        }

        // 如果鼠标仍被按下，检查是否达到长按时长
        if (Input.GetMouseButton(0) && timerOn)
        {
            float duration = Time.time - mouseDownTime;
            if(duration >= 0.3f && duration<holdMouseDuration)
            {
                if(mouseUIPrefab.sprite == mouseUIs[0]) { changeMouseUI(3); }
                
                mouseUIPrefab.fillAmount = duration/holdMouseDuration;
            }
            if (duration >= holdMouseDuration)
            {
                mouseUIPrefab.fillAmount = 1;
                holdLongEnough = true; // 表示鼠标已经按下足够长的时间
            }
        }

        // 当鼠标抬起时重置状态，准备下一次检测
        if (Input.GetMouseButtonUp(0))
        {
            if(mouseUIPrefab.sprite == mouseUIs[3]) { changeMouseUI(0); }

            mouseUIPrefab.fillAmount = 1;
            timerOn = false; // 重置计时器标志
            // 重置长按标志，以便下一次检测
            holdLongEnough = false;
        }
        Debug.Log("current STage is "+currentStage);
    }
    void assignPrimaryTask()
    {
        if (GameManager.instance.currentState == GameStates.STORE)
        {
            //如果鼠标左键点击且点到了物体
            if(isMouseButtonDown() && DetectMouseButton() == "Left" && CheckClickTarget()!=null)
            {
                Debug.Log("click");
                GameObject clicked = CheckClickTarget().gameObject;
                if (clicked != null)
                {
                    ////如果点击的是bot脚本携带者
                    ////if clicked is a bot script carrier
                    if (clicked.GetComponent<Bot>() != null)
                    {
                        Bot clickedBot = clicked.GetComponent<Bot>();
                        //如果点击的是员工
                        //if clicked is an employee
                        if(clickedBot.tags == BotTags.employee)
                        {
                            clickedEmployeeForEmployee = clicked.GetComponent<NormalEmployee>();
                            //如果员工处于standBy状态
                            //if the employee is in standBy state
                            if (clickedEmployeeForEmployee.eStage == employeeStage.standBy)
                            {
                                
                                clickedEmployeeForEmployee.eStage = employeeStage.isSelected;
                                if(mouseUIs.Length > 0)
                                {
                                    changeMouseUI(1);
                                }
                                currentStage = interactionStage.findTarget;
                                assignedTask = assignTaskTargetForEmployee;
                            }
                            
                        }
                        
                    }
                    ////如果点击的是货架
                    ////if clicked is a shelf
                    if(clicked.GetComponent<ShelfScript>() != null)
                    {
                        Debug.Log("shelf clicked");
                        //单次点击目前没有功能
                    }
                }
            }
            //如果鼠标长按并释放且点到了shelf
            else if(holdLongEnough && CheckClickTarget().GetComponent<ShelfScript>() != null  && CheckClickTarget().GetComponent<ShelfScript>().loadAllowed)
            {
                GameObject clicked = CheckClickTarget().gameObject;
                Debug.Log("hold long enough");
                clickedShelfForShelf = clicked.GetComponent<ShelfScript>();
                clickedShelfForShelf.loadAllowed = false;
                currentStage = interactionStage.findTarget;
                assignedTask = assignTaskTargetForShelf;
                shadowShelf = Instantiate(shadowShelfPrefab, PrintMousePosition(), Quaternion.identity);
                shadowShelf.GetComponent<SpriteRenderer>().sprite = clickedShelfForShelf.GetComponent<SpriteRenderer>().sprite;
                clickedShelfForShelf.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                clickedShelfForShelf.gameObject.tag = "interactedShelf";
                shelfPlacementManager.SetCurrentShelfInstance(shadowShelf);
                if (mouseUIs.Length > 0)
                {
                    changeMouseUI(2);
                }
            }
        }
    }

    //该程序选择任务目标
    void assignTaskTarget()
    {

        //如果点击右键,则取消当前行为
        if(DetectMouseButton() == "Right")
        {
            Debug.Log("cancel");
            currentStage = interactionStage.primary;
            resetEverythingToDefault();
            return;
        }
        //如果点击的是左键，则为任务选择目标
        if(DetectMouseButton() == "Left")
        {
            if(assignedTask!=null)
            {
                assignedTask(CheckClickTarget());
                
            }
        }

        if (assignedTask == assignTaskTargetForShelf && shelfPlacementManager.GetCurrentShelfInstance()!=null)
        {
            
            shelfPlacementManager.SnapShelfToGridAvoidOverlap();
        }
        
    }

    [SerializeField] Button[] shelfStockButtons;
    //员工执行任务时所分配的目标
    void assignTaskTargetForEmployee(Collider2D clickedCol)
    {
        if (clickedCol == null) { return; }
        GameObject clicked = clickedCol.gameObject;
        //如果点击的是员工
        if (clicked.GetComponent<NormalEmployee>() != null)
        {
            //清除当前员工状态
            clickedEmployeeForEmployee.eStage = employeeStage.standBy;
            clickedEmployeeForEmployee.eAction = employeeAction.noAction;
            //切换至新点击员工并改变其状态
            clickedEmployeeForEmployee = clicked.GetComponent<NormalEmployee>();
            clickedEmployeeForEmployee.eStage = employeeStage.isSelected;
        }
        //如果点击的是货架 
        else if (!EventSystem.current.IsPointerOverGameObject() && clicked.GetComponent<ShelfScript>() != null && clicked.GetComponent<ShelfScript>().loadAllowed)
        {
            //如果当前没有货架被选中则选择当前货架
            if(clickedShelfForEmployee == null) { clickedShelfForEmployee = clicked.GetComponent<ShelfScript>(); }
            //如果点击的货架不是当前选中的货架，则关闭按钮后切换货架
            else if(clickedShelfForEmployee != clicked.GetComponent<ShelfScript>())
            {
                Debug.Log("switch shelf");
                foreach (Button button in clickedShelfForEmployee.GetComponentsInChildren<Button>(true))
                {
                    button.onClick.RemoveAllListeners();
                    button.gameObject.SetActive(false);
                }
                clickedShelfForEmployee = clicked.GetComponent<ShelfScript>();
            }
            
            
            shelfStockButtons = clickedShelfForEmployee.GetComponentsInChildren<Button>(true);

            //如果货架上没有物品则显示三个按钮
            if(clickedShelfForEmployee.loadAmount == 0)
            {
                for (int i = 0; i < shelfStockButtons.Length; i++)
                {
                    Button button = shelfStockButtons[i];
                    button.gameObject.SetActive(true);
                    button.onClick.RemoveAllListeners();
                    //根据不同货架类型显示不同的物品
                    shelvesType type = clickedShelfForEmployee.thisType;
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
                    
                    button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80 + i*80f, 110.4f);
                    goods product = clickedShelfForEmployee.itemsCanBeSold[i].GetItem();
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
                    button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 110.4f);
                    if (i == 0)
                    {
                        button.gameObject.SetActive(true);
                        foreach(Transform child in button.transform)
                        {
                            Image image = child.GetComponent<Image>();
                            image.sprite = ItemManager.Instance.shelfItemSprites[(int)clickedShelfForEmployee.sellingItem.GetItem()];
                        }
                        
                    }else if (i != 0)
                    {
                        button.gameObject.SetActive(false);
                    }
                    
                    button.onClick.AddListener(() => OnButtonClick(clickedShelfForEmployee.sellingItem.GetItem()));
                }
            }
            
        }
        
    }

    public void OnButtonClick(goods product)
    {
        clickedEmployeeForEmployee.eAction = employeeAction.reload;
        clickedEmployeeForEmployee.reloadShelf(clickedShelfForEmployee,product);
        
        resetSomethingToDefault();
    }

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
                employee.moveShelf(clickedShelfForShelf,shadowShelf);
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

    //重置某些状态至默认,选择完target后使用
    void resetSomethingToDefault()
    {
        if(assignedTask == assignTaskTargetForEmployee)
        {
            if(shelfStockButtons != null)
            {
                foreach(Button button in shelfStockButtons)
                {
                    button.onClick.RemoveAllListeners();
                    button.gameObject.SetActive(false);
                }
                shelfStockButtons = null;
            }
            if(clickedEmployeeForEmployee != null)
            {
                clickedEmployeeForEmployee = null;
                
            }
            if(clickedShelfForEmployee != null)
            {
                clickedShelfForEmployee = null;
            }
            changeMouseUI(0);
            currentStage = interactionStage.primary;
        }
        if (assignedTask == assignTaskTargetForShelf)
        {
            if (clickedShelfForShelf != null)
            {
                clickedShelfForShelf = null;
            }
            if (shadowShelf != null)
            {
                shadowShelf = null;
            }
            changeMouseUI(0);
            assignedTask = null;
            shelfPlacementManager.SetCurrentShelfInstance(null);
        }
    }

    //重置所有状态至默认,取消选项时使用
    void resetEverythingToDefault()
    {
        if(assignedTask == assignTaskTargetForEmployee)
        {
            if(clickedEmployeeForEmployee != null)
            {
                clickedEmployeeForEmployee.eStage = employeeStage.standBy;
                clickedEmployeeForEmployee.eAction = employeeAction.noAction;
                clickedEmployeeForEmployee = null;
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
            if(clickedShelfForEmployee != null)
            {
                clickedShelfForEmployee = null;
            }
            assignedTask = null;
        }

        if(assignedTask == assignTaskTargetForShelf)
        {
            if(clickedShelfForShelf != null)
            {
                clickedShelfForShelf.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                clickedShelfForShelf.loadAllowed = true;
                clickedShelfForShelf = null;
            }
            if(shadowShelf!=null)
            {
                Destroy(shadowShelf);
            }
            changeMouseUI(0);
            assignedTask = null;
            shelfPlacementManager.SetCurrentShelfInstance(null);
        }
    }

    private void ResetAllAtEndStage()
    {
        if (clickedEmployeeForEmployee != null)
        {
            clickedEmployeeForEmployee.eStage = employeeStage.standBy;
            clickedEmployeeForEmployee.eAction = employeeAction.noAction;
            clickedEmployeeForEmployee = null;
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
        if (clickedShelfForEmployee != null)
        {
            clickedShelfForEmployee = null;
        }
        if (clickedShelfForShelf != null)
        {
            clickedShelfForShelf.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            clickedShelfForShelf.loadAllowed = true;
            clickedShelfForShelf = null;
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


    bool isMouseButtonDown()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            return true;
        }
        return false;
    }

    string DetectMouseButton()
    {
        if (Input.GetMouseButtonDown(0))
        {
            timerOn = true;
            mouseDownTime = Time.time;
            return "Left";
        }
        else if (Input.GetMouseButtonUp(0))
        {
            timerOn = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            return "Right";
        }

        return null;
    }

    Collider2D CheckClickTarget()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider;
        }
        return null;
    }

    void MouseHoverOnInteractable()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(worldPoint.x, worldPoint.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero,Mathf.Infinity,interactionLayer);

        if(hit.collider!=null)
        {
            onCorrectLayer = true;
        }
        else
        {
            onCorrectLayer = false;
        }
        Debug.Log("on correct layer is "+onCorrectLayer);
        Debug.Log("on hit layer is "+LayerMask.LayerToName(hit.collider.gameObject.layer));
        Debug.DrawRay(worldPoint, Vector2.zero, Color.red);

    }

    Vector3 PrintMousePosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mousePosition.x, mousePosition.y,0);
    }

}
