using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HSVStudio.Tutorial.Outline
{
    public class OutlineEffectsController : MonoBehaviour, IOutlineController
    {
        #region singleton
        private static OutlineEffectsController instance;
        public static OutlineEffectsController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<OutlineEffectsController>();

                    if (instance == null)
                    {
                        GameObject tutorialManager = new GameObject("OutlineController");
                        instance = tutorialManager.AddComponent<OutlineEffectsController>();
                    }

                }
                return instance;
            }
        }

        #endregion

        #region Outline Effects
        public Color outlineColor;
        public CameraEvent BufferDrawEvent = CameraEvent.BeforeImageEffects;
        #endregion

        public Camera m_camera;
        public IOutlineRendererCache outlineCache;
        [Header("Blur Settings")]
        [Range(0, 1)]
        public int Downsample = 1; // NOTE: downsampling will make things more efficient, as well as make the outline a bit thicker
        [Range(0.0f, 3.0f)]
        public float BlurSize = 1.0f;

        private CommandBuffer commandBuffer;

        private int m_outlineRTID, m_blurredRTID, m_temporaryRTID, m_depthRTID, m_idRTID;
        [SerializeField]
        private Material m_outlineMaterial;
        private int m_rtWidth = 512;
        private int m_rtHeight = 512;

        private void Awake()
        {
            commandBuffer = new CommandBuffer();
            commandBuffer.name = "HSVOutlineEffect Commandbuffer";
            m_depthRTID = Shader.PropertyToID("_DepthRT");
            m_outlineRTID = Shader.PropertyToID("_OutlineRT");
            m_blurredRTID = Shader.PropertyToID("_BlurredRT");
            m_temporaryRTID = Shader.PropertyToID("_TemporaryRT");
            m_idRTID = Shader.PropertyToID("_idRT");
            if (m_camera == null)
            {
                m_camera = Camera.main;
            }

            if (m_outlineMaterial == null)
            {
                m_outlineMaterial = new Material(Shader.Find("Hidden/UnityOutline"));
            }

            if(m_camera != null)
            {
                m_camera.depthTextureMode = DepthTextureMode.Depth;
                m_camera.AddCommandBuffer(BufferDrawEvent, commandBuffer);

                m_rtWidth = m_camera.pixelWidth;
                m_rtHeight = m_camera.pixelHeight;
            }

            outlineCache = GetComponentInChildren<IOutlineRendererCache>();
            if (outlineCache == null)
            {
                var cache = (new GameObject("OutlineCache")).AddComponent<OutlineRendererCache>();
                cache.transform.SetParent(this.transform);
                outlineCache = cache;
            }
        }

        private void Start()
        {
            if(GraphicsSettings.renderPipelineAsset != null)
            {
                //it is using SRP, we don need this
                Destroy(this);
                return;
            }
        }

        public void OutlineObjects(GameObject[] gameObjects)
        {
            if (outlineCache != null)
            {
                IList<Renderer> renderers = GetRenderers(gameObjects);
                outlineCache.AddRenderer(renderers.ToArray());
            }
            RecreateCommandBuffer();
        }

        public void RemoveObjects(GameObject[] gameObjects)
        {
            if (outlineCache != null)
            {
                IList<Renderer> renderers = GetRenderers(gameObjects);
                outlineCache.RemoveRenderer(renderers.ToArray());
            }
            RecreateCommandBuffer();
        }

        private IList<Renderer> GetRenderers(IList<GameObject> gameObjects)
        {
            List<Renderer> result = new List<Renderer>();

            for (int i = 0; i < gameObjects.Count; i++)
            {
                var renderers = gameObjects[i].GetComponentsInChildren<Renderer>();

                for (int j = 0; j < renderers.Length; j++)
                {
                    if (renderers[j].gameObject.activeInHierarchy && (renderers[j].gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)
                    {
                        result.Add(renderers[j]);
                    }
                }
            }

            return result;
        }

        public void RecreateCommandBuffer()
        {
            if (m_camera == null || commandBuffer == null)
            {
                return;
            }

            RenderTargetIdentifier depthRTID = BuiltinRenderTextureType.Depth;
            if (m_camera.targetTexture != null && GraphicsSettings.renderPipelineAsset == null)
            {
                m_rtWidth = Screen.width;
                m_rtHeight = Screen.height;
                depthRTID = BuiltinRenderTextureType.CurrentActive;
            }
            else
            {
                m_rtWidth = m_camera.pixelWidth;
                m_rtHeight = m_camera.pixelHeight;
            }

            commandBuffer.Clear();

            if (outlineCache.Renderers.Count == 0)
            {
                return;
            }


            FilterMode filterMode = FilterMode.Point;
            // initialization
            commandBuffer.GetTemporaryRT(m_depthRTID, m_rtWidth, m_rtHeight, 0, filterMode, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            commandBuffer.SetRenderTarget(m_depthRTID, depthRTID);
            commandBuffer.ClearRenderTarget(false, true, Color.clear);

            // render selected objects into a mask buffer, with different colors for visible vs occluded ones 
            float id = 0f;
            for (int i = 0; i < outlineCache.Renderers.Count; i++)
            {
                Renderer renderer = outlineCache.Renderers[i];
                if (renderer != null)
                {
                    if (((1 << renderer.gameObject.layer) & m_camera.cullingMask) != 0 && renderer.enabled)
                    {
                        id += 0.25f;
                        commandBuffer.SetGlobalFloat("_ObjectId", id);
                        int submeshCount = renderer.sharedMaterials.Length;
                        for (int s = 0; s < submeshCount; ++s)
                        {
                            commandBuffer.DrawRenderer(renderer, m_outlineMaterial, s, 1);
                            commandBuffer.DrawRenderer(renderer, m_outlineMaterial, s, 0);
                        }
                    }
                }
                else
                {
                    outlineCache.Renderers.Remove(renderer);
                }

            }

            // object ID edge dectection pass
            commandBuffer.GetTemporaryRT(m_idRTID, m_rtWidth, m_rtHeight, 0, filterMode, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            commandBuffer.Blit(m_depthRTID, m_idRTID, m_outlineMaterial, 3);

            // Blur
            int rtW = m_rtWidth >> Downsample;
            int rtH = m_rtHeight >> Downsample;

            commandBuffer.GetTemporaryRT(m_temporaryRTID, rtW, rtH, 0, filterMode, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            commandBuffer.GetTemporaryRT(m_blurredRTID, rtW, rtH, 0, filterMode, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

            commandBuffer.Blit(m_idRTID, m_blurredRTID);

            commandBuffer.SetGlobalVector("_BlurDirection", new Vector2(BlurSize, 0));
            commandBuffer.Blit(m_blurredRTID, m_temporaryRTID, m_outlineMaterial, 2);
            commandBuffer.SetGlobalVector("_BlurDirection", new Vector2(0, BlurSize));
            commandBuffer.Blit(m_temporaryRTID, m_blurredRTID, m_outlineMaterial, 2);

            // final overlay
            commandBuffer.SetGlobalColor("_OutlineColor", outlineColor);
            commandBuffer.Blit(m_blurredRTID, BuiltinRenderTextureType.CameraTarget, m_outlineMaterial, 4);

            // release tempRTs
            commandBuffer.ReleaseTemporaryRT(m_blurredRTID);
            commandBuffer.ReleaseTemporaryRT(m_outlineRTID);
            commandBuffer.ReleaseTemporaryRT(m_temporaryRTID);
            commandBuffer.ReleaseTemporaryRT(m_depthRTID);
        }
    }
}