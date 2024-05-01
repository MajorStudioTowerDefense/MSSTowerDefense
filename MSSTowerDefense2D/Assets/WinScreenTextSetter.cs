using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinScreenTextSetter : MonoBehaviour
{
    public TextMeshProUGUI employeeNum, revenue, timePlayed;
    string employeeStartText, revenueStartText, timePlayedStartText;

    private void Start()
    {
        employeeStartText = employeeNum.text;
        revenueStartText = revenue.text;
        timePlayedStartText = timePlayed.text;
    }

    void OnEnable()
    {
        employeeNum.text = employeeStartText + " " + GameObject.Find("GameManager").GetComponent<UpgradeSystem>().addEmployeeUseList.Count.ToString();
        revenue.text = revenueStartText + " " + GameManager.instance.money;
        timePlayed.text = timePlayedStartText + " " + Time.realtimeSinceStartup.ToString();
    }

}
