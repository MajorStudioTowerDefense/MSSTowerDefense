using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HSVStudio.Tutorial.TMEditor
{
    [CustomPropertyDrawer(typeof(HSVModuleConfig))]
    public class HSVModuleConfigDrawer : PropertyDrawer
    {
        GUIStyle boxStyle;
        GUIStyle wrapStyle;
        GUIStyle textAreaWrap;
        GUIStyle titleFoldoutBold;
        Color guiColor, backgroundColor;
        Rect propertyRect, tempRect;
        float height;
        bool foldModuleSubConfig = true;
        bool modulePrefabCorrect = false;
        string modulePath;

        #region Module Config Cache
        string propertyTypeName, messageValue;
        bool overridePrefabColorValue, overrideFollowTargetValue, animateArrowValue, autoCalculateRotationValue, autoAnchorValue, overrideStyleValue, trackOutOfScreenValue,
            displayArrowValue, displayDistanceValue;
        UnityEngine.Object modulePrefabValue, maskPrefabValue;
        int distUnitValue;
        float distThresholdValue, distThreshMax;
        HSVTutorialModule tModuleValue;
        SerializedProperty modulePrefab, maskPrefab, instantTransition, m_smoothValue, fadeTime, customTransition, overridePrefabColor, moduleColor, m_sprite, overrideFollowTarget,
            followTarget, allowNoTarget;
        SerializedProperty animateArrow, animateCurve, animateSpeed, animateMagnitude, autoCalculateRotation, arrowDirection, arrowRotation, arrowSize, autoAnchor, pivotPoint, anchor, m_rectOffset, trackOutOfScreen, screenOffset;
        SerializedProperty popupRect, overrideStyle, fontConfig, scaleInOut, message;
        SerializedProperty displayRect, delayTime, autoStop;
        SerializedProperty positionOffset, maskSize, displayArrow, arrowSprite, arrowColor, displayDistance, fontRect, useCamera, distThreshold, distUnit;
        SerializedProperty rotation, scale, magnitudeOffset;
        #endregion

        #region V1.3 Close, Advance, Back Button text
        SerializedProperty overrideCloseButtonStyle, closeButtonStyle, closeButtonText,
            overrideAdvanceButtonStyle, advanceButtonStyle, advanceButtonText,
            overrideBackButtonStyle, backButtonStyle, backButtonText;
        bool overrideCloseButtonStyleValue, overrideAdvanceButtonStyleValue, overrideBackButtonStyleValue;
        #endregion

        private void CacheModule(SerializedProperty property)
        {
            propertyTypeName = property.type;
            modulePrefab = property.FindPropertyRelative("modulePrefab");
            modulePrefabValue = modulePrefab.objectReferenceValue;
            if (modulePrefabValue != null)
            {
                tModuleValue = (modulePrefabValue as GameObject).GetComponent<HSVTutorialModule>();
            }
            else
            {
                tModuleValue = null;
            }

            //Common properties
            maskPrefab = property.FindPropertyRelative("maskPrefab");
            instantTransition = property.FindPropertyRelative("instantTransition");
            m_smoothValue = property.FindPropertyRelative("m_smoothValue");
            fadeTime = property.FindPropertyRelative("fadeTime");
            customTransition = property.FindPropertyRelative("customTransition");
            overridePrefabColor = property.FindPropertyRelative("overridePrefabColor");
            overridePrefabColorValue = overridePrefabColor.boolValue;
            moduleColor = property.FindPropertyRelative("color");
            m_sprite = property.FindPropertyRelative("m_sprite");

            //property Properties
            overrideFollowTarget = property.FindPropertyRelative("overrideFollowTarget");
            if (overrideFollowTarget != null)
            {
                overrideFollowTargetValue = overrideFollowTarget.boolValue;
            }
            followTarget = property.FindPropertyRelative("followTarget");
            allowNoTarget = property.FindPropertyRelative("allowNoTarget");

            //ArrowModule
            animateArrow = property.FindPropertyRelative("animateArrow");
            if (animateArrow != null)
            {
                animateArrowValue = animateArrow.boolValue;
            }
            animateCurve = property.FindPropertyRelative("animateCurve");
            animateSpeed = property.FindPropertyRelative("animateSpeed");
            animateMagnitude = property.FindPropertyRelative("animateMagnitude");
            autoCalculateRotation = property.FindPropertyRelative("autoCalculateRotation");
            if (autoCalculateRotation != null)
            {
                autoCalculateRotationValue = autoCalculateRotation.boolValue;
            }
            arrowDirection = property.FindPropertyRelative("arrowDirection");
            arrowRotation = property.FindPropertyRelative("arrowRotation");
            arrowSize = property.FindPropertyRelative("arrowSize");
            autoAnchor = property.FindPropertyRelative("autoAnchor");
            if (autoAnchor != null)
            {
                autoAnchorValue = autoAnchor.boolValue;
            }
            pivotPoint = property.FindPropertyRelative("pivotPoint");
            anchor = property.FindPropertyRelative("anchor");
            m_rectOffset = property.FindPropertyRelative("m_rectOffset");
            trackOutOfScreen = property.FindPropertyRelative("trackOutOfScreen");
            if (trackOutOfScreen != null)
            {
                trackOutOfScreenValue = trackOutOfScreen.boolValue;
            }
            screenOffset = property.FindPropertyRelative("screenOffset");

            //PopupRect
            popupRect = property.FindPropertyRelative("popupRect");
            overrideStyle = property.FindPropertyRelative("overrideStyle");
            if (overrideStyle != null)
            {
                overrideStyleValue = overrideStyle.boolValue;
            }
            fontConfig = property.FindPropertyRelative("fontConfig");
            scaleInOut = property.FindPropertyRelative("scaleInOut");
            message = property.FindPropertyRelative("message");
            if (message != null)
            {
                messageValue = message.stringValue;
            }

            #region V1.3 Adding Font Config for three buttons
            overrideCloseButtonStyle = property.FindPropertyRelative("overrideCloseButtonStyle");
            if(overrideCloseButtonStyle != null)
            {
                overrideCloseButtonStyleValue = overrideCloseButtonStyle.boolValue;
            }
            closeButtonStyle = property.FindPropertyRelative("closeButtonStyle");
            closeButtonText = property.FindPropertyRelative("closeButtonText");

            overrideAdvanceButtonStyle = property.FindPropertyRelative("overrideAdvanceButtonStyle");
            if (overrideAdvanceButtonStyle != null)
            {
                overrideAdvanceButtonStyleValue = overrideAdvanceButtonStyle.boolValue;
            }
            advanceButtonStyle = property.FindPropertyRelative("advanceButtonStyle");
            advanceButtonText = property.FindPropertyRelative("advanceButtonText");

            overrideBackButtonStyle = property.FindPropertyRelative("overrideBackButtonStyle");
            if (overrideBackButtonStyle != null)
            {
                overrideBackButtonStyleValue = overrideBackButtonStyle.boolValue;
            }
            backButtonStyle = property.FindPropertyRelative("backButtonStyle");
            backButtonText = property.FindPropertyRelative("backButtonText");
            #endregion

            //Position Module
            positionOffset = property.FindPropertyRelative("positionOffset");
            maskSize = property.FindPropertyRelative("maskSize");
            displayArrow = property.FindPropertyRelative("displayArrow");
            if (displayArrow != null)
            {
                displayArrowValue = displayArrow.boolValue;
            }
            arrowSprite = property.FindPropertyRelative("arrowSprite");
            arrowColor = property.FindPropertyRelative("arrowColor");
            displayDistance = property.FindPropertyRelative("displayDistance");
            if (displayDistance != null)
            {
                displayDistanceValue = displayDistance.boolValue;
            }
            fontRect = property.FindPropertyRelative("fontRect");
            useCamera = property.FindPropertyRelative("useCamera");
            distThreshold = property.FindPropertyRelative("distThreshold");
            if (distThreshold != null)
            {
                distThresholdValue = distThreshold.floatValue;
            }
            distUnit = property.FindPropertyRelative("distUnit");
            if (distUnit != null)
            {
                distUnitValue = distUnit.enumValueIndex;
            }

            //BoundModule
            rotation = property.FindPropertyRelative("rotation");
            scale = property.FindPropertyRelative("scale");
            magnitudeOffset = property.FindPropertyRelative("magnitudeOffset");

            //Info DisplayModule
            displayRect = property.FindPropertyRelative("displayRect");
            //Time Module
            delayTime = property.FindPropertyRelative("delayTime");
            autoStop = property.FindPropertyRelative("autoStop");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (modulePath != property.propertyPath || HSVTutorialManager.moduleChange)
            {
                modulePath = property.propertyPath;
                CacheModule(property);
            }

            propertyRect = position;
            if (property.serializedObject == null)
                return;

            if (titleFoldoutBold == null)
            {
                titleFoldoutBold = new GUIStyle(EditorStyles.foldoutHeader);
                titleFoldoutBold.fontSize = 13;
                titleFoldoutBold.fontStyle = FontStyle.Bold;
            }

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(EditorStyles.helpBox);
                boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                boxStyle.fontSize = 12;
                boxStyle.fontStyle = FontStyle.Bold;
                boxStyle.alignment = TextAnchor.UpperLeft;
            }

            if (wrapStyle == null)
            {
                wrapStyle = new GUIStyle(GUI.skin.label);
                wrapStyle.fontStyle = FontStyle.Normal;
                wrapStyle.wordWrap = true;
                wrapStyle.alignment = TextAnchor.UpperLeft;
            }

            if (textAreaWrap == null)
            {
                textAreaWrap = new GUIStyle(EditorStyles.textArea);
                textAreaWrap.fontStyle = FontStyle.Normal;
                textAreaWrap.wordWrap = true;
            }

            backgroundColor = GUI.backgroundColor;
            guiColor = GUI.color;
            label = EditorGUI.BeginProperty(propertyRect, label, property);
            propertyRect.y += 10;
            tempRect = propertyRect;
            //Draw foldout
            propertyRect.height = 20;
            guiColor = GUI.color;
            GUI.color = new Color(1f, 0.7f, 0.2f, 1f);
            propertyRect.width -= 20;
            propertyRect.x += 15;
            foldModuleSubConfig = EditorGUI.BeginFoldoutHeaderGroup(propertyRect, foldModuleSubConfig, new GUIContent("Modules Config Setup"), titleFoldoutBold);
            EditorGUI.EndFoldoutHeaderGroup();
            propertyRect.y += propertyRect.height + 5;
            propertyRect.x = tempRect.x;
            propertyRect.width = tempRect.width;
            GUI.color = guiColor;

            if (foldModuleSubConfig)
            {
                EditorGUI.BeginChangeCheck();
                DrawProperty(ref propertyRect, modulePrefab, new GUIContent("Module Prefab:", "The module prefab used for current module type"));

                if (EditorGUI.EndChangeCheck())
                {
                    modulePrefabValue = modulePrefab.objectReferenceValue;

                    if (modulePrefabValue != null)
                    {
                        tModuleValue = (modulePrefabValue as GameObject).GetComponent<HSVTutorialModule>();
                    }
                    else
                    {
                        tModuleValue = null;
                    }
                }

                if (modulePrefab == null)
                    return;

                if (modulePrefabValue == null)
                {
                    GUI.backgroundColor = Color.red;
                    GUI.color = new Color(1f, 0.3f, 0f, 1f);
                    propertyRect.height = 20;
                    EditorGUI.LabelField(propertyRect, new GUIContent("Please assign the module prefab"));
                    propertyRect.y += propertyRect.height + 5;
                    GUI.backgroundColor = backgroundColor;
                    GUI.color = guiColor;
                }
                else
                {
                    modulePrefabCorrect = false;

                    if (tModuleValue != null && propertyTypeName.Contains(tModuleValue.GetType().Name))
                    {
                        modulePrefabCorrect = true;
                        DrawProperty(ref propertyRect, maskPrefab, new GUIContent("Mask Prefab:", "The display template used by the module"));

                        //Draw each sub class property
                        #region Arrow Module
                        //Arrow Module
                        if (propertyTypeName.Contains("HSVArrowModuleConfig"))
                        {
                            DrawModuleConfigCommonProperty(property);

                            DrawProperty(ref propertyRect, arrowSize, new GUIContent("Arrow Size:", "The size of the image "));
                            DrawProperty(ref propertyRect, arrowDirection, new GUIContent("Arrow Sprite Direction:", "The Arrow Sprite image direction that would be used to auto calculate rotation, Vector3(0, 1, 0) means pointing upwards for the displayed image"));

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, animateArrow, new GUIContent("Animate Arrow:", "The arrow will do the pointing direction animation"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                animateArrowValue = animateArrow.boolValue;
                            }

                            if (animateArrowValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, animateCurve, new GUIContent("Animation Curve", "The arrow will move along pointing direction with respect to the curve"));
                                DrawProperty(ref propertyRect, animateSpeed, new GUIContent("Animation Speed", "The speed of the animation"));
                                DrawProperty(ref propertyRect, animateMagnitude, new GUIContent("Animation Magnitude", "The move distance of the animation"));
                                EditorGUI.indentLevel--;

                            }

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, autoCalculateRotation, new GUIContent("Auto Rotation:", "The rotation of arrow would be automatically calculated instead of using rotation presets"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                autoCalculateRotationValue = autoCalculateRotation.boolValue;
                            }

                            if (!autoCalculateRotationValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, arrowRotation, new GUIContent("Rotation:", "The local rotation of the arrow when displaying"));
                                EditorGUI.indentLevel--;
                            }

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, autoAnchor, new GUIContent("Auto Anchor:", "If true, the arrow will auto position itself with respect to the target rect with respect to the center of the screen"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                autoAnchorValue = autoAnchor.boolValue;
                            }

                            if (autoAnchorValue)
                            {
                                DrawProperty(ref propertyRect, pivotPoint, new GUIContent("Pivot:", "The pivot point is used to position arrow around the display rect, the value is screen point position"));
                            }
                            else
                            {
                                DrawProperty(ref propertyRect, anchor, new GUIContent("Anchor:", "The anchor point used to position arrow"));
                            }

                            DrawProperty(ref propertyRect, m_rectOffset, new GUIContent("Rect Offset:", "The display offset of the mask"));
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, trackOutOfScreen, new GUIContent("Track OffScreen:", "Should the arrow module keep tracking target when it is out of screen"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                trackOutOfScreenValue = trackOutOfScreen.boolValue;
                            }

                            if (trackOutOfScreenValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, screenOffset, new GUIContent("Screen Offset:", "This offsets the arrow from screen edge, use value 0-1"));
                                EditorGUI.indentLevel--;
                            }

                        }
                        #endregion
                        //Focus Dim Module
                        else if (propertyTypeName.Contains("HSVFocusDimModuleConfig"))
                        {
                            DrawModuleConfigCommonProperty(property);
                            DrawProperty(ref propertyRect, m_rectOffset, new GUIContent("Rect Offset:", "The display offset of the mask"));
                        }
                        //Highlight Module
                        else if (propertyTypeName.Contains("HSVHighlightModuleConfig"))
                        {
                            DrawModuleConfigCommonProperty(property);
                            DrawProperty(ref propertyRect, m_rectOffset, new GUIContent("Rect Offset:", "The display offset of the mask"));
                        }
                        #region Popup Module
                        //Popup Module
                        else if (propertyTypeName.Contains("HSVPopupModuleConfig"))
                        {
                            DrawModuleConfigCommonProperty(property);

                            DrawProperty(ref propertyRect, allowNoTarget, new GUIContent("Allow No Target:", "Module will be enabled even there is no target object reference"));

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, autoAnchor, new GUIContent("Auto Anchor:", "If true, the arrow will auto position itself with respect to the target rect with respect to the center of the screen"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                autoAnchorValue = autoAnchor.boolValue;
                            }

                            if (autoAnchorValue)
                            {
                                DrawProperty(ref propertyRect, pivotPoint, new GUIContent("Pivot:", "The pivot point is used to position Popup rect around the display rect, the value is screen point position"));
                            }
                            else
                            {
                                DrawProperty(ref propertyRect, anchor, new GUIContent("Anchor:", "The anchor point used to position arrow"));
                            }

                            DrawProperty(ref propertyRect, scaleInOut, new GUIContent("Scale Animation:", "Should the popup rect use scale in or out animation? you can use your own animation by leaving this uncheck"));
                            DrawProperty(ref propertyRect, popupRect, new GUIContent("Display Rect:", "Rect size and display location for message module"));
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, trackOutOfScreen, new GUIContent("Track OffScreen:", "Should the popup module keep tracking target when it is out of screen"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                trackOutOfScreenValue = trackOutOfScreen.boolValue;
                            }

                            if (trackOutOfScreenValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, screenOffset, new GUIContent("Screen Offset:", "This offsets the popup from screen edge, use value 0-1"));
                                EditorGUI.indentLevel--;
                            }


                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, overrideStyle, new GUIContent("Override Font Style:"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                overrideStyleValue = overrideStyle.boolValue;
                            }

                            if (overrideStyleValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, fontConfig, new GUIContent("Font Style:"), true);
                                EditorGUI.indentLevel--;
                            }

                            propertyRect.height = 20;
                            EditorGUI.LabelField(propertyRect, "Display Message: ");
                            propertyRect.y += propertyRect.height + 5;

                            propertyRect.height = 100;
                            EditorGUI.BeginChangeCheck();
                            messageValue = EditorGUI.TextArea(propertyRect, messageValue, textAreaWrap);
                            if (EditorGUI.EndChangeCheck())
                            {
                                message.stringValue = messageValue;
                            }

                            propertyRect.y += propertyRect.height + 5;
                            //Close Button
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, overrideCloseButtonStyle, new GUIContent("Override Close Button Style:"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                overrideCloseButtonStyleValue = overrideCloseButtonStyle.boolValue;
                            }

                            if (overrideCloseButtonStyleValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, closeButtonStyle, new GUIContent("Close Button Style:"), true);
                                EditorGUI.indentLevel--;
                            }
                            DrawProperty(ref propertyRect, closeButtonText, new GUIContent("Close Button Display Text"));
                            //Advance Button
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, overrideAdvanceButtonStyle, new GUIContent("Override Advance Button Style:"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                overrideAdvanceButtonStyleValue = overrideAdvanceButtonStyle.boolValue;
                            }

                            if (overrideAdvanceButtonStyleValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, advanceButtonStyle, new GUIContent("Advance Button Style:"), true);
                                EditorGUI.indentLevel--;
                            }
                            DrawProperty(ref propertyRect, advanceButtonText, new GUIContent("Advance Button Display Text"));
                            //Back Button
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, overrideBackButtonStyle, new GUIContent("Override Back Button Style:"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                overrideBackButtonStyleValue = overrideBackButtonStyle.boolValue;
                            }

                            if (overrideBackButtonStyleValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, backButtonStyle, new GUIContent("Back Button Style:"), true);
                                EditorGUI.indentLevel--;
                            }
                            DrawProperty(ref propertyRect, backButtonText, new GUIContent("Back Button Display Text"));
                        }
                        #endregion
                        #region Position Module
                        //Position Module
                        else if (propertyTypeName.Contains("HSVPositionModuleConfig"))
                        {
                            DrawModuleConfigCommonProperty(property);

                            DrawProperty(ref propertyRect, positionOffset, new GUIContent("Position Offset:", "Display offset from target location. If not tracking target outside screen, this would be used as world position offset from target"));
                            DrawProperty(ref propertyRect, maskSize, new GUIContent("Mask Size:", "The size of the image "));

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, displayArrow, new GUIContent("Display Arrow:", "Should the position module display arrow around mask"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                displayArrowValue = displayArrow.boolValue;
                            }

                            if (displayArrowValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, arrowSprite, new GUIContent("Arrow Sprite:", "Overriding arrow sprite"));
                                DrawProperty(ref propertyRect, arrowColor, new GUIContent("Arrow Color:", "Overriding arrow color"));
                                DrawProperty(ref propertyRect, arrowSize, new GUIContent("Arrow Size:", "Size of the arrow"));
                                DrawProperty(ref propertyRect, arrowDirection, new GUIContent("Arrow Sprite Direction:", "The Arrow Sprite image direction that would be used to auto calculate rotation, Vector3(0, 1, 0) means pointing upwards for the displayed image"));
                                DrawProperty(ref propertyRect, m_rectOffset, new GUIContent("Arrow Offset:", "Offset distance of arrow around display mask"));
                                EditorGUI.indentLevel--;
                            }

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, trackOutOfScreen, new GUIContent("Track OffScreen:", "Should the position module keep tracking target when it is out of screen"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                trackOutOfScreenValue = trackOutOfScreen.boolValue;
                            }

                            if (trackOutOfScreenValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, screenOffset, new GUIContent("Screen Offset:", "This offsets the arrow from screen edge, use value 0-1"));
                                EditorGUI.indentLevel--;
                            }

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, displayDistance, new GUIContent("Display Distance:", "Should display target distance from player"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                displayDistanceValue = displayDistance.boolValue;
                            }

                            if (displayDistanceValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, fontRect, new GUIContent("Font Rect:", "Display text recttransform size and position"));
                                DrawProperty(ref propertyRect, useCamera, new GUIContent("Use Camera:", "Should use camera as player object for displaying distance"));

                                EditorGUI.BeginChangeCheck();
                                DrawProperty(ref propertyRect, distUnit, new GUIContent("Distance Unit:", "Unit of display distance"));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    distThreshold.floatValue = ConverUnit((Unit)distUnitValue, (Unit)distUnit.enumValueIndex, distThresholdValue);
                                    distThresholdValue = distThreshold.floatValue;
                                    distUnitValue = distUnit.enumValueIndex;
                                }

                                switch ((Unit)distUnitValue)
                                {
                                    case Unit.Meter:
                                        distThreshMax = 10000;
                                        break;
                                    case Unit.KiloMeter:
                                        distThreshMax = 10;
                                        break;
                                    case Unit.Mile:
                                        distThreshMax = 10;
                                        break;
                                    case Unit.Feet:
                                        distThreshMax = 10000;
                                        break;
                                }

                                propertyRect.height = EditorGUI.GetPropertyHeight(distThreshold);
                                EditorGUI.BeginChangeCheck();
                                distThresholdValue = EditorGUI.IntSlider(propertyRect, new GUIContent("Distance Threshold", "Threshold for displaying distance"), (int)distThresholdValue, 0, (int)distThreshMax);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    distThreshold.floatValue = distThresholdValue;
                                }

                                propertyRect.y += propertyRect.height + 5;

                                //Display Font Style
                                EditorGUI.BeginChangeCheck();
                                DrawProperty(ref propertyRect, overrideStyle, new GUIContent("Override Font Style:"));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    overrideStyleValue = overrideStyle.boolValue;
                                }

                                if (overrideStyleValue)
                                {
                                    EditorGUI.indentLevel++;
                                    DrawProperty(ref propertyRect, fontConfig, new GUIContent("Font Style:"), true);
                                    EditorGUI.indentLevel--;
                                }
                                EditorGUI.indentLevel--;
                            }
                        }
                        #endregion
                        #region BoundModule
                        else if (propertyTypeName.Contains("HSVBoundModuleConfig"))
                        {
                            DrawModuleConfigCommonProperty(property);
                            DrawProperty(ref propertyRect, positionOffset, new GUIContent("Position Offset:", "Display offset from target location."));
                            DrawProperty(ref propertyRect, magnitudeOffset, new GUIContent("Magnitude Offset:", "Magnitude offset along the direction pointing towards bound center."));
                            DrawProperty(ref propertyRect, scale, new GUIContent("Scale:", "Display scale of world object."));

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, autoCalculateRotation, new GUIContent("Auto Rotation:", "The rotation of world object would be automatically calculated instead of using rotation presets"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                autoCalculateRotationValue = autoCalculateRotation.boolValue;
                            }

                            if (!autoCalculateRotationValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, rotation, new GUIContent("Rotation:", "The local rotation of the world object when displaying"));
                                EditorGUI.indentLevel--;
                            }

                            DrawProperty(ref propertyRect, anchor, new GUIContent("Anchor:", "The anchor point used to position world object"));
                        }
                        #endregion
                        #region Time Module
                        //TimeModule Config
                        //Info Display Module
                        else if (propertyTypeName.Contains("HSVInfoDisplayModuleConfig"))
                        {
                            DrawModuleConfigCommonProperty(property);

                            DrawProperty(ref propertyRect, scaleInOut, new GUIContent("Scale Animation:", "Should the popup rect use scale in or out animation? you can use your own animation by leaving this uncheck"));

                            DrawProperty(ref propertyRect, displayRect, new GUIContent("Display Rect:", "Rect size and display location for message module"));

                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, overrideStyle, new GUIContent("Override Font Style:"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                overrideStyleValue = overrideStyle.boolValue;
                            }

                            if (overrideStyleValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, fontConfig, new GUIContent("Font Style:"), true);
                                EditorGUI.indentLevel--;
                            }

                            propertyRect.height = 20;
                            EditorGUI.LabelField(propertyRect, "Display Message: ");
                            propertyRect.y += propertyRect.height + 5;

                            var message = property.FindPropertyRelative("message");
                            propertyRect.height = 100;
                            EditorGUI.BeginChangeCheck();
                            messageValue = EditorGUI.TextArea(propertyRect, messageValue, textAreaWrap);
                            if (EditorGUI.EndChangeCheck())
                            {
                                message.stringValue = messageValue;
                            }

                            propertyRect.y += propertyRect.height + 5;
                            //Close Button
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, overrideCloseButtonStyle, new GUIContent("Override Close Button Style:"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                overrideCloseButtonStyleValue = overrideCloseButtonStyle.boolValue;
                            }

                            if (overrideCloseButtonStyleValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, closeButtonStyle, new GUIContent("Close Button Style:"), true);
                                EditorGUI.indentLevel--;
                            }
                            DrawProperty(ref propertyRect, closeButtonText, new GUIContent("Close Button Display Text"));
                            //Advance Button
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, overrideAdvanceButtonStyle, new GUIContent("Override Advance Button Style:"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                overrideAdvanceButtonStyleValue = overrideAdvanceButtonStyle.boolValue;
                            }

                            if (overrideAdvanceButtonStyleValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, advanceButtonStyle, new GUIContent("Advance Button Style:"), true);
                                EditorGUI.indentLevel--;
                            }
                            DrawProperty(ref propertyRect, advanceButtonText, new GUIContent("Advance Button Display Text"));
                            //Back Button
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, overrideBackButtonStyle, new GUIContent("Override Back Button Style:"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                overrideBackButtonStyleValue = overrideBackButtonStyle.boolValue;
                            }

                            if (overrideBackButtonStyleValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, backButtonStyle, new GUIContent("Back Button Style:"), true);
                                EditorGUI.indentLevel--;
                            }
                            DrawProperty(ref propertyRect, backButtonText, new GUIContent("Back Button Display Text"));
                        }
                        //Time Delay Module
                        else if (propertyTypeName.Contains("HSVTimeDelayModuleConfig"))
                        {
                            DrawProperty(ref propertyRect, delayTime, new GUIContent("Delay Time:", "The module will just do nothing but delay with this time period"));
                            DrawProperty(ref propertyRect, autoStop, new GUIContent("Auto End Tutorial:", "Should the tutorial end after delayed time"));
                        }
                        else
                        {
                            var endProp = property.GetEndProperty();
                            var firstProp = property.FindPropertyRelative("maskPrefab");
                            while (firstProp.NextVisible(true) && !EqualContents(firstProp, endProp))
                            {
                                DrawProperty(ref propertyRect, firstProp, new GUIContent(firstProp.displayName));
                            }
                        }
                        #endregion
                    }

                    if (!modulePrefabCorrect)
                    {
                        GUI.backgroundColor = Color.red;
                        GUI.color = new Color(0.75f, 0.5f, 0f, 1f);
                        propertyRect.height = 20;
                        EditorGUI.LabelField(propertyRect, new GUIContent("Incorrect Module Component on the prefab, please check"));
                        propertyRect.y += propertyRect.height + 5;
                        GUI.backgroundColor = backgroundColor;
                        GUI.color = guiColor;
                    }

                }
            }

            EditorGUI.EndProperty();
            height = propertyRect.y - position.y;

            propertyRect.height = height;
            propertyRect.position = position.position;
            EditorGUI.indentLevel--;
            EditorGUI.LabelField(propertyRect, GUIContent.none, boxStyle);
            EditorGUI.indentLevel++;
            GUI.backgroundColor = backgroundColor;
            GUI.color = guiColor;

            HSVTutorialManager.targetChange = false;
        }

        private bool EqualContents(SerializedProperty a, SerializedProperty b)
        {
            return SerializedProperty.EqualContents(a, b);
        }

        private void DrawModuleConfigCommonProperty(SerializedProperty property)
        {
            DrawProperty(ref propertyRect, m_sprite, new GUIContent("Override Sprite:", "This will override sprite used by module"));
            EditorGUI.BeginChangeCheck();
            DrawProperty(ref propertyRect, overridePrefabColor, new GUIContent("Override Mask Color:", "Should override all the masks color using color"));
            if (EditorGUI.EndChangeCheck())
            {
                overridePrefabColorValue = overridePrefabColor.boolValue;
            }

            if (overridePrefabColorValue)
            {
                EditorGUI.indentLevel++;
                DrawProperty(ref propertyRect, moduleColor, new GUIContent("Color:", "The global color that used by modules"));
                EditorGUI.indentLevel--;
            }

            if (overrideFollowTarget != null)
            {
                EditorGUI.BeginChangeCheck();
                DrawProperty(ref propertyRect, overrideFollowTarget, new GUIContent("Override Follow Target:", "Should module override following Target Config"));
                if (EditorGUI.EndChangeCheck())
                {
                    overrideFollowTargetValue = overrideFollowTarget.boolValue;
                }

                if (overrideFollowTargetValue)
                {
                    EditorGUI.indentLevel++;
                    DrawProperty(ref propertyRect, followTarget, new GUIContent("Follow Target:", "If Override, module will use this to position UI"));
                    EditorGUI.indentLevel--;
                }
            }
            DrawProperty(ref propertyRect, customTransition, new GUIContent("Custom Transition", "Use your own custom transition solution"));
            DrawProperty(ref propertyRect, instantTransition, new GUIContent("Instant Transition:", "UI Transition would be instantly fade in instead of animated"));
            DrawProperty(ref propertyRect, m_smoothValue, new GUIContent("Smooth Value:", "UI transition speed"));
            DrawProperty(ref propertyRect, fadeTime, new GUIContent("Fade Time:", "UI Fade Total Time"));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }

        private void DrawProperty(ref Rect rect, SerializedProperty property, GUIContent content, bool includeChildren = false)
        {
            if (property != null)
            {
                rect.height = EditorGUI.GetPropertyHeight(property);
                EditorGUI.PropertyField(rect, property, content, includeChildren);
                rect.y += rect.height + 5;
            }
        }

        private float ConverUnit(Unit oldUnit, Unit newUnit, float value)
        {
            float result = value;
            //Conver to Meter first
            switch (oldUnit)
            {
                case Unit.Meter:
                    break;
                case Unit.KiloMeter:
                    result *= 1000;
                    break;
                case Unit.Mile:
                    result *= 1609.344f;
                    break;
                case Unit.Feet:
                    result *= 0.3048f;
                    break;
            }

            switch (newUnit)
            {
                case Unit.Meter:
                    break;
                case Unit.KiloMeter:
                    result /= 1000;
                    break;
                case Unit.Mile:
                    result *= 0.00062137f;
                    break;
                case Unit.Feet:
                    result *= 3.28084f;
                    break;
            }

            return result;
        }
    }
}