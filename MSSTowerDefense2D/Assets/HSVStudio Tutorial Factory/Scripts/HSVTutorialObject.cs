using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HSVStudio.Tutorial
{
    [Serializable]
    public class HSVTutorialObject
    {
        //current tutorial object index, starting from 1
        public int index;
        //Stage index the tutorial object belongs to
        public int stageIndex;
        //name of tutorial object
        public string Name;
        //runtime state
        public PlayState state;
        //trigger to start tutorial
        public HSVTriggerConfig startTrigger;
        //trigger to end tutorial
        public HSVTriggerConfig endTrigger;
        //advancing configuration
        public HSVAdvanceConfig advanceConfig;
        //Array of events for stage
        public HSVEvent[] events;
        //tracking targets
        public HSVTarget[] focusTargets;
        //module configs used by tutorial
        [SerializeReference]
        public HSVModuleConfig[] moduleConfigs;
        //Reorderablelist has some bug that need this field to temporary fix that
        public bool test;
        [NonSerialized]
        private Dictionary<HSVEventType, HSVEvent> m_allEvents;
        public Dictionary<HSVEventType, HSVEvent> AllEvents
        {
            get { return m_allEvents; }
        }

        #region Initialization
        public HSVTutorialObject()
        {
            SetupObject();
        }

        public HSVTutorialObject(int index)
        {
            SetupObject();
            this.index = index;
        }

        public HSVTutorialObject(int stageIndex, int index)
        {
            SetupObject();
            this.stageIndex = stageIndex;
            this.index = index;
        }

        //Clones the tutorial object
        public HSVTutorialObject(HSVTutorialObject obj)
        {
            index = obj.index;
            stageIndex = obj.stageIndex;
            Name = obj.Name;
            state = obj.state;
            startTrigger = new HSVTriggerConfig(obj.startTrigger);
            endTrigger = new HSVTriggerConfig(obj.endTrigger);
            advanceConfig = new HSVAdvanceConfig(obj.advanceConfig);
            events = HSVEvent.CopyEvents(obj.events);
            focusTargets = new HSVTarget[obj.focusTargets.Length];
            for(int i = 0; i < obj.focusTargets.Length;i++)
            {
                focusTargets[i] = HSVTutorialManager.JsonCopy(obj.focusTargets[i]);
                focusTargets[i].triggerConfig = new HSVTriggerConfig(obj.focusTargets[i].triggerConfig);
            }

            moduleConfigs = new HSVModuleConfig[obj.moduleConfigs.Length];
            for (int i = 0; i < obj.moduleConfigs.Length; i++)
            {
                var type = obj.moduleConfigs[i].GetType();

                moduleConfigs[i] = obj.moduleConfigs[i].Clone();
            }
        }

        /// <summary>
        /// Setup the tutorial object
        /// </summary>
        private void SetupObject()
        {
            state = PlayState.Idle;
            focusTargets = new HSVTarget[0];
            moduleConfigs = new HSVModuleConfig[0];
            startTrigger = new HSVTriggerConfig();
            endTrigger = new HSVTriggerConfig();
            advanceConfig = new HSVAdvanceConfig();
            events = new HSVEvent[0];
        }
        #endregion

        private bool forceUpdate = false;
        #region Tutorial Playing Section
        /// <summary>
        /// Updates the screen rect size for the tracking target
        /// </summary>
        public void UpdateTargetScreenRect()
        {
            forceUpdate = false;
            if (HSVTutorialManager.Instance != null && HSVTutorialManager.Instance.CheckCameraStateChange())
            {
                forceUpdate = true;
            }

            for (int i = 0; i < focusTargets.Length; i++)
            {
                if (focusTargets[i].rtTarget != null && focusTargets[i].rtTarget.activeInHierarchy)
                {
                    focusTargets[i].UpdateTargetScreenRect(forceUpdate);
                }
            }
        }
        #endregion

        #region Helper Function
        /// <summary>
        /// Helper method to get module config type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public T GetModuleConfig<T>(Type type) where T : HSVModuleConfig
        {
            for (int i = 0; i < moduleConfigs.Length; i++)
            {
                if ((moduleConfigs[i] is T) && moduleConfigs[i].GetType().Name.Contains(type.Name))
                {
                    return moduleConfigs[i] as T;
                }
            }

            return null;
        }

        public void InitializedEvents()
        {
            if (m_allEvents == null)
            {
                m_allEvents = new Dictionary<HSVEventType, HSVEvent>();
            }
            m_allEvents.Clear();
            for (int i = 0; i < events.Length; i++)
            {
                if (!m_allEvents.ContainsKey(events[i].type))
                {
                    m_allEvents.Add(events[i].type, events[i]);
                }
            }
        }
        #endregion
    }
}