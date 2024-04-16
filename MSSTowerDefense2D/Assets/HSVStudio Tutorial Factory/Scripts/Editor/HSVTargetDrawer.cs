using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace HSVStudio.Tutorial.TMEditor
{
    [CustomPropertyDrawer(typeof(HSVTarget))]
    public class HSVTargetDrawer : PropertyDrawer
    {
        GUIStyle boxStyle;
        GUIStyle wrapStyle;
        GUIStyle titleFoldoutBold;
        Color guiColor, backgroundColor;
        Rect propertyRect, tempRect;
        Rect buttonRect;
        float height;
        bool foldTargetDrawer = true;
        string targetPath;

        #region Cache
        string[] moduleConfigType;
        bool useModuleMaskValue, overrideSpawnValue, spawnEffectValue, overrideEffectInfoValue, useTriggerValue;
        int moduleMaskValue, TargetBoundTypeValue;
        UnityEngine.Object targetValue;
        SerializedProperty target, startInactive, highlightTarget, followTarget, useModuleMask, moduleMask, TargetBoundType;
        SerializedProperty overrideSpawn, spawnInfo, spawnPos, spawnRot, spawnScale;
        //Version 1.1
        bool foldTargetSpawn = true;
        SerializedProperty spawnEffect, effectPrefab, overrideEffectInfo, effectSpawnInfo, effectPos, effectRot, effectScale;
        //Version 1.2
        bool foldTriggerConfig = true;
        SerializedProperty useTrigger, triggerConfig, triggerState;
        SerializedProperty triggerType, colliderConfig, collider, triggerOnClick, layerFiltering, filterLayer, tagFiltering, filterTag, useRigidbodyTag, colliderPointerTrigger, graphicConfig, graphic, graphicPointerTrigger, keyCode;
        int triggerTypeValue;
        bool layerFilteringValue, tagFilteringValue;
        #endregion

        private void CacheTarget(SerializedProperty property)
        {
            target = property.FindPropertyRelative("target");
            if(target != null)
            {
                targetValue = target.objectReferenceValue;
            }
            overrideSpawn = property.FindPropertyRelative("overrideSpawn");
            if(overrideSpawn != null)
            {
                overrideSpawnValue = overrideSpawn.boolValue;
            }
            spawnInfo = property.FindPropertyRelative("spawnInfo");

            if(spawnInfo != null)
            {
                spawnPos = spawnInfo.FindPropertyRelative("spawnPos");
                spawnRot = spawnInfo.FindPropertyRelative("spawnRot");
                spawnScale = spawnInfo.FindPropertyRelative("spawnScale");
            }

            startInactive = property.FindPropertyRelative("startInactive");
            highlightTarget = property.FindPropertyRelative("highlightTarget");
            followTarget = property.FindPropertyRelative("followTarget");
            useModuleMask = property.FindPropertyRelative("useModuleMask");
            useModuleMaskValue = useModuleMask.boolValue;
            moduleMask = property.FindPropertyRelative("moduleMask");
            moduleMaskValue = moduleMask.intValue;
            TargetBoundType = property.FindPropertyRelative("TargetBoundType");
            TargetBoundTypeValue = TargetBoundType.enumValueIndex;

            moduleConfigType = HSVTutorialManager.Instance.GetModuleConfigTypeNames();

            //Version 1.1
            spawnEffect = property.FindPropertyRelative("spawnEffect");
            if(spawnEffect != null)
            {
                spawnEffectValue = spawnEffect.boolValue;
            }
            effectPrefab = property.FindPropertyRelative("effectPrefab");
            overrideEffectInfo = property.FindPropertyRelative("overrideEffectInfo");
            if(overrideEffectInfo != null)
            {
                overrideEffectInfoValue = overrideEffectInfo.boolValue;
            }

            effectSpawnInfo = property.FindPropertyRelative("effectSpawnInfo");
            if(effectSpawnInfo != null)
            {
                effectPos = effectSpawnInfo.FindPropertyRelative("spawnPos");
                effectRot = effectSpawnInfo.FindPropertyRelative("spawnRot");
                effectScale = effectSpawnInfo.FindPropertyRelative("spawnScale");
            }

            //Version 1.2
            useTrigger = property.FindPropertyRelative("useTrigger");
            if(useTrigger != null)
            {
                useTriggerValue = useTrigger.boolValue;
            }

            triggerConfig = property.FindPropertyRelative("triggerConfig");
            if(triggerConfig != null)
            {
                triggerType = triggerConfig.FindPropertyRelative("triggerType");
                triggerTypeValue = triggerType.enumValueIndex;
                colliderConfig = triggerConfig.FindPropertyRelative("colliderConfig");
                if (colliderConfig != null)
                {
                    collider = colliderConfig.FindPropertyRelative("collider");
                    triggerOnClick = colliderConfig.FindPropertyRelative("triggerOnClick");
                    layerFiltering = colliderConfig.FindPropertyRelative("layerFiltering");
                    layerFilteringValue = layerFiltering.boolValue;
                    filterLayer = colliderConfig.FindPropertyRelative("filterLayer");
                    tagFiltering = colliderConfig.FindPropertyRelative("tagFiltering");
                    tagFilteringValue = tagFiltering.boolValue;
                    filterTag = colliderConfig.FindPropertyRelative("filterTag");
                    useRigidbodyTag = colliderConfig.FindPropertyRelative("useRigidBodyTag");
                    colliderPointerTrigger = colliderConfig.FindPropertyRelative("pointerTrigger");
                }
                graphicConfig = triggerConfig.FindPropertyRelative("graphicConfig");
                if (graphicConfig != null)
                {
                    graphic = graphicConfig.FindPropertyRelative("graphic");
                    graphicPointerTrigger = graphicConfig.FindPropertyRelative("pointerTrigger");
                }

                keyCode = triggerConfig.FindPropertyRelative("keyCode");
            }

            triggerState = property.FindPropertyRelative("triggerState");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(targetPath != property.propertyPath || HSVTutorialManager.targetChange)
            {
                targetPath = property.propertyPath;
                CacheTarget(property);
            }

            backgroundColor = GUI.backgroundColor;
            guiColor = GUI.color;
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

            label = EditorGUI.BeginProperty(propertyRect, label, property);
            propertyRect.y += 10;
            propertyRect.height = 20;
            GUI.color = Color.cyan;
            propertyRect.width -= 20;
            propertyRect.x += 15;
            foldTargetDrawer = EditorGUI.BeginFoldoutHeaderGroup(propertyRect, foldTargetDrawer, new GUIContent("Target Setup:"), titleFoldoutBold);
            EditorGUI.EndFoldoutHeaderGroup();
            propertyRect.y += propertyRect.height + 5;
            propertyRect.x = position.x;
            propertyRect.width = position.width;
            GUI.color = guiColor;
            if (foldTargetDrawer)
            {
                if(targetValue is GameObject && ((GameObject)targetValue).scene.name == null)
                {
                    //Prefab value
                    EditorGUI.BeginChangeCheck();
                    DrawProperty(ref propertyRect, overrideSpawn, new GUIContent("Override Spawn", "Prefab spawn transform would be overriden"));
                    if(EditorGUI.EndChangeCheck())
                    {
                        overrideSpawnValue = overrideSpawn.boolValue;
                    }

                    if(overrideSpawnValue)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperty(ref propertyRect, spawnPos, new GUIContent("Spawn Position"));
                        DrawProperty(ref propertyRect, spawnRot, new GUIContent("Spawn Rotation"));
                        DrawProperty(ref propertyRect, spawnScale, new GUIContent("Spawn Scale"));
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.BeginChangeCheck();
                DrawProperty(ref propertyRect, useModuleMask, new GUIContent("Use Module Mask", "Should the module mask be used. Module mask will determin which module should be used for the target. If not tick, all module would be applied to the target"));
                if(EditorGUI.EndChangeCheck())
                {
                    useModuleMaskValue = useModuleMask.boolValue;
                }

                if(useModuleMaskValue)
                {
                    EditorGUI.indentLevel++;
                    propertyRect.height = EditorGUI.GetPropertyHeight(moduleMask);
                    EditorGUI.BeginChangeCheck();
                    moduleMaskValue = EditorGUI.MaskField(propertyRect, new GUIContent("Module Mask", "Determins which module could be used by the target"), moduleMaskValue, moduleConfigType);
                    if (EditorGUI.EndChangeCheck())
                    {
                        moduleMask.intValue = moduleMaskValue;
                    }

                    propertyRect.y += propertyRect.height + 5;
                    EditorGUI.indentLevel--;
                }

                DrawProperty(ref propertyRect, startInactive, new GUIContent("Start Inactive", "Will the target object start inactive"));
                DrawProperty(ref propertyRect, highlightTarget, new GUIContent("Highlight Target", "Will target renderer objects be highlighted, this only works for non UI objects"));
                DrawProperty(ref propertyRect, followTarget, new GUIContent("Follow Target", "Should the mask follow target in realtime, do not use this if object is stationary for better performance"));

                EditorGUI.BeginChangeCheck();
                DrawProperty(ref propertyRect, TargetBoundType, new GUIContent("Target Bound Type", "The bound type would be used to calculate the target object bounding size for highlighting mask"));
                if(EditorGUI.EndChangeCheck())
                {
                    TargetBoundTypeValue = TargetBoundType.enumValueIndex;
                }

                switch ((BoundType)TargetBoundTypeValue)
                {
                    case BoundType.All:
                        break;
                    case BoundType.RectTransform:
                        if (targetValue != null)
                        {
                            var recttransform = (targetValue as GameObject).GetComponent<RectTransform>();
                            if (recttransform == null)
                            {
                                GUI.color = new Color(1f, 0.3f, 0f, 1f);
                                propertyRect.height = 20;
                                EditorGUI.LabelField(propertyRect, new GUIContent("Recttransform could not be found on target. Please check the target reference field"), wrapStyle);
                                propertyRect.y += propertyRect.height + 5;
                            }
                        }
                        break;
                    default:
                        break;
                }

                #region Spawn Effect
                EditorGUI.BeginChangeCheck();
                tempRect = propertyRect;
                DrawProperty(ref propertyRect, spawnEffect, new GUIContent("Spawn Effect", "Should spawn effect prefab"));
                if(EditorGUI.EndChangeCheck())
                {
                    spawnEffectValue = spawnEffect.boolValue;
                }

                if(spawnEffectValue)
                {
                    foldTargetSpawn = EditorGUI.Foldout(tempRect, foldTargetSpawn, "");
                    if (foldTargetSpawn)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperty(ref propertyRect, effectPrefab, new GUIContent("Effect Prefab", "GameObject prefab would be instantiated when tutorial starts"));

                        EditorGUI.BeginChangeCheck();
                        DrawProperty(ref propertyRect, overrideEffectInfo, new GUIContent("Override Effect Spawn", "Should override effect spawn position, rotation and scale, otherwise, it would use target transform position, rotation and scale"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            overrideEffectInfoValue = overrideEffectInfo.boolValue;
                        }

                        if (overrideEffectInfoValue)
                        {
                            buttonRect = propertyRect;
                            buttonRect.x += 30;
                            buttonRect.width -= 60;
                            buttonRect.height = 30;
                            EditorGUI.indentLevel++;
                            if (GUI.Button(buttonRect, new GUIContent("Copy Transform", "Copies target transform information to the spawn information")))
                            {
                                if (targetValue != null && targetValue is GameObject)
                                {
                                    var targetInfo = targetValue as GameObject;
                                    effectPos.vector3Value = targetInfo.transform.position;
                                    effectRot.vector3Value = targetInfo.transform.rotation.eulerAngles;
                                    effectScale.vector3Value = targetInfo.transform.localScale;
                                }
                            }
                            propertyRect.y += 40;

                            DrawProperty(ref propertyRect, effectPos, new GUIContent("Effect Position"));
                            DrawProperty(ref propertyRect, effectRot, new GUIContent("Effect Rotation"));
                            DrawProperty(ref propertyRect, effectScale, new GUIContent("Effect Scale"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                #endregion

                #region Trigger Section
                EditorGUI.BeginChangeCheck();
                tempRect = propertyRect;
                DrawProperty(ref propertyRect, useTrigger, new GUIContent("Use Trigger", "Use trigger for target, this is different from Tutorial trigger"));
                if(EditorGUI.EndChangeCheck())
                {
                    useTriggerValue = useTrigger.boolValue;
                }

                if(useTriggerValue)
                {
                    foldTriggerConfig = EditorGUI.Foldout(tempRect, foldTriggerConfig, "");
                    if (foldTriggerConfig)
                    {
                        EditorGUI.indentLevel++;
                        DrawTriggerSection();
                        EditorGUI.indentLevel--;
                    }
                }
                #endregion
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }

        private void DrawProperty(ref Rect rect, SerializedProperty property, GUIContent content)
        {
            rect.height = EditorGUI.GetPropertyHeight(property);
            EditorGUI.PropertyField(rect, property, content);
            rect.y += rect.height + 5;
        }

        private void DrawTriggerSection()
        {
            EditorGUI.BeginChangeCheck();
            DrawProperty(ref propertyRect, triggerType, new GUIContent("Target Trigger", "Manual: Manual Trigger could be called using script API or tutorial object setup\n" +
                          "Collider: Collider/Trigger object used to trigger Tutorial Object\n" +
                          "Graphic: UI Graphic used to detect UI click"));
            if (EditorGUI.EndChangeCheck())
            {
                triggerTypeValue = triggerType.enumValueIndex;
                if((TriggerType)triggerTypeValue == TriggerType.Collider)
                {
                    if(collider != null && collider.objectReferenceValue == null)
                    {
                        if(targetValue != null && targetValue is GameObject)
                        {
                            var targetInfo = targetValue as GameObject;
                            var targetCollider = targetInfo.GetComponent<Collider>();
                            if(targetCollider != null)
                            {
                                collider.objectReferenceValue = targetCollider;
                            }
                        }
                    }
                }
                else if((TriggerType)triggerTypeValue == TriggerType.UI)
                {
                    if (graphic != null && graphic.objectReferenceValue == null)
                    {
                        if (targetValue != null && targetValue is GameObject)
                        {
                            var targetInfo = targetValue as GameObject;
                            var targetGraphic = targetInfo.GetComponent<RectTransform>();
                            if (targetGraphic != null)
                            {
                                graphic.objectReferenceValue = targetGraphic;
                            }
                        }
                    }
                }
            }

            EditorGUI.indentLevel++;
            switch ((TriggerType)triggerTypeValue)
            {
                case TriggerType.Collider:
                    if (colliderConfig != null)
                    {
                        DrawProperty(ref propertyRect, collider, new GUIContent("Trigger Collider", "Collider object used to trigger Target"));
                        DrawProperty(ref propertyRect, triggerOnClick, new GUIContent("Trigger On Click", "Should the Target being triggered when mouse or touch down on scene object"));
                        DrawProperty(ref propertyRect, colliderPointerTrigger, new GUIContent("Pointer Trigger", "PointerDown: Event trigger on pointer down\nPointerUp: Event trigger on pointer up\nPointerClick: Event trigger on pointer click"));
                        EditorGUI.BeginChangeCheck();
                        DrawProperty(ref propertyRect, layerFiltering, new GUIContent("Use Layer Filtering"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            layerFilteringValue = layerFiltering.boolValue;
                        }

                        if (layerFilteringValue)
                        {
                            EditorGUI.indentLevel++;
                            DrawProperty(ref propertyRect, filterLayer, new GUIContent("Layer", "Filter layer for gameobject that collides trigger object"));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.BeginChangeCheck();
                        DrawProperty(ref propertyRect, tagFiltering, new GUIContent("Use Tag Filtering"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            tagFilteringValue = tagFiltering.boolValue;
                        }

                        if (tagFilteringValue)
                        {
                            EditorGUI.indentLevel++;
                            DrawProperty(ref propertyRect, filterTag, new GUIContent("Tag", "Filter Tag for gameobject that collides trigger object"));
                            DrawProperty(ref propertyRect, useRigidbodyTag, new GUIContent("Use Rigidbody Tag", "Should use rigidbody's tag instead of trigger gameobject"));
                            EditorGUI.indentLevel--;
                        }
                    }
                    break;
                case TriggerType.Manual:
                    break;
                case TriggerType.UI:
                    if (graphicConfig != null)
                    {
                        DrawProperty(ref propertyRect, graphic, new GUIContent("Graphic", "Graphic used to detect UI click"));
                        DrawProperty(ref propertyRect, graphicPointerTrigger, new GUIContent("Pointer Trigger", "PointerDown: Event trigger on pointer down\nPointerUp: Event trigger on pointer up\nPointerClick: Event trigger on pointer click"));
                    }
                    break;
                case TriggerType.KeyCode:
                    DrawProperty(ref propertyRect, keyCode, new GUIContent("Key Code", "Key press to trigger stop Target event"));
                    break;
            }
            EditorGUI.indentLevel--;
        }
    }
}