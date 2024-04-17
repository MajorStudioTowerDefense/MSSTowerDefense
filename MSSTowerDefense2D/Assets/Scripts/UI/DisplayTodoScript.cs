using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class DisplayTodoScript : MonoBehaviour
{
    private Image first;
    private Image second;
    private Image third;
    private Image fourth;
    private Image fifth;

    public Sprite cart;
    public Sprite move;
    public Sprite stock;
    public Sprite stand;

    private Image[] test;

    public NormalEmployee employee;
    private SkeletonCardStackingUIScript script;

    private void Start()
    {
        script = GameObject.Find("Employee IDs").GetComponent<SkeletonCardStackingUIScript>();
        employee = script.lastSkeleton.GetComponent<NormalEmployee>();
        test = GetComponentsInChildren<Image>();
        first = transform.GetChild(0).GetComponent<Image>();
        second = transform.GetChild(1).GetComponent<Image>();
        third = transform.GetChild(2).GetComponent<Image>();
        fourth = transform.GetChild(3).GetComponent<Image>();
        fifth = transform.GetChild(4).GetComponent<Image>();
    }

    private void Update()
    {
        if (employee.eStage == employeeStage.standBy)
        {
            int i = 0;
            foreach (Image image in test)
            {
                if (i == 0)
                {
                    i++;
                    image.sprite = stand;
                }
                else
                {
                    image.sprite = null;
                }
            }
            first.sprite = stand;
            second.sprite = null;
            third.sprite = null;
            fourth.sprite = null;
            fifth.sprite = null;
        }
        else if (employee.eStage == employeeStage.isSelected)
        {
            first.sprite = cart;
            second.sprite = move;
            third.sprite = stock;
            fourth.sprite = move;
            fifth.sprite = stand;
        }
        else if (employee.eStage == employeeStage.running)
        {
            first.sprite = move;
            second.sprite = stock;
            third.sprite = move;
            fourth.sprite = stand;
            fifth.sprite = null;
        }
        else if (employee.eStage == employeeStage.finishing)
        {
            first.sprite = stock;
            second.sprite = move;
            third.sprite = stand;
            fourth.sprite = null;
            fifth.sprite = null;
        }
        else if (employee.eStage == employeeStage.backToStandBy)
        {
            first.sprite = move;
            second.sprite = stand;
            third.sprite = null;
            fourth.sprite = null;
            fifth.sprite = null;
        }

    }

}



