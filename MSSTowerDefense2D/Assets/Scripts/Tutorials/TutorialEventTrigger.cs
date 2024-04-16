using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialEventTrigger : MonoBehaviour
{
    public GameObject customerTutorialTriggerUIObject;

    private bool isCustomerTutorialTriggered = false;

    private GameObject firstCustomer;
    private GameObject firstDoor;

    void Update()
    {
        if (firstCustomer != null && firstDoor != null)
        {
            float distance = Vector3.Distance(firstCustomer.transform.position, firstDoor.transform.position);

            if (distance < 1 && !isCustomerTutorialTriggered)
            {
                TriggerCustomerTutorial();
                isCustomerTutorialTriggered = true;
            }
        }
        else
        {
            firstCustomer = GameObject.FindWithTag("Customer");
            firstDoor = GameObject.FindWithTag("Door");
        }
    }

    void TriggerCustomerTutorial()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Camera.main.WorldToScreenPoint(customerTutorialTriggerUIObject.transform.position);

        ExecuteEvents.Execute(customerTutorialTriggerUIObject, pointerEventData, ExecuteEvents.pointerClickHandler);
    }
}