using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
    [Serializable]
    public class HSVAdvanceConfig
    {
        //How the stage or tutorial should advance
        public AdvanceType advanceType;
        //if advance set to index, it will use index to advance
        public int index;
        //if advance set to name, it will use name to advance
        public string name;

        public HSVAdvanceConfig()
        {
            advanceType = AdvanceType.Automatic;
            index = -1;
            name = string.Empty;
        }

        public HSVAdvanceConfig(HSVAdvanceConfig config)
        {
            advanceType = config.advanceType;
            index = config.index;
            name = config.name;
        }
    }
}