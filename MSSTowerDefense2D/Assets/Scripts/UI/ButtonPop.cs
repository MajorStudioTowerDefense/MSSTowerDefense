using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonPop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    Vector2 orgSize;

    public void Start()
    {
        orgSize = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector2(transform.localScale.x * 1.1f, transform.localScale.y * 1.1f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector2(transform.localScale.x / 1.1f, transform.localScale.y / 1.1f);

    }

    public void ToMainGame()
    {
        SceneManager.LoadScene("MainScene");

    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("NewStartScene");

    }

    public void NewGame()
    {

    }

    private void OnDisable()
    {
        transform.localScale = orgSize;
    }
}
