using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    public class HSVPositionUIMask : HSVUIMask
    {
        public TMPro.TextMeshProUGUI displayText;
        public Image arrowMask;

        private Color originArrowColor;
        private Vector3 originArrowPos;
        private Vector2 originArrowSize;

        private HSVFontStyle originalStyle;

        public override void AwakeOverride()
        {
            base.AwakeOverride();
            var arrow = transform.Find("Arrow");
            if(arrow != null)
            {
                arrowMask = arrow.GetComponentInChildren<Image>();
            }

            if (displayText != null)
            {
                originalStyle = new HSVFontStyle();
                originalStyle.fontSize = displayText.fontSize;
                originalStyle.fontStyle = displayText.fontStyle;
                originalStyle.textAlignment = displayText.alignment;
                originalStyle.textColor = displayText.color;
            }
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (arrowMask != null)
            {
                originArrowColor = arrowMask.color;
                originArrowPos = arrowMask.rectTransform.localPosition;
                originArrowSize = arrowMask.rectTransform.sizeDelta;
            }

            if (displayText != null)
            {
                displayText.fontSize = originalStyle.fontSize;
                displayText.fontStyle = originalStyle.fontStyle;
                displayText.alignment = originalStyle.textAlignment;
                displayText.color = displayText.color;
                displayText.text = string.Empty;
            }
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            if (arrowMask != null)
            {
                arrowMask.color = originArrowColor;
                arrowMask.rectTransform.localPosition = originArrowPos;
                arrowMask.rectTransform.sizeDelta = originArrowSize;
            }

            if (displayText != null)
            {
                displayText.fontSize = originalStyle.fontSize;
                displayText.fontStyle = originalStyle.fontStyle;
                displayText.alignment = originalStyle.textAlignment;
                displayText.color = displayText.color;
            }
        }

        public void SetDisplayText(bool display, string dist)
        {
            if (displayText != null)
            {
                if (displayText.gameObject.activeSelf != display)
                {
                    displayText.gameObject.SetActive(displayText);
                }

                displayText.text = dist;
            }
        }
    }
}