using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

namespace HSVStudio.Tutorial
{
    [Serializable]
    public struct HSVFontStyle
    {
        public float fontSize;
        public TMPro.FontStyles fontStyle;
        public TMPro.TextAlignmentOptions textAlignment;
        public Color textColor;
        public TMPro.TMP_FontAsset fontAsset;
    }

    #region Module Global Config
    [Serializable]
    public abstract class HSVModuleConfig
    {
        //Module prefab used by the current config, please make sure correct Module component is used
        public GameObject modulePrefab;
        //Mask prefab used by the module
        public GameObject maskPrefab;
        //should the module transition in instantly.
        public bool instantTransition;
        //used by transition animation
        public float m_smoothValue;
        //used by transition animation
        public float fadeTime;
        //Custom Transition
        public bool customTransition;
        //should mask color be overrided
        public bool overridePrefabColor;
        //overriding color
        public Color color;
        //overriding mask sprite, if null, it will not override
        public Sprite m_sprite;

        public HSVModuleConfig()
        {
            overridePrefabColor = false;
            instantTransition = true;
            color = Color.white;
            m_smoothValue = 10f;
            fadeTime = 0.5f;
        }

        /// <summary>
        /// Clone method
        /// </summary>
        /// <returns></returns>
        public virtual HSVModuleConfig Clone()
        {
            return (HSVModuleConfig)this.MemberwiseClone();
        }
    }

    #region TargetModule Config Region
    [Serializable]
    public abstract class HSVTargetModuleConfig : HSVModuleConfig
    {
        public bool overrideFollowTarget;
        public bool followTarget;
        public bool allowNoTarget;

        public HSVTargetModuleConfig() : base()
        {
            overrideFollowTarget = false;
            followTarget = false;
            allowNoTarget = false;
        }
    }

    [Serializable]
    public class HSVArrowModuleConfig : HSVTargetModuleConfig
    {
        #region Arrow Config
        //should arrow animate along arrow direction
        public bool animateArrow;
        //animation curve used by arrow
        public AnimationCurve animateCurve;
        //how fast it animates
        public float animateSpeed;
        //displacement of the animation
        public float animateMagnitude;
        //should the arrow pointing direction be calculated automatically, otherwise it would use specified arrow rotation
        public bool autoCalculateRotation;
        //arrow direction would be used on auto calculation
        public Vector3 arrowDirection;
        //specified arrow rotation
        public Vector3 arrowRotation;
        //size of the arrow
        public Vector2 arrowSize;
        //should arrow automatically anchor around target
        public bool autoAnchor;
        //Pivot point used by auto calculation, it can be any point on screen, uses screen point
        public Vector2 pivotPoint;
        //if not auto anchor, it uses this anchor to position arrow
        public Vector2 anchor;
        //uses this to offset arrow around target rect
        public Vector2 m_rectOffset;
        //should arrow track target even it is outside screen
        public bool trackOutOfScreen;
        //offset from the screen when tracking out of screen
        [Range(0, 1)]
        public float screenOffset;
        #endregion

        public HSVArrowModuleConfig() : base()
        {
            var curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 1), new Keyframe(0.75f, -1), new Keyframe(1, 0));
            curve.preWrapMode = WrapMode.Loop;
            curve.postWrapMode = WrapMode.Loop;
            animateArrow = false;
            animateCurve = curve;
            animateSpeed = 1f;
            animateMagnitude = 5f;
            arrowRotation = Vector3.zero;
            arrowSize = new Vector2(100, 100);
            anchor = new Vector2(0.5f, 0.5f);
            arrowDirection = Vector3.right;
            autoAnchor = false;
            autoCalculateRotation = false;
            pivotPoint = Vector2.zero;
            m_rectOffset = new Vector2(10, 10);
            trackOutOfScreen = false;
            screenOffset = 1f;
        }

    }

    [Serializable]
    public class HSVFocusDimModuleConfig : HSVTargetModuleConfig
    {
        #region FocusDim Config
        //offset of the target rect, so it increases the mask display rect
        public Vector2 m_rectOffset;
        #endregion

        public HSVFocusDimModuleConfig() : base()
        {
            color = new Color(0.25f, 0.25f, 0.25f, 0.6f);
            m_rectOffset = new Vector2(10, 10);
        }
    }

    [Serializable]
    public class HSVHighlightModuleConfig : HSVTargetModuleConfig
    {
        //offset of the target rect, so it increases the mask display rect
        public Vector2 m_rectOffset;

        public HSVHighlightModuleConfig() : base()
        {
            m_rectOffset = new Vector2(10, 10);
        }
    }

    [Serializable]
    public class HSVPopupModuleConfig : HSVTargetModuleConfig
    {
        #region popRect Config
        //should popup rect automatically anchor around target
        public bool autoAnchor;
        //Pivot point used by auto calculation, it can be any point on screen, uses screen point
        public Vector2 pivotPoint;
        //if not auto anchor, it uses this anchor to position popup rect
        public Vector2 anchor;
        //display rect of the popup mask
        public Rect popupRect;
        //should popup track target even it is outside screen
        public bool trackOutOfScreen;
        //offset from the screen when tracking out of screen
        [Range(0, 1)]
        public float screenOffset;
        //should override the texmeshpro style
        public bool overrideStyle;
        //override font config
        public HSVFontStyle fontConfig;
        //should rect scale in or out
        public bool scaleInOut;
        //display message
        public string message;

        #region V1.3
        public bool overrideCloseButtonStyle;
        public HSVFontStyle closeButtonStyle;
        public string closeButtonText;
        public bool overrideAdvanceButtonStyle;
        public HSVFontStyle advanceButtonStyle;
        public string advanceButtonText;
        public bool overrideBackButtonStyle;
        public HSVFontStyle backButtonStyle;
        public string backButtonText;
        #endregion
        #endregion

        public HSVPopupModuleConfig() : base()
        {
            anchor = new Vector2(0.5f, 0.5f);
            autoAnchor = false;
            message = string.Empty;
            pivotPoint = Vector2.zero;
            popupRect = new Rect(0, 0, 100, 100);
            trackOutOfScreen = false;
            screenOffset = 1f;
            overrideStyle = false;
            fontConfig = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
            overrideCloseButtonStyle = false;
            closeButtonStyle = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
            overrideAdvanceButtonStyle = false;
            advanceButtonStyle = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
            overrideBackButtonStyle = false;
            backButtonStyle = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
        }
    }

    [Serializable]
    public class HSVPositionModuleConfig : HSVTargetModuleConfig
    {
        //position offset from target location
        public Vector3 positionOffset;
        //mask display size
        public Vector2 maskSize;
        //Should arrow being displayed
        public bool displayArrow;
        //overriding arrow sprite
        public Sprite arrowSprite;
        //Color of Arrow
        public Color arrowColor;
        //arrow direction would be used on auto calculation
        public Vector3 arrowDirection;
        //arrow display size
        public Vector2 arrowSize;
        //offset of mask to arrow sprite. It determins where arrow would be.
        public float m_rectOffset;
        //should arrow track target even it is outside screen
        public bool trackOutOfScreen;
        //offset from the screen when tracking out of screen
        [Range(0, 1)]
        public float screenOffset;
        //Should display distance below icon
        public bool displayDistance;
        //display font rect size
        public Rect fontRect;
        //Use main camera as distance reference point
        public bool useCamera;
        //should override the texmeshpro style
        public bool overrideStyle;
        //override font config
        public HSVFontStyle fontConfig;
        //minimum distance to display
        public float distThreshold;
        //display unit
        public Unit distUnit;

        public HSVPositionModuleConfig() : base()
        {
            positionOffset = Vector3.zero;
            maskSize = new Vector2(10, 10);
            displayArrow = false;
            arrowColor = Color.white;
            arrowDirection = Vector3.up;
            arrowSize = new Vector2(10, 10);
            m_rectOffset = 10;
            trackOutOfScreen = false;
            screenOffset = 1f;
            displayDistance = false;
            useCamera = false;
            distThreshold = 1;
            overrideStyle = false;
            fontConfig = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
            fontRect = new Rect(0, 0, 10, 10); 
            distUnit = Unit.Meter;
        }
    }

    [Serializable]
    public class HSVBoundModuleConfig : HSVTargetModuleConfig
    {
        //position offset from target location
        public Vector3 positionOffset;
        //magnitude offset when positioning on the edge of Bound
        public float magnitudeOffset;
        //Rotation of World Object
        public Vector3 rotation;
        //Scale of the world object
        public Vector3 scale;
        //Anchor to position world object
        public Vector3 anchor;
        //Auto orientated world object to have transform forward always pointing at the center of target bound
        public bool autoCalculateRotation;

        public HSVBoundModuleConfig() : base()
        {
            positionOffset = Vector3.zero;
            magnitudeOffset = 0;
            rotation = Vector3.zero;
            scale = Vector3.one;
            anchor = Vector3.zero;
            autoCalculateRotation = false;
        }
    }
    #endregion

    #region TimeModule Region
    [Serializable]
    public abstract class HSVTimeModuleConfig : HSVModuleConfig
    {

    }

    [Serializable]
    public class HSVInfoDisplayModuleConfig : HSVTimeModuleConfig
    {
        //display rect of the mask
        public Rect displayRect;
        //should rect scale in or out
        public bool scaleInOut;
        //display message
        public string message;
        //should override the texmeshpro style
        public bool overrideStyle;
        //override font config
        public HSVFontStyle fontConfig;

        #region V1.3
        public bool overrideCloseButtonStyle;
        public HSVFontStyle closeButtonStyle;
        public string closeButtonText;
        public bool overrideAdvanceButtonStyle;
        public HSVFontStyle advanceButtonStyle;
        public string advanceButtonText;
        public bool overrideBackButtonStyle;
        public HSVFontStyle backButtonStyle;
        public string backButtonText;
        #endregion

        public HSVInfoDisplayModuleConfig() : base()
        {
            displayRect = new Rect(0, 0, 500, 500);
            message = string.Empty;
            overrideStyle = false;
            fontConfig = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
            overrideCloseButtonStyle = false;
            closeButtonStyle = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
            overrideAdvanceButtonStyle = false;
            advanceButtonStyle = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
            overrideBackButtonStyle = false;
            backButtonStyle = new HSVFontStyle
            {
                fontSize = 30f,
                fontStyle = TMPro.FontStyles.Normal,
                textAlignment = TMPro.TextAlignmentOptions.Center,
                textColor = Color.white
            };
        }
    }

    [Serializable]
    public class HSVTimeDelayModuleConfig : HSVTimeModuleConfig
    {
        //elapsed time of the module
        public float delayTime;
        //should the module automatically stop when time elapsed
        public bool autoStop;

        public HSVTimeDelayModuleConfig() : base()
        {
            delayTime = 1f;
            autoStop = true;
        }
    }
    #endregion
    #endregion
}