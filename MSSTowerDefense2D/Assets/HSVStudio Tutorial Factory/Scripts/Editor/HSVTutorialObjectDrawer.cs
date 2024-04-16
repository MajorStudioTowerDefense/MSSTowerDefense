using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace HSVStudio.Tutorial.TMEditor
{
    [CustomPropertyDrawer(typeof(HSVTutorialObject))]
    public class HSVTutorialObjectsDrawer : PropertyDrawer
    {
        #region Serialized Properties
        private ReorderableList hsvTargetList;
        private ReorderableList hsvModuleConfigList;
        private string tObjPropertyPath;
        private string targetPropertyPath;
        private string moduleConfigPropertyPath;
        private int previousModuleIndex, previousTargetIndex = -1;
        bool updateCache = false;
        #endregion

        GUIStyle titleStyle;
        GUIStyle boxStyle;
        GUIStyle wrapStyle;
        GUIStyle textAreaWrap;
        GUIStyle titleFoldout;
        GUIStyle titleFoldoutBold;
        GUIStyle buttonStyle;
        bool objectSetupFoldout = true;
        bool foldStartTrigger = true, foldEndTrigger = true;
        bool foldEvents = true;
        bool foldTargetSetup = true;
        bool foldModuleConfig = true;
        Rect propertyRect, tempRect;
        float height;
        Color guiColor, backgroundColor;
        SerializedProperty currentTObj;

        Texture2D targetBgTex, moduleConfigBgTex;

        #region TutorialObject Cache
        int indexValue, stageIndexValue, focusTargetsSize, moduleConfigSize;
        string NameValue;
        SerializedProperty index, stageIndex, Name, startTrigger, endTrigger, advanceConfig, focusTargets, moduleConfigs;
        #endregion

        #region HSVEvents
        SerializedProperty events;
        List<SerializedProperty> tObjEvents = new List<SerializedProperty>();
        List<HSVEventType> eventsTypes = new List<HSVEventType>();
        List<bool> eventEnable = new List<bool>();
        List<SerializedProperty> subEvents = new List<SerializedProperty>();
        #endregion

        #region Target Cache
        List<UnityEngine.Object> allTargetsValue = new List<UnityEngine.Object>();
        List<SerializedProperty> allTargets = new List<SerializedProperty>();
        SerializedProperty targetElement;
        #endregion

        #region Module Config Cache
        List<string> moduleTypeNames = new List<string>();
        SerializedProperty targetModule;
        #endregion

        #region Trigger Cache
        int startTriggerTypeValue, endTriggerTypeValue;
        bool startLayerFilteringValue, endLayerFilteringValue, startTagFilteringValue, endTagFilteringValue;

        SerializedProperty startTriggerType, startColliderConfig, startCollider, startTriggerOnClick, startLayerFiltering, startFilterLayer, startTagFiltering, startFilterTag, startUseRigidbodyTag, startGraphicConfig, startGraphic, startPointerTrigger, startKeyCode,
            endTriggerType, endColliderConfig, endCollider, endTriggerOnClick, endLayerFiltering, endFilterLayer, endTagFiltering, endFilterTag, endUseRigidbodyTag, endGraphicConfig, endGraphic, endPointerTrigger, endKeyCode;

        int advanceTypeValue;
        SerializedProperty advanceType, advanceIndex, advanceName;
        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            currentTObj = property;
            backgroundColor = GUI.backgroundColor;
            guiColor = GUI.color;
            propertyRect = position;
            if (property.serializedObject == null)
                return;

            if (titleFoldout == null)
            {
                titleFoldout = new GUIStyle(EditorStyles.foldout);
                titleFoldout.fontSize = 13;
                titleFoldout.fontStyle = FontStyle.Bold;
            }

            if (titleFoldoutBold == null)
            {
                titleFoldoutBold = new GUIStyle(EditorStyles.foldoutHeader);
                titleFoldoutBold.fontSize = 13;
                titleFoldoutBold.fontStyle = FontStyle.Bold;
            }

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontSize = 13;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.MiddleCenter;
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

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.label);
                Color[] pix = new Color[] { Color.clear };
                Texture2D result = new Texture2D(1, 1);
                result.SetPixels(pix);
                result.Apply();
                buttonStyle.normal.background = result;
                Texture2D onHover = new Texture2D(1, 1);
                pix = new Color[] { new Color(0.25f, 0.25f, 0.25f, 0.5f) };
                onHover.SetPixels(pix);
                onHover.Apply();
                buttonStyle.onNormal.background = result;
                buttonStyle.onActive.background = onHover;
                buttonStyle.active.background = onHover;
                buttonStyle.fontSize = 20;
                buttonStyle.alignment = TextAnchor.MiddleLeft;
                buttonStyle.margin = new RectOffset(0, 0, 0, 0);
            }

            if (tObjPropertyPath != property.propertyPath || updateCache || HSVTutorialManager.undoRedoPerformed || HSVTutorialManager.tmModified)
            {
                tObjPropertyPath = property.propertyPath;
                CacheTObjectProperties(property);
            }

            label = EditorGUI.BeginProperty(position, label, property);
            propertyRect.height = 20;
            GUI.color = Color.green;
            EditorGUI.indentLevel++;
            propertyRect.width -= 10;
            propertyRect.x += 10;
            objectSetupFoldout = EditorGUI.BeginFoldoutHeaderGroup(propertyRect, objectSetupFoldout, new GUIContent("Object Setup"), titleFoldoutBold);
            EditorGUI.EndFoldoutHeaderGroup();
            GUI.color = guiColor;
            propertyRect.y += propertyRect.height + 5;

            propertyRect.x = position.x;
            propertyRect.width = position.width;
            if (objectSetupFoldout)
            {
                propertyRect.height = EditorGUI.GetPropertyHeight(index);
                EditorGUI.LabelField(propertyRect, "Stage Index: " + stageIndexValue.ToString() + ",   Object Index:    " + indexValue.ToString());
                propertyRect.y += propertyRect.height + 5;
                EditorGUI.BeginChangeCheck();
                DrawProperty(ref propertyRect, Name, new GUIContent("Name:"));

                if (EditorGUI.EndChangeCheck())
                {
                    if (!HSVTutorialManager.Instance.CheckCurrentStageTutorialObjectName(stageIndexValue, Name.stringValue))
                    {
                        Name.stringValue = NameValue;
                    }
                    else
                    {
                        NameValue = Name.stringValue;
                        HSVTutorialManager.tObjNameChange = true;
                    }
                }

                #region Trigger
                DrawStartTriggerSection();
                DrawEndTriggerSection();
                DrawAdvanceConfigSection();
                #endregion

                #region Callback
                DrawEvents();
                #endregion
                propertyRect.y += 10;

                #region Target Setup Region
                propertyRect.height = 20;
                guiColor = GUI.color;
                GUI.color = Color.cyan;
                propertyRect.width -= 20;
                propertyRect.x += 15;
                foldTargetSetup = EditorGUI.BeginFoldoutHeaderGroup(propertyRect, foldTargetSetup, new GUIContent("Targets"), titleFoldout);
                EditorGUI.EndFoldoutHeaderGroup();
                propertyRect.y += propertyRect.height + 5;
                GUI.color = guiColor;
                propertyRect.x = position.x;
                propertyRect.width = position.width;
                if (foldTargetSetup)
                {
                    if (focusTargets != null)
                    {
                        propertyRect.y += 10;
                        if (hsvTargetList == null || targetPropertyPath != focusTargets.propertyPath || HSVTutorialManager.undoRedoPerformed || HSVTutorialManager.tmModified)
                        {
                            if (HSVTutorialManager.undoRedoPerformed && hsvTargetList != null)
                            {
                                EditorPrefs.SetInt("TargetListIndex", hsvTargetList.index);
                            }

                            targetPropertyPath = focusTargets.propertyPath;
                            hsvTargetList = BuildTarget(focusTargets);

                            if (HSVTutorialManager.undoRedoPerformed && hsvTargetList != null)
                            {
                                var listIndex = EditorPrefs.GetInt("TargetListIndex", -1);
                                listIndex = Mathf.Min(listIndex, hsvTargetList.serializedProperty.arraySize - 1);
                                hsvTargetList.index = listIndex;
                                CacheTarget(hsvTargetList);
                                HSVTutorialManager.targetChange = true;
                            }
                        }

                        if (updateCache)
                        {
                            CacheTarget(hsvTargetList);
                        }

                        propertyRect.height = hsvTargetList.GetHeight();
                        hsvTargetList.DoList(propertyRect);
                        propertyRect.y += propertyRect.height + 10;

                        if (focusTargetsSize > 0 && hsvTargetList != null && hsvTargetList.index != -1 && hsvTargetList.index < allTargets.Count && hsvTargetList.index < allTargetsValue.Count)
                        {
                            if (allTargets[hsvTargetList.index] != null && allTargetsValue[hsvTargetList.index] == null)
                            {
                                guiColor = GUI.color;
                                GUI.color = Color.red;
                                propertyRect.height = 40;
                                EditorGUI.LabelField(propertyRect, "There is no object reference for the target.\nCertain modules may not work without target", wrapStyle);
                                propertyRect.y += propertyRect.height + 5;
                                GUI.color = guiColor;
                            }

                            if (targetElement != null)
                            {
                                tempRect = propertyRect;
                                propertyRect.height = EditorGUI.GetPropertyHeight(targetElement, true);
                                EditorGUI.PropertyField(propertyRect, targetElement);
                                propertyRect.y += propertyRect.height + 5;
                                tempRect.height = propertyRect.y - tempRect.y - 5;
                                Color targetRectColor = new Color(0.1f, 0.85f, 0.85f, 0.03f);
                                EditorGUI.DrawRect(tempRect, targetRectColor);
                            }
                        }
                    }
                }
                #endregion
                tempRect = propertyRect;
                DrawUILine(Color.gray, ref propertyRect);
                propertyRect.x = tempRect.x;
                propertyRect.width = tempRect.width;

                propertyRect.y += 10;
                #region Module Setup Region
                propertyRect.height = 20;
                guiColor = GUI.color;
                GUI.color = new Color(1f, 0.7f, 0.2f, 1f);
                propertyRect.width -= 20;
                propertyRect.x += 15;
                foldModuleConfig = EditorGUI.BeginFoldoutHeaderGroup(propertyRect, foldModuleConfig, new GUIContent("Modules"), titleFoldout);
                EditorGUI.EndFoldoutHeaderGroup();
                propertyRect.y += propertyRect.height + 5;
                propertyRect.x = position.x;
                propertyRect.width = position.width;
                GUI.color = guiColor;

                if (foldModuleConfig)
                {
                    if (moduleConfigs != null)
                    {
                        propertyRect.y += 10;

                        if (hsvModuleConfigList == null || moduleConfigPropertyPath != moduleConfigs.propertyPath || HSVTutorialManager.undoRedoPerformed || HSVTutorialManager.tmModified)
                        {
                            if (HSVTutorialManager.undoRedoPerformed && hsvModuleConfigList != null)
                            {
                                EditorPrefs.SetInt("ModuleConfigIndex", hsvModuleConfigList.index);
                            }

                            moduleConfigPropertyPath = moduleConfigs.propertyPath;
                            hsvModuleConfigList = BuildModuleConfig(moduleConfigs);

                            if (HSVTutorialManager.undoRedoPerformed && hsvModuleConfigList != null)
                            {
                                var listIndex = EditorPrefs.GetInt("ModuleConfigIndex", -1);
                                listIndex = Mathf.Min(listIndex, hsvModuleConfigList.serializedProperty.arraySize - 1);
                                hsvModuleConfigList.index = listIndex;
                                CacheModuleConfig(hsvModuleConfigList);
                            }
                        }

                        if (updateCache)
                        {
                            CacheModuleConfig(hsvModuleConfigList);
                        }

                        propertyRect.height = hsvModuleConfigList.GetHeight();
                        hsvModuleConfigList.DoList(propertyRect);
                        propertyRect.y += propertyRect.height + 5;

                        if (moduleConfigSize > 0 && hsvModuleConfigList != null && hsvModuleConfigList.index != -1)
                        {
                            if (hsvModuleConfigList.index >= moduleConfigSize)
                            {
                                CacheModuleConfig(hsvModuleConfigList);
                                hsvModuleConfigList.index = moduleConfigSize - 1;
                            }

                            if (previousModuleIndex != hsvModuleConfigList.index)
                            {
                                CacheModuleConfig(hsvModuleConfigList);
                            }

                            if (targetModule != null)
                            {
                                tempRect = propertyRect;
                                propertyRect.height = EditorGUI.GetPropertyHeight(targetModule, true);
                                EditorGUI.PropertyField(propertyRect, targetModule);
                                propertyRect.y += propertyRect.height + 5;
                                tempRect.height = propertyRect.y - tempRect.y - 5;
                                Color targetRectColor = new Color(0.8f, 0.3f, 0f, 0.05f);
                                EditorGUI.DrawRect(tempRect, targetRectColor);
                            }
                        }

                        if (hsvModuleConfigList != null)
                        {
                            previousModuleIndex = hsvModuleConfigList.index;
                        }
                    }
                }
                #endregion
            }
            EditorGUI.EndProperty();
            GUI.backgroundColor = backgroundColor;
            GUI.color = guiColor;
            height = propertyRect.y - position.y;
            if (updateCache)
            {
                updateCache = false;
            }
        }

        private void CacheTObjectProperties(SerializedProperty property)
        {
            index = property.FindPropertyRelative("index");
            indexValue = index.intValue;
            stageIndex = property.FindPropertyRelative("stageIndex");
            stageIndexValue = stageIndex.intValue;
            Name = property.FindPropertyRelative("Name");
            NameValue = Name.stringValue;

            #region Trigger Cache
            startTrigger = property.FindPropertyRelative("startTrigger");
            if (startTrigger != null)
            {
                startTriggerType = startTrigger.FindPropertyRelative("triggerType");
                startTriggerTypeValue = startTriggerType.enumValueIndex;
                startColliderConfig = startTrigger.FindPropertyRelative("colliderConfig");
                if (startColliderConfig != null)
                {
                    startCollider = startColliderConfig.FindPropertyRelative("collider");
                    startTriggerOnClick = startColliderConfig.FindPropertyRelative("triggerOnClick");
                    startLayerFiltering = startColliderConfig.FindPropertyRelative("layerFiltering");
                    startLayerFilteringValue = startLayerFiltering.boolValue;
                    startFilterLayer = startColliderConfig.FindPropertyRelative("filterLayer");
                    startTagFiltering = startColliderConfig.FindPropertyRelative("tagFiltering");
                    startTagFilteringValue = startTagFiltering.boolValue;
                    startFilterTag = startColliderConfig.FindPropertyRelative("filterTag");
                    startUseRigidbodyTag = startColliderConfig.FindPropertyRelative("useRigidBodyTag");
                }
                startGraphicConfig = startTrigger.FindPropertyRelative("graphicConfig");
                if (startGraphicConfig != null)
                {
                    startGraphic = startGraphicConfig.FindPropertyRelative("graphic");
                    startPointerTrigger = startGraphicConfig.FindPropertyRelative("pointerTrigger");
                }

                startKeyCode = startTrigger.FindPropertyRelative("keyCode");
            }

            endTrigger = property.FindPropertyRelative("endTrigger");
            if (endTrigger != null)
            {
                endTriggerType = endTrigger.FindPropertyRelative("triggerType");
                endTriggerTypeValue = endTriggerType.enumValueIndex;
                endColliderConfig = endTrigger.FindPropertyRelative("colliderConfig");
                if (endColliderConfig != null)
                {
                    endCollider = endColliderConfig.FindPropertyRelative("collider");
                    endTriggerOnClick = endColliderConfig.FindPropertyRelative("triggerOnClick");
                    endLayerFiltering = endColliderConfig.FindPropertyRelative("layerFiltering");
                    endLayerFilteringValue = endLayerFiltering.boolValue;
                    endFilterLayer = endColliderConfig.FindPropertyRelative("filterLayer");
                    endTagFiltering = endColliderConfig.FindPropertyRelative("tagFiltering");
                    endTagFilteringValue = endTagFiltering.boolValue;
                    endFilterTag = endColliderConfig.FindPropertyRelative("filterTag");
                    endUseRigidbodyTag = endColliderConfig.FindPropertyRelative("useRigidBodyTag");
                }
                endGraphicConfig = endTrigger.FindPropertyRelative("graphicConfig");
                if (endGraphicConfig != null)
                {
                    endGraphic = endGraphicConfig.FindPropertyRelative("graphic");
                    endPointerTrigger = endGraphicConfig.FindPropertyRelative("pointerTrigger");
                }
                endKeyCode = endTrigger.FindPropertyRelative("keyCode");
            }

            advanceConfig = property.FindPropertyRelative("advanceConfig");
            if (advanceConfig != null)
            {
                advanceType = advanceConfig.FindPropertyRelative("advanceType");
                advanceTypeValue = advanceType.enumValueIndex;
                advanceIndex = advanceConfig.FindPropertyRelative("index");
                advanceName = advanceConfig.FindPropertyRelative("name");
            }
            #endregion

            #region Events
            events = property.FindPropertyRelative("events");
            if (events == null || events.arraySize != Enum.GetValues(typeof(HSVEventType)).Length)
            {
                var tObj = HSVTutorialManager.Instance.GetCurrentTutorialObject(stageIndexValue - 1, indexValue - 1);
                if(tObj != null)
                {
                    HSVEvent.CreateEvents(ref tObj.events);
                    property.serializedObject.UpdateIfRequiredOrScript();
                    events = property.FindPropertyRelative("events");
                }
            }

            if (events != null)
            {
                tObjEvents.Clear();
                eventsTypes.Clear();
                eventEnable.Clear();
                subEvents.Clear();
                for (int i = 0; i < events.arraySize; i++)
                {
                    var element = events.GetArrayElementAtIndex(i);
                    tObjEvents.Add(element);
                    eventsTypes.Add((HSVEventType)element.FindPropertyRelative("type").enumValueIndex);
                    eventEnable.Add(element.FindPropertyRelative("enable").boolValue);
                    subEvents.Add(element.FindPropertyRelative("subEvent"));
                }
            }
            #endregion

            focusTargets = property.FindPropertyRelative("focusTargets");
            focusTargetsSize = focusTargets.arraySize;
            moduleConfigs = property.FindPropertyRelative("moduleConfigs");
            moduleConfigSize = moduleConfigs.arraySize;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }

        private ReorderableList BuildTarget(SerializedProperty property)
        {
            ReorderableList list = new ReorderableList(property.serializedObject, property, false, true, true, true);
            if (targetBgTex == null)
                targetBgTex = new Texture2D(1, 1);

            CacheTarget(list);

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Target Reference");
            };
            list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                if (property.arraySize > 0 && allTargetsValue.Count > 0 && index < allTargetsValue.Count)
                {
                    if (active)
                    {
                        if (allTargetsValue[index] == null)
                        {
                            targetBgTex.SetPixel(0, 0, new Color(1, 0, 0, 0.33f));
                        }
                        else
                        {
                            targetBgTex.SetPixel(0, 0, new Color(0.1f, 0.33f, 1f, 0.33f));
                        }
                        targetBgTex.Apply();

                        GUI.DrawTexture(rect, targetBgTex as Texture);
                    }
                }
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                if (property.arraySize > 0 && allTargets.Count > 0 && index < allTargets.Count)
                {
                    rect.y += 2;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(rect, allTargets[index], new GUIContent("Target:"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.ApplyModifiedProperties();
                        HSVTutorialManager.targetChange = true;
                        CacheTarget(list);
                        HSVTutorialManager.Instance.InitializeCurrentTutorialObject(currentTObj.FindPropertyRelative("stageIndex").intValue, currentTObj.FindPropertyRelative("index").intValue);
                    }

                    if(isActive)
                    {
                        if(previousTargetIndex != index)
                        {
                            CacheTarget(list);
                        }
                    }
                }
            };
            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
            {
                GenericMenu menu = new GenericMenu();

                foreach (BoundType type in Enum.GetValues(typeof(BoundType)))
                {
                    AddMenuItemForTarget(menu, type.ToString(), type);
                }

                menu.ShowAsContext();
            };
            list.onSelectCallback = (ReorderableList List) =>
            {
                CacheTarget(list);
            };
            list.onChangedCallback = (ReorderableList list) =>
            {
                CacheTarget(list);
            };
            list.onReorderCallback = (ReorderableList list) =>
            {
                CacheTarget(list);
            };

            return list;
        }

        private void CacheTarget(ReorderableList list)
        {
            if (allTargets == null)
                allTargets = new List<SerializedProperty>();

            allTargets.Clear();

            if (allTargetsValue == null)
                allTargetsValue = new List<UnityEngine.Object>();

            allTargetsValue.Clear();
            if (list.serializedProperty.arraySize > 0)
            {
                var property = list.serializedProperty;
                if (list.index != -1)
                {
                    targetElement = property.GetArrayElementAtIndex(list.index);
                    previousTargetIndex = list.index;
                }

                for (int i = 0; i < property.arraySize; i++)
                {
                    var target = property.GetArrayElementAtIndex(i).FindPropertyRelative("target");
                    allTargets.Add(target);
                    allTargetsValue.Add(target.objectReferenceValue);
                }
            }
        }

        // a method to simplify adding menu items
        void AddMenuItemForTarget(GenericMenu menu, string menuPath, BoundType type)
        {
            menu.AddItem(new GUIContent(menuPath), false, OnTargetSelected, type);
        }

        void OnTargetSelected(object type)
        {
            var index = hsvTargetList.serializedProperty.arraySize;
            hsvTargetList.serializedProperty.arraySize++;
            hsvTargetList.index = index;

            var element = hsvTargetList.serializedProperty.GetArrayElementAtIndex(hsvTargetList.index);
            element.FindPropertyRelative("TargetBoundType").enumValueIndex = (int)type;

            hsvTargetList.serializedProperty.serializedObject.ApplyModifiedProperties();
            updateCache = true;
        }

        #region Module Config

        private ReorderableList BuildModuleConfig(SerializedProperty property)
        {
            ReorderableList list = new ReorderableList(property.serializedObject, property, false, true, true, true);
            if (moduleConfigBgTex == null)
                moduleConfigBgTex = new Texture2D(1, 1);

            CacheModuleConfig(list);

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Module Config");
            };
            list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                if (property.arraySize > 0)
                {
                    if (active)
                    {
                        moduleConfigBgTex.SetPixel(0, 0, new Color(0.8f, 0.3f, 0f, 0.33f));
                        moduleConfigBgTex.Apply();
                        GUI.DrawTexture(rect, moduleConfigBgTex as Texture);
                    }
                }
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                if (property.arraySize > 0 && moduleTypeNames.Count > 0 && index < moduleTypeNames.Count)
                {
                    rect.y += 2;
                    EditorGUI.LabelField(rect, new GUIContent("Module Config:     " + moduleTypeNames[index]));
                    tempRect = new Rect(rect.xMax - 30, rect.y, 20, 20);

                    if (GUI.Button(tempRect, EditorGUIUtility.IconContent("_Menu@2x"), buttonStyle))
                    {
                        GenericMenu menu = new GenericMenu();

                        menu.AddItem(new GUIContent("Copy Settings"), false, CopySettingConfig, (object)index);
                        if (HSVTutorialManager.moduleIndex != null)
                        {
                            menu.AddItem(new GUIContent("Paste Settings"), false, PasteSettingConfig, (object)index);
                            menu.AddItem(new GUIContent("Paste Settings As New"), false, PasteSettingConfigAsNew, (object)index);
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Paste Settings"));
                            menu.AddDisabledItem(new GUIContent("Paste Settings As New"));
                        }
                        menu.ShowAsContext();
                    }
                }
            };
            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
            {
                GenericMenu menu = new GenericMenu();

                List<Type> showTypes = HSVTutorialManager.Instance.ModuleConfigTypes;

                for (int i = 0; i < showTypes.Count; ++i)
                {
                    Type type = showTypes[i];
                    string typeName = showTypes[i].Name;
                    //bool alreadyHasIt = DoesReordListHaveElementOfType(hsvModuleConfigList, typeName);
                    //if (alreadyHasIt)
                    //    continue;

                    typeName = typeName.Replace("HSV", "");
                    InsertSpaceBeforeCaps(ref typeName);
                    menu.AddItem(new GUIContent(typeName), false, OnAddItemFromDropdown, (object)type);
                }

                menu.ShowAsContext();
            };
            list.onSelectCallback = (ReorderableList List) =>
            {
                CacheModuleConfig(list);
            };
            list.onChangedCallback = (ReorderableList list) =>
            {
                CacheModuleConfig(list);
            };
            list.onReorderCallback = (ReorderableList list) =>
            {
                CacheModuleConfig(list);
            };

            return list;
        }

        private void CacheModuleConfig(ReorderableList list)
        {
            if (moduleTypeNames == null)
                moduleTypeNames = new List<string>();

            moduleTypeNames.Clear();

            if (list.serializedProperty.arraySize > 0)
            {
                var property = list.serializedProperty;
                if (list.index != -1)
                {
                    targetModule = property.GetArrayElementAtIndex(list.index);
                }

                for (int i = 0; i < property.arraySize; i++)
                {
                    var filterName = GetSubStrings(property.GetArrayElementAtIndex(i).type, "<HSV", "Config>");
                    moduleTypeNames.Add(filterName);
                }
            }
        }

        private void CopySettingConfig(object index)
        {
            var stageIndex = currentTObj.FindPropertyRelative("stageIndex");
            var tObjIndex = currentTObj.FindPropertyRelative("index");
            var moduleConfig = GetModuleConfig(stageIndex.intValue, tObjIndex.intValue, (int)index);
            if (moduleConfig != null)
            {
                HSVTutorialManager.moduleIndex = new HSVModuleConfigIndex()
                {
                    stageIndex = stageIndex.intValue,
                    tObjIndex = tObjIndex.intValue,
                    index = (int)index
                };
            }
        }

        private void PasteSettingConfig(object index)
        {
            if (HSVTutorialManager.moduleIndex != null)
            {
                var stageIndex = currentTObj.FindPropertyRelative("stageIndex");
                var tObjIndex = currentTObj.FindPropertyRelative("index");
                var tObj = HSVTutorialManager.Instance.GetCurrentTutorialObject(stageIndex.intValue - 1, tObjIndex.intValue - 1);
                if (tObj != null && tObj.moduleConfigs != null && tObj.moduleConfigs.Length > 0)
                {
                    if ((int)index < tObj.moduleConfigs.Length)
                    {
                        var newConfig = GetModuleConfig(HSVTutorialManager.moduleIndex.stageIndex, HSVTutorialManager.moduleIndex.tObjIndex, HSVTutorialManager.moduleIndex.index);
                        if (newConfig != null)
                        {
                            if (tObj.moduleConfigs[(int)index] != null)
                            {
                                if (tObj.moduleConfigs[(int)index].GetType() != newConfig.GetType())
                                {
                                    Debug.LogWarning("Config copy unsuccessful due to incompatible module config type :" + newConfig.GetType());
                                    return;
                                }
                            }

                            Undo.RecordObject(HSVTutorialManager.Instance, "Paste Config As New");
                            tObj.moduleConfigs[(int)index] = newConfig.Clone();
                            updateCache = true;
                        }
                    }
                }
            }
        }

        private void PasteSettingConfigAsNew(object index)
        {
            if (HSVTutorialManager.moduleIndex != null)
            {
                var newConfig = GetModuleConfig(HSVTutorialManager.moduleIndex.stageIndex, HSVTutorialManager.moduleIndex.tObjIndex, HSVTutorialManager.moduleIndex.index);

                var stageIndex = currentTObj.FindPropertyRelative("stageIndex");
                var tObjIndex = currentTObj.FindPropertyRelative("index");
                var tObj = HSVTutorialManager.Instance.GetCurrentTutorialObject(stageIndex.intValue - 1, tObjIndex.intValue - 1);

                if (tObj != null && tObj.moduleConfigs != null && newConfig != null)
                {
                    for (int i = 0; i < tObj.moduleConfigs.Length; i++)
                    {
                        if (tObj.moduleConfigs[i].GetType() == newConfig.GetType())
                        {
                            Debug.LogWarning("Config copy unsuccessful, Current copying type exist in the setting, try paste option :" + newConfig.GetType());
                            return;
                        }
                    }

                    Undo.RecordObject(HSVTutorialManager.Instance, "Paste Config As New");
                    int last = hsvModuleConfigList.serializedProperty.arraySize;
                    hsvModuleConfigList.serializedProperty.InsertArrayElementAtIndex(last);
                    hsvModuleConfigList.index = last;

                    SerializedProperty lastProp = hsvModuleConfigList.serializedProperty.GetArrayElementAtIndex(last);
                    lastProp.managedReferenceValue = newConfig.Clone();
                    hsvModuleConfigList.serializedProperty.serializedObject.ApplyModifiedProperties();
                    updateCache = true;
                }
            }
        }

        private HSVModuleConfig GetModuleConfig(int stageIndex, int tObjIndex, int index)
        {
            var tObj = HSVTutorialManager.Instance.GetCurrentTutorialObject(stageIndex - 1, tObjIndex - 1);
            if (tObj != null && tObj.moduleConfigs != null && tObj.moduleConfigs.Length > 0)
            {
                if (index < tObj.moduleConfigs.Length)
                {
                    if (tObj.moduleConfigs[index] != null)
                    {
                        return tObj.moduleConfigs[index];
                    }
                }
            }

            return null;
        }

        private void OnAddItemFromDropdown(object obj)
        {
            Type settingsType = (Type)obj;
            int last = hsvModuleConfigList.serializedProperty.arraySize;
            hsvModuleConfigList.serializedProperty.InsertArrayElementAtIndex(last);
            hsvModuleConfigList.index = last;

            SerializedProperty lastProp = hsvModuleConfigList.serializedProperty.GetArrayElementAtIndex(last);
            var configInstance = Activator.CreateInstance(settingsType);
            string modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_None.prefab";
            string maskPrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/Masks Prefab/DefaultMask.prefab";
            if (configInstance is HSVArrowModuleConfig)
            {
                modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_Arrow.prefab";
                maskPrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/Masks Prefab/ArrowMask.prefab";
            }
            else if (configInstance is HSVFocusDimModuleConfig)
            {
                modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_FocusDim.prefab";
                maskPrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/Masks Prefab/RectMask.prefab";
            }
            else if (configInstance is HSVHighlightModuleConfig)
            {
                modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_Highlight.prefab";
                maskPrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/Masks Prefab/HighlightMask.prefab";
            }
            else if (configInstance is HSVPopupModuleConfig)
            {
                modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_Popup.prefab";
                maskPrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/Masks Prefab/PopupMask.prefab";
            }
            else if (configInstance is HSVPositionModuleConfig)
            {
                modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_Position.prefab";
                maskPrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/Masks Prefab/PositionMask.prefab";
            }
            else if(configInstance is HSVBoundModuleConfig)
            {
                modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_Bound.prefab";
                maskPrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/Masks Prefab/BoundMask.prefab";
            }
            else if (configInstance is HSVInfoDisplayModuleConfig)
            {
                modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_InfoDisplay.prefab";
                maskPrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/Masks Prefab/PopupMask.prefab";
            }
            else if (configInstance is HSVTimeDelayModuleConfig)
            {
                modulePrefabPath = "Assets/HSVStudio Tutorial Factory/Prefabs/HSVModule_TimeDelay.prefab";
                maskPrefabPath = string.Empty;
            }

            var config = (HSVModuleConfig)configInstance;
            config.modulePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(modulePrefabPath, typeof(GameObject));
            config.maskPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(maskPrefabPath, typeof(GameObject));

            lastProp.managedReferenceValue = configInstance;
            hsvModuleConfigList.serializedProperty.serializedObject.ApplyModifiedProperties();
            updateCache = true;
        }

        private bool DoesReordListHaveElementOfType(ReorderableList list, string type)
        {
            for (int i = 0; i < list.serializedProperty.arraySize; ++i)
            {
                if (list.serializedProperty.GetArrayElementAtIndex(i).type.Contains(type))
                    return true;
            }

            return false;
        }

        private void InsertSpaceBeforeCaps(ref string theString)
        {
            for (int i = 0; i < theString.Length; ++i)
            {
                char currChar = theString[i];

                if (char.IsUpper(currChar))
                {
                    theString = theString.Insert(i, " ");
                    ++i;
                }
            }
        }
        #endregion

        #region Trigger Drawing Section
        private void DrawStartTriggerSection()
        {
            EditorGUI.BeginChangeCheck();
            DrawProperty(ref propertyRect, startTriggerType, new GUIContent("Start Trigger", "Manual: Manual Trigger could be called using script API or tutorial object setup\n" +
                          "Collider: Collider/Trigger object used to trigger Tutorial Object\n" +
                          "Graphic: UI Graphic used to detect UI click"));
            if (EditorGUI.EndChangeCheck())
            {
                startTriggerTypeValue = startTriggerType.enumValueIndex;
            }

            EditorGUI.indentLevel++;
            switch ((TriggerType)startTriggerTypeValue)
            {
                case TriggerType.Collider:
                    if (startColliderConfig != null)
                    {
                        foldStartTrigger = EditorGUI.Foldout(propertyRect, foldStartTrigger, "Collider Config", titleFoldout);
                        propertyRect.y += propertyRect.height + 5;
                        if (foldStartTrigger)
                        {
                            DrawProperty(ref propertyRect, startCollider, new GUIContent("Trigger Collider", "Collider object used to trigger Tutorial Object"));
                            DrawProperty(ref propertyRect, startTriggerOnClick, new GUIContent("Trigger On Click", "Should the tutorial object being triggered when mouse or touch down on scene object"));
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, startLayerFiltering, new GUIContent("Use Layer Filtering"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                startLayerFilteringValue = startLayerFiltering.boolValue;
                            }

                            if (startLayerFilteringValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, startFilterLayer, new GUIContent("Layer", "Filter layer for gameobject that collides trigger object"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, startTagFiltering, new GUIContent("Use Tag Filtering"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                startTagFilteringValue = startTagFiltering.boolValue;
                            }

                            if (startTagFilteringValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, startFilterTag, new GUIContent("Tag", "Filter Tag for gameobject that collides trigger object"));
                                DrawProperty(ref propertyRect, startUseRigidbodyTag, new GUIContent("Use Rigidbody Tag", "Should use rigidbody's tag instead of trigger gameobject"));
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                    break;
                case TriggerType.Manual:
                    break;
                case TriggerType.UI:
                    if (startGraphicConfig != null)
                    {
                        foldStartTrigger = EditorGUI.Foldout(propertyRect, foldStartTrigger, "Graphic Config", titleFoldout);
                        propertyRect.y += propertyRect.height + 5;
                        if (foldStartTrigger)
                        {
                            DrawProperty(ref propertyRect, startGraphic, new GUIContent("Graphic", "Graphic used to detect UI click"));
                            DrawProperty(ref propertyRect, startPointerTrigger, new GUIContent("Pointer Trigger", "PointerDown: Event trigger on pointer down\nPointerUp: Event trigger on pointer up\nPointerClick: Event trigger on pointer click"));
                        }
                    }
                    break;
                case TriggerType.KeyCode:
                    DrawProperty(ref propertyRect, startKeyCode, new GUIContent("Key Code", "Key press to trigger start tutorial event"));
                    break;
            }
            EditorGUI.indentLevel--;
        }

        private void DrawEndTriggerSection()
        {
            EditorGUI.BeginChangeCheck();
            DrawProperty(ref propertyRect, endTriggerType, new GUIContent("End Trigger", "Manual: Manual Trigger could be called using script API or tutorial object setup\n" +
                          "Collider: Collider/Trigger object used to trigger Tutorial Object\n" +
                          "Graphic: UI Graphic used to detect UI click"));
            if (EditorGUI.EndChangeCheck())
            {
                endTriggerTypeValue = endTriggerType.enumValueIndex;
            }

            EditorGUI.indentLevel++;
            switch ((TriggerType)endTriggerTypeValue)
            {
                case TriggerType.Collider:
                    if (endColliderConfig != null)
                    {
                        foldEndTrigger = EditorGUI.Foldout(propertyRect, foldEndTrigger, "Collider Config", titleFoldout);
                        propertyRect.y += propertyRect.height + 5;
                        if (foldEndTrigger)
                        {
                            DrawProperty(ref propertyRect, endCollider, new GUIContent("Trigger Collider", "Collider object used to trigger Tutorial Object"));
                            DrawProperty(ref propertyRect, endTriggerOnClick, new GUIContent("Trigger On Click", "Should the tutorial object being triggered when mouse or touch down on scene object"));
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, endLayerFiltering, new GUIContent("Use Layer Filtering"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                endLayerFilteringValue = endLayerFiltering.boolValue;
                            }

                            if (endLayerFilteringValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, endFilterLayer, new GUIContent("Layer", "Filter layer for gameobject that collides trigger object"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.BeginChangeCheck();
                            DrawProperty(ref propertyRect, endTagFiltering, new GUIContent("Use Tag Filtering"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                endTagFilteringValue = endTagFiltering.boolValue;
                            }

                            if (endTagFilteringValue)
                            {
                                EditorGUI.indentLevel++;
                                DrawProperty(ref propertyRect, endFilterTag, new GUIContent("Tag", "Filter Tag for gameobject that collides trigger object"));
                                DrawProperty(ref propertyRect, endUseRigidbodyTag, new GUIContent("Use Rigidbody Tag", "Should use rigidbody's tag instead of trigger gameobject"));
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                    break;
                case TriggerType.Manual:
                    break;
                case TriggerType.UI:
                    if (endGraphicConfig != null)
                    {
                        foldEndTrigger = EditorGUI.Foldout(propertyRect, foldEndTrigger, "Graphic Config", titleFoldout);
                        propertyRect.y += propertyRect.height + 5;
                        if (foldEndTrigger)
                        {
                            DrawProperty(ref propertyRect, endGraphic, new GUIContent("Graphic", "Graphic used to detect UI click"));
                            DrawProperty(ref propertyRect, endPointerTrigger, new GUIContent("Pointer Trigger", "PointerDown: Event trigger on pointer down\nPointerUp: Event trigger on pointer up\nPointerClick: Event trigger on pointer click"));
                        }
                    }
                    break;
                case TriggerType.KeyCode:
                    DrawProperty(ref propertyRect, endKeyCode, new GUIContent("Key Code", "Key press to trigger stop tutorial event"));
                    break;
            }
            EditorGUI.indentLevel--;
        }

        private void DrawAdvanceConfigSection()
        {
            EditorGUI.BeginChangeCheck();
            DrawProperty(ref propertyRect, advanceType, new GUIContent("Advance Type", "None: Do nothing after stopping\n" +
               "Automatic: it would increment index and play the tutorial object\n" +
               "Index: play the next tutorial object by index\n" +
               "Name: play the next tutorial object by name"));
            if (EditorGUI.EndChangeCheck())
            {
                advanceTypeValue = advanceType.enumValueIndex;
            }

            EditorGUI.indentLevel++;
            switch ((AdvanceType)advanceTypeValue)
            {
                case AdvanceType.None:
                    break;
                case AdvanceType.Index:
                    DrawProperty(ref propertyRect, advanceIndex, new GUIContent("Index", "Next Tutorial Object Index"));
                    break;
                case AdvanceType.Name:
                    DrawProperty(ref propertyRect, advanceName, new GUIContent("Name", "Next Tutorial Object Name"));
                    break;
            }
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Events Section
        private void DrawEvents()
        {
            foldEvents = EditorGUI.Foldout(propertyRect, foldEvents, "Tutorial Events", titleFoldout);
            propertyRect.y += propertyRect.height + 5;
            if (foldEvents)
            {
                var tempColor = new Color(0.1f, 0.3f, 0.1f, 0.5f);
                GUI.backgroundColor = tempColor;
                tempRect = propertyRect;
                propertyRect.width -= 10;
                propertyRect.x += 5;
                propertyRect.y += 5;
                propertyRect.height = 20;

                propertyRect.width = propertyRect.width / tObjEvents.Count - 5;
                for (int i = 0; i < tObjEvents.Count; i++)
                {
                    GUI.backgroundColor = eventEnable[i] ? Color.green : tempColor;
                    
                    if (GUI.Button(propertyRect, new GUIContent(eventsTypes[i].ToString(), GetToolTipForEvent(eventsTypes[i]))))
                    {
                        var enable = tObjEvents[i].FindPropertyRelative("enable");
                        enable.boolValue = !enable.boolValue;
                        eventEnable[i] = enable.boolValue;
                    }

                    propertyRect.x += propertyRect.width + 5;
                    GUI.backgroundColor = tempColor;
                }

                propertyRect.width = tempRect.width - 10;
                propertyRect.y += propertyRect.height + 5;
                propertyRect.height = tempRect.height;
                propertyRect.x = tempRect.x + 5;
                GUI.backgroundColor = backgroundColor;
                for (int i = 0; i < tObjEvents.Count; i++)
                {
                    if (eventEnable[i])
                    {
                        DrawProperty(ref propertyRect, subEvents[i], new GUIContent(eventsTypes[i].ToString()));
                    }
                }
                GUI.backgroundColor = tempColor;
                GUI.backgroundColor = backgroundColor;

                propertyRect.x = tempRect.x;
                propertyRect.width = tempRect.width;
                tempRect.height = propertyRect.y - tempRect.y;
                EditorGUI.indentLevel--;
                EditorGUI.LabelField(tempRect, GUIContent.none, boxStyle);
                EditorGUI.indentLevel++;
            }
        }

        private string GetToolTipForEvent(HSVEventType eventType)
        {
            string eventToolTip = string.Empty;
            switch (eventType)
            {
                case HSVEventType.OnStart:
                    eventToolTip = "Event would be invoked once when tutorial starts.";
                    break;
                case HSVEventType.OnUpdate:
                    eventToolTip = "Event would be invoked on every frame when tutorial started. It will be invoked continuously until the end of the tutorial";
                    break;
                case HSVEventType.OnExecuting:
                    eventToolTip = "Event would be invoked once when entering executing state. Tutorial will enter executing state when all module have done creating configuring masks";
                    break;
                case HSVEventType.OnEnding:
                    eventToolTip = "Event would be invoked once when trying to end tutorial";
                    break;
                case HSVEventType.OnComplete:
                    eventToolTip = "Event would be invoked once when stage is completed.";
                    break;
            }

            return eventToolTip;
        }
        #endregion

        private void DrawProperty(ref Rect rect, SerializedProperty property, GUIContent content, bool includeChildren = false)
        {
            if (property != null)
            {
                rect.height = EditorGUI.GetPropertyHeight(property);
                EditorGUI.PropertyField(rect, property, content, includeChildren);
                rect.y += rect.height + 5;
            }
        }

        private void DrawUILine(Color color, ref Rect currentRect, int thickness = 2, int padding = 10)
        {
            currentRect.y += thickness + padding;
            currentRect.height = thickness;
            currentRect.y += padding / 2;
            currentRect.x += 2;
            currentRect.width -= 6;
            EditorGUI.DrawRect(currentRect, color);
            currentRect.y += currentRect.height + 5;
        }

        private string GetSubStrings(string input, string start, string end)
        {
            var filterRegex = new Regex(Regex.Escape(start) + "([^()]*)" + Regex.Escape(end));

            return filterRegex.Match(input).Groups[1].Value;
        }
    }
}
