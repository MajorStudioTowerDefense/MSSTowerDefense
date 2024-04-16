using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// This is used to add target to the tutorial step, please specify which stage index or tutorial index to use
    /// </summary>
    public class HSVTargetTrigger : MonoBehaviour
    {
        public int stageIndex;
        public int tutorialIndex;
        public HSVTarget targetConfig;

        private bool addToManager = false;

        private void Awake()
        {
            if(targetConfig.target == null)
            {
                targetConfig.target = this.gameObject;
                targetConfig.rtTarget = this.gameObject;
            }

            AddToManager();
        }

        private void AddToManager()
        {
            if(HSVTutorialManager.Instance != null)
            {
                if(HSVTutorialManager.Instance.stageObjects != null && stageIndex > 0 && stageIndex <= HSVTutorialManager.Instance.stageObjects.Length)
                {
                    var stageObj = HSVTutorialManager.Instance.stageObjects[stageIndex - 1];
                    if(stageObj != null && stageObj.tutorialObjects != null && tutorialIndex > 0 && tutorialIndex <= stageObj.tutorialObjects.Length)
                    {
                        var tObj = stageObj.tutorialObjects[tutorialIndex-1];
                        if(tObj != null && tObj.focusTargets != null)
                        {
                            Array.Resize<HSVTarget>(ref tObj.focusTargets, tObj.focusTargets.Length + 1);

                            if (targetConfig.TargetBoundType != BoundType.RectTransform)
                            {
                                var status = targetConfig.rtTarget.activeSelf;
                                var parent = targetConfig.rtTarget.transform.parent;
                                targetConfig.rtTarget.transform.SetParent(null);
                                targetConfig.rtTarget.SetActive(true);
                                targetConfig.UnscaledBounds.Clear();
                                targetConfig.UnscaledBounds = TransformExtensionUtility.CalculateAllTransBound(targetConfig.rtTarget.transform);
                                targetConfig.UpdateTargetBound();
                                targetConfig.rtTarget.SetActive(status);
                                targetConfig.rtTarget.transform.SetParent(parent);
                            }

                            if (targetConfig.rtTarget != null)
                            {
                                targetConfig.UpdateCanvas();
                                targetConfig.UpdateTargetScreenRect(true);
                            }

                            tObj.focusTargets[tObj.focusTargets.Length - 1] = targetConfig;
                            addToManager = true;
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if(!addToManager)
            {
                AddToManager();
            }
            else
            {
                Destroy(this);
            }
        }
    }
}