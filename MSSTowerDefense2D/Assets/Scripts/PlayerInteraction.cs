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
        if (currentStage == interactionStage.primary) { assignPrimaryTask(); }
        if(currentStage == interactionStage.findTarget) { assignTaskTarget(); }

        
        // �����갴���¼�
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownTime = Time.time; // ��¼����ʱ��
            timerOn = true;
        }

        // �������Ա����£�����Ƿ�ﵽ����ʱ��
        if (Input.GetMouseButton(0) && timerOn)
        {
            float duration = Time.time - mouseDownTime;
            if (duration >= holdMouseDuration)
            {
                holdLongEnough = true; // ��ʾ����Ѿ������㹻����ʱ��
            }
        }

        // �����̧��ʱ����״̬��׼����һ�μ��
        if (Input.GetMouseButtonUp(0))
        {
            timerOn = false; // ���ü�ʱ����־
            // ���ó�����־���Ա���һ�μ��
            holdLongEnough = false;
        }
    }
    void assignPrimaryTask()
    {
        if (GameManager.instance.currentState == GameStates.STORE)
        {
            //�������������ҵ㵽������
            if(isMouseButtonDown() && DetectMouseButton() == "Left" && CheckClickTarget()!=null)
            {

                GameObject clicked = CheckClickTarget().gameObject;
                if (clicked != null)
                {
                    ////����������bot�ű�Я����
                    ////if clicked is a bot script carrier
                    if (clicked.GetComponent<Bot>() != null)
                    {
                        Bot clickedBot = clicked.GetComponent<Bot>();
                        //����������Ա��
                        //if clicked is an employee
                        if(clickedBot.tags == BotTags.employee)
                        {
                            clickedEmployeeForEmployee = clicked.GetComponent<NormalEmployee>();
                            //���Ա������standBy״̬
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
                    ////���������ǻ���
                    ////if clicked is a shelf
                    if(clicked.GetComponent<ShelfScript>() != null)
                    {
                        Debug.Log("shelf clicked");
                        //���ε��Ŀǰû�й���
                    }
                }
            }
            //�����곤�����ͷ��ҵ㵽��shelf
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
                shelfPlacementManager.SetCurrentShelfInstance(shadowShelf);
                if (mouseUIs.Length > 0)
                {
                    changeMouseUI(2);
                }
            }
        }
    }

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
            clickedEmployeeForEmployee.eStage = employeeStage.standBy;
            clickedEmployeeForEmployee.eAction = employeeAction.noAction;
            //�л����µ��Ա�����ı���״̬
            clickedEmployeeForEmployee = clicked.GetComponent<NormalEmployee>();
            clickedEmployeeForEmployee.eStage = employeeStage.isSelected;
        }
        //���������ǻ���
        else if (clicked.GetComponent<ShelfScript>() != null && clicked.GetComponent<ShelfScript>().loadAllowed)
        {
            //�����ǰû�л��ܱ�ѡ����ѡ��ǰ����
            if(clickedShelfForEmployee == null) { clickedShelfForEmployee = clicked.GetComponent<ShelfScript>(); }
            //�������Ļ��ܲ��ǵ�ǰѡ�еĻ��ܣ���رհ�ť���л�����
            else if(clickedShelfForEmployee != clicked.GetComponent<ShelfScript>())
            {
                foreach (Button button in clickedShelfForEmployee.GetComponentsInChildren<Button>(true))
                {
                    button.onClick.RemoveAllListeners();
                    button.gameObject.SetActive(false);
                }
                clickedShelfForEmployee = clicked.GetComponent<ShelfScript>();
            }
            
            
            shelfStockButtons = clickedShelfForEmployee.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < shelfStockButtons.Length; i++)
            {
                Button button = shelfStockButtons[i];
                button.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();

                int index = i; // ����ѭ�������ĵ�ǰֵ
                button.onClick.AddListener(() => OnButtonClick(index));
            }
        }
        
    }

    public void OnButtonClick(int index)
    {
        Debug.Log("Button clicked: " + index);
        clickedEmployeeForEmployee.eAction = employeeAction.reload;
        Debug.Log("clickedEmployee is" + clickedEmployeeForEmployee.gameObject.name);
        clickedEmployeeForEmployee.reloadShelf(clickedShelfForEmployee,index);
        
        resetSomethingToDefault();
    }

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
                employee.moveShelf(clickedShelfForShelf,shadowShelf);
                currentStage = interactionStage.primary;
                resetSomethingToDefault();
                break;
            }
        }

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

    //��������״̬��Ĭ��,ȡ��ѡ��ʱʹ��
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

    Vector3 PrintMousePosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mousePosition.x, mousePosition.y,0);
    }

}
