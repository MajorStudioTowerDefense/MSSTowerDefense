using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HSVStudio.Tutorial
{
    [Serializable]
    public class HSVTutorialStage
    {
        //stage index, starting from 1
        public int index;
        //stage name
        public string stageName;
        //stage state
        public PlayState state;
        //trigger to start stage
        public HSVTriggerConfig startTrigger;
        //trigger to end stage
        public HSVTriggerConfig endTrigger;
        //advancing config
        public HSVAdvanceConfig advanceConfig;
        //Array of events for stage
        public HSVEvent[] events;
        //tutorial playing step
        public int currentStep;
        //auto start tutorial object index
        public int startObjectIndex;
        //should allow multiple tutorial objects playing
        public bool allowMultiple;
        public HSVTutorialObject[] tutorialObjects;
        [NonSerialized]
        private Dictionary<HSVEventType, HSVEvent> m_allEvents;
        public Dictionary<HSVEventType, HSVEvent> AllEvents
        {
            get { return m_allEvents; }
        }

        public HSVTutorialStage()
        {
            state = PlayState.Idle;
            startTrigger = new HSVTriggerConfig();
            endTrigger = new HSVTriggerConfig();
            events = new HSVEvent[0];
            currentStep = 1;
            startObjectIndex = -1;
            allowMultiple = false;
            tutorialObjects = new HSVTutorialObject[0];
        }

        public HSVTutorialStage(HSVTutorialStage stageObj)
        {
            index = stageObj.index;
            stageName = stageObj.stageName;
            state = stageObj.state;
            startTrigger = new HSVTriggerConfig(stageObj.startTrigger);
            endTrigger = new HSVTriggerConfig(stageObj.endTrigger);
            events = HSVEvent.CopyEvents(stageObj.events);
            currentStep = stageObj.currentStep;
            startObjectIndex = stageObj.startObjectIndex;
            allowMultiple = stageObj.allowMultiple;
            tutorialObjects = new HSVTutorialObject[stageObj.tutorialObjects.Length];
            for (int i = 0; i < stageObj.tutorialObjects.Length; i++)
            {
                tutorialObjects[i] = new HSVTutorialObject(stageObj.tutorialObjects[i]);
            }
        }

        public void InitializedEvents()
        {
            if(m_allEvents == null)
            {
                m_allEvents = new Dictionary<HSVEventType, HSVEvent>();
            }
            m_allEvents.Clear();
            for(int i = 0; i < events.Length; i++)
            {
                if(!m_allEvents.ContainsKey(events[i].type))
                {
                    m_allEvents.Add(events[i].type, events[i]);
                }
            }
        }
    }
}