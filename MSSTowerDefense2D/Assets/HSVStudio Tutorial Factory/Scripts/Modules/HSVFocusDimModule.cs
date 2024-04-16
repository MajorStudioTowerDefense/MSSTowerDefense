using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    public class HSVFocusDimModule : HSVTargetModule
    {
        //The dimming image for the entire screen
        public Image dimImage;

        private HSVFocusDimModuleConfig subConfig;

        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        public override void ModuleStart()
        {
            Canvas.overrideSorting = true;
            Canvas.sortingOrder = Canvas.rootCanvas.sortingOrder - 10;
            //Setting dimming color to clear if there is no target
            if (dimImage != null && config.overridePrefabColor)
            {
                dimImage.color = masks.Count > 0? config.color : Color.clear;
            }

            base.ModuleStart();
        }

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            subConfig = config as HSVFocusDimModuleConfig;
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
            if (subConfig != null)
            {
                base.ConfigMask(mask);
                //setting dimming image as last child, so it would correctly mask out the target area
                if (dimImage != null)
                {
                    dimImage.transform.SetAsLastSibling();
                }

                CalculateRect(mask);
                mask.mask.rectTransform.sizeDelta = new Vector2(mask.maskRect.size.x + subConfig.m_rectOffset.x, mask.maskRect.size.y + subConfig.m_rectOffset.y);

                mask.mask.rectTransform.localPosition = mask.maskRect.center;
            }
        }

        public override void FollowObject(HSVUIMask mask)
        {
            if (subConfig != null)
            {
                base.FollowObject(mask);
                CalculateRect(mask);
                //shows the mask image when not out of screen or has overlaps
                if (!mask.target.OutOfScreen && mask.maskRect.size.magnitude != 0 && dimImage.rectTransform.rect.Overlaps(mask.mask.rectTransform.rect))
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
}