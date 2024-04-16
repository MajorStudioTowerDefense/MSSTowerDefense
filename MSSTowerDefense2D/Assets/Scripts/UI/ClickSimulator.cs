using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSimulator : MonoBehaviour
{
    public GameObject uiObject;

    void TriggerClick()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Camera.main.WorldToScreenPoint(uiObject.transform.position);

        ExecuteEvents.Execute(uiObject, pointerEventData, ExecuteEvents.pointerClickHandler);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            print("Oh clicked!");
            TriggerClick();
        }
    }
}