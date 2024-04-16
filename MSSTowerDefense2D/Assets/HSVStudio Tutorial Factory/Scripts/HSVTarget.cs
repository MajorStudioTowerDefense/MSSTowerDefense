using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    // BoundType All would be enough for most Scene Objects, and Recttransform would be good for UI elements. Other BoundType would be added in the future version
    public enum BoundType
    {
        None,
        All,
        RectTransform,
        Transform
    }

    [Serializable]
    public struct SpawnInfo
    {
        //Prefab spawn location
        public Vector3 spawnPos;
        //Prefab spawn rotation
        public Vector3 spawnRot;
        //prefab spawn scale
        public Vector3 spawnScale;
    }

    [Serializable]
    public class HSVTarget
    {
        //Target game object, could be scene or prefab object
        public GameObject target;
        //this is runtime reference to the target, if prefab is used, it is reference to spawned scene object
        public GameObject rtTarget;
        //Override prefab spawn location
        public bool overrideSpawn;
        //Spawn Information
        public SpawnInfo spawnInfo;
        //this is the canvas for UI object
        public Canvas rtCanvas;
        //if the target should start inactive
        public bool startInactive;
        //if the target should be highlighted using outline effect
        public bool highlightTarget;
        //if module should follow target
        public bool followTarget;
        //if module mask should used by module
        public bool useModuleMask;
        //specifies which module could be used for current target
        public int moduleMask;
        //if target is out of screen
        private bool outOfScreen;
        public bool OutOfScreen
        {
            get { return outOfScreen; }
        }

        #region Object Bounds
        //this stores unscaled bound of target, except recttransform
        private Dictionary<Transform, Bounds> m_unscaledBounds = new Dictionary<Transform, Bounds>();
        public Dictionary<Transform, Bounds> UnscaledBounds
        {
            get
            {
                if(m_unscaledBounds == null)
                {
                    m_unscaledBounds = new Dictionary<Transform, Bounds>();
                }
                return m_unscaledBounds;
            }
            set
            {
                m_unscaledBounds = value;
            }
        }
        //this is the recttransform bound used by World Space Canvas UI
        private Bounds targetBound = new Bounds();
        //this stores all recttransform reference
        private List<RectTransform> allRectTrans = new List<RectTransform>();
        //screen display rect of the target
        public Rect screenRect;
        //no bounds
        private static readonly Bounds m_nobound = new Bounds();
        //Which bound type should use for target, please make sure use correct bound type 
        public BoundType TargetBoundType;
        //Bounds of the target
        public Bounds Bounds
        {
            get
            {
                if (TargetBoundType == BoundType.All)
                {
                    if (rtTarget != null && UnscaledBounds.ContainsKey(rtTarget.transform))
                    {
                        return UnscaledBounds[target.transform];
                    }
                }
                else if (TargetBoundType == BoundType.RectTransform)
                {
                    if(rtTarget!= null && rtCanvas != null)
                    {
                        if(rtCanvas.renderMode == RenderMode.WorldSpace)
                        {
                            if(rtTarget.transform.hasChanged || CheckCameraStateChange())
                            {
                                targetBound = TransformExtensionUtility.CalculateBounds(rtTarget.transform);
                            }
                        }
                    }
                    return targetBound;
                }
                else if(TargetBoundType == BoundType.Transform)
                {
                    return targetBound;
                }

                return m_nobound;
            }
        }
        #endregion

        #region Spawn Effect
        public bool spawnEffect;
        public GameObject effectPrefab;
        public GameObject effectRT;
        public bool overrideEffectInfo;
        public SpawnInfo effectSpawnInfo;
        #endregion

        #region Target Trigger Config
        public HSVTriggerConfig triggerConfig;
        public bool useTrigger;
        public PlayState triggerState;
        #endregion

        public HSVTarget()
        {
            startInactive = false;
            useModuleMask = false;
            moduleMask = 0;
            overrideSpawn = false;
            spawnInfo = new SpawnInfo()
            {
                spawnPos = Vector3.zero,
                spawnRot = Vector3.zero,
                spawnScale = Vector3.one
            };

            spawnEffect = false;
            effectPrefab = null;
            effectRT = null;
            overrideEffectInfo = false;
            effectSpawnInfo = new SpawnInfo()
            {
                spawnPos = Vector3.zero,
                spawnRot = Vector3.zero,
                spawnScale = Vector3.one
            };

            useTrigger = false;
            triggerConfig = new HSVTriggerConfig();
            triggerState = PlayState.Idle;
        }

        /// <summary>
        /// Updates the target screen rect if necessary
        /// </summary>
        /// <param name="forceUpdate"></param>
        public void UpdateTargetScreenRect(bool forceUpdate = false)
        {
            if (forceUpdate || (rtTarget != null && rtTarget.activeInHierarchy && rtTarget.transform.hasChanged))
            {
                screenRect = CalculateRect();
                rtTarget.transform.hasChanged = false;
            }
        }

        /// <summary>
        /// Caculates rect depends on which type is used
        /// </summary>
        /// <returns></returns>
        private Rect CalculateRect()
        {
            var rect = new Rect();
            if (HSVTutorialManager.Instance != null)
            {
                switch (TargetBoundType)
                {
                    case BoundType.RectTransform:
                        if(rtCanvas != null)
                        {
                            if (rtCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                            {
                                rect = TransformExtensionUtility.RectTransformToScreenSpace(allRectTrans, null);
                            }
                            else if(rtCanvas.renderMode == RenderMode.ScreenSpaceCamera)
                            {
                                rect = TransformExtensionUtility.RectTransformToScreenSpace(allRectTrans, rtCanvas.worldCamera);
                            }
                            else if(rtCanvas.renderMode == RenderMode.WorldSpace)
                            {
                                rect = TransformExtensionUtility.BoundsToScreenRect(rtCanvas.worldCamera, targetBound);
                            }
                        }
                        break;
                    case BoundType.Transform:
                        if (rtTarget != null)
                        {
                            rect = TransformExtensionUtility.CalculateCombineScreenRect(rtTarget.transform, targetBound, HSVTutorialManager.Instance.Camera, out outOfScreen);
                        }
                        break;
                    default:
                        rect = TransformExtensionUtility.CalculateCombineScreenRect(UnscaledBounds, HSVTutorialManager.Instance.Camera, out outOfScreen);
                        break;
                }
            }

            return rect;
        }

        /// <summary>
        /// Updates Canvas reference
        /// </summary>
        public void UpdateCanvas()
        {
            if(rtTarget != null && rtTarget.transform is RectTransform)
            {
                rtCanvas = rtTarget.GetComponentInParent<Canvas>();
                if(TargetBoundType == BoundType.RectTransform)
                {
                    targetBound = TransformExtensionUtility.CalculateBounds(rtTarget.transform);
                }
                allRectTrans.Clear();
                allRectTrans.AddRange(rtTarget.GetComponentsInChildren<RectTransform>().ToList());
            }
        }

        /// <summary>
        /// Uodates Individual target bound
        /// </summary>
        public void UpdateTargetBound()
        {
            if(rtTarget != null && TargetBoundType == BoundType.Transform && UnscaledBounds.ContainsKey(rtTarget.transform))
            {
                targetBound = UnscaledBounds[rtTarget.transform];
            }
        }

        private Matrix4x4 cameraToWorldMatrix;
        public bool CheckCameraStateChange()
        {
            if (rtCanvas != null && rtCanvas.renderMode == RenderMode.WorldSpace && rtCanvas.worldCamera != null)
            {
                if (cameraToWorldMatrix != rtCanvas.worldCamera.cameraToWorldMatrix)
                {
                    cameraToWorldMatrix = rtCanvas.worldCamera.cameraToWorldMatrix;
                    return true;
                }
            }

            return false;
        }
    }
}