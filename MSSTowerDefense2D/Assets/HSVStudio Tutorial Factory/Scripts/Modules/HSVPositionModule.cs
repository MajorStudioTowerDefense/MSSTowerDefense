using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    public class HSVPositionModule : HSVTargetModule
    {
        private Vector3 targetLocation;
        private Vector3 arrowLocation;
        private float arrowOffset;
        private Vector3 pointDirection;
        private Vector3 targetScreenPos;
        private Quaternion arrowRotation;
        private bool showArrow = false;

        private HSVPositionModuleConfig subConfig;

        private bool outOfScreen;
        private string displayDist;
        private string distFormat;
        private string postfix;

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
            PositionMask(mask, true);
        }

        public override void FollowObject(HSVUIMask mask)
        {
            base.FollowObject(mask);
            PositionMask(mask);
        }

        public override void MaskAction(HSVUIMask mask)
        {
            base.MaskAction(mask);
        }

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            subConfig = config as HSVPositionModuleConfig;
            base.SetTutorialObject(tObj);
        }

        /// <summary>
        /// Positions Mask using config setting
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="init"></param>
        private void PositionMask(HSVUIMask mask, bool init = false)
        {
            if (subConfig != null)
            {
                var rectTransform = mask.transform as RectTransform;
                var pMask = mask as HSVPositionUIMask;
                if (pMask != null)
                {
                    arrowOffset = (subConfig.maskSize / 2).magnitude + subConfig.m_rectOffset;
                }

                if (rectTransform != null)
                {
                    if (init)
                    {
                        //Sets position mask size to config specified size on initialization
                        rectTransform.sizeDelta = subConfig.maskSize;
                        if (pMask != null)
                        {
                            if (pMask.arrowMask != null)
                            {
                                pMask.arrowMask.rectTransform.sizeDelta = subConfig.arrowSize;
                                pMask.arrowMask.overrideSprite = subConfig.arrowSprite;
                                pMask.arrowMask.color = subConfig.arrowColor;

                                pMask.arrowMask.rectTransform.localPosition = subConfig.arrowDirection * arrowOffset;
                            }

                            if (pMask.displayText != null)
                            {
                                pMask.displayText.rectTransform.sizeDelta = subConfig.fontRect.size;
                                pMask.displayText.rectTransform.localPosition = subConfig.fontRect.position;

                                if (subConfig.overrideStyle)
                                {
                                    pMask.displayText.fontSize = subConfig.fontConfig.fontSize;
                                    pMask.displayText.fontStyle = subConfig.fontConfig.fontStyle;
                                    pMask.displayText.alignment = subConfig.fontConfig.textAlignment;
                                    pMask.displayText.color = subConfig.fontConfig.textColor;
                                }
                            }
                        }
                    }

                    //if using tracking when target is out of screen, the position will circle around the screen, otherwise, it would just fade out
                    if (mask.target.rtTarget != null)
                    {
                        if (HSVUtility.CalculateOutOfScreenPos(mask.target.rtTarget.transform.position + subConfig.positionOffset, HSVTutorialManager.Instance.Camera, Canvas, ref targetLocation, subConfig.screenOffset, out outOfScreen))
                        {
                            if (subConfig.trackOutOfScreen || !outOfScreen)
                            {
                                SetMaskStatus(mask, true);
                                SetDistance(pMask, subConfig.displayDistance, mask.target.rtTarget.transform.position);
                                rectTransform.localPosition = init ? targetLocation : Vector3.Slerp(rectTransform.localPosition, targetLocation, config.m_smoothValue * Time.unscaledDeltaTime);

                                if (subConfig.displayArrow)
                                {
                                    if (pMask != null && pMask.arrowMask != null)
                                    {
                                        if (outOfScreen)
                                        {
                                            //Calculate how far the target is from display mask
                                            if (HSVUtility.CalculateLocalRectPoint(mask.target.rtTarget.transform.position, HSVTutorialManager.Instance.Camera, Canvas, out targetScreenPos))
                                            {
                                                //Determine if arrow should be shown 
                                                showArrow = false;
                                                //if it is in front of camera, use target screen pos with recttransform, else, use center and recttransform
                                                if (targetScreenPos.z > 0)
                                                {
                                                    pointDirection = targetScreenPos - rectTransform.localPosition;
                                                    if (pointDirection.magnitude > arrowOffset)
                                                    {
                                                        showArrow = true;
                                                    }
                                                }
                                                else
                                                {
                                                    pointDirection = rectTransform.position - new Vector3(HSVTutorialManager.Instance.Camera.pixelWidth / 2, HSVTutorialManager.Instance.Camera.pixelHeight / 2);
                                                    showArrow = true;
                                                }

                                                //Adjust rotation with position
                                                if (showArrow)
                                                {
                                                    pointDirection.z = 0;
                                                    SetArrowMask(pMask, true);

                                                    if (pMask.arrowMask.gameObject.activeSelf)
                                                    {
                                                        arrowLocation = pointDirection.normalized * arrowOffset;
                                                        pMask.arrowMask.rectTransform.localPosition = init ? arrowLocation : Vector3.Slerp(pMask.arrowMask.rectTransform.localPosition, arrowLocation, config.m_smoothValue * Time.unscaledDeltaTime);
                                                        arrowRotation = Quaternion.FromToRotation(subConfig.arrowDirection, pointDirection);
                                                        pMask.arrowMask.rectTransform.localRotation = init ? arrowRotation : Quaternion.Slerp(pMask.arrowMask.rectTransform.localRotation, arrowRotation, config.m_smoothValue * Time.unscaledDeltaTime);
                                                    }
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }

                                SetArrowMask(pMask, false);
                                return;
                            }
                        }
                    }

                    SetDistance(pMask, false, mask.target.rtTarget.transform.position);
                    SetArrowMask(pMask, false);
                    SetMaskStatus(mask, false);
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
                if (!isShown)
                {
                    FadeAction(true);
                }
            }
        }

        private void SetArrowMask(HSVPositionUIMask pMask, bool enable)
        {
            if (pMask != null && pMask.arrowMask != null && pMask.arrowMask.gameObject.activeSelf != enable)
            {
                pMask.arrowMask.gameObject.SetActive(enable);
            }
        }

        private void SetDistance(HSVPositionUIMask mask, bool display, Vector3 targetLocation)
        {
            if (mask != null)
            {
                bool shouldDisplay = display;
                displayDist = null;
                if (display)
                {
                    var refObj = subConfig.useCamera ? HSVTutorialManager.Instance.Camera.gameObject : HSVTutorialManager.Instance.Player;

                    if (refObj != null)
                    {
                        var dist = (targetLocation - refObj.transform.position).magnitude;
                        switch (subConfig.distUnit)
                        {
                            case Unit.Meter:
                                postfix = "m";
                                distFormat = "N0";
                                break;
                            case Unit.KiloMeter:
                                dist /= 1000;
                                postfix = "km";
                                distFormat = "g4";
                                break;
                            case Unit.Mile:
                                dist *= 0.0006213712f;
                                postfix = "mi";
                                distFormat = "g4";
                                break;
                            case Unit.Feet:
                                dist *= 3.28084f;
                                postfix = "ft";
                                distFormat = "N0";
                                break;
                        }

                        if (dist > subConfig.distThreshold)
                        {
                            displayDist = dist.ToString(distFormat) + " " + postfix;
                        }
                        else
                        {
                            shouldDisplay = false;
                        }
                    }
                    else
                    {
                        shouldDisplay = false;
                    }
                }

                mask.SetDisplayText(shouldDisplay, displayDist);
            }
        }
    }
}