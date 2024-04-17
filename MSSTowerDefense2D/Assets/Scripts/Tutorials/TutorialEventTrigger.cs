using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialEventTrigger : MonoBehaviour
{
    [Header ("Customer Tutorial")]
    public GameObject customerTutorialTriggerUIObject;
    private bool isCustomerTutorialTriggered = false;
    private GameObject firstCustomer;
    private GameObject firstDoor;

    [Header("Customer Tutorial")]
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

    void TriggerEmployeeandRestockTutorial()
    {

    }
}