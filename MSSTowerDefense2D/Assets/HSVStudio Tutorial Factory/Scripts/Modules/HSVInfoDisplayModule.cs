using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    public class HSVInfoDisplayModule : HSVTimeModule
    {
        private Coroutine scaleCor;

        private HSVInfoDisplayModuleConfig subConfig;
        private Vector3 m_smoothVelocity;
        public override void AwakeOverride()
        {
            base.AwakeOverride();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            scaleCor = null;
        }

        public override void UpdateOverride()
        {
            base.UpdateOverride();
        }

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            subConfig = config as HSVInfoDisplayModuleConfig;
            base.SetTutorialObject(tObj);
        }

        public override void ModuleStart()
        {
            base.ModuleStart();
            ScaleInOutMasks(true);
        }

        public override void ModuleEnd()
        {
            base.ModuleEnd();
            for (int i = 0; i < masks.Count; i++)
            {
                ConfigureDisplayUIMask(masks[i], true);
            }
            ScaleInOutMasks(false);
        }

        public override void CreateMask()
        {
            base.CreateMask();
        }

        public override void ConfigMask(HSVUIMask mask)
        {
            base.ConfigMask(mask);
            ConfigureDisplayUIMask(mask);
            PositionDisplay(mask);
        }

        /// <summary>
        /// Position the rect using config setting
        /// </summary>
        /// <param name="mask"></param>
        private void PositionDisplay(HSVUIMask mask)
        {
            if (subConfig != null)
            {
                var rectTransform = mask.transform as RectTransform;

                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = subConfig.displayRect.size;
                    rectTransform.localPosition = subConfig.displayRect.position;
                }
            }
        }

        /// <summary>
        /// Scales in or out the mask when module starts
        /// </summary>
        /// <param name="scaleIn"></param>
        private void ScaleInOutMasks(bool scaleIn)
        {
            if(scaleCor != null)
                StopCoroutine(scaleCor);

            if (subConfig != null)
            {
                for (int i = 0; i < masks.Count; i++)
                {
                    if (subConfig.scaleInOut)
                    {
                        scaleCor = StartCoroutine(ScaleInOutMask(masks[i], scaleIn));
                    }
                }
            }
        }

        private IEnumerator ScaleInOutMask(HSVUIMask mask, bool scaleIn)
        {
            var targetScale = scaleIn ? Vector3.one : Vector3.zero;
            mask.transform.localScale = scaleIn ? Vector3.zero : mask.transform.localScale;

            while ((mask.transform.localScale - targetScale).magnitude > 0.01f)
            {
                mask.transform.localScale = Vector3.SmoothDamp(mask.transform.localScale, targetScale, ref m_smoothVelocity, config.fadeTime, config.m_smoothValue, Time.unscaledDeltaTime);
                yield return null;
            }

            mask.transform.localScale = targetScale;
            scaleCor = null;
        }

        /// <summary>
        /// Configure mask and add listeners to the button if present
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="clear"></param>
        private void ConfigureDisplayUIMask(HSVUIMask mask, bool clear = false)
        {
            var popMask = mask as HSVPopupUIMask;
            if (popMask != null)
            {
                Raycaster.enabled = !clear;

                if(!clear)
                {
                    popMask.closeButton?.onClick.RemoveAllListeners();
                    popMask.closeButton?.onClick.AddListener(CloseModule);

                    popMask.advanceButton?.onClick.RemoveAllListeners();
                    popMask.advanceButton?.onClick.AddListener(AdvanceModule);

                    popMask.backButton?.onClick.RemoveAllListeners();
                    popMask.backButton?.onClick.AddListener(StepBackModule);

                    if (popMask.displayText != null)
                    {
                        if (subConfig.overrideStyle)
                        {
                            popMask.displayText.fontSize = subConfig.fontConfig.fontSize;
                            popMask.displayText.fontStyle = subConfig.fontConfig.fontStyle;
                            popMask.displayText.alignment = subConfig.fontConfig.textAlignment;
                            popMask.displayText.color = subConfig.fontConfig.textColor;

                            if (subConfig.fontConfig.fontAsset != null)
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

                if(HSVTutorialManager.Instance.CheckStageStart(m_currentTObject.index))
                {
                    popMask.backButton?.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// This calls StopTutorial method, if advance type set to automatic, it will still advance
        /// </summary>
        private void CloseModule()
        {
            HSVTutorialManager.Instance?.StopTutorial(m_currentTObject.index);
        }

        /// <summary>
        /// This advance current tutorial step to next step, however, it will not use advance config setting
        /// </summary>
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