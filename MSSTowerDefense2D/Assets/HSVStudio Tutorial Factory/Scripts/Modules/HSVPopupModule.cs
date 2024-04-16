using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// This module will spawn display rect around target or specified location
    /// </summary>
    public class HSVPopupModule : HSVTargetModule
    {
        private Vector3 targetLocation;
        private List<Coroutine> scaleCor;

        private HSVPopupModuleConfig subConfig;

        private Vector2 interceptPt;
        private Vector3 dampVelocity;

        public override void AwakeOverride()
        {
            base.AwakeOverride();
            scaleCor = new List<Coroutine>();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (scaleCor != null)
            {
                scaleCor.Clear();
            }
            CanvasGroup.interactable = true;
            Raycaster.enabled = true;
        }

        public override void ModuleStart()
        {
            base.ModuleStart();
            ScaleInOutMasks(true);
        }

        public override void ModuleEnd()
        {
            base.ModuleEnd();
            for(int i = 0; i < masks.Count; i++)
            {
                ConfigurePopupUIMask(masks[i], true);
            }
            ScaleInOutMasks(false);
        } 

        public override void CreateMask(HSVTarget target)
        {
            base.CreateMask(target);
        }

        public override void ConfigMask(HSVUIMask mask)
        {
            base.ConfigMask(mask);
            ConfigurePopupUIMask(mask);
            PositionPopup(mask, true);
        }

        public override void FollowObject(HSVUIMask mask)
        {
            base.FollowObject(mask);
            PositionPopup(mask);
        }

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            subConfig = config as HSVPopupModuleConfig;
            base.SetTutorialObject(tObj);
        }

        /// <summary>
        /// Positions the popup rect
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="init"></param>
        private void PositionPopup(HSVUIMask mask, bool init = false)
        {
            if (subConfig != null)
            {
                var rectTransform = mask.transform as RectTransform;

                if (rectTransform != null)
                {
                    if (init)
                    {
                        rectTransform.sizeDelta = subConfig.popupRect.size;
                        rectTransform.localPosition = subConfig.popupRect.position;
                    }

                    if (subConfig.trackOutOfScreen && mask.target.OutOfScreen)
                    {
                        if (mask.target.rtTarget != null)
                        {
                            SetMaskStatus(mask, true);
                            HSVUtility.CalculateOutOfScreenPos(mask.target.rtTarget, HSVTutorialManager.Instance.Camera, Canvas, ref targetLocation, subConfig.screenOffset);
                            rectTransform.localPosition = init ? targetLocation : Vector3.Slerp(rectTransform.localPosition, targetLocation, config.m_smoothValue * Time.unscaledDeltaTime);
                        }
                        else
                        {
                            SetMaskStatus(mask, false);
                        }
                    }
                    else 
                    {
                        CalculateRect(mask);

                        if (mask.maskRect.size.magnitude != 0)
                        {
                            SetMaskStatus(mask, true);
                            if (subConfig.autoAnchor)
                            {
                                interceptPt = HSVUtility.CalculateEdgeIntercept(subConfig.pivotPoint, mask.maskRect);

                                targetLocation = HSVUtility.CalculateOffsetPoint(mask.maskRect.center, interceptPt, rectTransform.sizeDelta);
                            }
                            else
                            {
                                targetLocation = new Vector2(mask.maskRect.center.x + (subConfig.anchor.x - 0.5f) * (mask.maskRect.width + rectTransform.rect.width),
                                mask.maskRect.center.y + (0.5f - subConfig.anchor.y) * (mask.maskRect.height + rectTransform.rect.height));
                            }

                            rectTransform.localPosition = init ? targetLocation : Vector3.Slerp(rectTransform.localPosition, targetLocation, config.m_smoothValue * Time.unscaledDeltaTime);
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
                if (!isShown)
                {
                    FadeAction(true);
                }
            }
        }

        private void ScaleInOutMasks(bool scaleIn)
        {
            foreach(var cor in scaleCor)
            {
                StopCoroutine(cor);
            }

            if (subConfig != null)
            {
                for (int i = 0; i < masks.Count; i++)
                {
                    if (subConfig.scaleInOut)
                    {
                        var cor = StartCoroutine(ScaleInOutMask(masks[i], scaleIn));
                        scaleCor.Add(cor);
                    }
                }
            }
        }

        private IEnumerator ScaleInOutMask(HSVUIMask mask, bool scaleIn)
        {
            var targetScale = scaleIn ? Vector3.one : Vector3.zero;
            mask.transform.localScale = scaleIn ? Vector3.zero : mask.transform.localScale;

            while((mask.transform.localScale - targetScale).magnitude > 0.01f)
            {
                mask.transform.localScale = Vector3.SmoothDamp(mask.transform.localScale, targetScale, ref dampVelocity, config.fadeTime, config.m_smoothValue, Time.unscaledDeltaTime);
                yield return null;
            }

            mask.transform.localScale = targetScale;
        }

        /// <summary>
        /// Configures popup rect UI
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="clear"></param>
        private void ConfigurePopupUIMask(HSVUIMask mask, bool clear = false)
        {
            var popMask = mask as HSVPopupUIMask;
            if (popMask != null)
            {
                Raycaster.enabled = !clear;

                if (!clear)
                {
                    popMask.closeButton?.onClick.RemoveAllListeners();
                    popMask.closeButton?.onClick.AddListener(CloseModule);

                    popMask.advanceButton?.onClick.RemoveAllListeners();
                    popMask.advanceButton?.onClick.AddListener(AdvanceModule);

                    popMask.backButton?.onClick.RemoveAllListeners();
                    popMask.backButton?.onClick.AddListener(StepBackModule);

                    if(popMask.displayText != null)
                    {
                        if (subConfig.overrideStyle)
                        {
                            popMask.displayText.fontSize = subConfig.fontConfig.fontSize;
                            popMask.displayText.fontStyle = subConfig.fontConfig.fontStyle;
                            popMask.displayText.alignment = subConfig.fontConfig.textAlignment;
                            popMask.displayText.color = subConfig.fontConfig.textColor;

                            if(subConfig.fontConfig.fontAsset != null)
                            {
                                popMask.displayText.font = subConfig.fontConfig.fontAsset;
                            }
                        }
                        popMask.displayText.text = subConfig.message;

                        HSVTutorialManager.Instance.OnTextDisplayChange(this, popMask);
                    }

                    if (popMask.closeText != null)
                    {
                        if (subConfig.overrideCloseButtonStyle)
                        {
                            popMask.closeText.fontSize = subConfig.closeButtonStyle.fontSize;
                            popMask.closeText.fontStyle = subConfig.closeButtonStyle.fontStyle;
                            popMask.closeText.alignment = subConfig.closeButtonStyle.textAlignment;
                            popMask.closeText.color = subConfig.closeButtonStyle.textColor;

                            if (subConfig.closeButtonStyle.fontAsset != null)
                            {
                                popMask.closeText.font = subConfig.closeButtonStyle.fontAsset;
                            }
                        }
                        popMask.closeText.text = subConfig.closeButtonText;
                    }

                    if (popMask.advanceText != null)
                    {
                        if (subConfig.overrideAdvanceButtonStyle)
                        {
                            popMask.advanceText.fontSize = subConfig.advanceButtonStyle.fontSize;
                            popMask.advanceText.fontStyle = subConfig.advanceButtonStyle.fontStyle;
                            popMask.advanceText.alignment = subConfig.advanceButtonStyle.textAlignment;
                            popMask.advanceText.color = subConfig.advanceButtonStyle.textColor;

                            if (subConfig.advanceButtonStyle.fontAsset != null)
                            {
                                popMask.advanceText.font = subConfig.advanceButtonStyle.fontAsset;
                            }
                        }
                        popMask.advanceText.text = subConfig.advanceButtonText;
                    }

                    if (popMask.backText != null)
                    {
                        if (subConfig.overrideBackButtonStyle)
                        {
                            popMask.backText.fontSize = subConfig.backButtonStyle.fontSize;
                            popMask.backText.fontStyle = subConfig.backButtonStyle.fontStyle;
                            popMask.backText.alignment = subConfig.backButtonStyle.textAlignment;
                            popMask.backText.color = subConfig.backButtonStyle.textColor;

                            if (subConfig.backButtonStyle.fontAsset != null)
                            {
                                popMask.backText.font = subConfig.backButtonStyle.fontAsset;
                            }
                        }
                        popMask.backText.text = subConfig.backButtonText;
                    }
                }
                else
                {
                    popMask.closeButton?.onClick.RemoveAllListeners();
                    popMask.advanceButton?.onClick.RemoveAllListeners();
                    popMask.backButton?.onClick.RemoveAllListeners();
                }

                if (HSVTutorialManager.Instance.CheckStageStart(m_currentTObject.index))
                {
                    popMask.backButton?.gameObject.SetActive(false);
                }
            }
        }

        private void CloseModule()
        {
            HSVTutorialManager.Instance?.StopTutorial(m_currentTObject.index, m_currentTObject.advanceConfig.advanceType == AdvanceType.Automatic);
        }

        private void AdvanceModule()
        {
            HSVTutorialManager.Instance?.AdvanceTutorial();
        }

        private void StepBackModule()
        {
            HSVTutorialManager.Instance?.StepbackTutorial();
        }
    }
}