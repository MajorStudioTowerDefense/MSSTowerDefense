using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    public class HSVHighlightModule : HSVTargetModule
    {
        private HSVHighlightModuleConfig subConfig;

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            subConfig = config as HSVHighlightModuleConfig;
            base.SetTutorialObject(tObj);
        }

        public override void CreateMask(HSVTarget target)
        {
            if (target.rtTarget == null)
                return;

            base.CreateMask(target);
        }

        public override void ConfigMask(HSVUIMask mask)
        {
            base.ConfigMask(mask);
            CalculateRect(mask);
            //Debug.Log("maskrect " + mask.maskRect);
            mask.mask.rectTransform.sizeDelta = new Vector2(mask.maskRect.size.x + subConfig.m_rectOffset.x, mask.maskRect.size.y + subConfig.m_rectOffset.y);

            mask.mask.rectTransform.localPosition = mask.maskRect.center;
        }

        public override void FollowObject(HSVUIMask mask)
        {
            base.FollowObject(mask);
            CalculateRect(mask);
            if (!mask.target.OutOfScreen && mask.maskRect.size.magnitude != 0)
            {
                SetMaskStatus(mask, true);
                mask.mask.rectTransform.sizeDelta = new Vector2(mask.maskRect.size.x + subConfig.m_rectOffset.x, mask.maskRect.size.y + subConfig.m_rectOffset.y);
                mask.mask.rectTransform.localPosition = mask.maskRect.center;
            }
            else
            {
                SetMaskStatus(mask, false);
            }
        }
    }
}