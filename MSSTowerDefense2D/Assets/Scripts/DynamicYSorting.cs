using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicYSorting : MonoBehaviour
{
    private int _baseSortingOrder;
    private float _ySortingOffset;
    [SerializeField] private SortableSprite[] _sortableSprites;
    [SerializeField] private Transform _sortOffsetMarker;

    private void Start()
    {
        _ySortingOffset = _sortOffsetMarker.localPosition.y;
    }
    // Update is called once per frame
    void Update()
    {
        _baseSortingOrder = transform.GetBaseSortingOrder(_ySortingOffset);
        foreach(var sortableSprite in _sortableSprites)
        {
            sortableSprite.spriteRenderer.sortingOrder = _baseSortingOrder + sortableSprite.relativeOrder;
        }
    }

    [System.Serializable]
    public struct SortableSprite
    {
        public SpriteRenderer spriteRenderer;
        public int relativeOrder;
    }
}
