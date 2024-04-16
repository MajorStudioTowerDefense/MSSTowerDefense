using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// This component is used to identify the mask created by module
    /// </summary>
    [ExecuteInEditMode]
    public class HSVUIMask : MonoBehaviour
    {
        public Image mask;
        public Rect maskRect;
        public bool oriActiveState;
        public HSVTarget target;
        public float animationProgressTimer;

        private Sprite originalSprite;
        private Color originMaskColor;
        private Vector2 originMaskSize;

        private void Awake()
        {
            AwakeOverride();
        }

        public virtual void AwakeOverride()
        {
            if (mask == null)
            {
                mask = GetComponentInChildren<Image>();
                if (mask == null)
                {
                    mask = gameObject.AddComponent<Image>();
                }

                originalSprite = mask.sprite;
                originMaskColor = mask.color;
                originMaskSize = mask.rectTransform.sizeDelta;
            }
        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {
                
        }

        public virtual void OnSpawn()
        {
            animationProgressTimer = 0;
        }

        public virtual void OnDespawn()
        {
            if (mask != null)
            {
                mask.sprite = originalSprite;
                mask.overrideSprite = null;
                mask.color = originMaskColor;
                mask.rectTransform.sizeDelta = originMaskSize;
            }
            target = null;
        }
    }
}