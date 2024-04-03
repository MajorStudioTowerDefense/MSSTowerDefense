using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static int GetBaseSortingOrder(this Transform transform, float yOffset = 0)
    {
        return -((int)((transform.position.y + yOffset) * 100));
    }
}
