using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

namespace HSVStudio.Tutorial.TMEditor
{
    [CustomEditor(typeof(HSVTutorialManager))]
    public class HSVTutorialManagerEditor : Editor
    {
        HSVTutorialManager m_TutorialManager;

        #region Serialized Properties
        private ReorderableList stageList;
        private ReorderableList tutorialObjectsList;
        SerializedProperty m_camera, m_canvas, m_player, m_worldObjectContainer, m_assignOnRuntime, m_autoStart, m_debugMode, m_database, m_autoStartIndex, m_initializeOnAwake;
        #endregion

        GUIStyle foldoutBold;
        GUIStyle titleFoldoutBold;
        GUIStyle titleStyle;
        GUIStyle subtitleStyle;
        GUIStyle boxStyle;
        GUIStyle wrapStyle;
        GUIStyle buttonStyle;

        Color defaultBackgroundColor;
        Color guiColor;
        float editorWidth;

        bool expandRuntimeReference;
        const string HSV_RuntimeReference = "HSVRuntimeReference";
        const string HSV_CurrentSelectStage = "HSVCurrentSelectStage";
        const string HSV_CurrentSelectTO = "HSVCurrentSelectTO";
        const string HSV_CurrentListStep = "HSVCurrentListStep";
        const string HSV_TMObjectPage = "HSVTMObjectPage";
        const string HSV_TMObjectPageCount = "HSVTMObjectPageCount";
        const string HSV_PreviewState = "HSVPreviewState";

        int currentSelectedStage;
        int currentTutorialStep;
        int currentSelectedStep;
        int currentPage;
        int currentPageCount;
        int totalPageCount;
        string stagePropertyPath;
        bool foldStageElement = true;
        bool foldStartTrigger = true, foldEndTrigger = true;
        bool foldEvents = true;
        bool foldTutorialObj = true;
        bool updateTObject = false;
        bool preview = false;
        bool stageElementChange = false;
        bool cachedState = false;
        bool cacheUndo = false;
        Rect tempRect;
        Texture2D stageBgTex, tObjBgTex;

        #region HSVEvents
        SerializedProperty events;
        List<SerializedProperty> stageEvents = new List<SerializedProperty>();
        List<HSVEventType> eventsTypes = new List<HSVEventType>();
        List<bool> eventEnable = new List<bool>();
        List<SerializedProperty> subEvents = new List<SerializedProperty>();
        #endregion

        #region cache property vale
        //Stage serialized property
        int stageArraySize, tutorialObjectsSize, tmEditorListSize;
        private List<string> stageNameList = new List<string>();
        private List<PlayState> stageStatesList = new List<PlayState>();
        string currentStageName;
        SerializedProperty stageName, stageIndex, startObjectIndex, startTrigger, endTrigger, advanceConfig,
            tmEditorList, currentStep, allowMultiple, tutorialObjects;
        private List<string> tObjNameList = new List<string>();
        private List<PlayState> tObjStatesList = new List<PlayState>();
        SerializedProperty currentStageObj, currentTObj, tObjName, currentSelectTObj;

        //Trigger Cache
        #region Trigger Cache
        int startTriggerTypeValue, endTriggerTypeValue;
        bool startLayerFilteringValue, endLayerFilteringValue, startTagFilteringValue, endTagFilteringValue;

        SerializedProperty startTriggerType, startColliderConfig, startCollider, startTriggerOnClick, startLayerFiltering, startFilterLayer, startTagFiltering, startFilterTag, startUseRigidbodyTag, startGraphicConfig, startGraphic, startPointerTrigger, startKeyCode,
            endTriggerType, endColliderConfig, endCollider, endTriggerOnClick, endLayerFiltering, endFilterLayer, endTagFiltering, endFilterTag, endUseRigidbodyTag, endGraphicConfig, endGraphic, endPointerTrigger, endKeyCode;

        int advanceTypeValue;
        SerializedProperty advanceType, advanceIndex, advanceName;
        #endregion
        #endregion

        void OnEnable()
        {
            m_TutorialManager = (HSVTutorialManager)target;
            Undo.undoRedoPerformed += UndoRedoPerformed;
            preview = EditorPrefs.GetBool(HSV_PreviewState, false);
            expandRuntimeReference = EditorPrefs.GetBool(HSV_RuntimeReference, true);
            currentSelectedStage = EditorPrefs.GetInt(HSV_CurrentSelectStage, 1);
            currentTutorialStep = EditorPrefs.GetInt(HSV_CurrentSelectTO, 1);
            currentSelectedStep = EditorPrefs.GetInt(HSV_CurrentListStep, 1);
            currentPage = EditorPrefs.GetInt(HSV_TMObjectPage, 1);
            currentPage = Mathf.Max(currentPage, 1);
            currentPageCount = EditorPrefs.GetInt(HSV_TMObjectPageCount, 10);
            currentPageCount = Mathf.Max(currentPageCount, 1);
            if (stageBgTex == null)
                stageBgTex = new Texture2D(1, 1);

            if (tObjBgTex == null)
                tObjBgTex = new Texture2D(1, 1);

            #region Serialized properties initializatio
            m_camera = serializedObject.FindProperty("Camera");
            m_canvas = serializedObject.FindProperty("Canvas");
            m_player = serializedObject.FindProperty("Player");
            m_worldObjectContainer = serializedObject.FindProperty("WorldObjectContainer");
            m_initializeOnAwake = serializedObject.FindProperty("initializeOnAwake");
            m_assignOnRuntime = serializedObject.FindProperty("assignOnRuntime");
            m_autoStart = serializedObject.FindProperty("autoStart");
            m_autoStartIndex = serializedObject.FindProperty("autoStartIndex");
            m_debugMode = serializedObject.FindProperty("debugMode");
            m_database = serializedObject.FindProperty("database");
            tmEditorList = serializedObject.FindProperty("tmEditorList");

            if (stageNameList == null)
                stageNameList = new List<string>();

            if (stageStatesList == null)
                stageStatesList = new List<PlayState>();

            stageList = BuildStageList(serializedObject.FindProperty("stageObjects"));

            UpdateStageIndex();
            UpdateCurrentPage(currentTutorialStep);
            UpdatePageInfo();
            #endregion
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            EditorPrefs.SetBool(HSV_RuntimeReference, expandRuntimeReference);
            EditorPrefs.SetInt(HSV_CurrentSelectStage, currentSelectedStage);
            EditorPrefs.SetInt(HSV_CurrentSelectTO, currentTutorialStep);
            EditorPrefs.SetInt(HSV_CurrentListStep, currentSelectedStep);
            EditorPrefs.SetInt(HSV_TMObjectPage, currentPage);
            EditorPrefs.SetInt(HSV_TMObjectPageCount, currentPageCount);
            EditorPrefs.SetBool(HSV_PreviewState, preview);
            tutorialObjectsList = null;
            stageBgTex = null;
            tObjBgTex = null;
        }

        private void UndoRedoPerformed()
        {
            HSVTutorialManager.undoRedoPerformed = true;
        }

        public override void OnInspectorGUI()
        {
            //Debug.Log("Who calls me: " + Event.current.type + " even name: " + Event.current.isScrollWheel + " " + Time.time);
            serializedObject.Update();

            editorWidth = EditorGUIUtility.currentViewWidth;

            #region GUI Style Initialization
            if (foldoutBold == null)
            {
                foldoutBold = new GUIStyle(EditorStyles.foldout);
                foldoutBold.fontStyle = FontStyle.Bold;
            }

            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontSize = 15;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (titleFoldoutBold == null)
            {
                titleFoldoutBold = new GUIStyle(EditorStyles.foldout);
                titleFoldoutBold.fontSize = 13;
                titleFoldoutBold.fontStyle = FontStyle.Bold;
            }

            if (subtitleStyle == null)
            {
                subtitleStyle = new GUIStyle(EditorStyles.label);
                subtitleStyle.fontSize = 13;
                subtitleStyle.fontStyle = FontStyle.Bold;
                subtitleStyle.alignment = TextAnchor.MiddleCenter;
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
            #endregion

            if (HSVTutorialManager.undoRedoPerformed && stageList != null)
            {
                CacheStageName(stageList.serializedProperty);

                CacheStageElement(stageList);
            }

            if (HSVTutorialManager.stateChange && stageList != null)
            {
                CacheStageName(stageList.serializedProperty);
            }

            defaultBackgroundColor = GUI.backgroundColor;
            guiColor = GUI.color;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Tutorial Manager Setup", titleStyle, GUILayout.Width(editorWidth - 10));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            #region Database Section
            EditorGUILayout.PropertyField(m_database, new GUIContent("Tutorial Database", "Assets containing all saved information of tutorial objects"));
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Database"))
            {
                SaveDatabase();
            }

            if (GUILayout.Button("Load Database"))
            {
                LoadDatabase();
                if (stageList != null && stageList.serializedProperty.arraySize > 0)
                {
                    CacheStageName(stageList.serializedProperty);

                    CacheStageElement(stageList);
                }
            }

            if (GUILayout.Button("Create New Database"))
            {
                CreateNewDatabase();
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUI.indentLevel++;
            expandRuntimeReference = EditorGUILayout.Foldout(expandRuntimeReference, "Runtime Reference", true, foldoutBold);
            EditorGUI.indentLevel--;
            //Runtime Reference Setup
            if (expandRuntimeReference)
            {
                if (!m_TutorialManager.assignOnRuntime)
                {
                    if (m_TutorialManager.Camera == null)
                    {
                        GUI.backgroundColor = Color.red;
                    }
                }

                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.PropertyField(m_initializeOnAwake, new GUIContent("Initialize On Awake", "True: Will initialize on Awake. False: Initialize needs to be called manually"));
                EditorGUILayout.PropertyField(m_assignOnRuntime, new GUIContent("Assign On Runtime", "If not assign on runtime, you need to set the reference to the camera"));
                EditorGUILayout.PropertyField(m_camera, new GUIContent("Camera : ", " Runtime camera reference for tutorial manager"));
                EditorGUILayout.PropertyField(m_canvas, new GUIContent("Canvas : ", " All the tutorial modules would be spawned under this canvas"));
                EditorGUILayout.PropertyField(m_player, new GUIContent("Player : ", " Main player gameobject"));
                EditorGUILayout.PropertyField(m_worldObjectContainer, new GUIContent("WorldO bject Container :", "Container for all scene world objects"));
                EditorGUILayout.EndVertical();

                GUI.backgroundColor = defaultBackgroundColor;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            string previewLabel;
            if (preview)
            {
                previewLabel = "Preview (ON)";
                GUI.backgroundColor = Color.green;
            }
            else
            {
                previewLabel = "Preview (OFF)";
            }

            if (GUILayout.Button(new GUIContent(previewLabel)))
            {
                preview = !preview;

                if (!preview)
                {
                    m_TutorialManager.PreviewTutorialObject(-1, -1, false);
                    EditorUtility.SetDirty(m_TutorialManager.gameObject);
                }
            }

            GUI.backgroundColor = defaultBackgroundColor;
            if (GUILayout.Button(new GUIContent("Initialize Tutorial Manager")))
            {
                if (Application.isPlaying)
                {
                    m_TutorialManager.InitializeTutorialManager();
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Play Stage")))
            {
                m_TutorialManager.PlayStage(currentSelectedStage);
            }

            if (GUILayout.Button(new GUIContent("Stop Stage")))
            {
                m_TutorialManager.StopStage();
            }
            EditorGUILayout.EndHorizontal();
            if(Application.isPlaying)
            {
                if (GUILayout.Button(new GUIContent("Reset Stage")))
                {
                    m_TutorialManager.ResetStage(currentSelectedStage);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(20);

            #region Draw all the tutorial Objects
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.Space(10);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_autoStart, new GUIContent("Auto Start", "Should automatically start first tutorial on start"));
            if (m_autoStart.boolValue)
            {
                EditorGUILayout.PropertyField(m_autoStartIndex, new GUIContent("Auto Start Stage Index", "Which Stage should be started automatically"));
            }
            EditorGUILayout.PropertyField(m_debugMode, new GUIContent("Debug Mode", "Log the tutorial events"));

            if (m_TutorialManager.stageObjects != null && currentSelectedStage <= m_TutorialManager.stageObjects.Length && m_TutorialManager.stageObjects.Length > 0)
            {
                currentSelectedStage = Mathf.Max(1, currentSelectedStage);
                EditorGUILayout.LabelField(new GUIContent("Selected Stage Step: " + currentSelectedStage + " / " + m_TutorialManager.stageObjects.Length));
                EditorGUILayout.LabelField(new GUIContent("Current Tutorial Stage Step: " + m_TutorialManager.currentStageStep));
                if (Application.isPlaying && m_TutorialManager.stageObjects[m_TutorialManager.currentStageStep - 1] != null)
                {
                    EditorGUILayout.LabelField("Stage Object: " + m_TutorialManager.stageObjects[m_TutorialManager.currentStageStep - 1].stageName);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Play State:");
                    switch (m_TutorialManager.stageObjects[m_TutorialManager.currentStageStep - 1].state)
                    {
                        case PlayState.Idle:
                            GUI.color = Color.gray;
                            break;
                        case PlayState.Start:
                            GUI.color = Color.cyan;
                            break;
                        case PlayState.Executing:
                            GUI.color = Color.green;
                            break;
                        case PlayState.Ending:
                            GUI.color = new Color(1.0f, 0.5f, 0.1f, 1f);
                            break;
                        case PlayState.End:
                            GUI.color = Color.red;
                            break;
                    }

                    EditorGUILayout.LabelField(m_TutorialManager.stageObjects[m_TutorialManager.currentStageStep - 1].state.ToString());

                    GUI.color = guiColor;
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField("");
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);
            stageList.DoLayoutList();
            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
            #endregion
            stageArraySize = stageList.serializedProperty.arraySize;

            if (stageArraySize > 0 && stageList.index != -1)
            {
                if (stageList.index >= stageArraySize)
                {
                    stageList.index = stageArraySize - 1;
                    CacheStageElement(stageList);
                }

                EditorGUILayout.BeginVertical(boxStyle);
                DrawStageElementWithDetail(currentStageObj);
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();

            if (HSVTutorialManager.undoRedoPerformed)
            {
                if (!cacheUndo)
                {
                    cacheUndo = true;
                }
                else
                {
                    HSVTutorialManager.undoRedoPerformed = false;
                    cacheUndo = false;
                }
            }

            if (HSVTutorialManager.tmModified)
            {
                HSVTutorialManager.tmModified = false;
            }

            if (HSVTutorialManager.stateChange)
            {
                if(!cachedState)
                {
                    cachedState = true;
                }
                else
                {
                    HSVTutorialManager.stateChange = false;
                    cachedState = false;
                }
            }
        }

        #region Stage Drawing
        private ReorderableList BuildStageList(SerializedProperty property)
        {
            var list = new ReorderableList(property.serializedObject, property, true, true, true, true);
            if (stageBgTex == null)
                stageBgTex = new Texture2D(1, 1);

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Stages", titleStyle);
            };
            list.drawElementBackgroundCallback = (rect, index, active, focused) =>
            {
                if (active)
                {
                    stageBgTex.SetPixel(0, 0, new Color(0.75f, 0.75f, 0.1f, 0.33f));
                    stageBgTex.Apply();
                    GUI.DrawTexture(rect, stageBgTex as Texture);
                }
            };
            list.drawElementCallback = DrawStageElement;
            list.onAddCallback = OnAddStageElement;
            list.onSelectCallback = OnSelectStage;
            list.onCanRemoveCallback = OnCanRemoveStage;
            list.onRemoveCallback = OnRemoveStage;
            list.onReorderCallbackWithDetails = OnReorderStageElementCallback;
            list.onChangedCallback = OnChangeCallBack;
            currentSelectedStage = Mathf.Min(list.serializedProperty.arraySize, currentSelectedStage);
            currentSelectedStage = Mathf.Max(1, currentSelectedStage);
            list.index = currentSelectedStage - 1;
            //Cache Section
            CacheStageName(property);

            CacheStageElement(list);

            return list;
        }

        private void DrawStageElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (stageNameList.Count == 0 || index >= stageNameList.Count)
                return;

            string postfix = " ";
            switch (stageStatesList[index])
            {
                case PlayState.Idle:
                    break;
                case PlayState.Start:
                    postfix += "(Start)";
                    break;
                case PlayState.Executing:
                    postfix += "(Executing)";
                    break;
                case PlayState.Ending:
                    postfix += "(Ending)";
                    break;
                case PlayState.End:
                    postfix += "(Complete)";
                    break;
            }

            rect.y += 2;
            EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, rect.width - 10, EditorGUIUtility.singleLineHeight), stageNameList[index] + postfix);
            tempRect = new Rect(rect.xMax - 30, rect.y, 20, 20);

            if (GUI.Button(tempRect, EditorGUIUtility.IconContent("_Menu@2x"), buttonStyle))
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy Settings"), false, CopySettingStage, (object)index);
                if (HSVTutorialManager.stageCopyBuffer != null && HSVTutorialManager.stageCopyBuffer.StartsWith("{\"index\""))
                {
                    menu.AddItem(new GUIContent("Paste Settings"), false, PasteSettingStage, (object)index);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste Settings"));
                }
                menu.AddItem(new GUIContent("Duplicate Settings"), false, DuplicateSettingsStage, (object)index);
                menu.AddItem(new GUIContent("Clear Settings"), false, ClearSettingStage, (object)index);
                menu.ShowAsContext();
            }
        }

        private void CopySettingStage(object index)
        {
            var stageObj = m_TutorialManager.stageObjects[stageList.index];
            if (stageObj != null)
            {
                HSVTutorialManager.stageCopyBuffer = JsonUtility.ToJson(stageObj);
            }
        }

        private void PasteSettingStage(object index)
        {
            if (HSVTutorialManager.stageCopyBuffer != null && HSVTutorialManager.stageCopyBuffer.StartsWith("{\"index\""))
            {
                var oldStage = m_TutorialManager.stageObjects[stageList.index];
                var newStage = JsonUtility.FromJson<HSVTutorialStage>(HSVTutorialManager.stageCopyBuffer);
                if (newStage != null)
                {
                    Undo.RecordObject(m_TutorialManager, "Paste Stage Object");
                    newStage.index = oldStage.index;
                    newStage.stageName = oldStage.stageName;
                    m_TutorialManager.stageObjects[stageList.index] = new HSVTutorialStage(newStage);
                    serializedObject.UpdateIfRequiredOrScript();
                    UpdateStageIndex();
                    UpdatePageInfo();
                    CacheStageName(stageList.serializedProperty);
                    CacheStageElement(stageList);
                }
            }
        }

        private void DuplicateSettingsStage(object index)
        {
            var stageObj = m_TutorialManager.stageObjects[stageList.index];
            if (stageObj != null)
            {
                var newStage = JsonUtility.FromJson<HSVTutorialStage>(JsonUtility.ToJson(stageObj));
                if (newStage != null)
                {
                    Undo.RecordObject(m_TutorialManager, "Duplicate Stage Object");
                    newStage.stageName = stageObj.stageName + "(Clone)";
                    m_TutorialManager.InsertStageElement(stageList.index + 1, new HSVTutorialStage(newStage));
                    serializedObject.UpdateIfRequiredOrScript();
                    UpdateStageIndex();
                    UpdatePageInfo();
                    CacheStageName(stageList.serializedProperty);
                    CacheStageElement(stageList);
                }
            }
        }

        private void ClearSettingStage(object index)
        {
            var stageObj = new HSVTutorialStage();
            var oldStageObj = m_TutorialManager.stageObjects[(int)index];
            stageObj.index = oldStageObj.index;
            stageObj.stageName = oldStageObj.stageName;
            Undo.RecordObject(m_TutorialManager, "Clear Stage Setting");
            m_TutorialManager.stageObjects[(int)index] = stageObj;
            serializedObject.UpdateIfRequiredOrScript();
            UpdateStageIndex();
            UpdatePageInfo();
            CacheStageName(stageList.serializedProperty);
            CacheStageElement(stageList);
        }

        private void OnAddStageElement(ReorderableList list)
        {
            Undo.RecordObject(m_TutorialManager, "Add Stage Element");
            var insertIndex = list.serializedProperty.arraySize;
            var stageObjects = serializedObject.FindProperty("stageObjects");
            if (stageObjects != null)
            {
                var newObj = Activator.CreateInstance(typeof(HSVTutorialStage));
                var stageObj = newObj as HSVTutorialStage;
                if (stageObj != null)
                {
                    var postfix = "New Stage";

                    do
                    {
                        postfix += "_" + (insertIndex + 1);
                        stageObj.stageName = postfix;
                    }
                    while (!m_TutorialManager.CheckStageObjectName(postfix));

                    m_TutorialManager.InsertStageElement(insertIndex, stageObj);
                }
            }

            list.serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            UpdateStageIndex();
            currentSelectedStage = insertIndex + 1;
            list.index = insertIndex;
        }

        private void OnSelectStage(ReorderableList list)
        {
            currentSelectedStage = list.index + 1;
            var tObj = list.serializedProperty.FindPropertyRelative("tutorialObjects");
            if (tObj != null)
            {
                currentTutorialStep = Mathf.Min(tObj.arraySize, currentTutorialStep);
                UpdateCurrentPage(currentTutorialStep);
            }
            stageElementChange = true;
            CacheStageElement(list);
        }

        private bool OnCanRemoveStage(ReorderableList list)
        {
            return list.count > 0;
        }

        private void OnRemoveStage(ReorderableList list)
        {
            Undo.RecordObject(m_TutorialManager, "Remove Stage Element");
            var stageObjs = serializedObject.FindProperty("stageObjects");
            if (stageObjs != null)
            {
                var index = list.index;
                stageObjs.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                list.index = Math.Max(0, list.index - 1);
                currentSelectedStage = list.index + 1;
                UpdateStageIndex();
                updateTObject = true;
            }
        }

        private void OnReorderStageElementCallback(ReorderableList list, int oldIndex, int newIndex)
        {
            Undo.RecordObject(m_TutorialManager, "Reorder Stage Element");
            UpdateStageIndex();
            serializedObject.UpdateIfRequiredOrScript();
            CacheStageName(list.serializedProperty);
            CacheStageElement(list);
        }

        private void OnChangeCallBack(ReorderableList list)
        {
            CacheStageName(list.serializedProperty);
            CacheStageElement(list);
        }

        private void CacheStageName(SerializedProperty property)
        {
            stageNameList.Clear();
            stageStatesList.Clear();
            for (int i = 0; i < property.arraySize; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                stageNameList.Add(element.FindPropertyRelative("stageName").stringValue);
                stageStatesList.Add((PlayState)element.FindPropertyRelative("state").enumValueIndex);
            }
        }

        private void CacheStageElement(ReorderableList list)
        {
            if (list.index == -1 || list.index > list.serializedProperty.arraySize || list.serializedProperty.arraySize == 0)
                return;

            var stageElement = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if (stageElement != null)
            {
                stageName = stageElement.FindPropertyRelative("stageName");
                currentStageName = stageName.stringValue;
                stageIndex = stageElement.FindPropertyRelative("index");
                startObjectIndex = stageElement.FindPropertyRelative("startObjectIndex");
                startTrigger = stageElement.FindPropertyRelative("startTrigger");
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

                endTrigger = stageElement.FindPropertyRelative("endTrigger");
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

                advanceConfig = stageElement.FindPropertyRelative("advanceConfig");
                if (advanceConfig != null)
                {
                    advanceType = advanceConfig.FindPropertyRelative("advanceType");
                    advanceTypeValue = advanceType.enumValueIndex;
                    advanceIndex = advanceConfig.FindPropertyRelative("index");
                    advanceName = advanceConfig.FindPropertyRelative("name");
                }

                events = stageElement.FindPropertyRelative("events");
                if(events == null || events.arraySize != Enum.GetValues(typeof(HSVEventType)).Length)
                {
                    if(stageIndex.intValue <= m_TutorialManager.stageObjects.Length)
                    {
                        HSVEvent.CreateEvents(ref m_TutorialManager.stageObjects[stageIndex.intValue - 1].events);
                        serializedObject.UpdateIfRequiredOrScript();
                        events = stageElement.FindPropertyRelative("events");
                    }
                }

                if(events != null)
                {
                    stageEvents.Clear();
                    eventsTypes.Clear();
                    eventEnable.Clear();
                    subEvents.Clear();
                    for(int i = 0; i < events.arraySize; i++)
                    {
                        var element = events.GetArrayElementAtIndex(i);
                        stageEvents.Add(element);
                        eventsTypes.Add((HSVEventType)element.FindPropertyRelative("type").enumValueIndex);
                        eventEnable.Add(element.FindPropertyRelative("enable").boolValue);
                        subEvents.Add(element.FindPropertyRelative("subEvent"));
                    }
                }

                tmEditorListSize = tmEditorList.arraySize;
                //cache tutorial object
                currentStep = stageElement.FindPropertyRelative("currentStep");
                if (currentStep.intValue < 1)
                {
                    currentStep.intValue = 1;
                }
                allowMultiple = stageElement.FindPropertyRelative("allowMultiple");
                tutorialObjects = stageElement.FindPropertyRelative("tutorialObjects");
                tutorialObjectsSize = tutorialObjects.arraySize;

                if (tutorialObjectsSize > 0)
                {
                    currentTObj = tutorialObjects.GetArrayElementAtIndex(currentStep.intValue - 1);
                }
                currentStageObj = stageElement;
            }
            HSVTutorialManager.tmModified = true;
        }

        private void DrawStageElementWithDetail(SerializedProperty property)
        {
            guiColor = GUI.color;
            GUI.color = Color.yellow;
            EditorGUI.indentLevel++;
            foldStageElement = EditorGUILayout.Foldout(foldStageElement, new GUIContent(currentStageName), titleFoldoutBold);
            EditorGUI.indentLevel--;
            GUI.color = guiColor;

            if (foldStageElement)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Play Tutorial"))
                {
                    m_TutorialManager.PlayTutorial(currentTutorialStep);
                }

                if (GUILayout.Button("Stop Tutorial"))
                {
                    m_TutorialManager.StopTutorial(currentTutorialStep);
                }

                if (GUILayout.Button("Advance Tutorial"))
                {
                    m_TutorialManager.AdvanceTutorial();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(10);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Stage Index:    " + stageIndex.intValue.ToString(), GUILayout.ExpandWidth(true));

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(stageName, new GUIContent("Name:"), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    if (!HSVTutorialManager.Instance.CheckStageObjectName(stageName.stringValue))
                    {
                        stageName.stringValue = currentStageName;
                    }
                    else
                    {
                        currentStageName = stageName.stringValue;
                        CacheStageName(stageList.serializedProperty);
                    }
                }

                EditorGUILayout.PropertyField(startObjectIndex, new GUIContent("Start Object Index", "When stage starts, it will start the first object index, index -1 would start from the first element of the array"));
                EditorGUILayout.PropertyField(allowMultiple, new GUIContent("Allow Multiple", "Should the stage allow multiple tutorial objects running. If set, it may causes some unwanted effect if advance type set to anything else than None"));
                EditorGUILayout.Space(5);
                #region Trigger
                DrawStartTriggerSection();
                DrawEndTriggerSection();
                DrawAdvanceConfigSection();
                #endregion

                DrawEvents();
                EditorGUILayout.Space(10);

                EditorGUI.indentLevel--;

                if (tmEditorList != null)
                {
                    if (tutorialObjects != null)
                    {
                        currentTutorialStep = Mathf.Min(tutorialObjects.arraySize, currentTutorialStep);
                        UpdateCurrentPage(currentTutorialStep);
                    }

                    if (currentStep != null && tutorialObjects != null && currentStep.intValue <= tutorialObjectsSize && currentTutorialStep <= tutorialObjectsSize && tutorialObjectsSize > 0)
                    {
                        currentTutorialStep = Mathf.Max(1, currentTutorialStep);
                        currentTutorialStep = Mathf.Min(currentTutorialStep, tutorialObjectsSize);
                        EditorGUILayout.LabelField(new GUIContent("Selected Tutorial Step: " + currentTutorialStep + " / " + tutorialObjectsSize));
                        EditorGUILayout.LabelField(new GUIContent("Current Tutorial Step: " + currentStep.intValue));
                        if (currentStep.intValue < 1)
                        {
                            currentStep.intValue = 1;
                        }

                        if (Application.isPlaying)
                        {
                            var tObj = tutorialObjects.GetArrayElementAtIndex(currentStep.intValue - 1);
                            if (tObj != null)
                            {
                                EditorGUILayout.LabelField("Tutorial Object: " + tObj.FindPropertyRelative("Name").stringValue);
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Play State:");
                                var state = tObj.FindPropertyRelative("state");
                                switch ((PlayState)state.enumValueIndex)
                                {
                                    case PlayState.Idle:
                                        GUI.color = Color.gray;
                                        break;
                                    case PlayState.Start:
                                        GUI.color = Color.cyan;
                                        break;
                                    case PlayState.Executing:
                                        GUI.color = Color.green;
                                        break;
                                    case PlayState.Ending:
                                        GUI.color = new Color(1.0f, 0.5f, 0.1f, 1f);
                                        break;
                                    case PlayState.End:
                                        GUI.color = Color.red;
                                        break;
                                }

                                EditorGUILayout.LabelField(((PlayState)state.enumValueIndex).ToString());

                                GUI.color = guiColor;
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("");
                        }
                    }

                    if (tutorialObjectsList == null)
                    {
                        tutorialObjectsList = BuildTutorialObjectList(tmEditorList);
                    }

                    if (stagePropertyPath != property.propertyPath || HSVTutorialManager.undoRedoPerformed || stageElementChange)
                    {
                        stagePropertyPath = property.propertyPath;
                        UpdatePageInfo();
                        if (stageElementChange)
                            tutorialObjectsList.index = currentSelectedStep - 1;

                        CacheTObjectProperty(tmEditorList);
                    }

                    if (updateTObject && Event.current.type == EventType.Repaint)
                    {
                        updateTObject = false;
                        UpdatePageInfo();
                        UpdateCurrentStepIndex(tutorialObjectsList.index);
                        CacheTObjectProperty(tmEditorList);
                    }

                    if (HSVTutorialManager.stateChange)
                    {
                        CacheTObjName(tutorialObjectsList.serializedProperty);
                    }

                    EditorGUILayout.BeginVertical(boxStyle);
                    guiColor = GUI.color;
                    GUI.color = Color.green;
                    EditorGUI.indentLevel++;
                    foldTutorialObj = EditorGUILayout.Foldout(foldTutorialObj, new GUIContent("Tutorial Objects"), titleFoldoutBold);
                    EditorGUI.indentLevel--;
                    GUI.color = guiColor;

                    if (foldTutorialObj)
                    {
                        EditorGUILayout.Space(10);
                        tutorialObjectsList.DoLayoutList();
                        EditorGUILayout.Space(10);
                        if (tmEditorListSize > 0 && currentSelectedStep <= tmEditorListSize && currentSelectTObj != null)
                        {
                            EditorGUILayout.PropertyField(currentSelectTObj);

                            if (HSVTutorialManager.tObjNameChange)
                            {
                                CacheTObjName(tutorialObjectsList.serializedProperty);
                                HSVTutorialManager.tObjNameChange = false;
                            }

                            if (!Application.isPlaying && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Q)
                            {
                                var element = tutorialObjectsList.serializedProperty.GetArrayElementAtIndex(currentSelectedStep - 1);
                                m_TutorialManager.PreviewTutorialObject(element.FindPropertyRelative("stageIndex").intValue, element.FindPropertyRelative("index").intValue, preview);
                                EditorUtility.SetDirty(m_TutorialManager.gameObject);
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    if (stageElementChange)
                        stageElementChange = false;
                }
            }
        }
        #endregion

        #region Tutorial Object Drawing
        private ReorderableList BuildTutorialObjectList(SerializedProperty property)
        {
            var list = new ReorderableList(property.serializedObject, property, true, true, true, true);
            list.drawHeaderCallback = DrawListHeaderCallback;

            // Set the color of the selected list item
            list.drawElementBackgroundCallback = (rect, index, active, focused) =>
            {
                if (active)
                {
                    tObjBgTex.SetPixel(0, 0, new Color(0.1f, 0.75f, 0.6f, 0.33f));
                    tObjBgTex.Apply();
                    GUI.DrawTexture(rect, tObjBgTex as Texture);
                }
            };
            list.drawElementCallback = DrawListElementCallback;
            list.onAddCallback = OnAddElementCallback;
            list.onReorderCallbackWithDetails = OnReorderCallBackWithDetail;
            list.onSelectCallback = OnListSelect;
            list.onCanRemoveCallback = OnCanRemoveList;
            list.onRemoveCallback = OnRemoveElement;
            list.onChangedCallback = OnListChange;
            currentSelectedStep = Mathf.Max(currentSelectedStep, 1);
            currentSelectedStep = Math.Min(list.serializedProperty.arraySize, currentSelectedStep);
            list.index = currentSelectedStep - 1;
            CacheTObjectProperty(property);

            return list;

        }

        private void CacheTObjectProperty(SerializedProperty property)
        {
            CacheTObjName(property);
            tmEditorListSize = tmEditorList.arraySize;
            if (property.arraySize > 0)
            {
                currentSelectTObj = property.GetArrayElementAtIndex(Math.Min(property.arraySize, currentSelectedStep) - 1);
            }
            HSVTutorialManager.tmModified = true;
            serializedObject.UpdateIfRequiredOrScript();
        }

        private void CacheTObjName(SerializedProperty property)
        {
            tObjNameList.Clear();
            tObjStatesList.Clear();
            for (int i = 0; i < property.arraySize; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                tObjNameList.Add(element.FindPropertyRelative("Name").stringValue);
                tObjStatesList.Add((PlayState)element.FindPropertyRelative("state").enumValueIndex);
            }
        }

        private void DrawListHeaderCallback(Rect rect)
        {
            var defaultSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            var buttonRect = new Rect(rect.xMax - 20, rect.y - 1, 20f, 20f);
            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("Animation.NextKey")))
            {
                //Move to next page of the array
                NextPage();
            }
            buttonRect.x -= 25;
            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("Animation.PrevKey")))
            {
                //Move to the previous page of the array
                PreviousPage();
            }
            EditorGUIUtility.SetIconSize(defaultSize);

            buttonRect.x -= 110;
            buttonRect.width = 40;
            EditorGUI.LabelField(buttonRect, "Page: ");
            buttonRect.x += 45;

            EditorGUI.BeginChangeCheck();
            buttonRect.width = 25;
            currentPage = EditorGUI.IntField(buttonRect, currentPage);
            if (EditorGUI.EndChangeCheck())
            {
                currentPage = Mathf.Max(1, currentPage);
                if (currentPage > totalPageCount)
                {
                    currentPage = totalPageCount;
                }
                UpdateCurrentStepIndex(tutorialObjectsList.index);
                UpdatePageInfo();
                if (tutorialObjectsList != null)
                {
                    tutorialObjectsList.index = currentSelectedStep - 1;
                }
                CacheTObjectProperty(tutorialObjectsList.serializedProperty);
            }

            buttonRect.x += 25;
            buttonRect.width += 40;
            EditorGUI.LabelField(buttonRect, " / " + totalPageCount);

            buttonRect.x = rect.x;
            buttonRect.width = 70;
            EditorGUI.LabelField(buttonRect, "List Count: ");
            buttonRect.x += 70;
            buttonRect.width = 30;
            EditorGUI.BeginChangeCheck();
            currentPageCount = EditorGUI.IntField(buttonRect, currentPageCount);
            if (EditorGUI.EndChangeCheck())
            {
                currentPageCount = Mathf.Max(1, Mathf.Min(currentPageCount, 30));
                UpdatePageInfo();
                if (tutorialObjectsList != null)
                {
                    tutorialObjectsList.index = currentSelectedStep - 1;
                }
                CacheTObjectProperty(tutorialObjectsList.serializedProperty);
            }
        }

        private void DrawListElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (tObjNameList.Count == 0 || index >= tObjNameList.Count)
                return;

            string postfix = " ";
            switch (tObjStatesList[index])
            {
                case PlayState.Idle:
                    break;
                case PlayState.Start:
                    postfix += "(Start)";
                    break;
                case PlayState.Executing:
                    postfix += "(Executing)";
                    break;
                case PlayState.Ending:
                    postfix += "(Ending)";
                    break;
                case PlayState.End:
                    postfix += "(Complete)";
                    break;
            }

            rect.y += 2;
            tempRect = new Rect(rect.x + 5, rect.y, Mathf.Max(30, rect.width - 20), EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(tempRect, tObjNameList[index] + postfix);
            tempRect = new Rect(rect.xMax - 30, rect.y, 20, 20);

            if (GUI.Button(tempRect, EditorGUIUtility.IconContent("_Menu@2x", "Operation Options"), buttonStyle))
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Copy Tutorial Object"), false, CopySettingTObject, (object)index);
                if (HSVTutorialManager.tObjCopyBuffer != null && HSVTutorialManager.tObjCopyBuffer.StartsWith("{\"index\""))
                {
                    menu.AddItem(new GUIContent("Paste Tutorial Object"), false, PasteSettingTObject, (object)index);
                    menu.AddItem(new GUIContent("Paste Tutorial Object As New"), false, PasteSettingTObjectAsNew, (object)index);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste Tutorial Object"));
                    menu.AddDisabledItem(new GUIContent("Paste Tutorial Object As New"));
                }
                menu.AddItem(new GUIContent("Duplicate Tutorial Object"), false, DuplicateSettingTObject, (object)index);
                menu.AddItem(new GUIContent("Clear Tutorial Object"), false, ClearSettingTObject, (object)index);
                menu.ShowAsContext();
            }

            tempRect.x = rect.xMax - 70;
            if (GUI.Button(tempRect, EditorGUIUtility.IconContent("animationvisibilitytoggleon@2x", "Preview Tutorial Object UI Effect"), buttonStyle))
            {
                var element = tutorialObjectsList.serializedProperty.GetArrayElementAtIndex(index);
                m_TutorialManager.PreviewTutorialObject(element.FindPropertyRelative("stageIndex").intValue, element.FindPropertyRelative("index").intValue, preview);
                EditorUtility.SetDirty(m_TutorialManager.gameObject);
            }
        }

        private void CopySettingTObject(object index)
        {
            var stageObj = m_TutorialManager.stageObjects[stageList.index];
            if (stageObj != null && stageObj.tutorialObjects != null)
            {
                var tIndex = (currentPage - 1) * currentPageCount + (int)index;
                var tObj = stageObj.tutorialObjects[tIndex];
                if (tObj != null)
                {
                    HSVTutorialManager.tObjCopyBuffer = JsonUtility.ToJson(tObj);
                }
            }
        }

        private void PasteSettingTObject(object index)
        {
            if (HSVTutorialManager.tObjCopyBuffer != null && HSVTutorialManager.tObjCopyBuffer.StartsWith("{\"index\""))
            {
                var stageObj = m_TutorialManager.stageObjects[stageList.index];
                if (stageObj != null && stageObj.tutorialObjects != null)
                {
                    var tIndex = (currentPage - 1) * currentPageCount + (int)index;
                    var oldObj = stageObj.tutorialObjects[tIndex];
                    var tObj = JsonUtility.FromJson<HSVTutorialObject>(HSVTutorialManager.tObjCopyBuffer);
                    if (tObj != null)
                    {
                        Undo.RecordObject(m_TutorialManager, "Paste Tutorial Object");
                        tObj.index = oldObj.index;
                        tObj.Name = oldObj.Name;
                        tObj.stageIndex = oldObj.stageIndex;
                        stageObj.tutorialObjects[tIndex] = new HSVTutorialObject(tObj);
                        serializedObject.UpdateIfRequiredOrScript();
                        UpdateElementIndex();
                        UpdatePageInfo();
                        CacheTObjectProperty(tmEditorList);
                    }
                }
            }
        }

        private void PasteSettingTObjectAsNew(object index)
        {
            if (HSVTutorialManager.tObjCopyBuffer != null && HSVTutorialManager.tObjCopyBuffer.StartsWith("{\"index\""))
            {
                var stageObj = m_TutorialManager.stageObjects[stageList.index];
                if (stageObj != null && stageObj.tutorialObjects != null)
                {
                    var insertIndex = (int)index;
                    insertIndex = insertIndex == -1 ? stageObj.tutorialObjects.Length : (insertIndex + 1 >= stageObj.tutorialObjects.Length ? stageObj.tutorialObjects.Length : insertIndex + 1);
                    insertIndex += (currentPage - 1) * currentPageCount;
                    var tObj = JsonUtility.FromJson<HSVTutorialObject>(HSVTutorialManager.tObjCopyBuffer);
                    if (tObj != null)
                    {
                        var postfix = tObj.Name;
                        tObj.stageIndex = stageList.index + 1;

                        do
                        {
                            postfix += "_(Clone)";
                            tObj.Name = postfix;
                        }
                        while (!m_TutorialManager.CheckTutorialObjectName(m_TutorialManager.stageObjects[stageList.index], postfix));
                        Undo.RecordObject(m_TutorialManager, "Paste Tutorial Object As New");
                        m_TutorialManager.InsertElementForTMObject(insertIndex, new HSVTutorialObject(tObj), stageList.index);
                        serializedObject.UpdateIfRequiredOrScript();
                        currentTutorialStep = insertIndex + 1;
                        UpdateElementIndex();
                        UpdatePageInfo();
                        CacheTObjectProperty(tmEditorList);
                    }
                }
            }
        }

        private void DuplicateSettingTObject(object index)
        {
            var stageObj = m_TutorialManager.stageObjects[stageList.index];
            if (stageObj != null && stageObj.tutorialObjects != null)
            {
                var tIndex = (currentPage - 1) * currentPageCount + (int)index;
                var tObj = stageObj.tutorialObjects[tIndex];
                if (tObj != null)
                {
                    var newTObj = new HSVTutorialObject(tObj);
                    if (newTObj != null)
                    {
                        Undo.RecordObject(m_TutorialManager, "Duplicate Tutorial Object");
                        newTObj.Name = tObj.Name + "(Clone)";
                        m_TutorialManager.InsertElementForTMObject(tIndex + 1, newTObj, stageList.index);
                        serializedObject.UpdateIfRequiredOrScript();
                        UpdateElementIndex();
                        UpdatePageInfo();
                        CacheTObjectProperty(tmEditorList);
                    }
                }
            }
        }

        private void ClearSettingTObject(object index)
        {
            var stageObj = m_TutorialManager.stageObjects[stageList.index];
            if (stageObj != null && stageObj.tutorialObjects != null)
            {
                var tIndex = (currentPage - 1) * currentPageCount + (int)index;
                var oldObj = stageObj.tutorialObjects[tIndex];
                var tObj = new HSVTutorialObject();
                if (tObj != null)
                {
                    Undo.RecordObject(m_TutorialManager, "Clear Tutorial Object");
                    tObj.index = oldObj.index;
                    tObj.Name = oldObj.Name;
                    tObj.stageIndex = oldObj.stageIndex;
                    stageObj.tutorialObjects[tIndex] = tObj;
                    serializedObject.UpdateIfRequiredOrScript();
                    UpdateElementIndex();
                    UpdatePageInfo();
                    CacheTObjectProperty(tmEditorList);
                }
            }
        }

        private void OnAddElementCallback(ReorderableList list)
        {
            Undo.RecordObject(m_TutorialManager, "Add Tutorial Object");
            var insertIndex = list.index == -1 ? list.serializedProperty.arraySize : (list.index + 1 >= list.serializedProperty.arraySize ? list.serializedProperty.arraySize : list.index + 1);

            insertIndex += (currentPage - 1) * currentPageCount;
            if (m_TutorialManager.stageObjects[stageList.index].tutorialObjects != null)
            {
                var newObj = Activator.CreateInstance(typeof(HSVTutorialObject));
                var tObj = newObj as HSVTutorialObject;
                if (tObj != null)
                {
                    var postfix = "New Tutorial Objects";
                    tObj.stageIndex = stageList.index + 1;

                    do
                    {
                        postfix += "_" + (insertIndex + 1);
                        tObj.Name = postfix;
                    }
                    while (!m_TutorialManager.CheckTutorialObjectName(m_TutorialManager.stageObjects[stageList.index], postfix));

                    m_TutorialManager.InsertElementForTMObject(insertIndex, tObj, stageList.index);
                }
            }
            list.serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            currentTutorialStep = insertIndex + 1;
            UpdateElementIndex();
            UpdatePageInfo();
            list.index = currentSelectedStep;
        }

        private void OnReorderCallBackWithDetail(ReorderableList list, int oldIndex, int newIndex)
        {
            if (stageList != null && stageList.index != -1)
            {
                list.serializedProperty.ClearArray();
                var objOldIndex = oldIndex + (currentPage - 1) * currentPageCount;
                var objNewIndex = newIndex + (currentPage - 1) * currentPageCount;
                var tutorialObjects = stageList.serializedProperty.GetArrayElementAtIndex(stageList.index).FindPropertyRelative("tutorialObjects");
                tutorialObjects.MoveArrayElement(objOldIndex, objNewIndex);
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
                currentTutorialStep = objNewIndex + 1;
                UpdateElementIndex();
                UpdatePageInfo();
                UpdateCurrentStepIndex(tutorialObjectsList.index);
                CacheTObjectProperty(tmEditorList);
            }
        }

        private void OnListSelect(ReorderableList list)
        {
            UpdateCurrentStepIndex(list.index);
            CacheTObjectProperty(list.serializedProperty);
        }

        private bool OnCanRemoveList(ReorderableList list)
        {
            return list.count > 0;
        }

        private void OnRemoveElement(ReorderableList list)
        {
            list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            var stageElement = stageList.serializedProperty.GetArrayElementAtIndex(stageList.index);
            if (stageElement != null)
            {
                var tutorialObject = stageElement.FindPropertyRelative("tutorialObjects");
                if (tutorialObject != null)
                {
                    Undo.RecordObject(m_TutorialManager, "Remove Tutorial Object");
                    var index = list.index + (currentPage - 1) * currentPageCount;
                    tutorialObject.DeleteArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                    list.index = Math.Max(0, list.index - 1);
                    currentTutorialStep = index + 1;
                    UpdateElementIndex();
                    UpdatePageInfo();
                    updateTObject = true;
                }
            }
        }

        private void OnListChange(ReorderableList list)
        {
            CacheTObjectProperty(list.serializedProperty);
        }

        private void UpdatePageInfo()
        {
            if (stageList != null && stageList.index != -1 && stageList.index < m_TutorialManager.stageObjects.Length)
            {
                totalPageCount = Mathf.Max(Mathf.CeilToInt(m_TutorialManager.stageObjects[stageList.index].tutorialObjects.Length / (float)currentPageCount), 1);
                currentTutorialStep = Mathf.Min(currentTutorialStep, m_TutorialManager.stageObjects[stageList.index].tutorialObjects.Length);
                currentTutorialStep = Mathf.Max(currentTutorialStep, 1);
                UpdateCurrentPage(currentTutorialStep);
                currentPage = Mathf.Min(totalPageCount, currentPage);
                m_TutorialManager.UpdateTMEditorList(currentPage, currentPageCount, stageList.index);
                serializedObject.UpdateIfRequiredOrScript();
                tutorialObjectsSize = tutorialObjects.arraySize;
            }
        }

        private void UpdateCurrentStepIndex(int index)
        {
            currentTutorialStep = (currentPage - 1) * currentPageCount + index + 1;
            currentSelectedStep = index + 1;
        }

        private void UpdateCurrentPage(int index)
        {
            currentPage = Mathf.Max(Mathf.CeilToInt(index / (float)currentPageCount), 1);
            currentSelectedStep = index - (currentPage - 1) * currentPageCount;
            currentSelectedStep = Mathf.Max(currentSelectedStep, 1);
        }

        private void UpdateElementIndex()
        {
            if (stageList != null && stageList.index != -1)
            {
                var tutorialObjects = stageList.serializedProperty.GetArrayElementAtIndex(stageList.index).FindPropertyRelative("tutorialObjects");
                for (int i = 0; i < tutorialObjects.arraySize; i++)
                {
                    var tObj = tutorialObjects.GetArrayElementAtIndex(i);
                    tObj.FindPropertyRelative("index").intValue = i + 1;
                }
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void UpdateStageIndex()
        {
            if (stageList != null)
            {
                for (int i = 0; i < stageList.serializedProperty.arraySize; i++)
                {
                    var stage = stageList.serializedProperty.GetArrayElementAtIndex(i);
                    stage.FindPropertyRelative("index").intValue = i + 1;
                    var tObjs = stage.FindPropertyRelative("tutorialObjects");
                    if (tObjs != null)
                    {
                        for (int j = 0; j < tObjs.arraySize; j++)
                        {
                            tObjs.GetArrayElementAtIndex(j).FindPropertyRelative("stageIndex").intValue = i + 1;
                        }
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void NextPage()
        {
            currentTutorialStep += currentPageCount;
            UpdatePageInfo();
            if (tutorialObjectsList != null)
            {
                tutorialObjectsList.index = currentSelectedStep - 1;
            }

            CacheTObjectProperty(tutorialObjectsList.serializedProperty);
        }

        private void PreviousPage()
        {
            currentTutorialStep -= currentPageCount;
            UpdatePageInfo();
            if (tutorialObjectsList != null)
            {
                tutorialObjectsList.index = currentSelectedStep - 1;
            }

            CacheTObjectProperty(tutorialObjectsList.serializedProperty);
        }

        #region Trigger Drawing Section
        private void DrawStartTriggerSection()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(startTriggerType, new GUIContent("Start Trigger", "Manual: Manual Trigger could be called using script API or in stage setup\n" +
                "Collider: Collider/Trigger object used to trigger Stage Object\n" +
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
                        foldStartTrigger = EditorGUILayout.Foldout(foldStartTrigger, "Collider Config", titleFoldoutBold);
                        if (foldStartTrigger)
                        {
                            EditorGUILayout.PropertyField(startCollider, new GUIContent("Trigger Collider", "Collider object used to trigger Stage Object"));
                            EditorGUILayout.PropertyField(startTriggerOnClick, new GUIContent("Trigger On Click:", "Should the tutorial object be triggered when clicked on screen"));
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(startLayerFiltering, new GUIContent("Use Layer Filtering"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                startLayerFilteringValue = startLayerFiltering.boolValue;
                            }

                            if (startLayerFilteringValue)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(startFilterLayer, new GUIContent("Layer", "Filter layer for gameobject that collides trigger object"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(startTagFiltering, new GUIContent("Use Tag Filtering"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                startTagFilteringValue = startTagFiltering.boolValue;
                            }


                            if (startTagFilteringValue)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(startFilterTag, new GUIContent("Tag", "Filter Tag for gameobject that collides trigger object"));
                                EditorGUILayout.PropertyField(endUseRigidbodyTag, new GUIContent("Use Rigidbody Tag", "Should use rigidbody's tag instead of trigger gameobject"));
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
                        foldStartTrigger = EditorGUILayout.Foldout(foldStartTrigger, "Graphic Config", titleFoldoutBold);
                        if (foldStartTrigger)
                        {
                            EditorGUILayout.PropertyField(startGraphic, new GUIContent("Graphic", "Graphic used to detect UI click"));

                            EditorGUILayout.PropertyField(startPointerTrigger, new GUIContent("Pointer Trigger", "PointerDown: Event trigger on pointer down\nPointerUp: Event trigger on pointer up\nPointerClick: Event trigger on pointer click"));
                        }
                    }
                    break;
                case TriggerType.KeyCode:
                    EditorGUILayout.PropertyField(startKeyCode, new GUIContent("Key Code", "Key press to trigger start stage event"));
                    break;
            }
            EditorGUI.indentLevel--;
        }

        private void DrawEndTriggerSection()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(endTriggerType, new GUIContent("End Trigger", "Manual: Manual Trigger could be called using script API or in stage setup\n" +
                "Collider: Collider/Trigger object used to trigger Stage Object\n" +
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
                        foldEndTrigger = EditorGUILayout.Foldout(foldEndTrigger, "Collider Config", titleFoldoutBold);
                        if (foldEndTrigger)
                        {
                            EditorGUILayout.PropertyField(endCollider, new GUIContent("Trigger Collider", "Collider object used to trigger Stage Object"));
                            EditorGUILayout.PropertyField(endTriggerOnClick, new GUIContent("Trigger On Click:", "Should the tutorial object be triggered when clicked on screen"));
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(endLayerFiltering, new GUIContent("Use Layer Filtering"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                endLayerFilteringValue = endLayerFiltering.boolValue;
                            }

                            if (endLayerFilteringValue)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(endFilterLayer, new GUIContent("Layer", "Filter layer for gameobject that collides trigger object"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(endTagFiltering, new GUIContent("Use Tag Filtering"));
                            if (EditorGUI.EndChangeCheck())
                            {
                                endTagFilteringValue = endTagFiltering.boolValue;
                            }

                            if (endTagFilteringValue)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(endFilterTag, new GUIContent("Tag", "Filter Tag for gameobject that collides trigger object"));
                                EditorGUILayout.PropertyField(endUseRigidbodyTag, new GUIContent("Use Rigidbody Tag", "Should use rigidbody's tag instead of trigger gameobject"));
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
                        foldEndTrigger = EditorGUILayout.Foldout(foldEndTrigger, "Graphic Config", titleFoldoutBold);
                        if (foldEndTrigger)
                        {
                            EditorGUILayout.PropertyField(endGraphic, new GUIContent("Graphic", "Graphic used to detect UI click"));

                            EditorGUILayout.PropertyField(endPointerTrigger, new GUIContent("Pointer Trigger", "PointerDown: Event trigger on pointer down\nPointerUp: Event trigger on pointer up\nPointerClick: Event trigger on pointer click"));
                        }
                    }
                    break;
                case TriggerType.KeyCode:
                    EditorGUILayout.PropertyField(endKeyCode, new GUIContent("Key Code", "Key press to trigger stop stage event"));
                    break;
            }
            EditorGUI.indentLevel--;
        }

        private void DrawAdvanceConfigSection()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(advanceType, new GUIContent("Advance Type", "None: Do nothing after stopping\n" +
                "Automatic: it would increment index and play the Stage Object\n" +
                "Index: play the next stage object by index\n" +
                "Name: play the next stage object by name"));
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
                    EditorGUILayout.PropertyField(advanceIndex, new GUIContent("Index", "Next Stage Object Index"));
                    break;
                case AdvanceType.Name:
                    EditorGUILayout.PropertyField(advanceName, new GUIContent("Name", "Next Stage Object Name"));
                    break;
            }
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Events Section
        private void DrawEvents()
        {
            foldEvents = EditorGUILayout.Foldout(foldEvents, "Stage Events", titleFoldoutBold);
            if(foldEvents)
            {
                var tempColor = new Color(0.2f, 0.1f, 0.2f, 0.5f);
                GUI.backgroundColor = tempColor;
                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < stageEvents.Count; i++)
                {
                    GUI.backgroundColor = eventEnable[i] ? Color.green : tempColor;

                    if(GUILayout.Button(new GUIContent(eventsTypes[i].ToString(), GetToolTipForEvent(eventsTypes[i])), GUILayout.Width(50f), GUILayout.ExpandWidth(true)))
                    {
                        var enable = stageEvents[i].FindPropertyRelative("enable");
                        enable.boolValue = !enable.boolValue;
                        eventEnable[i] = enable.boolValue;
                    }

                    GUI.backgroundColor = tempColor;
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = defaultBackgroundColor;
                for(int i = 0; i < stageEvents.Count;i++)
                {
                    if(eventEnable[i])
                    {
                        EditorGUILayout.Space(10);
                        EditorGUILayout.PropertyField(subEvents[i], new GUIContent(eventsTypes[i].ToString()));
                    }
                }
                GUI.backgroundColor = tempColor;
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = defaultBackgroundColor;
            }
        }

        private string GetToolTipForEvent(HSVEventType eventType)
        {
            string eventToolTip = string.Empty;
            switch(eventType)
            {
                case HSVEventType.OnStart:
                    eventToolTip = "Event would be invoked once when stage starts.";
                    break;
                case HSVEventType.OnUpdate:
                    eventToolTip = "Event would be invoked on every frame when stage started. It will be invoked continuously until the end of the stage";
                    break;
                case HSVEventType.OnExecuting:
                    eventToolTip = "Event would be invoked once when entering executing state. stage would enter executing state when it has done intializing triggers";
                    break;
                case HSVEventType.OnEnding:
                    eventToolTip = "Event would be invoked once when trying to end stage";
                    break;
                case HSVEventType.OnComplete:
                    eventToolTip = "Event would be invoked once when stage is completed.";
                    break;
            }

            return eventToolTip;
        }
        #endregion

        #region Helper Function
        private void DrawUILine(Color color, ref Rect currentRect, int thickness = 2, int padding = 10)
        {
            currentRect.y += thickness + padding;
            currentRect.height = thickness;
            currentRect.y += padding / 2;
            currentRect.x += 2;
            currentRect.width -= 6;
            EditorGUI.DrawRect(currentRect, color);
        }
        #endregion
        #endregion

        #region Database Section
        private void CreateNewDatabase()
        {
            if (Application.isPlaying)
                return;

            try
            {
                if (!Directory.Exists("Assets/HSVStudio Tutorial Factory/Resources"))
                {
                    Directory.CreateDirectory("Assets/HSVStudio Tutorial Factory/Resources");
                }
            }
            catch (IOException ex)
            {
                Debug.LogError(ex.Message);
            }

            var newAsset = ScriptableObject.CreateInstance<HSVTutorialData>();
            string namer = "DefaultHSVTutorialData";
            if (AssetDatabase.LoadAssetAtPath("Assets/HSVStudio Tutorial Factory/Resources/" + namer + ".asset", typeof(ScriptableObject)) == null)
            {
                AssetDatabase.CreateAsset(newAsset, "Assets/HSVStudio Tutorial Factory/Resources/" + namer + ".asset");
                EditorUtility.SetDirty(newAsset);
                AssetDatabase.SaveAssets();
                m_TutorialManager.database = newAsset;
            }
            else
            {
                int j = 0;
                string disNumberedName = namer;
                int toCut = 0;
                int endPos = 0;
                if (disNumberedName[disNumberedName.Length - 1] == ')')
                {
                    while (disNumberedName[disNumberedName.Length - 1 - toCut] != '(')
                    {
                        toCut++;
                        if (toCut <= 0)
                        {
                            toCut = -1;
                            break;
                        }
                    }
                    toCut++;
                    if (toCut >= 0)
                    {
                        endPos = disNumberedName.Length - 1 - toCut;
                        if (disNumberedName[endPos] == ' ')
                        {
                        }
                    }
                    else
                    {
                        endPos = disNumberedName.Length - 1;
                    }
                    disNumberedName = disNumberedName.Substring(0, endPos);
                }
                j = 0;
                while (AssetDatabase.LoadAssetAtPath("Assets/HSVStudio Tutorial Factory/Resources/" + disNumberedName + " (" + j.ToString() + ")" + ".asset", typeof(ScriptableObject)) != null)
                {
                    j++;
                }
                AssetDatabase.CreateAsset(newAsset, "Assets/HSVStudio Tutorial Factory/Resources/" + disNumberedName + " (" + j.ToString() + ")" + ".asset");
                EditorUtility.SetDirty(newAsset);
                AssetDatabase.SaveAssets();
                m_TutorialManager.database = newAsset;
            }

            AssetDatabase.Refresh();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        #region Saving Scene Reference 
        private void SaveDatabase()
        {
            if (m_TutorialManager.database != null)
            {
                var database = m_TutorialManager.database;
                database.assignOnRuntime = m_TutorialManager.assignOnRuntime;
                SetExposeReference<Camera>(ref database.targetCamera, m_TutorialManager.Camera);
                SetExposeReference<Canvas>(ref database.targetCanvas, m_TutorialManager.Canvas);
                database.autoStart = m_TutorialManager.autoStart;
                database.autoStartIndex = m_TutorialManager.autoStartIndex;
                database.debugMode = m_TutorialManager.debugMode;
                database.currentStageStep = m_TutorialManager.currentStageStep;

                database.stageObjects.Clear();
                for (int i = 0; i < m_TutorialManager.stageObjects.Length; i++)
                {
                    database.stageObjects.Add(GetSettings(m_TutorialManager.stageObjects[i]));
                }

                //Reset State
                for (int i = 0; i < database.stageObjects.Count; i++)
                {
                    database.stageObjects[i].state = PlayState.Idle;
                    for (int j = 0; j < database.stageObjects[i].tutorialObjects.Length; j++)
                    {
                        database.stageObjects[i].tutorialObjects[j].state = PlayState.Idle;
                    }
                }

                SaveTargetInfo(database);
                SaveTriggerInfo(database);
                SaveEventInfo(database);

                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        private void SaveTargetInfo(HSVTutorialData database)
        {
            if (database.targetInfo != null)
            {
                foreach (var target in database.targetInfo)
                {
                    ClearExposeReference(target.target);
                }
            }
            else
            {
                database.targetInfo = new List<HSVTargetSaveInfo>();
            }

            database.targetInfo.Clear();

            for (int i = 0; i < m_TutorialManager.stageObjects.Length; i++)
            {
                for (int j = 0; j < m_TutorialManager.stageObjects[i].tutorialObjects.Length; j++)
                {
                    for (int t = 0; t < m_TutorialManager.stageObjects[i].tutorialObjects[j].focusTargets.Length; t++)
                    {
                        var target = m_TutorialManager.stageObjects[i].tutorialObjects[j].focusTargets[t];
                        if (target.target != null && target.target.scene.name != null)
                        {
                            var info = new HSVLoopUpInfo()
                            {
                                stageIndex = i,
                                tObjIndex = j,
                                targetIndex = t
                            };

                            var exposeReference = new ExposedReference<GameObject>();
                            SetExposeReference<GameObject>(ref exposeReference, m_TutorialManager.stageObjects[i].tutorialObjects[j].focusTargets[t].target);
                            database.targetInfo.Add(new HSVTargetSaveInfo() { info = info, target = exposeReference });
                        }
                    }
                }
            }
        }

        private void SaveTriggerInfo(HSVTutorialData database)
        {
            if (database.triggerInfo != null)
            {
                foreach (var trigger in database.triggerInfo)
                {
                    ClearExposeReference(trigger.collider);
                    ClearExposeReference(trigger.graphic);
                }
            }
            else
            {
                database.triggerInfo = new List<HSVTriggerSaveInfo>();
            }

            database.triggerInfo.Clear();

            for (int i = 0; i < m_TutorialManager.stageObjects.Length; i++)
            {
                SetTriggerInfo(database, m_TutorialManager.stageObjects[i].startTrigger, true, true, false, i, -1, -1);
                SetTriggerInfo(database, m_TutorialManager.stageObjects[i].endTrigger, true, false, false, i, -1, -1);
                for (int j = 0; j < m_TutorialManager.stageObjects[i].tutorialObjects.Length; j++)
                {
                    SetTriggerInfo(database, m_TutorialManager.stageObjects[i].tutorialObjects[j].startTrigger, false, true, false, i, j, -1);
                    SetTriggerInfo(database, m_TutorialManager.stageObjects[i].tutorialObjects[j].endTrigger, false, false, false, i, j, -1);

                    for (int t = 0; t < m_TutorialManager.stageObjects[i].tutorialObjects[j].focusTargets.Length; t++)
                    {
                        SetTriggerInfo(database, m_TutorialManager.stageObjects[i].tutorialObjects[j].focusTargets[t].triggerConfig, false, false, true, i, j, t);
                    }
                }
            }
        }

        private void SetTriggerInfo(HSVTutorialData database, HSVTriggerConfig config, bool isStage, bool isStart, bool isTarget, int stageIndex, int tObjIndex, int targetIndex)
        {
            if ((config.colliderConfig.collider != null && config.colliderConfig.collider.gameObject.scene.name != null) || (config.graphicConfig.graphic != null && config.graphicConfig.graphic.gameObject.scene.name != null))
            {
                var info = new HSVLoopUpInfo()
                {
                    stageIndex = stageIndex,
                    tObjIndex = tObjIndex,
                    targetIndex = targetIndex
                };

                var triggerInfo = new HSVTriggerSaveInfo()
                {
                    isStage = isStage,
                    isStart = isStart,
                    isTarget = isTarget,
                    info = info
                };

                if (config.colliderConfig.collider != null && config.colliderConfig.collider.gameObject.scene.name != null)
                {
                    var colliderRef = new ExposedReference<Collider>();
                    SetExposeReference<Collider>(ref colliderRef, config.colliderConfig.collider);
                    triggerInfo.collider = colliderRef;
                }

                if (config.graphicConfig.graphic != null && config.graphicConfig.graphic.gameObject.scene.name != null)
                {
                    var graphicRef = new ExposedReference<Graphic>();
                    SetExposeReference<Graphic>(ref graphicRef, config.graphicConfig.graphic);
                    triggerInfo.graphic = graphicRef;
                }

                database.triggerInfo.Add(triggerInfo);
            }
        }

        private void SaveEventInfo(HSVTutorialData database)
        {

        }
        #endregion

        #region Loading Scene Reference
        private void LoadDatabase()
        {
            if (m_TutorialManager.database != null)
            {
                Undo.RecordObject(m_TutorialManager, "Load Config");
                var database = m_TutorialManager.database;
                LoadExposeReference<Camera>(database.targetCamera, ref m_TutorialManager.Camera);
                LoadExposeReference<Canvas>(database.targetCanvas, ref m_TutorialManager.Canvas);

                m_TutorialManager.autoStart = database.autoStart;
                m_TutorialManager.autoStartIndex = database.autoStartIndex;
                m_TutorialManager.debugMode = database.debugMode;
                m_TutorialManager.currentStageStep = database.currentStageStep;

                m_TutorialManager.stageObjects = new HSVTutorialStage[database.stageObjects.Count];
                for (int i = 0; i < database.stageObjects.Count; i++)
                {
                    m_TutorialManager.stageObjects[i] = GetSettings(database.stageObjects[i]);
                }

                LoadTargetInfo(database);
                LoadTriggerInfo(database);

                serializedObject.UpdateIfRequiredOrScript();
                updateTObject = true;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        private void LoadTargetInfo(HSVTutorialData database)
        {
            foreach (var target in database.targetInfo)
            {
                var info = target.info;
                if (info.stageIndex < m_TutorialManager.stageObjects.Length)
                {
                    var stageObj = m_TutorialManager.stageObjects[info.stageIndex];
                    if (stageObj != null && stageObj.tutorialObjects != null)
                    {
                        if (info.tObjIndex < m_TutorialManager.stageObjects[info.stageIndex].tutorialObjects.Length)
                        {
                            var tutorialObj = stageObj.tutorialObjects[info.tObjIndex];
                            if (tutorialObj != null && tutorialObj.focusTargets != null)
                            {
                                if (info.targetIndex < tutorialObj.focusTargets.Length)
                                {
                                    var targetObj = tutorialObj.focusTargets[info.targetIndex];
                                    if (targetObj != null)
                                    {
                                        LoadExposeReference<GameObject>(target.target, ref targetObj.target);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadTriggerInfo(HSVTutorialData database)
        {
            foreach(var triggerInfo in database.triggerInfo)
            {
                if (triggerInfo.info.stageIndex < m_TutorialManager.stageObjects.Length)
                {
                    var stageObj = m_TutorialManager.stageObjects[triggerInfo.info.stageIndex];
                    if (stageObj != null)
                    {
                        if (triggerInfo.isStage)
                        {
                            var config = triggerInfo.isStart ? stageObj.startTrigger : stageObj.endTrigger;
                            LoadToTrigger(triggerInfo, config);
                            continue;
                        }
                        else
                        {
                            if (triggerInfo.info.tObjIndex < stageObj.tutorialObjects.Length)
                            {
                                var tObj = stageObj.tutorialObjects[triggerInfo.info.tObjIndex];
                                if(tObj != null)
                                {
                                    if(triggerInfo.isTarget)
                                    {
                                        if(triggerInfo.info.targetIndex < tObj.focusTargets.Length)
                                        {
                                            LoadToTrigger(triggerInfo, tObj.focusTargets[triggerInfo.info.targetIndex].triggerConfig);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        var config = triggerInfo.isStart ? tObj.startTrigger : tObj.endTrigger;
                                        LoadToTrigger(triggerInfo, config);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadToTrigger(HSVTriggerSaveInfo triggerInfo, HSVTriggerConfig config)
        {
            if(config != null)
            {
                LoadExposeReference<Collider>(triggerInfo.collider, ref config.colliderConfig.collider);
                LoadExposeReference<Graphic>(triggerInfo.graphic, ref config.graphicConfig.graphic);
            }
        }
        #endregion

        private HSVTutorialStage GetSettings(HSVTutorialStage stage)
        {
            return new HSVTutorialStage(stage);
        }

        private void ClearExposeReference<T>(ExposedReference<T> exposedReference) where T : UnityEngine.Object
        {
            bool isValid = false;
            if (m_TutorialManager.ExposeMgr.GetReferenceValue(exposedReference.exposedName, out isValid) != null && isValid)
            {
                m_TutorialManager.ExposeMgr.ClearReferenceValue(exposedReference.exposedName);
            }
        }

        private void SetExposeReference<T>(ref ExposedReference<T> exposedReference, UnityEngine.Object obj) where T : UnityEngine.Object
        {
            ClearExposeReference(exposedReference);

            var exposeName = new PropertyName(GUID.Generate().ToString());
            m_TutorialManager.ExposeMgr.SetReferenceValue(exposeName, obj);
            exposedReference.exposedName = exposeName;
        }

        private void LoadExposeReference<T>(ExposedReference<T> exposedReference, ref T obj) where T : UnityEngine.Object
        {
            bool isValid = false;
            var newObject = m_TutorialManager.ExposeMgr.GetReferenceValue(exposedReference.exposedName, out isValid);
            if (isValid && newObject != null)
            {
                obj = (T)newObject;
            }
        }

        #endregion
    }
}