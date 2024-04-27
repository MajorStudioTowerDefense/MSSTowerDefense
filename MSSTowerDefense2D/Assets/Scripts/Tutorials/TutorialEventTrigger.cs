using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialEventTrigger : MonoBehaviour
{
    [Header("Entire Tutorials")]
    public GameObject tutorialPanel;
    public GameObject tutorialManager;

    [Header("Shelves Tutorial")]
    public GameObject appleShelfButton;

    [Header("Customer Tutorial")]
    public GameObject customerTutorialTriggerUIObject;
    private bool isCustomerTutorialTriggered = false;
    private GameObject firstCustomer;
    private GameObject firstDoor;
    public GameObject customerPurchaseUIObject;
    private bool isCustomerPurchaseTutorialTriggered = false;

    [Header("Employee and Restock Tutorial")]
    public GameObject employeeRestockTriggerUIObject;
    public GameObject emptyShelfMatcherObject;

    [Header("Day End Tutorial")]
    public GameObject dayEndTriggerUIObject;
    private bool isDayEndTutorialTriggered = false;

    void Update()
    {
        TriggerCustomerTutorial();

        TriggerEmployeeandRestockTutorial();

        TriggerCustomerPurchaseTutorial();

        TriggerDayEndTutorial();
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
                GameObject shelfObject;
                shelfObject = shelf.gameObject;

                emptyShelfMatcherObject.transform.position = shelfObject.transform.position;
                emptyShelfMatcherObject.transform.rotation = shelfObject.transform.rotation;
                emptyShelfMatcherObject.transform.localScale = shelfObject.transform.localScale;

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

    void TriggerCustomerPurchaseTutorial()
    {
        if (firstCustomer != null)
        {
            if (firstCustomer.GetComponent<NormalCustomer>().bot.isPurchasing && !isCustomerPurchaseTutorialTriggered)
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Camera.main.WorldToScreenPoint(customerTutorialTriggerUIObject.transform.position);

                ExecuteEvents.Execute(customerPurchaseUIObject, pointerEventData, ExecuteEvents.pointerClickHandler);

                isCustomerPurchaseTutorialTriggered = true;
            }
        }
    }

    void TriggerDayEndTutorial()
    {
        if (GameManager.instance.summaryPanel.activeInHierarchy)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Camera.main.WorldToScreenPoint(customerTutorialTriggerUIObject.transform.position);

            ExecuteEvents.Execute(dayEndTriggerUIObject, pointerEventData, ExecuteEvents.pointerClickHandler);

            isDayEndTutorialTriggered = true;
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
        OffTutorial();
        tutorialManager.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void TriggerAppleShelfTutorial()
    {
        appleShelfButton.GetComponent<Button>().interactable = true;
    }

    public void OnTutorial()
    {
        GameManager.instance.TutorialStarts();

        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (var button in allButtons)
        {
            if (button.transform.parent != null && button.transform.parent.gameObject == tutorialPanel)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    public void OffTutorial()
    {
        GameManager.instance.TutorialEnds();

        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (var button in allButtons)
        {
            button.interactable = true;
        }
    }

    public void DisableTutorialPanelButton()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (var button in allButtons)
        {
            if (button.transform.IsChildOf(tutorialManager.transform))
            {
                button.interactable = false;
            }
        }
    }

    public void EnableTutorialPanelButton()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (var button in allButtons)
        {
            if (button.transform.IsChildOf(tutorialManager.transform))
            {
                button.interactable = true;
            }
        }
    }
}