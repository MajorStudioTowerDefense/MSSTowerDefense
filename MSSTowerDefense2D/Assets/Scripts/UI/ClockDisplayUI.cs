using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI currentTime;
    public int hours;
    public int minutes;

    private void Awake()
    {
        currentTime = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        hours = Mathf.FloorToInt(GameManager.instance.timer / 60);
        minutes = Mathf.FloorToInt(GameManager.instance.timer - hours * 60);
        string message = GetMessageBasedOnTime(hours);
        currentTime.text = string.Format("{0:00}:{1:00} {2}", hours, minutes, message);
    }

    string GetMessageBasedOnTime(int hours)
    {
        return "";
        //if (hours < 9) return "Prepare your shop!";
        //else if (hours < 18) return "The Shop is opened!";
        //else return "The shop is closed...";
    }
}
