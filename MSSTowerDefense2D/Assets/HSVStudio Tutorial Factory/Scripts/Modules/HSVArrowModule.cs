using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    public class HSVArrowModule : HSVTargetModule
    {
        private Vector3 targetLocation;
        private Quaternion targetRotation;

        private HSVArrowModuleConfig subConfig;

        private Vector2 interceptPt;
        private Rect tempRect;

        public override void CreateMask(HSVTarget target)
        {
            if (target.rtTarget == null)
                return;

            base.CreateMask(target);
        }

        public override void ConfigMask(HSVUIMask mask)
        {
            if (mask.target.rtTarget == null)
                return;

            base.ConfigMask(mask);
            PositionArrow(mask, true);
        }

        public override void FollowObject(HSVUIMask mask)
        {
            base.FollowObject(mask);
            PositionArrow(mask);
        }

        public override void MaskAction(HSVUIMask mask)
        {
            base.MaskAction(mask);
            AnimateArrow(mask);
        }

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            subConfig = config as HSVArrowModuleConfig;
            base.SetTutorialObject(tObj);
        }

        /// <summary>
        /// Positions arrow using config setting
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="init"></param>
        private void PositionArrow(HSVUIMask mask, bool init = false)
        {
            if (subConfig != null)
            {
                var rectTransform = mask.transform as RectTransform;

                if (rectTransform != null)
                {
                    if (init)
                    {
                        //Sets arrow mask size to config specified size on initialization
                        rectTransform.sizeDelta = subConfig.arrowSize;
                    }

                    //if using tracking when target is out of screen, the arrow will circle around the screen, otherwise, it would just fade out
                    if (subConfig.trackOutOfScreen && mask.target.OutOfScreen)
                    {
                        if (mask.target.rtTarget != null)
                        {
                            SetMaskStatus(mask, true);
                            var center = new Vector3(HSVTutorialManager.Instance.Camera.pixelWidth/2, HSVTutorialManager.Instance.Camera.pixelHeight / 2);
                            HSVUtility.CalculateOutOfScreenPos(mask.target.rtTarget, HSVTutorialManager.Instance.Camera, Canvas, ref targetLocation, subConfig.screenOffset);
                            rectTransform.localPosition = init ? targetLocation : Vector3.Slerp(rectTransform.localPosition, targetLocation, config.m_smoothValue * Time.unscaledDeltaTime);

                            targetRotation = Quaternion.FromToRotation(subConfig.arrowDirection, (rectTransform.position - center).normalized);

                            rectTransform.localRotation = init ? targetRotation : Quaternion.Slerp(rectTransform.localRotation, targetRotation, config.m_smoothValue * Time.unscaledDeltaTime);
                        }
                        else
                        {
                            SetMaskStatus(mask, false);
                        }
                    }
                    else
                    {
                        CalculateRect(mask);
                        //Fade out mask when screen rect size is zero
                        if (mask.maskRect.size.magnitude != 0)
                        {
                            SetMaskStatus(mask, true);
                            if (subConfig.autoAnchor)
                            {
                                tempRect = mask.maskRect;
                                tempRect.size += subConfig.m_rectOffset;
                                tempRect.center -= subConfig.m_rectOffset / 2;
                                interceptPt = HSVUtility.CalculateEdgeIntercept(subConfig.pivotPoint, tempRect);

                                targetLocation = HSVUtility.CalculateOffsetPoint(tempRect.center, interceptPt, rectTransform.sizeDelta);
                            }
                            else
                            {
                                tempRect = mask.maskRect;
                                tempRect.size += subConfig.m_rectOffset;
                                tempRect.center -= subConfig.m_rectOffset / 2;
                                targetLocation = new Vector2(tempRect.center.x + (subConfig.anchor.x - 0.5f) * (tempRect.width + rectTransform.rect.width),
                                tempRect.center.y + (0.5f - subConfig.anchor.y) * (tempRect.height + rectTransform.rect.height));
                            }

                            rectTransform.localPosition = init ? targetLocation : Vector3.Slerp(rectTransform.localPosition, targetLocation, config.m_smoothValue * Time.unscaledDeltaTime);

                            if (subConfig.autoCalculateRotation)
                            {
                                targetRotation = Quaternion.FromToRotation(subConfig.arrowDirection, (new Vector3(mask.maskRect.center.x, mask.maskRect.center.y) - rectTransform.localPosition).normalized);
                            }
                            else
                            {
                                targetRotation = Quaternion.Euler(subConfig.arrowRotation);
                            }

                            rectTransform.localRotation = init ? targetRotation : Quaternion.Slerp(rectTransform.localRotation, targetRotation, config.m_smoothValue * Time.unscaledDeltaTime);
                        }
                        else
                        {
                            SetMaskStatus(mask, false);
                        }
                    }
                }
            }     
        }

        public override void CheckAllTargetOutOfScreen()
        {
            if (!subConfig.trackOutOfScreen)
            {
                base.CheckAllTargetOutOfScreen();
            }
            else
            {
                if(!isShown)
                {
                    FadeAction(true);
                }
            }
        }

        /// <summary>
        /// Animates arrow using animation curve with respect to arrow direction
        /// </summary>
        /// <param name="mask"></param>
        private void AnimateArrow(HSVUIMask mask)
        {
            if(subConfig != null && mask.gameObject.activeSelf)
            {
                if (subConfig.animateArrow)
                {
                    mask.animationProgressTimer += Time.unscaledDeltaTime * subConfig.animateSpeed;
                    mask.mask.rectTransform.localPosition = subConfig.animateCurve.Evaluate(mask.animationProgressTimer) * subConfig.animateMagnitude * subConfig.arrowDirection;
                }
                else
                {
                    if (mask.mask.rectTransform.localPosition != Vector3.zero)
                    {
                        mask.mask.rectTransform.localPosition = Vector3.zero;
                    }
                }
            }
        }
    }
}