using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class NoAvailableEmployee : MonoBehaviour
{

    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 40f * Time.deltaTime, this.transform.position.z);
        if (timer > 1)
        {
            this.gameObject.SetActive(false);
            timer = 0;
        }
    }
}
