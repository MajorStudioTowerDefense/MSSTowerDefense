using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCustomer : Bot
{
    [SerializeField] private GameObject desireVFX;

    protected override void Update()
    {
        base.Update();
        desireVFX.SetActive(isPurchasing);
    }
}
