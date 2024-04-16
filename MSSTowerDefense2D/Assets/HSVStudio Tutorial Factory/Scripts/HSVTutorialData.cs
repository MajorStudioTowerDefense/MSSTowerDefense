using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// Stores the data of the tutorial manager
    /// </summary>
    [Serializable]
    public class HSVTutorialData : ScriptableObject
    {
        public bool assignOnRuntime;
        public ExposedReference<Camera> targetCamera;
        public ExposedReference<Canvas> targetCanvas;
        public bool autoStart = false;
        public int autoStartIndex = -1;
        public bool debugMode = false;
        public int currentStageStep = 1;
        public List<HSVTutorialStage> stageObjects = new List<HSVTutorialStage>();
        public List<HSVTargetSaveInfo> targetInfo = new List<HSVTargetSaveInfo>();
        public List<HSVTriggerSaveInfo> triggerInfo = new List<HSVTriggerSaveInfo>();
    }

    [Serializable]
    public class HSVLoopUpInfo
    {
        public int stageIndex;
        public int tObjIndex;
        public int targetIndex;
    }

    [Serializable]
    public class HSVTargetSaveInfo
    {
        public HSVLoopUpInfo info;
        public ExposedReference<GameObject> target;
    }

    [Serializable]
    public class HSVTriggerSaveInfo
    {
        public bool isStage;
        public bool isStart;
        public bool isTarget;
        public HSVLoopUpInfo info;
        public ExposedReference<Collider> collider;
        public ExposedReference<Graphic> graphic;
    }

    [Serializable]
    public class HSVEventSaveInfo
    {
        public bool isStageEvent;
        public HSVLoopUpInfo info;
        public int eventIndex;
        public string methodName;
        public ExposedReference<Object> target;
    }
}