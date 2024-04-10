using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreenUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Stats;
    [SerializeField] TextMeshProUGUI Exclamation;

    [SerializeField] GameObject endScreenUI;//figure out how to set this true and make everything else start from it

    [SerializeField] GameObject winScreenUI;
    [SerializeField] GameObject loseScreenUI;

    [SerializeField] float employeeWage;
    [SerializeField] float rentf;

    private float moneyf;
    private int employees;
    private float total;

    private string moneyStr;
    private string employeePay;
    private string rentStr;
    private string totalStr;

    void Start()
    {
        moneyf = GameManager.instance.money;
        moneyStr = moneyf.ToString();
        employees = GameObject.FindGameObjectsWithTag("Employee").Length;
        total = moneyf -(employees*employeeWage) -rentf;

        employeePay = (employees * employeeWage).ToString();
        rentStr = rentf.ToString();
        totalStr = total.ToString();
        
    }

    void Update()
    {
        if (moneyf > 0)
        {
            DisplayWinThings();
        }
        else
        {
            DisplayLoseThings();
        }

        Stats.text = moneyStr + " Revenue /n -" + employeePay + " Employee Wages /n -" + rentStr + " Rent /n = $ " + totalStr;
    }

    public void DisplayWinThings()
    {
        Exclamation.text = "Level Cleared!";
        winScreenUI.SetActive(true);
    }

    public void DisplayLoseThings() {
        Exclamation.text = "Level Failed!";
        loseScreenUI.SetActive(true);
    }

}
