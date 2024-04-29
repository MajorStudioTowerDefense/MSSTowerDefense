using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DaySpeed : MonoBehaviour
{

    private TextMeshProUGUI daySpeed;

    // Start is called before the first frame update
    void Start()
    {
        daySpeed = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        daySpeed.text = "x" + Time.timeScale.ToString();
    }
}
