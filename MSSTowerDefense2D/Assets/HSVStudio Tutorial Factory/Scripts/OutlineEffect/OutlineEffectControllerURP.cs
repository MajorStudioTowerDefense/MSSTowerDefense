using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace HSVStudio.Tutorial.Outline
{
	/// <summary>
	/// This is URP version outline effect
	/// </summary>
    public class OutlineEffectControllerURP : MonoBehaviour, IOutlineController
    {
		#region singleton
		private static OutlineEffectControllerURP instance;
		public static OutlineEffectControllerURP Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<OutlineEffectControllerURP>();

					if (instance == null)
					{
						GameObject tutorialManager = new GameObject("OutlineEffectControllerURP");
						instance = tutorialManager.AddComponent<OutlineEffectControllerURP>();
					}

				}
				return instance;
			}
		}
		#endregion
		
		public IOutlineRendererCache outlineCache;

        private void Start()
        {
			if (GraphicsSettings.renderPipelineAsset == null)
			{
				//it is not SRP, we don need this
				Destroy(this);
				return;
			}

			outlineCache = GetComponentInChildren<IOutlineRendererCache>();
			if(outlineCache == null)
            {
				var cache = (new GameObject("OutlineCacheURP")).AddComponent<OutlineRendererCache>();
				cache.transform.SetParent(this.transform);
				outlineCache = cache;
			}
        }

		#region Interface
		public void OutlineObjects(GameObject[] gameObjects)
        {
			if (outlineCache != null)
			{
				IList<Renderer> renderers = GetRenderers(gameObjects);
				outlineCache.AddRenderer(renderers.ToArray());
			}
		}

        public void RemoveObjects(GameObject[] gameObjects)
        {
			if (outlineCache != null)
			{
				IList<Renderer> renderers = GetRenderers(gameObjects);
				outlineCache.RemoveRenderer(renderers.ToArray());
			}
		}

		private IList<Renderer> GetRenderers(IList<GameObject> gameObjects)
		{
			List<Renderer> result = new List<Renderer>();

			for (int i = 0; i < gameObjects.Count; i++)
			{
				var renderers = gameObjects[i].GetComponentsInChildren<Renderer>();

				for(int j = 0; j < renderers.Length; j++)
                {
					if (renderers[j].gameObject.activeInHierarchy && (renderers[j].gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)
					{
						result.Add(renderers[j]);
					}
				}
			}

			return result;
		}
		#endregion
	}
}