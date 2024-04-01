using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    //Mouse
    private float mouseDownTime;
    private bool timerOn = false;
    public float holdMouseDuration = 1f;

    delegate void AssignTaskDelegate(GameObject clickedObject);
    AssignTaskDelegate assignedTask;

    interactionStage currentStage = interactionStage.primary;

    [Header("UI")]
    public Sprite[] mouseUIs;
    public GameObject mouseUIPrefab;

    [Header("clickedGameObject")]
    [SerializeField] private NormalEmployee clickedEmployee;
    [SerializeField] private ShelfScript clickedShelf;

    enum interactionStage
    {
        primary,
        findTarget
    }

    private void Update()
    {
        if(currentStage == interactionStage.primary) { assignPrimaryTask(); }
        if(currentStage == interactionStage.findTarget) { assignTaskTarget(); }
    }
    void assignPrimaryTask()
    {
        if (GameManager.instance.currentState == GameStates.STORE)
        {
            if(isMouseButtonDown() && DetectMouseButton() == "Left" && CheckClickTarget()!=null)
            {

                GameObject clicked = CheckClickTarget().gameObject;
                if (clicked != null)
                {
                    //如果点击的是bot脚本携带者
                    //if clicked is a bot script carrier
                    if (clicked.GetComponent<Bot>() != null)
                    {
                        Bot clickedBot = clicked.GetComponent<Bot>();
                        //如果点击的是员工
                        //if clicked is an employee
                        if(clickedBot.tags == BotTags.employee)
                        {
                            clickedEmployee = clicked.GetComponent<NormalEmployee>();
                            //如果员工处于standBy状态
                            //if the employee is in standBy state
                            if (clickedEmployee.eStage == employeeStage.standBy)
                            {
                                
                                clickedEmployee.eStage = employeeStage.isSelected;
                                if(mouseUIs.Length > 0)
                                {
                                    changeMouseUI(1);
                                }
                                currentStage = interactionStage.findTarget;
                                assignedTask = assignTaskTargetForEmployee;
                            }
                            
                        }
                        
                    }
                    //如果点击的是货架
                    //if clicked is a shelf
                    if(clicked.GetComponent<ShelfScript>() != null)
                    {
                        ShelfScript clickedShelf = clicked.GetComponent<ShelfScript>();
                    }
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
        if(DetectMouseButton() == "Left" && CheckClickTarget()!=null)
        {
            GameObject clicked = CheckClickTarget().gameObject;
            if(clicked != null && assignedTask!=null)
            {
                assignedTask(clicked);
                
            }
        }
    }

    [SerializeField] Button[] shelfStockButtons;
    //员工执行任务时所分配的目标
    void assignTaskTargetForEmployee(GameObject clicked)
    {
        //如果点击的是员工
        if (clicked.GetComponent<NormalEmployee>() != null)
        {
            //清除当前员工状态
            clickedEmployee.eStage = employeeStage.standBy;
            clickedEmployee.eAction = employeeAction.noAction;
            //切换至新点击员工并改变其状态
            clickedEmployee = clicked.GetComponent<NormalEmployee>();
            clickedEmployee.eStage = employeeStage.isSelected;
        }
        //如果点击的是货架
        else if (clicked.GetComponent<ShelfScript>() != null)
        {
            //如果当前没有货架被选中则选择当前货架
            if(clickedShelf == null) { clickedShelf = clicked.GetComponent<ShelfScript>(); }
            //如果点击的货架不是当前选中的货架，则关闭按钮后切换货架
            else if(clickedShelf != clicked.GetComponent<ShelfScript>())
            {
                foreach (Button button in clickedShelf.GetComponentsInChildren<Button>(true))
                {
                    button.onClick.RemoveAllListeners();
                    button.gameObject.SetActive(false);
                }
                clickedShelf = clicked.GetComponent<ShelfScript>();
            }
            
            
            shelfStockButtons = clickedShelf.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < shelfStockButtons.Length; i++)
            {
                Button button = shelfStockButtons[i];
                button.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();

                int index = i; // 捕获循环变量的当前值
                button.onClick.AddListener(() => OnButtonClick(index));
            }
        }
        
    }

    public void OnButtonClick(int index)
    {
        Debug.Log("Button clicked: " + index);
        clickedEmployee.eAction = employeeAction.reload;
        Debug.Log("clickedEmployee is" + clickedEmployee.gameObject.name);
        clickedEmployee.reloadShelf(clickedShelf,index);
        
        resetSomethingToDefault();
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
            if(clickedEmployee != null)
            {
                clickedEmployee = null;
                
            }
            if(clickedShelf != null)
            {
                clickedShelf = null;
            }
            changeMouseUI(0);
            currentStage = interactionStage.primary;
        }
    }

    //重置所有状态至默认,取消选项时使用
    void resetEverythingToDefault()
    {
        if(assignedTask == assignTaskTargetForEmployee)
        {
            if(clickedEmployee != null)
            {
                clickedEmployee.eStage = employeeStage.standBy;
                clickedEmployee.eAction = employeeAction.noAction;
                clickedEmployee = null;
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
            if(clickedShelf != null)
            {
                clickedShelf = null;
            }
            assignedTask = null;
        }
    }
    void changeMouseUI(int index)
    {
        
    }

    bool isMouseButtonDown()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            return true;
        }
        return false;
    }

    bool isMouseButtonUp()
    {
        if(Input.GetMouseButtonUp(0))
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

    Vector2 PrintMousePosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mousePosition.x, mousePosition.y);
    }

    float CalculateMouseDownDuration()
    {
        if (!timerOn) return 0f;
        return Time.time - mouseDownTime;
    }

}
