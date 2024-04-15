using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockDisplayUI : MonoBehaviour
{
    TextMeshProUGUI currentTime;

    private void Awake()
    {
        currentTime = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        int hours = Mathf.FloorToInt(GameManager.instance.timer / 60);
        int minutes = Mathf.FloorToInt(GameManager.instance.timer - hours * 60);
        string message = GetMessageBasedOnTime(hours);
        currentTime.text = string.Format("{0:00}:{1:00} {2}", hours, minutes, message);
    }

    string GetMessageBasedOnTime(int hours)
    {
        return " ";
        if (hours < 9) return "Prepare your shop!";
        else if (hours < 18) return "The Shop is opened!";
        else return "The shop is closed...";
    }
}
