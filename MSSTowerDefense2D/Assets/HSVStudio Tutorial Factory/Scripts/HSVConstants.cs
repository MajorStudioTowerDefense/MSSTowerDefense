using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    public enum PlayState
    {
        Idle,
        Start,
        Executing,
        Ending,
        End
    }

    public enum ModuleType
    {
        None,
        Custom,
        Highlight,
        FocusDim,
        PopUp,
        Arrow,
        Info
    }

    public enum TriggerType
    {
        Manual,
        Collider,
        UI,
        KeyCode
    }

    public enum PointerTrigger
    {
        PointerDown,
        PointerUp,
        PointerClick,
        PointerEnter,
        PointerExit
    }

    public enum AdvanceType
    {
        None,
        Automatic,
        Index,
        Name
    }

    public enum Unit
    {
        Meter,
        KiloMeter,
        Mile,
        Feet
    }

#if UNITY_EDITOR
    public class HSVModuleConfigIndex
    {
        public int stageIndex;
        public int tObjIndex;
        public int index;
    }
#endif
}