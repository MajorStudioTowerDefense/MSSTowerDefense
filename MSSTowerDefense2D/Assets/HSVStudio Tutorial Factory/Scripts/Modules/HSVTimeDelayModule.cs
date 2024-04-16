using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// This doesn't spawn any mask, but do a delay action, it would call StopTutorial method when specified time elapsed
    /// </summary>
    public class HSVTimeDelayModule : HSVTimeModule
    {
        private int timerID = -1;

        public override void ModuleStart()
        {
            base.ModuleStart();
            timerID = -1;
            if (Application.isPlaying)
            {
                var subConfig = config as HSVTimeDelayModuleConfig;
                if (subConfig != null)
                {
                    timerID = HSVMainTimer.time.AddTimer(subConfig.delayTime, 1, StopTutorial);
                }
            }
        }

        public override void ModuleEnd()
        {
            if(timerID != -1)
            {
                HSVMainTimer.time.RemoveTimer(timerID);
            }
            base.ModuleEnd();
        }

        public override void CreateMask()
        {

        }

        private void StopTutorial()
        {
            var subConfig = config as HSVTimeDelayModuleConfig;
            if(subConfig != null && subConfig.autoStop)
            {
                HSVTutorialManager.Instance?.StopTutorial(m_currentTObject.index);
            }
        }
    }
}