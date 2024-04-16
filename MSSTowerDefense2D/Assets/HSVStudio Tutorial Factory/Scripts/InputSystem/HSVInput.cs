using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace HSVStudio.Tutorial
{
    public static class HSVInput
    {
#if ENABLE_INPUT_SYSTEM
    public static bool GetKeyDown(KeyCode keyCode)
        {
            return GetKeyDown(keyCode.ToString().ToLower());
        }

        public static bool GetKeyDown(string keyCode)
        {
             return ((KeyControl)Keyboard.current[keyCode]).wasPressedThisFrame;
        }

        public static bool GetKey(KeyCode keyCode)
        {
            return GetKey(keyCode.ToString().ToLower());
        }

        public static bool GetKey(string keyCode)
        {
            return ((KeyControl)Keyboard.current[keyCode]).isPressed;
        }

        public static bool GetMouseButtonDown(int button)
        {
            switch (button)
            {
                case 1:
                    return Mouse.current.rightButton.wasReleasedThisFrame;
                case 2:
                    return Mouse.current.middleButton.wasReleasedThisFrame;
                default:
                    return Mouse.current.leftButton.wasReleasedThisFrame;
            }
        }

        public static bool GetMouseButtonUp(int button)
        {
            switch (button)
            {
                case 1:
                    return Mouse.current.rightButton.wasPressedThisFrame;
                case 2:
                    return Mouse.current.middleButton.wasPressedThisFrame;
                default:
                    return Mouse.current.leftButton.wasPressedThisFrame;
            }
        }

        public static bool GetMouseButton(int button)
        {
            switch (button)
            {
                case 1:
                    return Mouse.current.rightButton.isPressed;
                case 2:
                    return Mouse.current.middleButton.isPressed;
                default:
                    return Mouse.current.leftButton.isPressed;
            }
        }

        public static int touchCount
        {
            get
            {
                return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
            }
        }

        public static UnityEngine.InputSystem.EnhancedTouch.Touch[] touches
        {
            get
            {
                return UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.ToArray();
            }
        }

        public static bool touchSupported
        {
            get
            {
                return UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.enabled;
            }
        }

        public static Vector3 mousePosition
        {
            get
            {
                return Mouse.current.position.ReadValue();
            }
        }

        public static Vector3 GetTouchPosition(int touchIdx)
        {
            return touches[touchIdx].screenPosition;
        }

#else
        public static bool GetKeyDown(KeyCode keyCode)
        {
            return GetKeyDown(keyCode.ToString().ToLower());
        }

        public static bool GetKeyDown(string keyCode)
        {
            return Input.GetKeyDown(keyCode);
        }

        public static bool GetKey(KeyCode keyCode)
        {
            return GetKey(keyCode.ToString().ToLower());
        }

        public static bool GetKey(string keyCode)
        {
            return Input.GetKey(keyCode);
        }

        public static bool GetMouseButtonDown(int button)
        {
            return Input.GetMouseButtonDown(button);
        }

        public static bool GetMouseButtonUp(int button)
        {
            return Input.GetMouseButtonUp(button);
        }

        public static bool GetMouseButton(int button)
        {
            return Input.GetMouseButton(button);
        }

        public static int touchCount
        {
            get
            {
                return Input.touchCount;
            }
        }

        public static Touch[] touches
        {
            get
            {
                return Input.touches;
            }
        }

        public static bool touchSupported
        {
            get
            {
                return Input.touchSupported;
            }
        }

        public static Vector3 mousePosition
        {
            get
            {
                return Input.mousePosition;
            }
        }

        public static Vector3 GetTouchPosition(int touchIdx)
        {
            return touches[touchIdx].position;
        }
#endif

        public static Vector3 GetPointerPosition()
        {
            if(touchSupported && touchCount > 0)
            {
                return GetTouchPosition(0);
            }
            return mousePosition;
        }
    }
}