using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    [Serializable]
    public class HSVTriggerConfig
    {
        //Which trigger type should be used
        public TriggerType triggerType;
        //if set to collider, it will use this config
        public HSVColliderConfig colliderConfig;
        //if set to UIClick, it will use this reference
        public HSVGraphicConfig graphicConfig;
        //if set to keycode, when key is pressed, it triggers event
        public KeyCode keyCode;

        public HSVTriggerConfig()
        {
            triggerType = TriggerType.Manual;
            colliderConfig = new HSVColliderConfig();
            graphicConfig = new HSVGraphicConfig();
            keyCode = KeyCode.None;
        }

        public HSVTriggerConfig(HSVTriggerConfig trigger)
        {
            triggerType = trigger.triggerType;
            colliderConfig = HSVTutorialManager.JsonCopy(trigger.colliderConfig);
            graphicConfig = HSVTutorialManager.JsonCopy(trigger.graphicConfig);
            keyCode = trigger.keyCode;
        }
    }

    [Serializable]
    public class HSVColliderConfig
    {
        public Collider collider;
        public bool triggerOnClick;
        public bool layerFiltering;
        public LayerMask filterLayer;
        public bool tagFiltering;
        public string filterTag;
        public bool useRigidBodyTag;
        public PointerTrigger pointerTrigger;

        public HSVColliderConfig()
        {
            triggerOnClick = false;
            layerFiltering = false;
            tagFiltering = false;
            useRigidBodyTag = false;
            pointerTrigger = PointerTrigger.PointerClick;
        }

        public HSVColliderConfig(HSVColliderConfig colliderConfig)
        {
            collider = colliderConfig.collider;
            triggerOnClick = colliderConfig.triggerOnClick;
            layerFiltering = colliderConfig.layerFiltering;
            filterLayer = colliderConfig.filterLayer;
            tagFiltering = colliderConfig.tagFiltering;
            filterTag = colliderConfig.filterTag;
            useRigidBodyTag = colliderConfig.useRigidBodyTag;
            pointerTrigger = colliderConfig.pointerTrigger;
        }
    }

    [Serializable]
    public class HSVGraphicConfig
    {
        public Graphic graphic;
        public PointerTrigger pointerTrigger;

        public HSVGraphicConfig()
        {
            graphic = null;
            pointerTrigger = PointerTrigger.PointerClick;
        }
    }
}