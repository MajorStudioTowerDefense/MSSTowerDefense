using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    public Vector2 offset = new Vector2(0, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �����λ�ô���Ļ�ռ�ת��Ϊ����ռ�
        Vector3 mouseScreenPosition = Input.mousePosition;

        // �������ǵ�Canvas��Screen Space - Overlay�����ǲ���Ҫת��Ϊ����ռ䣬����ֱ��ʹ����Ļ�ռ������
        // ����Ҫ����Canvas����������
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, mouseScreenPosition, null, out Vector2 localPoint);

        // ����UIԪ�ص�λ��
        transform.localPosition = localPoint + offset;
    }
}
