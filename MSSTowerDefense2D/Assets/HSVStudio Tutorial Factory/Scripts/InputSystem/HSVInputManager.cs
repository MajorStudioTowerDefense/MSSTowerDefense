using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HSVStudio.Tutorial
{
    public class HSVInputManager : MonoBehaviour
    {
        public static HSVInputManager instance;

        private HashSet<IHSVPointerClick> pointerClicks;
        private HashSet<IHSVPointerDown> pointerDowns;
        private HashSet<IHSVPointerUp> pointerUps;

        private EventSystem m_eventSystem;

        private HashSet<GameObject> clickDownObjects;
        private RaycastHit[] raycastHits = new RaycastHit[5];
        private bool touchDown = false;
        private Vector3 previousScreenPos;

        private void Awake()
        {
            instance = this;

            m_eventSystem = FindObjectOfType<EventSystem>();
            if (m_eventSystem == null)
            {
                m_eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
                m_eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                m_eventSystem.gameObject.AddComponent<StandaloneInputModule>();
#endif
            }

            pointerClicks = new HashSet<IHSVPointerClick>();
            pointerDowns = new HashSet<IHSVPointerDown>();
            pointerUps = new HashSet<IHSVPointerUp>();
            clickDownObjects = new HashSet<GameObject>();
        }

        private void OnEnable()
        {
            touchDown = false;
            clickDownObjects.Clear();
        }

        private void OnDisable()
        {
            touchDown = false;
        }

        private void Update()
        {
            if(HSVInput.GetMouseButtonDown(0) || (HSVInput.touchSupported && HSVInput.touchCount > 0 && !touchDown))
            {
                touchDown = true;
                clickDownObjects.Clear();
                pointerDowns.Clear();
                pointerClicks.Clear();
                //Detects click down or touch down
                var hitCount = Raycast(HSVInput.GetPointerPosition());
                if(hitCount > 0)
                {
                    for(int i = 0; i < hitCount; i++)
                    {
                        var ptrDown = raycastHits[i].collider.gameObject.GetComponentsInChildren<IHSVPointerDown>();
                        foreach (var pointer in ptrDown)
                        {
                            if (!pointerDowns.Contains(pointer))
                            {
                                pointerDowns.Add(pointer);
                            }
                        }

                        var ptrClick = raycastHits[i].collider.gameObject.GetComponentsInChildren<IHSVPointerClick>();
                        foreach (var pointer in ptrClick)
                        {
                            if (!pointerClicks.Contains(pointer))
                            {
                                pointerClicks.Add(pointer);
                            }
                        }
                    }
                }

                foreach(var ptrDown in pointerDowns)
                {
                    ptrDown.OnPointerDown();
                }
                pointerDowns.Clear();
            }

            if(HSVInput.GetMouseButton(0) || (HSVInput.touchSupported && HSVInput.touchCount > 0 && touchDown))
            {
                previousScreenPos = HSVInput.GetPointerPosition();
            }

            if(HSVInput.GetMouseButtonUp(0) || (HSVInput.touchSupported && HSVInput.touchCount == 0 && touchDown))
            {
                touchDown = false;
                pointerUps.Clear();
                var hitCount = Raycast(previousScreenPos);
                if (hitCount > 0)
                {
                    for (int i = 0; i < hitCount; i++)
                    {
                        var ptrClick = raycastHits[i].collider.gameObject.GetComponentsInChildren<IHSVPointerClick>();
                        foreach (var pointer in ptrClick)
                        {
                            if(pointerClicks.Contains(pointer))
                            {
                                pointer.OnPointerClick();
                                pointerClicks.Remove(pointer);
                            }
                        }

                        var ptrUp = raycastHits[i].collider.gameObject.GetComponentsInChildren<IHSVPointerUp>();
                        foreach (var pointer in ptrUp)
                        {
                            if (!pointerUps.Contains(pointer))
                            {
                                pointerUps.Add(pointer);
                            }
                        }
                    }
                }

                foreach(var ptr in pointerUps)
                {
                    ptr.OnPointerUp();
                }
                pointerUps.Clear();
                pointerClicks.Clear();
            }
        }

        private int Raycast(Vector3 screenPos)
        {
            var ray = HSVTutorialManager.Instance.Camera.ScreenPointToRay(screenPos);
            return Physics.RaycastNonAlloc(ray, raycastHits);
        }
    }
}