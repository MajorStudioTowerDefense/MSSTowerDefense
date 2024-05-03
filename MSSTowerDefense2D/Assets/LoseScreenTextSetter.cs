using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoseScreenTextSetter : MonoBehaviour
{
    public TextMeshProUGUI daysSurv, storeSize, debtOwed, empNums;
    string daysSurvStartTxt, storeSizeStartTxt, debtOwedStartTxt, empNumsStartTxt;

    private void Start()
    {
        daysSurvStartTxt = daysSurv.text;
        storeSizeStartTxt = storeSize.text;
        debtOwedStartTxt = debtOwed.text;
        empNumsStartTxt = empNums.text;
    }

    void OnEnable()
    {
        int num = GameObject.Find("DayManager").GetComponent<DayTrackerScript>().dayTracker;
        daysSurv.text = daysSurvStartTxt + " " + num.ToString();
        storeSize.text = storeSizeStartTxt + " " + (num-(num%7)).ToString();
        debtOwed.text = debtOwedStartTxt + " " + GameManager.instance.money.ToString();
        empNums.text = empNumsStartTxt + " ";
    }
}
