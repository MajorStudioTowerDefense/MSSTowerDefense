using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    [ExecuteInEditMode]
    public class HSVTutorialModule : MonoBehaviour
    {
        //current playing state of the module, used by tutorial object to determine if module ends
        public PlayState state;
        //Indicates if module is shown
        public bool isShown;
        [HideInInspector]
        public HSVTutorialObject m_currentTObject; 
        [HideInInspector]
        public HSVModuleConfig config;

        public List<HSVUIMask> masks;

        private Canvas m_canvas;
        public Canvas Canvas
        {
            get { return m_canvas; }
        }

        private CanvasGroup m_canvasGroup;
        public CanvasGroup CanvasGroup
        {
            get { return m_canvasGroup; }
        }

        private GraphicRaycaster m_raycaster;
        public GraphicRaycaster Raycaster
        {
            get { return m_raycaster; }
        }

        private Coroutine fadeCor;

        #region Original Attribute Storage
        private float smoothVelocity;
        private bool raycasterEnable;
        private bool canvasGroupInteractable;
        private float canvasGroupAlpha;
        private bool canvasOverrideSorting;
        private int canvasSortingOrder;
        #endregion

        private void Awake()
        {
            AwakeOverride();
        }

        public virtual void AwakeOverride()
        {
            masks = new List<HSVUIMask>();
            m_canvas = GetComponent<Canvas>();

            if(m_canvas == null)
            {
                m_canvas = gameObject.AddComponent<Canvas>();
            }

            m_canvasGroup = m_canvas.gameObject.GetComponent<CanvasGroup>();

            if (m_canvasGroup == null)
            {
                m_canvasGroup = m_canvas.gameObject.AddComponent<CanvasGroup>();
                m_canvasGroup.interactable = false;
            }

            m_raycaster = m_canvas.gameObject.GetComponent<GraphicRaycaster>();
            if (m_raycaster == null)
            {
                m_raycaster = m_canvas.gameObject.AddComponent<GraphicRaycaster>();
                m_raycaster.enabled = false;
            }
        }

        public virtual void Start()
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnSpawn()
        {
            config = null;
            m_currentTObject = null;
            state = PlayState.Idle;
            isShown = false;
            fadeCor = null;
            raycasterEnable = Raycaster.enabled;
            canvasGroupInteractable = CanvasGroup.interactable;
            canvasGroupAlpha = CanvasGroup.alpha;
            canvasOverrideSorting = Canvas.overrideSorting;
            canvasSortingOrder = Canvas.sortingOrder;
        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnDespawn()
        {
            config = null;
            m_currentTObject = null;
            fadeCor = null;
            Raycaster.enabled = raycasterEnable;
            CanvasGroup.interactable = canvasGroupInteractable;
            CanvasGroup.alpha = canvasGroupAlpha;
            Canvas.overrideSorting = canvasOverrideSorting;
            Canvas.sortingOrder = canvasSortingOrder;
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                UpdateOverride();
            }
        }

        /// <summary>
        /// Override this method if inherits this class
        /// </summary>
        public virtual void UpdateOverride()
        {
            
        }

        /// <summary>
        /// method called when module starts
        /// </summary>
        public virtual void ModuleStart()
        {
            state = PlayState.Start;
        }

        /// <summary>
        /// method called when module ends
        /// </summary>
        public virtual void ModuleEnd()
        {
            FadeAction(false, true);
        }

        /// <summary>
        /// Sets the tutorial object the module is running for
        /// </summary>
        /// <param name="tObj"></param>
        public virtual void SetTutorialObject(HSVTutorialObject tObj)
        {
            m_currentTObject = tObj;
        }

        #region Module Running Actions
        public void FadeAction(bool fadeIn, bool setModuleEnd = false)
        {
            if (config != null && config.customTransition)
                return;

            if (Application.isPlaying)
            {
                if (isShown != fadeIn || setModuleEnd)
                {
                    if (state != PlayState.Ending)
                    {
                        if (fadeCor != null)
                        {
                            StopCoroutine(fadeCor);
                            //change state to executing if coroutine is interrupted by other actions
                            if(state == PlayState.Start)
                            {
                                state = PlayState.Executing;
                                HSVTutorialManager.Instance.UpdateTutorialState(m_currentTObject, PlayState.Executing);
                            }
                        }
                        state = setModuleEnd ? PlayState.Ending : state;
                        isShown = fadeIn;
                        fadeCor = StartCoroutine(FadeActionCoroutine(fadeIn, setModuleEnd));
                    }
                }
            }
            else
            {
                if(fadeIn)
                {
                    CanvasGroup.alpha = 1f;
                }
            }
        }

        private IEnumerator FadeActionCoroutine(bool fadeIn, bool setModuleEnd = false)
        {
            float targetFadeValue = fadeIn ? 1f : 0f;
            if (!config.instantTransition)
            {
                while (Mathf.Abs(targetFadeValue - CanvasGroup.alpha) > 0.01f)
                {
                    CanvasGroup.alpha = Mathf.SmoothDamp(CanvasGroup.alpha, targetFadeValue, ref smoothVelocity, config.fadeTime, config.m_smoothValue, Time.unscaledDeltaTime);
                    yield return null;
                }
            }

            if(state == PlayState.Start)
            {
                state = PlayState.Executing;
                HSVTutorialManager.Instance.UpdateTutorialState(m_currentTObject, PlayState.Executing);
            }

            CanvasGroup.alpha = fadeIn ? 1 : 0;
            CanvasGroup.interactable = fadeIn;

            if (setModuleEnd)
            {
                if (!fadeIn)
                {
                    EndModule();
                }
            }

            fadeCor = null;
        }

        public void EndModule()
        {
            for (int i = 0; i < masks.Count; i++)
            {
                if (masks[i].target != null && masks[i].target.rtTarget != null)
                {
                    masks[i].target.rtTarget.SetActive(masks[i].oriActiveState);
                }
            }

            ClearMasks();
            state = PlayState.End;

            CanvasGroup.interactable = false;
            Raycaster.enabled = false;
        }

        public void SetMaskStatus(HSVUIMask mask, bool enable)
        {
            if(mask != null && mask.gameObject.activeSelf != enable)
            {
                mask.gameObject.SetActive(enable);
            }
        }

        //Please make sure calling this after animation is ended;
        public void ClearMasks()
        {
            for (int i = 0; i < masks.Count; i++)
            {
                FreeMaskImage(masks[i]);
            }

            masks.Clear();
        }

        private void FreeMaskImage(HSVUIMask mask)
        {
            if (mask.transform != null)
            {
                HSVObjectPool.Instance.SetEntityAsFree(mask.transform);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// this is used on editor
        /// </summary>
        /// <param name="tObj"></param>
        public virtual void PreviewModule(HSVTutorialObject tObj)
        {
            SetTutorialObject(tObj);
            ModuleStart();
        }
#endif
    #endregion
    }
}