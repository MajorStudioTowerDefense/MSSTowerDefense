using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    public class HSVBoundModule : HSVTargetModule
    {
        private HSVBoundModuleConfig subConfig;

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            subConfig = config as HSVBoundModuleConfig;
            base.SetTutorialObject(tObj);
        }

        public override void CreateMask(HSVTarget target)
        {
            if (config == null || target.rtTarget == null)
                return;

            //if target settings uses module mask, only selected mask layer will be used for the target
            if (target.useModuleMask)
            {
                var modulefound = false;
                var moduleTypeConfig = HSVTutorialManager.Instance.TargetModuleConfigTypes;
                for (int i = 0; i < moduleTypeConfig.Count; i++)
                {
                    var layerMask = 1 << i;
                    if ((layerMask & target.moduleMask) != 0)
                    {
                        if (config.GetType() == moduleTypeConfig[i])
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
                    maskObj.transform.SetParent(HSVTutorialManager.Instance.WorldObjectContainer);
                    maskObj.transform.localPosition = Vector3.zero;
                    maskObj.transform.localRotation = Quaternion.identity;
                    maskObj.transform.localScale = Vector3.one;
                    maskObj.gameObject.SetActive(false);
                }
                else
                {
                    HSVObjectPool.Instance.SetEntityAsFree(maskObj.transform);
                }
            }
        }

        public override void ConfigMask(HSVUIMask mask)
        {
            if (mask.target.rtTarget == null)
                return;

            PositionWorldObject(mask, true);
        }

        public override void FollowObject(HSVUIMask mask)
        {
            base.FollowObject(mask);
            PositionWorldObject(mask);
        }

        /// <summary>
        /// Positions world object using config setting
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="init"></param>
        private void PositionWorldObject(HSVUIMask mask, bool init = false)
        {
            if (subConfig != null)
            {
                if (mask.target.rtTarget != null)
                {
                    if (init)
                    {
                        mask.transform.localScale = subConfig.scale;
                    }

                    var rtBound = TransformExtensionUtility.CalculateBounds(mask.target.rtTarget.transform);
                    var anchor = GetAdjustAnchor(Mathf.Clamp(subConfig.anchor.x, 0, 1) - 0.5f, Mathf.Clamp(subConfig.anchor.y, 0, 1) - 0.5f, Mathf.Clamp(subConfig.anchor.z, 0, 1) - 0.5f);

                    Vector3 edgePos = rtBound.center;

                    edgePos.x += rtBound.size.x * anchor.x;
                    edgePos.y += rtBound.size.y * anchor.y;
                    edgePos.z += rtBound.size.z * anchor.z;

                    var direction = edgePos - rtBound.center;

                    mask.transform.position = direction.normalized * subConfig.magnitudeOffset + edgePos + subConfig.positionOffset;

                    if (subConfig.autoCalculateRotation)
                    {
                        var pointDirection = rtBound.center - mask.transform.position;
                        mask.transform.rotation = Quaternion.LookRotation(pointDirection);
                    }
                    else
                    {
                        mask.transform.rotation = Quaternion.Euler(subConfig.rotation.x, subConfig.rotation.y, subConfig.rotation.z);
                    }
                }
            }
        }

        private Vector3 GetAdjustAnchor(float x, float y, float z)
        {
            Vector3 result = Vector3.zero;
            var absX = Mathf.Abs(x);
            var absY = Mathf.Abs(y);
            var absZ = Mathf.Abs(z);
            if(absX >= absY && absX >= absZ)
            {
                result.x = Mathf.Sign(x) * 0.5f;
                result.y = y;
                result.z = z;
            }
            else if (absY >= absX && absY >= absZ)
            {
                result.x = x;
                result.y = Mathf.Sign(y) * 0.5f;
                result.z = z;
            }
            else if (absZ >= absX && absZ >= absY)
            {
                result.x = x;
                result.y = y;
                result.z = Mathf.Sign(z) * 0.5f;
            }

            return result;
        }
    }
}