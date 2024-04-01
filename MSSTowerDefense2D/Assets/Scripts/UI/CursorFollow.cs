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
        // 将鼠标位置从屏幕空间转换为世界空间
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 由于我们的Canvas是Screen Space - Overlay，我们不需要转换为世界空间，而是直接使用屏幕空间的坐标
        // 但需要考虑Canvas的缩放因子
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, mouseScreenPosition, null, out Vector2 localPoint);

        // 设置UI元素的位置
        transform.localPosition = localPoint + offset;
    }
}
