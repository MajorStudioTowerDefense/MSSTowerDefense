using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCustomer : Bot
{
    [SerializeField] private GameObject desireVFX;

    public Transform ShopExit;
    public AudioClip WalkingHeavy;
    public AudioClip Walking;

    public void Start()
    {
        AudioManager.instance.PlaySound(WalkingHeavy);
        AudioManager.instance.PlaySound(Walking);

    }
    
    public override void init()
    {
        base.init();
        ShopExit = GameObject.FindGameObjectWithTag("Exit").transform;
        
    }
    protected override void Update()
    {
        base.Update();
        desireVFX.SetActive(isPurchasing);
        forceToLeaveStoreInEndStage();

    }

    public virtual void forceToLeaveStoreInEndStage()
    {
        if(GameManager.instance.currentState==GameStates.END)
        {
            destinationSetter.target = ShopExit;
        }
    }

}
