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

    private bool isAnimating = false; // ���ڸ��ٶ����Ƿ����ڲ���

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
        anim.SetBool("PlayAnimation", true); // ��������Animator����һ����Ϊ"PlayAnimation"�Ĳ�������
        yield return new WaitForSeconds(0.3f); // ���趯�����ȣ�����ʵ���������
        bar.fillAmount = 0;
        anim.SetBool("PlayAnimation", false);
        isAnimating = false;
    }

    bool barFilled()
    {
        return bar.fillAmount > 0.95f;
    }
}
