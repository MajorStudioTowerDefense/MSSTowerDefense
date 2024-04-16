using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    /// <summary>
    /// Trigger object would added to target to trigger event
    /// </summary>
    public class HSVTriggerObject : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IHSVPointerClick, IHSVPointerDown, IHSVPointerUp
    {
        private HSVTriggerConfig config;
        private int stageIndex;
        private int tObjIndex;
        private bool startTrigger;
        public bool IsStartTrigger
        {
            get { return startTrigger; }
        }
        private bool isStage;
        public bool IsStage
        {
            get { return isStage; }
        }

        private bool touchDown = false;
        private bool enteredUI = false;
        private Vector3 previousTouchPos;

        private void Update()
        {
            //Due to unity input system, pointer up event would not be initiated if it is not pointer down on save graphic. This is the solution for user trying to drag UI element on to this graphic
            if (config != null)
            {
                if (config.triggerType == TriggerType.KeyCode)
                {
                    if (HSVInput.GetKeyDown(config.keyCode))
                    {
                        PlayAction();
                    }
                }
                else if ((config.triggerType == TriggerType.UI && config.graphicConfig.graphic != null && config.graphicConfig.pointerTrigger == PointerTrigger.PointerUp))
                {
                    //Pointer Down, Pointer Click and Pointer Up solution
                    //There is touch down on the screen, we check for release
                    if (HSVInput.GetMouseButtonDown(0) || HSVInput.GetMouseButton(0) || (HSVInput.touchSupported && HSVInput.touchCount > 0))
                    {
                        touchDown = true;
                        if (HSVInput.GetMouseButtonDown(0) || HSVInput.GetMouseButton(0))
                        {
                            previousTouchPos = HSVInput.mousePosition;
                        }
                        else if (HSVInput.touchSupported && HSVInput.touchCount > 0)
                        {
                            previousTouchPos = HSVInput.GetTouchPosition(0);
                        }
                    }
                    else
                    {
                        //touch is released
                        if (touchDown)
                        {
                            if (config.triggerType == TriggerType.UI)
                            {
                                if (enteredUI || RectTransformUtility.RectangleContainsScreenPoint(config.graphicConfig.graphic.rectTransform, previousTouchPos))
                                {
                                    PlayAction();
                                }
                            }
                            touchDown = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setup trigger object configuration for collider object
        /// </summary>
        /// <param name="config"></param>
        /// <param name="stageIndex"></param>
        /// <param name="tObjIndex"></param>
        /// <param name="start"></param>
        /// <param name="isStage"></param>
        public void SetupTriggerConfig(HSVTriggerConfig config, int stageIndex, int tObjIndex, bool start, bool isStage = false)
        {
            if (config != null)
            {
                this.config = config;
                this.stageIndex = stageIndex;
                this.tObjIndex = tObjIndex;
                this.isStage = isStage;
                startTrigger = start;
            }
            else
            {
                HSVTutorialManager.DebugLogWarning("Config file for stage index: " + stageIndex + " with tutorial index: " + tObjIndex + " for target is invalid when setting trigger config");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (config != null && config.triggerType == TriggerType.Collider)
            {
                var gameObj = (config.colliderConfig.tagFiltering && config.colliderConfig.useRigidBodyTag) ? other.attachedRigidbody.gameObject : other.gameObject;
                if (CheckTriggerEvent(gameObj))
                {
                    PlayAction();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (config != null && config.triggerType == TriggerType.Collider)
            {
                var gameObj = (config.colliderConfig.tagFiltering && config.colliderConfig.useRigidBodyTag) ? collision.rigidbody.gameObject : collision.gameObject;
                if (CheckTriggerEvent(gameObj))
                {
                    PlayAction();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (config != null)
            {
                if (config.graphicConfig.graphic != null && config.graphicConfig.pointerTrigger == PointerTrigger.PointerClick)
                {
                    PlayAction();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            enteredUI = true;
            if (config != null)
            {
                if (config.graphicConfig.graphic != null && config.graphicConfig.pointerTrigger == PointerTrigger.PointerDown)
                {
                    PlayAction();
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            enteredUI = true;
            if (config != null)
            {
                if (config.graphicConfig.graphic != null && config.graphicConfig.pointerTrigger == PointerTrigger.PointerEnter)
                {
                    PlayAction();
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enteredUI = false;
            if (config != null)
            {
                if (config.graphicConfig.graphic != null && config.graphicConfig.pointerTrigger == PointerTrigger.PointerExit)
                {
                    PlayAction();
                }
            }
        }

        #region Scene Object Interface
        public void OnPointerDown()
        {
            if (config != null)
            {
                if (config.colliderConfig.collider != null && config.colliderConfig.triggerOnClick && config.colliderConfig.pointerTrigger == PointerTrigger.PointerDown)
                {
                    PlayAction();
                }
            }
        }

        public void OnPointerClick()
        {
            if (config != null)
            {
                if (config.colliderConfig.collider != null && config.colliderConfig.triggerOnClick && config.colliderConfig.pointerTrigger == PointerTrigger.PointerClick)
                {
                    PlayAction();
                }
            }
        }

        public void OnPointerUp()
        {
            if (config != null)
            {
                if (config.colliderConfig.collider != null && config.colliderConfig.triggerOnClick && config.colliderConfig.pointerTrigger == PointerTrigger.PointerUp)
                {
                    PlayAction();
                }
            }
        }
        #endregion

        /// <summary>
        /// Checks if should trigger event
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CheckTriggerEvent(GameObject obj)
        {
            if (!config.colliderConfig.triggerOnClick)
            {
                if (config.colliderConfig.layerFiltering)
                {
                    if (config.colliderConfig.filterLayer != (config.colliderConfig.filterLayer | (1 << obj.layer)))
                    {
                        return false;
                    }
                }

                if (config.colliderConfig.tagFiltering)
                {
                    if (config.colliderConfig.filterTag != obj.tag)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Play the specified action
        /// </summary>
        private void PlayAction()
        {
            if (startTrigger)
            {
                if (isStage)
                {
                    HSVTutorialManager.Instance?.PlayStage(stageIndex);
                }
                else
                {
                    HSVTutorialManager.Instance?.PlayTutorial(stageIndex, tObjIndex);
                }
            }
            else
            {
                if (isStage)
                {
                    HSVTutorialManager.Instance?.StopStage();
                }
                else
                {
                    HSVTutorialManager.Instance?.StopTutorial(stageIndex, tObjIndex);
                }
            }
        }
    }
}