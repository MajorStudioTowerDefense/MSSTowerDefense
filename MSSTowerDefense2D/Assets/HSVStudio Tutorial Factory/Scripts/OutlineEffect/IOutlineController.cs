using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial.Outline
{
    /// <summary>
    /// Outline interface to do the highlight effect
    /// </summary>
    public interface IOutlineController
    {
        void OutlineObjects(GameObject[] gameObjects);

        void RemoveObjects(GameObject[] gameObjects);
    }
}