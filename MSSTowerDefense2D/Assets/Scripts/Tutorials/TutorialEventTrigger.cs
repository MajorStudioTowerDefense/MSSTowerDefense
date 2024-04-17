using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialEventTrigger : MonoBehaviour
{
    [Header("Entire Tutorials")]
    public GameObject tutorialPanel;
    public GameObject tutorialManager;

    [Header ("Customer Tutorial")]
    public GameObject customerTutorialTriggerUIObject;
    private bool isCustomerTutorialTriggered = false;
    private GameObject firstCustomer;
    private GameObject firstDoor;

    [Header("Employee and Restock Tutorial")]
    public GameObject employeeRestockTriggerUIObject;

    void Update()
    {
        TriggerCustomerTutorial();

        TriggerEmployeeandRestockTutorial();
    }

    void TriggerCustomerTutorial()
    {
        if (firstCustomer != null && firstDoor != null)
        {
            float distance = Vector3.Distance(firstCustomer.transform.position, firstDoor.transform.position);

            if (distance < 1 && !isCustomerTutorialTriggered)
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Camera.main.WorldToScreenPoint(customerTutorialTriggerUIObject.transform.position);

                ExecuteEvents.Execute(customerTutorialTriggerUIObject, pointerEventData, ExecuteEvents.pointerClickHandler);

                isCustomerTutorialTriggered = true;
            }
        }
        else
        {
            firstCustomer = GameObject.FindWithTag("Customer");
            firstDoor = GameObject.FindWithTag("Door");
        }
    }

    private bool isRestockTutorialTriggered = false; 

    void TriggerEmployeeandRestockTutorial()
    {
        if (isRestockTutorialTriggered) return;

        ShelfScript[] shelves = FindObjectsOfType<ShelfScript>();
        foreach (ShelfScript shelf in shelves)
        {
            if (shelf.loadAmount == 0)
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
                {
                    position = Camera.main.WorldToScreenPoint(employeeRestockTriggerUIObject.transform.position)
                };

                ExecuteEvents.Execute(employeeRestockTriggerUIObject, pointerEventData, ExecuteEvents.pointerClickHandler);

                isRestockTutorialTriggered = true;
                return;
            }
        }
    }

    public void ActivateTutorialPanel()
    {
        tutorialPanel.SetActive(true);
    }

    public void StartTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void SkipTutorial()
    {
        GameManager.instance.TutorialEnds();
        tutorialManager.SetActive(false);
        tutorialPanel.SetActive(false);
    }
}