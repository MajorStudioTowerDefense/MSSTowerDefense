using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    private TextMeshProUGUI currency;
    void Start()
    {
        currency = GetComponent<TextMeshProUGUI>();
        DisplayCurrentCurrency();
    }

    public void DisplayCurrentCurrency()
    {
        currency.text = GameManager.instance.money.ToString();
        Debug.Log(GameManager.instance.money);
    }
}
