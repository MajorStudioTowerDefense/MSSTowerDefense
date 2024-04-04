using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTodoScript : MonoBehaviour
{
    public GameObject first;
    public GameObject second;
    public GameObject third;
    public GameObject fourth;
    public GameObject fifth;

    public Sprite cart;
    public Sprite move;
    public Sprite stock;
    public Sprite stand;

    public NormalEmployee employee;
    private bool doIt = true;

    private void Update()
    {
        if (doIt && employee.eStage == employeeStage.running)
        {
            Reset();
            doIt = false;
        }

        if (employee.eStage == employeeStage.standBy)
        {
            first.GetComponent<Image>().sprite = null;
            second.GetComponent<Image>().sprite = null;
            third.GetComponent<Image>().sprite = null;
            fourth.GetComponent<Image>().sprite = null;
            fifth.GetComponent<Image>().sprite = null;
        }
    }

    private void Loop()
    {
        if (employee.eStage == employeeStage.finishing)
        {
            first.GetComponent<Image>().sprite = stock;
            second.GetComponent<Image>().sprite = move;
            third.GetComponent<Image>().sprite = stand;
            fourth.GetComponent<Image>().sprite = null;
            Loop();
        }
        else if (employee.eStage == employeeStage.backToStandBy)
        {
            first.GetComponent<Image>().sprite = move;
            second.GetComponent<Image>().sprite = stand;
            third.GetComponent<Image>().sprite = null;
            Loop();
        }
        else
        {
            first.GetComponent<Image>().sprite = stand;
            second.GetComponent<Image>().sprite = null;
            StartCoroutine(EndLoop());
        }
    
    }

    private void Reset()
    {
        first.GetComponent<Image>().sprite = cart;
        second.GetComponent<Image>().sprite = move;
        third.GetComponent<Image>().sprite = stock;
        fourth.GetComponent<Image>().sprite = move;
        fifth.GetComponent<Image>().sprite = stand;
        StartCoroutine(GetGoing());
    }

    IEnumerator GetGoing()
    {
        yield return new WaitForSeconds(0.6f);
        first.GetComponent<Image>().sprite = move;
        second.GetComponent<Image>().sprite = stock;
        third.GetComponent<Image>().sprite = move;
        fourth.GetComponent<Image>().sprite = stand;
        fifth.GetComponent<Image>().sprite = null;
        Loop();
        //wait for update

    }

    IEnumerator EndLoop()
    {
        doIt = true;
        yield return new WaitForSeconds(0.8f);
        first.GetComponent<Image>().sprite = null;

    }
}



