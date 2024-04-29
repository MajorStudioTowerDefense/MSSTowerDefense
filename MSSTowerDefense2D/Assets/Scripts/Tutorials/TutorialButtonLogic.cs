using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialButtonLogic : MonoBehaviour, IPointerClickHandler
{
    private Button button;

    public string buttonType;

    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("ButtonTriggerOnClick script requires a Button component on the same GameObject.");
        }
    }

    void Update()
    {
        if (buttonType == "Advance")
        {
            if (Input.GetMouseButtonDown(0))
            {
                TriggerButtonClick();
            }
        }
        else if (buttonType == "Back")
        {
            if (Input.GetMouseButtonDown(1))
            {
                TriggerButtonClick();
            }
        }
    }

    private void TriggerButtonClick()
    {
        if (button != null && button.interactable)
        {
            button.onClick.Invoke(); 
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
