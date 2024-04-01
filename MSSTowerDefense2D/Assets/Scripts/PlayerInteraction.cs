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
                    //����������bot�ű�Я����
                    //if clicked is a bot script carrier
                    if (clicked.GetComponent<Bot>() != null)
                    {
                        Bot clickedBot = clicked.GetComponent<Bot>();
                        //����������Ա��
                        //if clicked is an employee
                        if(clickedBot.tags == BotTags.employee)
                        {
                            clickedEmployee = clicked.GetComponent<NormalEmployee>();
                            //���Ա������standBy״̬
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
                    //���������ǻ���
                    //if clicked is a shelf
                    if(clicked.GetComponent<ShelfScript>() != null)
                    {
                        ShelfScript clickedShelf = clicked.GetComponent<ShelfScript>();
                    }
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
    //Ա��ִ������ʱ�������Ŀ��
    void assignTaskTargetForEmployee(GameObject clicked)
    {
        //����������Ա��
        if (clicked.GetComponent<NormalEmployee>() != null)
        {
            //�����ǰԱ��״̬
            clickedEmployee.eStage = employeeStage.standBy;
            clickedEmployee.eAction = employeeAction.noAction;
            //�л����µ��Ա�����ı���״̬
            clickedEmployee = clicked.GetComponent<NormalEmployee>();
            clickedEmployee.eStage = employeeStage.isSelected;
        }
        //���������ǻ���
        else if (clicked.GetComponent<ShelfScript>() != null)
        {
            //�����ǰû�л��ܱ�ѡ����ѡ��ǰ����
            if(clickedShelf == null) { clickedShelf = clicked.GetComponent<ShelfScript>(); }
            //�������Ļ��ܲ��ǵ�ǰѡ�еĻ��ܣ���رհ�ť���л�����
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

                int index = i; // ����ѭ�������ĵ�ǰֵ
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

    //��������״̬��Ĭ��,ȡ��ѡ��ʱʹ��
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
