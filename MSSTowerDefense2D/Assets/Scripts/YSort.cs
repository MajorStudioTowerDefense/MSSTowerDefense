using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSort : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y - spriteRenderer.bounds.extents.y) * 10f) * -1;
        spriteRenderer.sortingOrder = transform.GetBaseSortingOrder();
    }


}
