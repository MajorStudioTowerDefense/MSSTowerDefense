using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// Modules that will still run if there is no target
    /// </summary>
    public class HSVTimeModule : HSVTutorialModule
    {
        public override void AwakeOverride()
        {
            base.AwakeOverride();
        }

        public override void UpdateOverride()
        {
            base.UpdateOverride();
        }

        public override void SetTutorialObject(HSVTutorialObject tObj)
        {
            base.SetTutorialObject(tObj);
            CreateMask();
        }

        public override void ModuleStart()
        {
            base.ModuleStart();
            if (config == null)
            {
                HSVTutorialManager.DebugLogWarning("Please check if the module config is added to the Tutorial Object");
                return;
            }

            for (int i = 0; i < masks.Count; i++)
            {
                if (!masks[i].gameObject.activeSelf)
                {
                    masks[i].gameObject.SetActive(true);
                    ConfigMask(masks[i]);
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
            base.ModuleEnd();
        }

        /// <summary>
        /// Creates one mask only
        /// </summary>
        public virtual void CreateMask()
        {
            var maskObj = HSVObjectPool.Instance.GetFreeEntity(config.maskPrefab);
            if (maskObj != null)
            {
                var mask = maskObj.GetComponent<HSVUIMask>();
                if (mask != null)
                {
                    if (config.m_sprite != null)
                    {
                        if (mask.mask.sprite == null)
                        {
                            mask.mask.sprite = config.m_sprite;
                        }
                        else
                        {
                            mask.mask.overrideSprite = config.m_sprite;
                        }
                    }

                    maskObj.transform.SetParent(Canvas.transform);
                    maskObj.transform.localPosition = Vector3.zero;
                    maskObj.transform.localRotation = Quaternion.identity;
                    maskObj.transform.localScale = Vector3.one;
                    if (!masks.Contains(mask))
                    {
                        masks.Add(mask);
                    }

                    maskObj.gameObject.SetActive(false);
                }
                else
                {
                    HSVObjectPool.Instance.SetEntityAsFree(maskObj.transform);
                }
            }
        }

        /// <summary>
        /// Configure the mask
        /// </summary>
        /// <param name="mask"></param>
        public virtual void ConfigMask(HSVUIMask mask)
        {

        }
    }
}