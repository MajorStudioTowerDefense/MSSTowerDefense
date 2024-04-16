using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HSVStudio.Tutorial
{
    [Serializable]
    public class HSVEvent
    {
        public HSVEventType type;
        public bool enable;
        public UnityEvent subEvent;

        public HSVEvent()
        {
            type = HSVEventType.OnStart;
            enable = false;
            subEvent = new UnityEvent();
        }

        public HSVEvent(HSVEvent oldEvent)
        {
            type = oldEvent.type;
            enable = oldEvent.enable;
            subEvent = HSVTutorialManager.JsonCopy<UnityEvent>(oldEvent.subEvent);
        }

        public static void CreateEvents(ref HSVEvent[] events)
        {
            var eventTypes = (HSVEventType[])Enum.GetValues(typeof(HSVEventType));
            Array.Sort(eventTypes, (a, b) => a.CompareTo(b));

            if (events == null)
            {
                events = new HSVEvent[eventTypes.Length];
            }
            else
            {
                Array.Sort(events, (a, b) => a.type.CompareTo(b.type));

                if (events.Length != eventTypes.Length)
                {
                    Array.Resize(ref events, eventTypes.Length);
                }
            }

            for(int i = 0; i < events.Length; i++)
            {
                if(events[i] == null)
                {
                    var subEvent = new HSVEvent()
                    {
                        type = eventTypes[i],
                        enable = false,
                        subEvent = new UnityEvent()
                    };
                    events[i] = subEvent;
                }
            }
        }

        public static HSVEvent[] CopyEvents(HSVEvent[] events)
        {
            var newArray = new HSVEvent[events.Length];
            for(int i = 0; i < events.Length; i++)
            {
                newArray[i] = new HSVEvent(events[i]);
            }
            return newArray;
        }

        public static HSVEvent GetEvent(Dictionary<HSVEventType, HSVEvent> events, HSVEventType type)
        {
            if(events != null && events.ContainsKey(type))
            {
                return events[type];
            }
            return null;
        }

        public static HSVEvent GetEvent(Dictionary<HSVEventType, HSVEvent> events, PlayState state)
        {
            var type = HSVEventType.OnStart;
            switch(state)
            {
                case PlayState.Idle:
                    break;
                case PlayState.Start:
                    break;
                case PlayState.Executing:
                    type = HSVEventType.OnExecuting;
                    break;
                case PlayState.Ending:
                    type = HSVEventType.OnEnding;
                    break;
                case PlayState.End:
                    type = HSVEventType.OnComplete;
                    break;
            }

            return GetEvent(events, type);
        }
    }

    public enum HSVEventType
    {
        OnStart,
        OnUpdate,
        OnExecuting,
        OnEnding,
        OnComplete
    }

    public delegate void HSVGlobalStageEvent(HSVTutorialStage stageObj);
    public delegate void HSVGlobalTutorialEvent(HSVTutorialObject tObj);
    public delegate void HSVUIMaskEvent(HSVTutorialModule module, HSVUIMask mask);
}