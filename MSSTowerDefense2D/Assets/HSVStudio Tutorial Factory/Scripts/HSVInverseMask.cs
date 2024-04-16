using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace HSVStudio.Tutorial.UI
{
    public class HSVInverseMask : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material overrideMat = new Material(base.materialForRendering);
                overrideMat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return overrideMat;

            }
        }
    }
}
