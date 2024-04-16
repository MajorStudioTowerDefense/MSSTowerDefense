using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSVStudio.Tutorial.Outline;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// This class is mainly used for modules with target
    /// </summary>
    public class HSVTargetModule : HSVTutorialModule
    {
        [HideInInspector]
        public bool shouldFocus;
        public bool updateRealtimeBound;
        public HSVTargetModuleConfig targetConfig;
        public bool allTargetOffScreen = false;

        public override void OnSpawn()
        {
            //Reseting canvas status
            base.OnSpawn();
            shouldFocus = false;
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
        }

        public override void UpdateOverride()
        {
            base.UpdateOverride();
            if (shouldFocus)
            {
                if (m_currentTObject != null)
                {
                    CheckAllTargetOutOfScreen();

                    for (int i = 0; i < masks.Count; i++)
                    {
                        if(masks[i].target.useTrigger && masks[i].target.triggerState == PlayState.End)
                        {
                            if (masks[i].gameObject.activeSelf)
                            {
                                masks[i].gameObject.SetActive(false);
                            }
                            continue;
                        }

                        if (masks[i].target.rtTarget != null && masks[i].target.rtTarget.activeInHierarchy)
                        {
                            if (!masks[i].gameObject.activeSelf)
                            {
                                masks[i].gameObject.SetActive(true);
                            }

                            if ((targetConfig.overrideFollowTarget && targetConfig.followTarget) || (!targetConfig.overrideFollowTarget && masks[i].target.followTarget))
                            {
                                FollowObject(masks[i]);
                            }
                        }
                        else if(!targetConfig.allowNoTarget)
                        {
                            if(masks[i].gameObject.activeSelf)
                            {
                                masks[i].gameObject.SetActive(false);
                            }
                        }

                        MaskAction(masks[i]);
                    }
                }
            }
        }

        public override void ModuleStart()
        {
            base.ModuleStart();
            if (config == null)
            {
                HSVTutorialManager.DebugLogWarning("Please check if the module config is added to the Tutorial Object");
                return;
            }

            shouldFocus = true;
            for (int i = 0; i < masks.Count; i++)
            {
                if (masks[i].target.rtTarget != null)
                {
                    masks[i].target.rtTarget.SetActive(true);

                    if (masks[i].target.rtTarget.activeInHierarchy)
                    {
                        if (!masks[i].gameObject.activeSelf)
                        {
                            masks[i].gameObject.SetActive(true);
                            ConfigMask(masks[i]);
                        }
                    }
                    else 
                    {
                        if (masks[i].gameObject.activeSelf)
                        {
                            masks[i].gameObject.SetActive(false);
                        }
                    }
                }
                else if(targetConfig.allowNoTarget)
                {
                    if (!masks[i].gameObject.activeSelf)
                    {
                        masks[i].gameObject.SetActive(true);
                        ConfigMask(masks[i]);
                    }
                }

                if (config.overridePrefabColor)
                {
                    masks[i].mask.color = config.color;
                }
            }

            FadeAction(true);
        }

        public override void ModuleEnd()
        {
            shouldFocus = false;
            base.ModuleEnd();
        }

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            base.SetTutorialObject(tObj);
            targetConfig = config as HSVTargetModuleConfig;
            SetFocusTarget(tObj.focusTargets.ToList());
        }

        #region Override methods for targets
        public virtual void SetFocusTarget(List<HSVTarget> objects)
        {
            ClearMasks();
            for (int i = 0; i < objects.Count; i++)
            {
                CreateMask(objects[i]);
            }
        }

        /// <summary>
        /// Create mask for target, multiple target will have multiple mask
        /// </summary>
        /// <param name="target"></param>
        public virtual void CreateMask(HSVTarget target)
        {
            if (config == null)
                return;

            //if target settings uses module mask, only selected mask layer will be used for the target
            if(target.useModuleMask)
            {
                var modulefound = false;
                var moduleTypeConfig = HSVTutorialManager.Instance.TargetModuleConfigTypes;
                for(int i = 0; i < moduleTypeConfig.Count; i++)
                {
                    var layerMask = 1 << i;
                    if((layerMask & target.moduleMask) != 0)
                    {
                        if(config.GetType() == moduleTypeConfig[i])
                        {
                            modulefound = true;
                            break;
                        }
                    }
                }

                if (!modulefound)
                    return;
            }

            var maskObj = HSVObjectPool.Instance.GetFreeEntity(config.maskPrefab);
            if (maskObj != null)
            {
                var mask = maskObj.GetComponent<HSVUIMask>();
                if (mask != null)
                {
                    if(config.m_sprite != null)
                    {
                        //overrides mask sprite with settings sprite
                        if (mask.mask.sprite == null)
                        {
                            mask.mask.sprite = config.m_sprite;
                        }
                        else
                        {
                            mask.mask.overrideSprite = config.m_sprite;
                        }
                    }

                    mask.target = target;
                    if (target.target != null)
                    {
                        mask.oriActiveState = target.target.activeSelf;
                    }
                    if (!masks.Contains(mask))
                    {
                        masks.Add(mask);
                    }
                    //reseting transform when spawned
                    maskObj.transform.SetParent(Canvas.transform);
                    maskObj.transform.localPosition = Vector3.zero;
                    maskObj.transform.localRotation = Quaternion.identity;
                    maskObj.transform.localScale = Vector3.one;
                    maskObj.gameObject.SetActive(false);

                    if(target.rtTarget != null)
                    {
                        target.UpdateTargetScreenRect(true);
                    }
                }
                else
                {
                    HSVObjectPool.Instance.SetEntityAsFree(maskObj.transform);
                }
            }
        }

        /// <summary>
        /// Configures mask attribute when created
        /// </summary>
        /// <param name="mask"></param>
        public virtual void ConfigMask(HSVUIMask mask)
        {

        }

        /// <summary>
        /// Follows target on realtime
        /// </summary>
        /// <param name="mask"></param>
        public virtual void FollowObject(HSVUIMask mask)
        {

        }

        /// <summary>
        /// Mask specific action on realtime
        /// </summary>
        /// <param name="mask"></param>
        public virtual void MaskAction(HSVUIMask mask)
        {

        }
        #endregion

        #region Module Running Actions
        /// <summary>
        /// Calculates display rect for the mask
        /// </summary>
        /// <param name="mask"></param>
        public void CalculateRect(HSVUIMask mask)
        {
            switch (mask.target.TargetBoundType)
            {
                case BoundType.None:
                    mask.maskRect = new Rect();
                    break;
                case BoundType.RectTransform:
                    if (mask.target.rtCanvas != null)
                    {
                        ConvertRectToCanvasRect(mask.target.screenRect, ref mask.maskRect);
                    }
                    break;
                default:
                    ConvertRectToCanvasRect(mask.target.screenRect, ref mask.maskRect);
                    break;
            }
        }

        /// <summary>
        /// Convert screen rect to the local rect
        /// </summary>
        /// <param name="objectRect"></param>
        /// <param name="rect"></param>
        private void ConvertRectToCanvasRect(Rect objectRect, ref Rect rect)
        {
            if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                TransformExtensionUtility.ScreenRectToLocalRectInRectangle((RectTransform)Canvas.transform, objectRect, null, out rect);
            }
            else
            {
                TransformExtensionUtility.ScreenRectToLocalRectInRectangle((RectTransform)Canvas.transform, objectRect, Canvas.worldCamera, out rect);
            }
        }

        /// <summary>
        /// Checks if all targets are out of screen
        /// </summary>
        public virtual void CheckAllTargetOutOfScreen()
        {
            allTargetOffScreen = true;
            for(int i = 0; i < masks.Count; i++)
            {
                if(!masks[i].target.OutOfScreen)
                {
                    allTargetOffScreen = false;
                    break;
                }
            }

            FadeAction(!allTargetOffScreen);
        }
        #endregion
    }
}