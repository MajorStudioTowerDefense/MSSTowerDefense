using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCustomer : Bot
{
    [SerializeField] private GameObject desireVFX;

    private void Update()
    {
        desireVFX.SetActive(isPurchasing);
    }
}
