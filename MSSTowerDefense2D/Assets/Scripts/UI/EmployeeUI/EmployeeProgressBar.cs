using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeProgressBar : MonoBehaviour
{
    [SerializeField] Image bar;
    [SerializeField] Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        bar = GetComponent<Image>();
    }

    private bool isAnimating = false; // 用于跟踪动画是否正在播放

    // Update is called once per frame
    void Update()
    {
        if (barFilled() && !isAnimating)
        {
            Debug.Log("filled");
            StartCoroutine(BarFilledAnim());
        }
    }

    IEnumerator BarFilledAnim()
    {
        isAnimating = true;
        anim.SetBool("PlayAnimation", true); // 假设你在Animator中有一个名为"PlayAnimation"的布尔参数
        yield return new WaitForSeconds(0.3f); // 假设动画长度，根据实际情况调整
        bar.fillAmount = 0;
        anim.SetBool("PlayAnimation", false);
        isAnimating = false;
    }

    bool barFilled()
    {
        return bar.fillAmount > 0.95f;
    }
}
