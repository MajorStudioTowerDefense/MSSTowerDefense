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

    delegate void AssignTaskDelegate(Collider2D clickedCol);
    AssignTaskDelegate assignedTask;

    interactionStage currentStage = interactionStage.primary;

    [Header("UI")]
    public Sprite[] mouseUIs;
    public Image mouseUIPrefab;

    [Header("clickedGameObject")]
    [SerializeField] private NormalEmployee EmployeeChosenInPrimaryTask;
    [SerializeField] private ShelfScript clickedShelfForEmployee;
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
        //if(currentStage == interactionStage.findTarget) { assignTaskTarget(); }

        
        
        Debug.Log("current STage is "+currentStage);
    }
    
    void assignPrimaryTask()
    {
        if (GameManager.instance.currentState == GameStates.STORE)
        {
            //�������������ҵ㵽������
            //If mose left click and clicked on interacted object
            if(Input.GetMouseButtonDown(0) && onCorrectLayer)
            {
                GameObject clicked = hoverGameObject;
                if (clicked != null)
                {
                    //����������Ա��
                    //if clicked is an employee
                    if(clicked.tag == "Employee")
                    {
                        EmployeeChosenInPrimaryTask = clicked.GetComponent<NormalEmployee>();
                        //���Ա������standBy״̬
                        //if the employee is in standBy state
                        if (EmployeeChosenInPrimaryTask.eStage == employeeStage.standBy)
                        {
                                
                            EmployeeChosenInPrimaryTask.eStage = employeeStage.isSelected;
                            if(mouseUIs.Length > 0)
                            {
                                changeMouseUI(1);
                            }
                            currentStage = interactionStage.findTarget;
                            //assignedTask = assignTaskTargetForEmployee;
                        }
                            
                    }
                    ////���������ǻ���
                    ////if clicked is a shelf
                    if (clicked.tag == "interactedShelf")
                    {
                        Debug.Log("shelf clicked");
                        //���ε��Ŀǰû�й���
                    }
                }
            }
            //�����곤�����ͷ��ҵ㵽��shelf
            else if(holdLongEnough && longHoldGameObject!=null  && longHoldGameObject.tag == "interactedShelf" && longHoldGameObject.GetComponent<ShelfScript>().loadAllowed)
            {
                GameObject clicked = longHoldGameObject;
                Debug.Log("hold long enough");
                ShelfChosenInPrimaryTask = clicked.GetComponent<ShelfScript>();
                if(ShelfChosenInPrimaryTask.loadAmount == ShelfChosenInPrimaryTask.loadAmountMax)
                {
                    Debug.Log("shelf full");
                    return;
                }/**
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
                }**/
            }
        }
    }

    /**
    //�ó���ѡ������Ŀ��
    void assignTaskTarget()
    {

        //�������Ҽ�,��ȡ����ǰ��Ϊ
        if(DetectMouseButton() == "Right")
        {
            Debug.Log("cancel");
            currentStage = interactionStage.primary;
            resetEverythingToDefault();
            return;
        }
        //�����������������Ϊ����ѡ��Ŀ��
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

    /**
    [SerializeField] Button[] shelfStockButtons;
    //Ա��ִ������ʱ�������Ŀ��
    void assignTaskTargetForEmployee(Collider2D clickedCol)
    {
        if (clickedCol == null) { return; }
        GameObject clicked = clickedCol.gameObject;
        //����������Ա��
        if (clicked.GetComponent<NormalEmployee>() != null)
        {
            //�����ǰԱ��״̬
            EmployeeChosenInPrimaryTask.eStage = employeeStage.standBy;
            EmployeeChosenInPrimaryTask.eAction = employeeAction.noAction;
            //�л����µ��Ա�����ı���״̬
            EmployeeChosenInPrimaryTask = clicked.GetComponent<NormalEmployee>();
            EmployeeChosenInPrimaryTask.eStage = employeeStage.isSelected;
        }
        //���������ǻ��� 
        else if (!EventSystem.current.IsPointerOverGameObject() && clicked.GetComponent<ShelfScript>() != null && clicked.GetComponent<ShelfScript>().loadAllowed)
        {
            //�����ǰû�л��ܱ�ѡ����ѡ��ǰ����
            if(clickedShelfForEmployee == null) { clickedShelfForEmployee = clicked.GetComponent<ShelfScript>(); }
            //�������Ļ��ܲ��ǵ�ǰѡ�еĻ��ܣ���رհ�ť���л�����
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

            //���������û����Ʒ����ʾ������ť
            if(clickedShelfForEmployee.loadAmount == 0)
            {
                for (int i = 0; i < shelfStockButtons.Length; i++)
                {
                    Button button = shelfStockButtons[i];
                    button.gameObject.SetActive(true);
                    button.onClick.RemoveAllListeners();
                    //���ݲ�ͬ����������ʾ��ͬ����Ʒ
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
            //�������������Ʒ��ֻ��ʾһ����ť����ȡ��ǰ������Ʒ
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
        EmployeeChosenInPrimaryTask.eAction = employeeAction.reload;
        EmployeeChosenInPrimaryTask.reloadShelf(clickedShelfForEmployee,product);
        
        resetSomethingToDefault();
    }

    public TextMeshProUGUI warningText;
    //���ܷ���ʱ�������Ŀ��
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

    //����ĳЩ״̬��Ĭ��,ѡ����target��ʹ��
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
            if(EmployeeChosenInPrimaryTask != null)
            {
                EmployeeChosenInPrimaryTask = null;
                
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
        }
    }

    //��������״̬��Ĭ��,ȡ��ѡ��ʱʹ��
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
            if(clickedShelfForEmployee != null)
            {
                clickedShelfForEmployee = null;
            }
            assignedTask = null;
        }

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
        }
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
        if (clickedShelfForEmployee != null)
        {
            clickedShelfForEmployee = null;
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
    

    **/

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

    #endregion

    Vector3 PrintMousePosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mousePosition.x, mousePosition.y,0);
    }

}
