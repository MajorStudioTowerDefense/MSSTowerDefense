using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial.Outline
{
    /// <summary>
    /// This is used to store the highlight renderers
    /// </summary>
    public interface IOutlineRendererCache
    {
        IList<Renderer> Renderers
        {
            get;
        }

        void AddRenderer(Renderer[] renderers);

        void AddRenderer(Renderer renderer);

        void RemoveRenderer(Renderer[] renderers);

        void RemoveRenderer(Renderer renderer);
    }

    public class OutlineRendererCache : MonoBehaviour, IOutlineRendererCache
    {
        private readonly List<Renderer> m_renderers = new List<Renderer>();
        public IList<Renderer> Renderers
        {
            get { return m_renderers; }
        }

        public void AddRenderer(Renderer[] renderers)
        {
            for(int i = 0; i < renderers.Length; i++)
            {
                AddRenderer(renderers[i]);
            }
        }

        public void AddRenderer(Renderer renderer)
        {
            if (!m_renderers.Contains(renderer))
            {
                m_renderers.Add(renderer);
            }
        }

        public void RemoveRenderer(Renderer[] renderers)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                RemoveRenderer(renderers[i]);
            }
        }

        public void RemoveRenderer(Renderer renderer)
        {
            if (renderer != null && m_renderers.Contains(renderer))
            {
                m_renderers.Remove(renderer);
            }
        }
    }
}