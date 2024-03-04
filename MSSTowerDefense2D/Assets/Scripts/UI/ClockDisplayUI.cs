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
        currentTime.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }
}
