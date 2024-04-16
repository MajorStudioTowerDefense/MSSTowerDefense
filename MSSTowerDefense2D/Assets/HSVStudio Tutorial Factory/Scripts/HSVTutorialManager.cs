using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HSVStudio.Tutorial.Outline;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace HSVStudio.Tutorial
{
    [ExecuteInEditMode]
    [AddComponentMenu("HSVStudio/Managers/TutorialManager")]
    public class HSVTutorialManager : MonoBehaviour
    {
        #region singleton
        private static HSVTutorialManager instance;
        public static HSVTutorialManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<HSVTutorialManager>();
                }
                return instance;
            }
        }
        #endregion

        #region References
        //Initialization on Awake, otherwise, you need to call manual intialization
        public bool initializeOnAwake;
        //If true, camera and canvas will automatically look for reference during runtime if there are none for both field
        public bool assignOnRuntime;
        //If true, the stage would be automatically started in playmode
        public bool autoStart;
        //The automatic start stage index, value -1 indicate the first stage.
        public int autoStartIndex = -1;
        //The debug message will print to console if true
        public bool debugMode;
        //The main camera the tutorial manager UI is spawning to. Please use the Canvas camera for this field.
        //This is important for calculation. Normally, setting to Camera.main or the main camera in Scene would be enough
        public Camera Camera;
        //It is used by tutorial manager to spawn all the modules
        public Canvas Canvas;
        //Player Object
        public GameObject Player;
        //World Object Container
        public Transform WorldObjectContainer;

        private bool startFirstTutorial = false;
        private bool tmInitialized = false;
#if UNITY_EDITOR
        private bool enterFromEditor = false;
#endif
        #endregion

        #region Tutorial
        //This contains all the stage objects.
        public HSVTutorialStage[] stageObjects;
        //Current playing stage reference on runtime
        private HSVTutorialStage currentPlayingStage;
        //The current step of the stage in stageObjects array. It is using array index plus one notation. Please be careful with this!
        public int currentStageStep;
        //Current playing tutorial objects reference. If stage allows multiple running tutorial objects, all the playing tutorial object will be stored here
        //If stage ends, this would be cleared. However, in certain circumstances, multiple running tutorial obejcts may cause unwanted effect with certain settins
        public HashSet<HSVTutorialObject> currentPlayingTObjects;
        public HashSet<HSVTutorialObject> removalTObjs;
        #endregion

        #region Runtime Reference
        //Reference to all the running modules 
        private Dictionary<HSVTutorialObject, List<HSVTutorialModule>> modules;
        //reference to all the highlighting target objects
        private Dictionary<HSVTutorialObject, List<GameObject>> outlineObjects;
        //The reference to the database scriptable objects, this is used to store all the configuration of tutorial manager
        public HSVTutorialData database;
        //the reference to the expose manager, this is used by database to store scene object reference, you don't need to play with this
        private HSVExposeReferenceMgr m_exposeMgr;
        //Outline controller used to highlight target
        private IOutlineController outlineController;
        //Preview Effect reference
        private Transform previewEffect;

#if UNITY_EDITOR
        //These fields are used by editor. Please do not alter them
        public static string tObjCopyBuffer;
        public static string stageCopyBuffer;
        public static HSVModuleConfigIndex moduleIndex;
        public static bool undoRedoPerformed = false;
        public static bool tmModified = false;
        public static bool stateChange = false;
        public static bool tObjNameChange = false;
        public static bool targetChange = false;
        public static bool moduleChange = false;
#endif
        //This stores how many types of module configs. 
        private List<Type> moduleConfigTypes;
        public List<Type> ModuleConfigTypes
        {
            get
            {
                if (moduleConfigTypes == null)
                {
                    moduleConfigTypes = new List<Type>();
                }

                if (moduleConfigTypes.Count == 0)
                {
                    moduleConfigTypes = GetNonAbstractTypesSubclassOf<HSVModuleConfig>(true);
                }
                return moduleConfigTypes;
            }
        }

        //This stores how many types of target module configs, it is subset of module configs
        private List<Type> targetModuleConfigTypes;
        public List<Type> TargetModuleConfigTypes
        {
            get
            {
                if (targetModuleConfigTypes == null)
                {
                    targetModuleConfigTypes = new List<Type>();
                }

                if (targetModuleConfigTypes.Count == 0)
                {
                    targetModuleConfigTypes = GetNonAbstractTypesSubclassOf<HSVTargetModuleConfig>(true);
                }
                return targetModuleConfigTypes;
            }
        }

        //Expose manager reference
        public HSVExposeReferenceMgr ExposeMgr
        {
            get
            {
                if (m_exposeMgr == null)
                {
                    m_exposeMgr = new GameObject("HSVExposeReferenceManager").AddComponent<HSVExposeReferenceMgr>();
                    m_exposeMgr.gameObject.hideFlags = HideFlags.HideInHierarchy;
                }
                return m_exposeMgr;
            }
        }
        #endregion

        #region Global Events
        public event HSVGlobalStageEvent OnStageStart;
        public event HSVGlobalStageEvent OnStageUpdate;
        public event HSVGlobalStageEvent OnStageExecuting;
        public event HSVGlobalStageEvent OnStageEnding;
        public event HSVGlobalStageEvent OnStageComplete;

        public event HSVGlobalTutorialEvent OnTObjStart;
        public event HSVGlobalTutorialEvent OnTObjUpdate;
        public event HSVGlobalTutorialEvent OnTObjExecuting;
        public event HSVGlobalTutorialEvent OnTObjEnding;
        public event HSVGlobalTutorialEvent OnTObjComplete;

        public event HSVUIMaskEvent OnDisplayTextChange;
        #endregion

        #region Initialization
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }

            tmInitialized = false;

            //Initialize Manager setting and reference
            if (!Application.isPlaying || initializeOnAwake)
            {
                InitTutorialManager();
            }

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                enterFromEditor = true;
            }
#endif
        }

        /// <summary>
        /// Manually initialize tutorial manager
        /// </summary>
        public void InitializeTutorialManager()
        {
            if (tmInitialized)
                return;

            InitTutorialManager();
            StartCoroutine(InitializeTutorialManagerObjects());
        }

        private IEnumerator InitializeTutorialManagerObjects()
        {
            if (Application.isPlaying && tmInitialized)
            {
                //Initialize stage objects
                yield return InitializeStageObjects();
                startFirstTutorial = autoStart;
            }
        }

        private void InitTutorialManager()
        {
            //Finds expose manager on scene, it will not be visible on hierarchy. Please use Tools->HSVStudio->TutorialManager->ClearReference to clean it up if you don't want it.
            m_exposeMgr = FindObjectOfType<HSVExposeReferenceMgr>();
            if (m_exposeMgr == null)
            {
                m_exposeMgr = new GameObject("HSVExposeReferenceManager").AddComponent<HSVExposeReferenceMgr>();
                m_exposeMgr.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }

            //Time Action Manager Component, it is needed by some functions
            var timer = GetComponent<HSVMainTimer>();
            if (timer == null)
            {
                gameObject.AddComponent<HSVMainTimer>();
            }

            //Initialize reference dict and list
            currentPlayingTObjects = new HashSet<HSVTutorialObject>();
            removalTObjs = new HashSet<HSVTutorialObject>();
            modules = new Dictionary<HSVTutorialObject, List<HSVTutorialModule>>();
            outlineObjects = new Dictionary<HSVTutorialObject, List<GameObject>>();
            if (previewEffect == null)
            {
                previewEffect = transform.Find("PreviewEffect");
                if (!Application.isPlaying)
                {
                    if (previewEffect == null)
                    {
                        previewEffect = new GameObject("PreviewEffect").transform;
                        previewEffect.SetParent(this.transform, false);
                    }
                }
            }

            if (stageObjects == null)
            {
                stageObjects = new HSVTutorialStage[0];
            }

            if (assignOnRuntime)
            {
                //If there is no camera reference, uses Camera.Main on scene
                if (Camera == null)
                {
                    Camera = Camera.main;
                }

                //If there is no Canvas reference, uses first Canvas found in children
                if (Canvas == null)
                {
                    Canvas = GetComponentInChildren<Canvas>();
                }

                if (Player == null)
                {
                    Player = GameObject.FindWithTag("Player");
                }
            }

            //If there is no camera or canvas, run the setup
            if (Canvas == null || Camera == null || WorldObjectContainer == null)
            {
                Setup();
            }

            if (Application.isPlaying)
            {
                if (HSVInputManager.instance == null)
                {
                    gameObject.AddComponent<HSVInputManager>();
                }
            }

            if (Camera != null)
            {
                Camera.depthTextureMode = DepthTextureMode.Depth;
            }

            //Setup the outline controller reference if using URP or Builtin
            if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset == null)
            {
                var outline = FindObjectOfType<OutlineEffectsController>();
                if (outline != null)
                {
                    outlineController = outline.GetComponent<IOutlineController>();
                }
            }
            else
            {
                var outline = FindObjectOfType<OutlineEffectControllerURP>();
                if (outline != null)
                {
                    outlineController = outline.GetComponent<IOutlineController>();
                }
            }

            //Finds module types reference
            moduleConfigTypes = GetNonAbstractTypesSubclassOf<HSVModuleConfig>(true);
            targetModuleConfigTypes = GetNonAbstractTypesSubclassOf<HSVTargetModuleConfig>(true);

            tmInitialized = true;
        }

        private void OnValidate()
        {
            moduleConfigTypes = GetNonAbstractTypesSubclassOf<HSVModuleConfig>(true);
            targetModuleConfigTypes = GetNonAbstractTypesSubclassOf<HSVTargetModuleConfig>(true);
        }

        /// <summary>
        /// Setup for the tutorial manager
        /// </summary>
        public void Setup()
        {
            if (stageObjects == null)
            {
                stageObjects = new HSVTutorialStage[0];
            }

            //Add Canvas object if no canvas is found
            var canvas = GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                canvas = (new GameObject("Canvas")).AddComponent<Canvas>();
                canvas.transform.SetParent(this.transform, false);
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 200;
                var canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 0.5f;

                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            if (Canvas == null)
            {
                Canvas = canvas;
            }

            //Add the camera to the scene, if nothing is found
            if (Camera == null)
            {
                if (Camera.main == null)
                {
                    Camera = new GameObject("MainCamera").AddComponent<Camera>();
                    Camera.tag = "MainCamera";
                }
                else
                {
                    Camera = Camera.main;
                }
            }

            //Adds object pooling for runtime performance
            var pool = FindObjectOfType<HSVObjectPool>();
            if (pool == null)
            {
                pool = (new GameObject("ObjectPool")).AddComponent<HSVObjectPool>();

                pool.transform.SetParent(this.transform, false);
                pool.poolParent = pool.transform;
            }

            if (WorldObjectContainer == null)
            {
                WorldObjectContainer = transform.Find("WorldObjectContainer");
                if (WorldObjectContainer == null)
                {
                    WorldObjectContainer = new GameObject("WorldObjectContainer").transform;
                    WorldObjectContainer.SetParent(this.transform, false);
                }
            }
        }

        private IEnumerator Start()
        {
            if (Application.isPlaying && tmInitialized)
            {
                //Initialize stage objects
                yield return InitializeStageObjects();
                startFirstTutorial = autoStart;
            }
        }

        /// <summary>
        /// Initialize all the stage objects
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitializeStageObjects()
        {
            for (int i = 0; i < stageObjects.Length; i++)
            {
                yield return InitializeTutorialObjectsForStage(stageObjects[i]);
            }
        }

        private void OnEnable()
        {
            if (Application.isPlaying && tmInitialized)
            {
#if UNITY_EDITOR
                if (!enterFromEditor)
                {
                    enterFromEditor = true;

                    //Resets Canvas, in case user preview objects are still remained on scene
                    ResetCanvas();
                    //Reset Effect Prefab
                    ResetEffectPrefab();
                    ResetWorldObject();

                    //Sets first stage step to first one on enable
                    currentStageStep = (stageObjects != null && stageObjects.Length > 0) ? 1 : 0;
                    //Clear current playing stage
                    currentPlayingStage = null;
                }
#endif
            }

            if (currentPlayingTObjects == null)
            {
                currentPlayingTObjects = new HashSet<HSVTutorialObject>();
            }

            if (removalTObjs == null)
            {
                removalTObjs = new HashSet<HSVTutorialObject>();
            }

            if (!Application.isPlaying)
            {
                //Sets first stage step to first one on enable
                currentStageStep = (stageObjects != null && stageObjects.Length > 0) ? 1 : 0;
                //Clear current playing stage
                currentPlayingStage = null;

                currentPlayingTObjects.Clear();
            }

            removalTObjs.Clear();
        }

        /// <summary>
        /// Initialize multiple objects across frames to reduce performance impact
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitializeTutorialObjectsForStage(HSVTutorialStage stageObject)
        {
            //Sets the current play step to first one
            stageObject.currentStep = 1;
            //Reset playing state to idle
            SetState(ref stageObject.state, PlayState.Idle);
            //Initialize all events in stageObject
            stageObject.InitializedEvents();
            //Initialize start trigger for the stage object
            InitializeTriggerConfig(stageObject, true);

            for (int i = 0; i < stageObject.tutorialObjects.Length; i++)
            {
                //Sets all the tutorial object stage index to the stage it belongs to
                if (stageObject.tutorialObjects[i].stageIndex != stageObject.index)
                {
                    stageObject.tutorialObjects[i].stageIndex = stageObject.index;
                }
                //Initialize tutorial objects for the stage
                InitializedTutorialObject(stageObject.tutorialObjects[i]);
                yield return null;
            }
        }

        /// <summary>
        /// Initialize trigger for the stage object
        /// </summary>
        /// <param name="stageObject"></param>
        /// <param name="startTrigger"></param>
        private void InitializeTriggerConfig(HSVTutorialStage stageObject, bool startTrigger = true)
        {
            //Find respective trigger config reference
            var config = startTrigger ? stageObject.startTrigger : stageObject.endTrigger;
            Component comp = null;
            bool triggerExist = false;
            switch (config.triggerType)
            {
                case TriggerType.Manual:
                    break;
                case TriggerType.Collider:
                    //finds the trigger object component on collider specified. If it doesn't exist, add to it. Remember to use different collider for different stage. Otherwise it wont't be started.
                    //Using same collider for multiple stage may cause unintented behavior
                    if (config.colliderConfig.collider != null)
                    {
                        comp = config.colliderConfig.collider;
                        var triggerObjs = config.colliderConfig.collider.GetComponents<HSVTriggerObject>();
                        for (int i = 0; i < triggerObjs.Length; i++)
                        {
                            if (triggerObjs[i].IsStage && triggerObjs[i].IsStartTrigger == startTrigger)
                            {
                                triggerExist = true;
                                break;
                            }
                        }
                    }
                    break;
                case TriggerType.UI:
                    //finds the trigger object component on UI component specified. If it doesn't exist, add to it. Remember to use different UI component for different stage. Otherwise it wont't be started.
                    //Using same collider for multiple stage may cause unintented behavior
                    if (config.graphicConfig.graphic != null)
                    {
                        comp = config.graphicConfig.graphic;
                        var triggerObjs = config.graphicConfig.graphic.GetComponents<HSVTriggerObject>();
                        for (int i = 0; i < triggerObjs.Length; i++)
                        {
                            if (triggerObjs[i].IsStage && triggerObjs[i].IsStartTrigger == startTrigger)
                            {
                                triggerExist = true;
                                break;
                            }
                        }
                    }
                    break;
            }

            if (config.triggerType == TriggerType.Collider || config.triggerType == TriggerType.UI)
            {
                if (!triggerExist && comp != null)
                {
                    var triggerObj = comp.gameObject.AddComponent<HSVTriggerObject>();
                    //Setup the trigger object collider
                    triggerObj.SetupTriggerConfig(config, stageObject.index, -1, startTrigger, true);
                }
                else
                {
                    DebugLogWarning("Cannot add trigger object to the component: " + " on " + (startTrigger ? "Start Trigger" : "End Trigger") + " with stage Index: " + stageObject.index + ", as there is another trigger object on it. Please use different collider/graphic for different stage");
                }
            }

        }

        //Method used to set camera reference on runtime
        public void SetCamera(Camera camera)
        {
            Camera = camera;
        }

        public void SetPlayer(GameObject player)
        {
            Player = player;
        }
        #endregion

        private void Update()
        {
            if (Application.isPlaying && tmInitialized)
            {
                //if no stage is playing and auto start tutorial, this only runs once
                if (currentPlayingStage == null && startFirstTutorial)
                {
                    //if auto start index specified, it will play that index, -1 will play the first stage
                    currentStageStep = (autoStartIndex == -1) ? 1 : Mathf.Min(stageObjects.Length, Mathf.Max(1, autoStartIndex));
                    PlayStage();
                    startFirstTutorial = false;
                }

                //Checks event trigger for the current playing stage and tutorial objects
                if (currentPlayingStage != null)
                {
                    CheckEventTrigger(currentPlayingStage, false);
                    UpdateEvent(currentPlayingStage);

                    foreach (var tObj in currentPlayingTObjects)
                    {
                        //Updates the tracking target bounding rect when playing
                        tObj.UpdateTargetScreenRect();
                        CheckEventTrigger(tObj, false);
                        UpdateEvent(tObj);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (removalTObjs.Count == 0)
                return;

            foreach (var tObj in removalTObjs)
            {
                if (currentPlayingTObjects.Contains(tObj))
                {
                    currentPlayingTObjects.Remove(tObj);
                }
            }

            removalTObjs.Clear();
        }

        #region Tutorial Play Section
        #region Stage
        /// <summary>
        /// Plays stage using stage index
        /// </summary>
        /// <param name="stageStep"></param> It is using element array position plus one notation. Please be careful with index. 
        /// <param name="autoStart"></param> Indicates if tutorial object should be started automatically. Make sure stage object starts first before starting tutorial objects
        public void PlayStage(int stageStep, bool autoStart = true)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            //Finds stageObject using index
            var stageObj = GetCurrentStageObject(stageStep - 1);
            if (stageObj != null)
            {
                //Check if stage can be played
                if (CanPlayStage(stageObj))
                {
                    StartStage(stageObj, autoStart);
                }
            }
            else
            {
                DebugLogWarning("No Stage Object for current stage steps " + stageStep);
            }
        }

        /// <summary>
        /// Plays stage using stage name
        /// </summary>
        /// <param name="stageName"></param> It is using Stage Name to find the stageObject
        /// <param name="autoStart"></param> Indicates if tutorial object should be started automatically. Make sure stage object starts first before starting tutorial objects
        public void PlayStage(string stageName, bool autoStart = true)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            //Finds stageObject using Name
            var stageObj = GetCurrentStageObject(stageName);
            if (stageObj != null)
            {
                //Check if stage can be played
                if (CanPlayStage(stageObj))
                {
                    StartStage(stageObj, autoStart);
                }
            }
            else
            {
                DebugLogWarning("No Tutorial Stage found for current stage steps " + stageName);
            }
        }

        /// <summary>
        /// Plays stage using currentStageStep
        /// </summary>
        /// <param name="autoStart"></param> Indicates if tutorial object should be started automatically. Make sure stage object starts first before starting tutorial objects
        public void PlayStage(bool autoStart = true)
        {
            PlayStage(currentStageStep, autoStart);
        }

        /// <summary>
        /// Can the stage object be started, if there is playing stage, returns false
        /// </summary>
        /// <param name="stageObj"></param>
        /// <returns></returns>
        private bool CanPlayStage(HSVTutorialStage stageObj)
        {
            if (currentPlayingStage != null)
            {
                if (currentPlayingStage == stageObj)
                {
                    DebugLogWarning("Stage: " + stageObj.stageName + " cannot be started, The stage is already playing ");
                }
                else
                {
                    DebugLogWarning("Stage: " + stageObj.stageName + " cannot be started, other stage is currently playing " + currentPlayingStage.stageName);
                }

                return false;
            }
            else
            {
                if (stageObj.state != PlayState.Idle)
                {
                    DebugLogWarning("Stage: " + stageObj.stageName + " cannot be started, The stage is at state: " + stageObj.state + ". Please reset the stage object");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Starts stage object
        /// </summary>
        /// <param name="stageObj"></param>
        /// <param name="autoStart"></param> If true, will start tutorial object immediately
        private void StartStage(HSVTutorialStage stageObj, bool autoStart = true)
        {
            DebugLog("Starting Stage Object: " + stageObj.stageName + ", Index: " + stageObj.index);

            currentPlayingStage = stageObj;
            currentStageStep = stageObj.index;
            //Resets tutorial object states
            ResetTutorialObjectsState(stageObj);
            //Clear the start trigger for the current stage
            ClearStageTriggerConfig(stageObj, true);
            //Setup the end trigger for the current stage
            InitializeTriggerConfig(stageObj, false);
            //Sets state to start
            SetState(ref stageObj.state, PlayState.Start);
            OnStageStart?.Invoke(stageObj);
            //Event callback
            if (RunEvent(stageObj.AllEvents, HSVEventType.OnStart))
            {
                DebugLog("Invoking 'OnStart' event on stage: " + stageObj.stageName + ", Index: " + stageObj.index);
            }
            //Initialize the start trigger for all the tutorial objects
            for (int i = 0; i < stageObj.tutorialObjects.Length; i++)
            {
                InitializeTriggerConfig(stageObj.tutorialObjects[i], true);
            }
            //Start tutorial object using startObjectIndex
            if (autoStart)
            {
                var firstObjIndex = stageObj.startObjectIndex == -1 ? 1 : Mathf.Min(stageObj.tutorialObjects.Length, Mathf.Max(1, stageObj.startObjectIndex));
                PlayTutorial(firstObjIndex);
            }
            //Sets state to Execute
            SetState(ref stageObj.state, PlayState.Executing);
            OnStageExecuting?.Invoke(stageObj);
            //Event callback
            if (RunEvent(stageObj.AllEvents, HSVEventType.OnExecuting))
            {
                DebugLog("Invoking 'OnExecuting' event on stage: " + stageObj.stageName + ", Index: " + stageObj.index);
            }
        }

        /// <summary>
        /// Stop the current playing stage object
        /// </summary>
        public void StopStage()
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            if (currentPlayingStage == null)
            {
                DebugLog("No Running Stage at the moment. StopStage Unsuccessful");
                currentPlayingTObjects.Clear();
                return;
            }

            //can only end stage if it is either at start or executing state
            if (currentPlayingStage.state == PlayState.Start || currentPlayingStage.state == PlayState.Executing)
            {
                //Clear the end trigger for the playing stage
                ClearStageTriggerConfig(currentPlayingStage, false);
                StartCoroutine(StopStage(true));
            }
            else
            {
                DebugLogWarning("Current stage:" + currentPlayingStage.stageName + " is ending, please wait until the stage is complete.");
            }
        }

        /// <summary>
        /// Stop the current playing stage. This will check the state of tutorial object across many frames
        /// </summary>
        /// <param name="autoAdvance"></param>
        /// <returns></returns>
        private IEnumerator StopStage(bool autoAdvance)
        {
            DebugLog("Stopping Tutorial Stage: " + currentPlayingStage.stageName + ", Index: " + currentPlayingStage.index);
            //Setting state to end
            SetState(ref currentPlayingStage.state, PlayState.Ending);
            OnStageEnding?.Invoke(currentPlayingStage);
            //Event callback
            if (RunEvent(currentPlayingStage.AllEvents, HSVEventType.OnEnding))
            {
                DebugLog("Invoking 'OnEnding' event on stage: " + currentPlayingStage.stageName + ", Index: " + currentPlayingStage.index);
            }
            //Call stop tutorial for all the playing tutorial objects
            foreach (var tObj in currentPlayingTObjects)
            {
                StopTutorial(tObj.index, false);
            }

            //Continuous checking if all tutorial objects are completed
            while (!CheckAllTutorialObjectComplete())
            {
                yield return null;
            }

            currentPlayingTObjects.Clear();
            //Setting state to end
            SetState(ref currentPlayingStage.state, PlayState.End);
            OnStageComplete?.Invoke(currentPlayingStage);
            //Event callback
            if (RunEvent(currentPlayingStage.AllEvents, HSVEventType.OnComplete))
            {
                DebugLog("Invoking 'OnComplete' event on stage: " + currentPlayingStage.stageName + ", Index: " + currentPlayingStage.index);
            }
            //Check advance config
            var advanceConfig = currentPlayingStage.advanceConfig;
            currentPlayingStage = null;

            if (currentStageStep < stageObjects.Length)
            {
                //increment the stage step
                currentStageStep++;

                bool addStartTrigger = true;
                if (autoAdvance)
                {
                    switch (advanceConfig.advanceType)
                    {
                        case AdvanceType.None:
                            //Do nothing
                            break;
                        case AdvanceType.Automatic:
                            //Play next stage
                            addStartTrigger = false;
                            PlayStage(currentStageStep);
                            break;
                        case AdvanceType.Index:
                            //Play stage using index
                            addStartTrigger = false;
                            PlayStage(advanceConfig.index);
                            break;
                        case AdvanceType.Name:
                            //Play stage using name
                            addStartTrigger = false;
                            PlayStage(advanceConfig.name);
                            break;
                    }
                }

                //Adding start trigger to the target
                if (addStartTrigger)
                {
                    var newStage = GetCurrentStageObject(currentStageStep);
                    if (newStage != null)
                    {
                        InitializeTriggerConfig(newStage, true);
                    }
                }
            }
            else
            {
                //Reset stage step to 1
                //currentStageStep = 1;
                DebugLog("Reaches the end of the stage objects! Reseting Current Stage Step to 1");
            }
        }

        /// <summary>
        /// Reset stage object to initial state and initialize triggers, use stage index instead of array position
        /// </summary>
        /// <param name="index"></param>
        public void ResetStage(int index)
        {
            var stageObj = GetCurrentStageObject(index - 1);
            if (stageObj != null)
            {
                ResetStage(stageObj);
            }
        }

        /// <summary>
        /// Reset stage object to initial state and initialize triggers, use stage index instead of array position
        /// </summary>
        /// <param name="name"></param>
        public void ResetStage(string name)
        {
            var stageObj = GetCurrentStageObject(name);
            if (stageObj != null)
            {
                ResetStage(stageObj);
            }
        }

        /// <summary>
        /// Reset stage object to initial state and initialize triggers, use stage index instead of array position
        /// </summary>
        /// <param name="stageObj"></param>
        public void ResetStage(HSVTutorialStage stageObj)
        {
            if (stageObj != null)
            {
                StartCoroutine(ResetStageObject(stageObj));
            }
        }

        private IEnumerator ResetStageObject(HSVTutorialStage stageObj)
        {
            yield return InitializeTutorialObjectsForStage(stageObj);

            DebugLog("Complete Resetting stage: " + stageObj.stageName + " index: " + stageObj.index);
        }
        #endregion

        #region Tutorial Object
        /// <summary>
        /// Play Tutorial using step index
        /// </summary>
        /// <param name="step"></param>
        public void PlayTutorial(int step, bool reset = true)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            if (currentPlayingStage == null)
                return;

            var tObj = GetCurrentTutorialObject(currentPlayingStage, step - 1);
            if (tObj != null)
            {
                if (reset)
                {
                    InitializedTutorialObject(tObj);
                }

                if (CanPlayTutorial(tObj))
                {
                    StartTutorial(tObj);
                }
            }
            else
            {
                DebugLogWarning("No Tutorial Object for current tutorial steps: " + step + " in stage: " + currentPlayingStage.stageName);
            }
        }

        /// <summary>
        /// Play Tutorial using tutorial name
        /// </summary>
        /// <param name="name"></param>
        public void PlayTutorial(string name, bool reset = true)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            if (currentPlayingStage == null)
            {
                DebugLogWarning("No stage playing at the moment, please start stage first or use PlayTutorial(int stageIndex, int tObjIndex) method");
                return;
            }

            var tObj = GetCurrentTutorialObject(currentPlayingStage, name);
            if (tObj != null)
            {
                if (reset)
                {
                    InitializedTutorialObject(tObj);
                }

                if (CanPlayTutorial(tObj))
                {
                    StartTutorial(tObj);
                }
            }
            else
            {
                DebugLogWarning("No Tutorial Object for current tutorial steps: " + currentPlayingStage.currentStep + " in stage: " + currentPlayingStage.stageName);
            }
        }

        /// <summary>
        /// Play Tutorial using current step index
        /// </summary>
		public void PlayTutorial()
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            if (currentPlayingStage == null)
                return;

            PlayTutorial(currentPlayingStage.currentStep);
        }

        /// <summary>
        /// Play tutorial using stage index and tutorial index, if there is any playing stage, but not the stage with stageindex, the playing won't work
        /// </summary>
        /// <param name="stageIndex"></param>
        /// <param name="tObjIndex"></param>
        public void PlayTutorial(int stageIndex, int tObjIndex)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            if (currentPlayingStage == null)
            {
                //Start stage first, but do not autostart
                PlayStage(stageIndex, false);
                PlayTutorial(tObjIndex);
            }
            else if (currentPlayingStage.index == stageIndex)
            {
                //if current playing stage is equal to stageIndex, we play tutorial, however, if not allow multiple tutorial object, the tutorial won't start if there is other tutorial playing
                PlayTutorial(tObjIndex);
            }
            else
            {
                DebugLogWarning("Stage with index: " + stageIndex + " cannot be played due to current playing stage: " + currentPlayingStage.stageName);
            }
        }

        /// <summary>
        /// Check if tutorial object can be played, if not allow multiple, only one tutorial object can be played
        /// </summary>
        /// <param name="tObj"></param>
        /// <returns></returns>
        private bool CanPlayTutorial(HSVTutorialObject tObj)
        {
            if (currentPlayingStage != null && !currentPlayingStage.allowMultiple && GetPlayingTObjCount() > 0)
            {
                DebugLogWarning("Stage doesn't allow multiple tutorial objects running. Please end other tutorial first before starting this: " + tObj.Name);
                return false;
            }

            if (!currentPlayingTObjects.Contains(tObj))
            {
                //Check tutorial object state
                if (tObj.state != PlayState.Idle)
                {
                    DebugLogWarning("Tutorial: " + tObj.Name + " cannot be started, The tutorial is at state: " + tObj.state + ". Please reset the tutorial object");
                    return false;
                }

                //check if module prefab is missing
                for (int i = 0; i < tObj.moduleConfigs.Length; i++)
                {
                    if (tObj.moduleConfigs[i].modulePrefab == null)
                    {
                        DebugLogWarning("Tutorial Object " + tObj.Name + " do not have module prefab for: " + tObj.moduleConfigs[i].GetType());
                        return false;
                    }
                }

                return true;
            }
            else
            {
                DebugLogWarning("Tutorial Object " + tObj.Name + " cannot be started, it is currently playing");
            }

            return false;
        }

        private int GetPlayingTObjCount()
        {
            int count = 0;
            if (currentPlayingTObjects != null)
            {
                foreach (var tObj in currentPlayingTObjects)
                {
                    if (tObj.state != PlayState.End)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Starts the tutorial object
        /// </summary>
        /// <param name="tObj"></param>
        private void StartTutorial(HSVTutorialObject tObj)
        {
            DebugLog("Starting Tutorial Object: " + tObj.Name + ", Index: " + tObj.index);
            if (!currentPlayingTObjects.Contains(tObj))
            {
                currentPlayingTObjects.Add(tObj);
            }
            currentPlayingStage.currentStep = tObj.index;
            //Clear the start trigger for the tutorial object
            ClearTutorialTriggerConfig(tObj);
            //Setup the end trigger for the tutorial object
            InitializeTriggerConfig(tObj, false);
            SetState(ref tObj.state, PlayState.Start);
            OnTObjStart?.Invoke(tObj);
            //run start event
            if (RunEvent(tObj.AllEvents, HSVEventType.OnStart))
            {
                DebugLog("Invoking 'OnStart' event on tutorial: " + tObj.Name + ", Index: " + tObj.index);
            }

            //Sets the target active for the tutorial object
            SetTargetStatus(tObj);

            //Initialize the module for the tutorial object
            for (int i = 0; i < tObj.moduleConfigs.Length; i++)
            {
                InitializeModule(tObj);
            }

            //Sets module reference and starts module
            if (modules.ContainsKey(tObj))
            {
                for (int i = 0; i < modules[tObj].Count; i++)
                {
                    var module = modules[tObj][i];
                    if (module != null)
                    {
                        module.gameObject.SetActive(true);
                        module.SetTutorialObject(tObj);
                        module.ModuleStart();
                    }
                }
            }
            else
            {
                DebugLog("Something went wrong with module initialization");
            }

            //Highlights target for the tutorial object
            HighlightTarget(tObj, true);
        }

        /// <summary>
        /// Stop tutorial
        /// </summary>
        /// <param name="autoAdvance"></param> If true, will use advance config to do the advance
        public void StopTutorial(bool autoAdvance = true)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            if (currentPlayingStage == null)
            {
                DebugLogWarning("No stage playing at the moment, please use StopTutorial(int stageIndex, int tObjIndex, bool autoAdvance = true) method");
                return;
            }

            StopTutorial(currentPlayingStage.currentStep, autoAdvance);
        }

        /// <summary>
        /// Stop tutorial using step index
        /// </summary>
        /// <param name="step"></param>
        /// <param name="autoAdvance"></param> If true, will use advance config to do the advance
        public void StopTutorial(int step, bool autoAdvance = true)
        {
            if (!Application.isPlaying)
                return;

            if (currentPlayingStage == null)
            {
                DebugLogWarning("No stage playing at the moment, please use StopTutorial(int stageIndex, int tObjIndex, bool autoAdvance = true) method");
                return;
            }

            var tObj = GetCurrentTutorialObject(currentPlayingStage, step - 1);
            if (tObj != null)
            {
                //check if current playing list contains the tutorial object
                if (currentPlayingTObjects.Contains(tObj))
                {
                    if (tObj.state == PlayState.Start || tObj.state == PlayState.Executing)
                    {
                        StartCoroutine(StopTutorialStep(tObj, autoAdvance));
                    }
                    else
                    {
                        DebugLogWarning("Current tutorial:" + tObj.Name + " is ending, please wait until the tutorial is complete.");
                    }
                }
                else
                {
                    DebugLogWarning("There is no running tutorial object atm");
                }
            }
            else
            {
                DebugLogWarning("Could not find tutorial object with step " + currentPlayingStage.currentStep + " when stopping");
            }
        }

        /// <summary>
        /// Stop tutorial using stageindex, and tutorial index
        /// </summary>
        /// <param name="stageIndex"></param>
        /// <param name="tObjIndex"></param>
        /// <param name="autoAdvance"></param> If true, will use advance config to do the advance
        public void StopTutorial(int stageIndex, int tObjIndex, bool autoAdvance = true)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            if (currentPlayingStage != null && currentPlayingStage.index == stageIndex)
            {
                StopTutorial(tObjIndex, autoAdvance);
            }
            else
            {
                DebugLogWarning("Stage with index: " + stageIndex + " is not playing at the moment");
            }
        }

        /// <summary>
        /// Stop tutorial using Name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="autoAdvance"></param> If true, will use advance config to do the advance
        public void StopTutorial(string name, bool autoAdvance = true)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            if (currentPlayingStage == null)
            {
                DebugLogWarning("No stage playing at the moment, please use StopTutorial(int stageIndex, int tObjIndex, bool autoAdvance = true) method");
                return;
            }

            var tObj = GetCurrentTutorialObject(currentPlayingStage, name);
            if (tObj != null)
            {
                if (currentPlayingTObjects.Contains(tObj))
                {
                    if (tObj.state == PlayState.Start || tObj.state == PlayState.Executing)
                    {
                        StartCoroutine(StopTutorialStep(tObj, autoAdvance));
                    }
                    else
                    {
                        DebugLogWarning("Current tutorial:" + tObj.Name + " is ending, please wait until the tutorial is complete.");
                    }
                }
                else
                {
                    DebugLogWarning("There is no running tutorial object atm");
                }
            }
            else
            {
                DebugLogWarning("Could not find tutorial object with step " + currentPlayingStage.currentStep + " when stopping");
            }
        }

        /// <summary>
        /// Coroutine for stopping tutorial
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="autoAdvance"></param> If true, will use advance config to do the advance
        /// <returns></returns>
        private IEnumerator StopTutorialStep(HSVTutorialObject tObj, bool autoAdvance = true, bool stepBack = false)
        {
            DebugLog("Stopping Tutorial Object: " + tObj.Name + ", Index: " + tObj.index);
            SetState(ref tObj.state, PlayState.Ending);
            OnTObjEnding?.Invoke(tObj);
            if (RunEvent(tObj.AllEvents, HSVEventType.OnEnding))
            {
                DebugLog("Invoking 'OnEnding' event on tutorial: " + tObj.Name + ", Index: " + tObj.index);
            }

            //Clear end trigger for the tutorial
            ClearTutorialTriggerConfig(tObj, false);

            SetTargetStatus(tObj, true);

            //Set all tutorial module to end
            if (modules.ContainsKey(tObj))
            {
                for (int i = 0; i < modules[tObj].Count; i++)
                {
                    var module = modules[tObj][i];
                    if (module != null)
                    {
                        module.ModuleEnd();
                    }
                }

                //Highlight off targets
                HighlightTarget(tObj, false);

                //Check if all module ends, and clean up module spawned
                yield return StartCoroutine(CheckTutorialComplete(tObj));

                if (!stepBack)
                {
                    //If the tutorial is the last of the belonging stage, we stop stage and do not advance
                    if (CheckStageEnd(tObj))
                    {
                        StopStage();
                    }
                    else
                    {
                        //if advance type is automatic, advance the index
                        if (currentPlayingStage != null && tObj.advanceConfig.advanceType == AdvanceType.Automatic)
                        {
                            currentPlayingStage.currentStep++;
                        }

                        //Check the advance config
                        bool addStartTrigger = true;
                        if (autoAdvance)
                        {
                            switch (tObj.advanceConfig.advanceType)
                            {
                                case AdvanceType.None:
                                    //Do nothing
                                    break;
                                case AdvanceType.Automatic:
                                    addStartTrigger = false;
                                    PlayTutorial();
                                    break;
                                case AdvanceType.Index:
                                    addStartTrigger = false;
                                    PlayTutorial(tObj.advanceConfig.index);
                                    break;
                                case AdvanceType.Name:
                                    addStartTrigger = false;
                                    PlayTutorial(tObj.advanceConfig.name);
                                    break;
                            }
                        }

                        //Adds the start trigger to the target collider if advance set to none
                        if (addStartTrigger)
                        {
                            var newTObj = GetCurrentTutorialObject(tObj.stageIndex, tObj.index + 1);
                            if (newTObj != null)
                            {
                                InitializeTriggerConfig(newTObj, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Advance the tutorial to the next step. You don't need to call StopTutorial before this
        /// </summary>
        public void AdvanceTutorial()
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            StartCoroutine(AdvanceTutorialToNext());
        }

        /// <summary>
        /// Coroutine for advancing tutorial
        /// </summary>
        /// <returns></returns>
        private IEnumerator AdvanceTutorialToNext()
        {
            if (currentPlayingStage != null)
            {
                //Check if current playing tutorial is end of tutorial
                bool stageEnd = CheckStageEnd(currentPlayingStage.currentStep);

                var tObj = GetCurrentTutorialObject(currentPlayingStage, currentPlayingStage.currentStep - 1);

                if (tObj != null && currentPlayingTObjects.Contains(tObj))
                {
                    DebugLog("Advancing tutorial object " + tObj.Name + " with Index: " + tObj.index + " stage Index: " + tObj.stageIndex);
                    //Stop current playing tutorial if there is any
                    yield return StopTutorialStep(tObj, false);
                    //Plays tutorial if not the end of stage
                    if (!stageEnd && currentPlayingStage != null)
                    {
                        PlayTutorial();
                    }
                }
                else
                {
                    //Plays tutorial if not the end of stage
                    if (!stageEnd && tObj != null)
                    {
                        PlayTutorial();
                    }
                    else
                    {
                        DebugLogWarning("cannot advance tutorial as no Tutorial Object for current tutorial steps");
                    }
                }
            }
            else
            {
                //Play stage if it is not playing
                PlayStage();
            }
        }

        public void StepbackTutorial()
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            StartCoroutine(StepbackTutorialToPrevious());
        }

        /// <summary>
        /// Coroutine for advancing tutorial
        /// </summary>
        /// <returns></returns>
        private IEnumerator StepbackTutorialToPrevious()
        {
            if (currentPlayingStage != null)
            {
                //Check if current playing tutorial is start of tutorial
                bool stageStart = CheckStageStart(currentPlayingStage.currentStep);

                var tObj = GetCurrentTutorialObject(currentPlayingStage, currentPlayingStage.currentStep - 1);
                var currentStep = currentPlayingStage.currentStep;

                if (tObj != null && currentPlayingTObjects.Contains(tObj))
                {
                    DebugLog("Steping back tutorial object " + tObj.Name + " with Index: " + tObj.index + " stage Index: " + tObj.stageIndex);
                    //Stop current playing tutorial if there is any
                    yield return StopTutorialStep(tObj, false, true);
                    //Plays tutorial if not the start of stage
                    if (!stageStart && currentPlayingStage != null)
                    {
                        var previousTObj = GetCurrentTutorialObject(currentPlayingStage, currentStep - 2);
                        if (previousTObj != null)
                        {
                            PlayTutorial(previousTObj.index);
                        }
                    }
                }
                else
                {
                    //Plays tutorial if not the end of stage
                    if (!stageStart && tObj != null)
                    {
                        var previousTObj = GetCurrentTutorialObject(currentPlayingStage, currentStep - 2);
                        if (previousTObj != null)
                        {
                            PlayTutorial(previousTObj.index);
                        }
                    }
                    else
                    {
                        DebugLogWarning("cannot step tutorial as the tutorial is the first tutorial object");
                    }
                }
            }
            else
            {
                //Play stage if it is not playing
                PlayStage();
            }
        }

        /// <summary>
        /// Coroutine for checking if tutorial completes
        /// </summary>
        /// <param name="tObj"></param>
        /// <returns></returns>
        private IEnumerator CheckTutorialComplete(HSVTutorialObject tObj)
        {
            while (!CheckComplete(tObj))
            {
                yield return null;
            }

            //Set all modules inactive
            if (modules.ContainsKey(tObj))
            {
                for (int i = 0; i < modules[tObj].Count; i++)
                {
                    var module = modules[tObj][i];
                    if (module != null)
                    {
                        module.gameObject.SetActive(false);
                    }
                }
            }

            DebugLog("Tutorial Object: " + tObj.Name + ", Index: " + tObj.index + " is now complete");
            if (currentPlayingTObjects.Contains(tObj))
            {
                if (!removalTObjs.Contains(tObj))
                {
                    removalTObjs.Add(tObj);
                }
                else
                {
                    DebugLogWarning("Removal list contains tutorial object " + tObj.Name);
                }
            }
            else
            {
                DebugLogWarning("Could not find " + tObj.Name + " when removing from playing list");
            }

            TutorialComplete(tObj);
        }

        /// <summary>
        /// Update Tutorial state if all the modules are in executing state
        /// </summary>
        /// <param name="tObj"></param>
        public void UpdateTutorialState(HSVTutorialObject tObj, PlayState state)
        {
            if (currentPlayingTObjects.Contains(tObj))
            {
                if (modules.ContainsKey(tObj))
                {
                    foreach (var mod in modules[tObj])
                    {
                        if (mod.state != state)
                        {
                            return;
                        }
                    }

                    //Change tutorial tobj state
                    SetState(ref tObj.state, state);
                    if (state == PlayState.Executing)
                    {
                        OnTObjExecuting?.Invoke(tObj);
                    }

                    if (RunEvent(tObj.AllEvents, tObj.state))
                    {
                        DebugLog("Invoking '" + state + "' event on tutorial: " + tObj.Name + ", Index: " + tObj.index);
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Tutorial Create Section
        /// <summary>
        /// Initialize stage's tutorial objects
        /// </summary>
        /// <param name="stageIndex"></param>
        public void InitializeCurrentTutorialObject(int stageIndex, int tObjIndex)
        {
            if (!Application.isPlaying || !tmInitialized)
                return;

            var stageElement = GetCurrentStageObject(stageIndex - 1);
            if (stageElement != null)
            {
                var tObj = GetCurrentTutorialObject(stageElement, tObjIndex - 1);
                if (tObj != null)
                {
                    //Initialize all target information for the tutorial objects
                    InitializeTargetInfo(tObj);
                }
            }
        }

        /// <summary>
        /// Initialize the module for tutorial object
        /// </summary>
        /// <param name="tObj"></param>
        private void InitializeModule(HSVTutorialObject tObj)
        {
            if (!modules.ContainsKey(tObj))
            {
                modules.Add(tObj, new List<HSVTutorialModule>());
            }

            if (modules[tObj] == null)
            {
                modules[tObj] = new List<HSVTutorialModule>();
            }

            //Clean up module first if any 
            ClearModules(tObj);

            //Spawn module prefabs for the tutorial object
            for (int i = 0; i < tObj.moduleConfigs.Length; i++)
            {
                var mod = HSVObjectPool.Instance.GetFreeEntity(tObj.moduleConfigs[i].modulePrefab).GetComponent<HSVTutorialModule>();
                if (mod != null)
                {
                    mod.config = tObj.moduleConfigs[i];
                    modules[tObj].Add(mod);
                    //set mod to the child of Canvas
                    mod.transform.SetParent(Canvas.transform, false);
                    mod.transform.localPosition = Vector3.zero;
                    mod.transform.localRotation = Quaternion.identity;
                    mod.transform.localScale = Vector3.one;
                    (mod.transform as RectTransform).sizeDelta = Vector2.zero;
                    mod.gameObject.SetActive(false);
                }
                else
                {
                    DebugLogWarning("Could not find the module prefab in the tutorial object configs :" + tObj.Name);
                }
            }
        }

        /// <summary>
        /// Check if module is spawned to prevent repetitive spawning
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CheckModuleConfigSpawn(HSVTutorialObject tObj, Type type)
        {
            for (int i = 0; i < modules[tObj].Count; i++)
            {
                if (type.Name.Contains(modules[tObj].GetType().Name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Initialize tutorial object
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="reset"></param> resetting target information
        private void InitializedTutorialObject(HSVTutorialObject tObj, bool reset = true)
        {
            SetState(ref tObj.state, PlayState.Idle);
            tObj.InitializedEvents();
            //Remove all target info first
            UninitializeTargetInfo(tObj);
            InitializeTargetInfo(tObj, reset);
        }

        /// <summary>
        /// Initialize target information
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="reset"></param> resetting target information
        private void InitializeTargetInfo(HSVTutorialObject tObj, bool reset = true)
        {
            if (tObj != null)
            {
                for (int i = 0; i < tObj.focusTargets.Length; i++)
                {
                    UpdateTargetRect(tObj.focusTargets[i], reset);
                }
            }
        }

        /// <summary>
        /// Unitialize target information
        /// </summary>
        /// <param name="tObj"></param>
        private void UninitializeTargetInfo(HSVTutorialObject tObj)
        {
            if (tObj != null)
            {
                for (int i = 0; i < tObj.focusTargets.Length; i++)
                {
                    tObj.focusTargets[i].rtTarget = null;
                    tObj.focusTargets[i].rtCanvas = null;
                }
            }
        }

        /// <summary>
        /// Updates the target information including reference
        /// </summary>
        /// <param name="target"></param>
        /// <param name="reset"></param>
        private void UpdateTargetRect(HSVTarget target, bool reset = true)
        {
            //Check if it is not prefab
            if (target != null && target.target != null)
            {
                //if the target is prefab, we spawned it on scene
                if (Application.isPlaying && target.target.scene.name == null)
                {
                    target.rtTarget = GameObject.Instantiate(target.target);
                    if (target.overrideSpawn)
                    {
                        target.rtTarget.transform.position = target.spawnInfo.spawnPos;
                        target.rtTarget.transform.rotation = Quaternion.Euler(target.spawnInfo.spawnRot);
                        target.rtTarget.transform.localScale = target.spawnInfo.spawnScale;
                    }
                }
                //if target is scene object, we set the runtime reference
                else if (target.target.scene.name != null)
                {
                    target.rtTarget = target.target;
                }

                //if target is not using recttransform, we get the unscalebounds
                if (reset && target.rtTarget != null && target.TargetBoundType != BoundType.RectTransform)
                {
                    var status = target.rtTarget.activeSelf;
                    var parent = target.rtTarget.transform.parent;
                    target.rtTarget.transform.SetParent(null);
                    target.rtTarget.SetActive(true);
                    target.UnscaledBounds.Clear();
                    target.UnscaledBounds = TransformExtensionUtility.CalculateAllTransBound(target.rtTarget.transform);
                    target.UpdateTargetBound();
                    target.rtTarget.SetActive(status);
                    target.rtTarget.transform.SetParent(parent);
                }

                //Update the canvas reference if target has any
                if (target.rtTarget != null)
                {
                    target.UpdateCanvas();
                    //Update the screen rect size of the target
                    target.UpdateTargetScreenRect(true);
                }

                //if starts inactive, we disable the scene target
                if (target.startInactive && target.rtTarget != null)
                {
                    target.rtTarget.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Initialize trigger for the tutorial object
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="startTrigger"></param>
        private void InitializeTriggerConfig(HSVTutorialObject tObj, bool startTrigger = true)
        {
            var config = startTrigger ? tObj.startTrigger : tObj.endTrigger;
            Component comp = null;
            bool triggerExist = false;
            switch (config.triggerType)
            {
                case TriggerType.Manual:
                    break;
                case TriggerType.Collider:
                    if (config.colliderConfig.collider != null)
                    {
                        comp = config.colliderConfig.collider;
                        var triggerObjs = config.colliderConfig.collider.GetComponents<HSVTriggerObject>();
                        for (int i = 0; i < triggerObjs.Length; i++)
                        {
                            if (!triggerObjs[i].IsStage && triggerObjs[i].IsStartTrigger == startTrigger)
                            {
                                triggerExist = true;
                                break;
                            }
                        }
                    }
                    break;
                case TriggerType.UI:
                    if (config.graphicConfig.graphic != null)
                    {
                        comp = config.graphicConfig.graphic;
                        var triggerObjs = config.graphicConfig.graphic.GetComponents<HSVTriggerObject>();
                        for (int i = 0; i < triggerObjs.Length; i++)
                        {
                            if (!triggerObjs[i].IsStage && triggerObjs[i].IsStartTrigger == startTrigger)
                            {
                                triggerExist = true;
                                break;
                            }
                        }
                    }
                    break;
            }

            if (config.triggerType == TriggerType.Collider || config.triggerType == TriggerType.UI)
            {
                //May add multiple trigger tutorial object in the future, rightnow only support one trigger per tutorial object
                if (!triggerExist && comp != null)
                {
                    var triggerObj = comp.gameObject.AddComponent<HSVTriggerObject>();
                    triggerObj.SetupTriggerConfig(config, tObj.stageIndex, tObj.index, startTrigger);
                }
                else
                {
                    DebugLogWarning("Cannot add trigger object to the Component: " + " on " + (startTrigger ? "Start Trigger" : "End Trigger") + " with TObj Name: " + tObj.Name + ", as there is another trigger object on it");
                }
            }
        }

        /// <summary>
        /// Sets the runtime scene target to be active when tutorial runs
        /// </summary>
        /// <param name="tObj"></param>
        private void SetTargetStatus(HSVTutorialObject tObj, bool clear = false)
        {
            if (tObj != null)
            {
                for (int i = 0; i < tObj.focusTargets.Length; i++)
                {
                    var target = tObj.focusTargets[i];
                    if (clear)
                    {
                        if (target.effectRT != null)
                        {
                            HSVObjectPool.Instance.SetEntityAsFree(target.effectRT.transform);
                            target.effectRT = null;
                        }

                        ClearTargetTrigger(target);
                    }
                    else
                    {
                        if (target.useTrigger)
                        {
                            SetupTargetTrigger(tObj, target);
                        }

                        if (target.rtTarget != null && !target.rtTarget.activeSelf)
                        {
                            target.rtTarget.SetActive(true);
                        }

                        if (target.rtTarget != null && target.spawnEffect)
                        {
                            target.effectRT = HSVObjectPool.Instance.GetFreeEntity(target.effectPrefab);
                            if (target.effectRT != null)
                            {
                                target.effectRT.transform.position = target.overrideEffectInfo ? target.effectSpawnInfo.spawnPos : target.rtTarget.transform.position;
                                target.effectRT.transform.rotation = target.overrideEffectInfo ? Quaternion.Euler(target.effectSpawnInfo.spawnRot) : target.rtTarget.transform.rotation;
                                target.effectRT.transform.localScale = target.overrideEffectInfo ? target.effectSpawnInfo.spawnScale : target.rtTarget.transform.localScale;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setup target trigger
        /// </summary>
        /// <param name="target"></param>
        private void SetupTargetTrigger(HSVTutorialObject tObj, HSVTarget target)
        {
            target.triggerState = PlayState.Idle;
            var config = target.triggerConfig;
            Component comp = null;
            if (config != null)
            {
                switch (config.triggerType)
                {
                    case TriggerType.Collider:
                        comp = config.colliderConfig.collider;
                        break;
                    case TriggerType.UI:
                        comp = config.graphicConfig.graphic;
                        break;
                    case TriggerType.KeyCode:
                        break;
                    case TriggerType.Manual:
                        break;
                }

                if (comp != null)
                {
                    var triggerObjs = comp.GetComponents<HSVTargetTriggerObject>();
                    foreach (var obj in triggerObjs)
                    {
                        DestroyImmediate(obj);
                    }

                    var triggerObj = comp.gameObject.AddComponent<HSVTargetTriggerObject>();
                    triggerObj.SetupTriggerConfig(config, target, tObj.stageIndex, tObj.index);
                }
            }

        }
        #endregion

        #region TutorialObject Function
        /// <summary>
        /// Check if the tutorial objects is finished by checking module state
        /// </summary>
        /// <param name="tObj"></param>
        /// <returns></returns>
        private bool CheckComplete(HSVTutorialObject tObj)
        {
            if (modules.ContainsKey(tObj))
            {
                for (int i = 0; i < modules[tObj].Count; i++)
                {
                    if (modules[tObj][i].state != PlayState.End)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Completes tutorial by setting tutorial state and clean up module spawned
        /// </summary>
        /// <param name="tObj"></param>
        private void TutorialComplete(HSVTutorialObject tObj)
        {
            SetState(ref tObj.state, PlayState.End);
            OnTObjComplete?.Invoke(tObj);
            if (RunEvent(tObj.AllEvents, HSVEventType.OnComplete))
            {
                DebugLog("Invoking 'OnComplete' event on tutorial: " + tObj.Name + ", Index: " + tObj.index);
            }

            ClearModules(tObj, true);
        }

        /// <summary>
        /// Resetting tutorial object states for the stage
        /// </summary>
        /// <param name="stage"></param>
        private void ResetTutorialObjectsState(HSVTutorialStage stage)
        {
            for (int i = 0; i < stage.tutorialObjects.Length; i++)
            {
                SetState(ref stage.tutorialObjects[i].state, PlayState.Idle);
            }
        }

        /// <summary>
        /// Check if tutorial object is at the end of stage
        /// </summary>
        /// <param name="tObj"></param>
        /// <returns></returns>
        private bool CheckStageEnd(HSVTutorialObject tObj)
        {
            if (currentPlayingStage != null)
            {
                if (tObj.index == currentPlayingStage.tutorialObjects.Length)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if stage end using tutorian object index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CheckStageEnd(int index)
        {
            if (currentPlayingStage != null)
            {
                if (index == currentPlayingStage.tutorialObjects.Length)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckStageStart(int index)
        {
            if (currentPlayingStage != null)
            {
                if (index == 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clean up the modules for tutorial object
        /// </summary>
        /// <param name="tObj"></param>
        private void ClearModules(HSVTutorialObject tObj, bool remove = false)
        {
            if (modules.ContainsKey(tObj) && modules[tObj] != null)
            {
                foreach (var module in modules[tObj])
                {
                    HSVObjectPool.Instance.SetEntityAsFree(module.transform);
                }

                modules[tObj].Clear();

                if (remove)
                {
                    modules.Remove(tObj);
                }
            }
        }

        /// <summary>
        /// Highlight tutorial object target
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="highlight"></param>
        public void HighlightTarget(HSVTutorialObject tObj, bool highlight)
        {
            if (highlight)
            {
                if (!outlineObjects.ContainsKey(tObj))
                {
                    outlineObjects.Add(tObj, new List<GameObject>());
                }

                if (outlineObjects[tObj] == null)
                {
                    outlineObjects[tObj] = new List<GameObject>();
                }

                for (int i = 0; i < tObj.focusTargets.Length; i++)
                {
                    if (tObj.focusTargets[i].highlightTarget && tObj.focusTargets[i].rtTarget != null)
                    {
                        outlineObjects[tObj].Add(tObj.focusTargets[i].rtTarget);
                    }
                }

                if (outlineObjects[tObj].Count > 0)
                {
                    if (outlineController != null)
                    {
                        outlineController.OutlineObjects(outlineObjects[tObj].ToArray());
                    }
                }
            }
            else
            {
                if (outlineObjects[tObj].Count > 0)
                {
                    if (outlineController != null)
                    {
                        outlineController.RemoveObjects(outlineObjects[tObj].ToArray());
                    }
                }

                outlineObjects[tObj].Clear();
            }
        }

        /// <summary>
        /// Check tutorial object name when spawning new tutorial objects
        /// </summary>
        /// <param name="stageObj"></param>
        /// <param name="objName"></param>
        /// <returns></returns>
        public bool CheckTutorialObjectName(HSVTutorialStage stageObj, string objName)
        {
            for (int i = 0; i < stageObj.tutorialObjects.Length; i++)
            {
                if (objName == stageObj.tutorialObjects[i].Name)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check tutorial object name when spawning new tutorial objects
        /// </summary>
        /// <param name="stageIndex"></param> this is stageObject.Index
        /// <param name="objName"></param>
        /// <returns></returns>
        public bool CheckCurrentStageTutorialObjectName(int stageIndex, string objName)
        {
            if (stageIndex <= stageObjects.Length && stageObjects.Length > 0)
            {
                var index = Mathf.Max(0, stageIndex - 1);
                if (stageObjects[index] != null)
                {
                    return CheckTutorialObjectName(stageObjects[index], objName);
                }
            }

            return false;
        }

        /// <summary>
        /// Getting tutorial object using stage and tutorial step index
        /// </summary>
        /// <param name="stageObj"></param>
        /// <param name="step"></param> this index is array position, it's tutorialObject.Index - 1
        /// <returns></returns>
        public HSVTutorialObject GetCurrentTutorialObject(HSVTutorialStage stageObj, int step)
        {
            if (step < stageObj.tutorialObjects.Length && stageObj.tutorialObjects.Length > 0)
            {
                return stageObj.tutorialObjects[Mathf.Max(0, step)];
            }

            return null;
        }

        /// <summary>
        /// Getting tutorial object using stage and tutorial name
        /// </summary>
        /// <param name="stageObj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public HSVTutorialObject GetCurrentTutorialObject(HSVTutorialStage stageObj, string name)
        {
            for (int i = 0; i < stageObj.tutorialObjects.Length; i++)
            {
                if (stageObj.tutorialObjects[i].Name == name)
                {
                    return stageObj.tutorialObjects[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Getting tutorial object using stageindex and tutorial index
        /// </summary>
        /// <param name="stageIndex"></param> this index is array position, it's stageObject.Index - 1
        /// <param name="step"></param> this index is array position, it's tutorialObject.Index - 1
        /// <returns></returns>
        public HSVTutorialObject GetCurrentTutorialObject(int stageIndex, int step)
        {
            var stageObj = GetCurrentStageObject(stageIndex);
            if (stageObj != null)
            {
                return GetCurrentTutorialObject(stageObj, step);
            }

            return null;
        }

        /// <summary>
        /// Getting stage object using stageindex
        /// </summary>
        /// <param name="stageStep"></param> this index is array position, it's stageObject.Index - 1
        /// <returns></returns>
        public HSVTutorialStage GetCurrentStageObject(int stageStep)
        {
            if (stageStep < stageObjects.Length && stageObjects.Length > 0)
            {
                return stageObjects[Mathf.Max(0, stageStep)];
            }

            return null;
        }

        /// <summary>
        /// Getting stage object using stage name
        /// </summary>
        /// <param name="stageName"></param>
        /// <returns></returns>
        public HSVTutorialStage GetCurrentStageObject(string stageName)
        {
            for (int i = 0; i < stageObjects.Length; i++)
            {
                if (stageObjects[i].stageName == stageName)
                {
                    return stageObjects[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Check stage object name when spawning new stage object
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public bool CheckStageObjectName(string objName)
        {
            for (int i = 0; i < stageObjects.Length; i++)
            {
                if (objName == stageObjects[i].stageName)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if all the tutorial objects are completed
        /// </summary>
        /// <returns></returns>
        public bool CheckAllTutorialObjectComplete()
        {
            if (currentPlayingStage == null)
                return true;

            foreach (var tObj in currentPlayingTObjects)
            {
                if (tObj.state != PlayState.End)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Clear stage trigger object
        /// </summary>
        /// <param name="stageObj"></param>
        /// <param name="startTrigger"></param>
        private void ClearStageTriggerConfig(HSVTutorialStage stageObj, bool startTrigger = true)
        {
            var config = startTrigger ? stageObj.startTrigger : stageObj.endTrigger;
            Transform configObj = null;

            switch (config.triggerType)
            {
                case TriggerType.Collider:
                    configObj = config.colliderConfig.collider.transform;
                    break;
                case TriggerType.UI:
                    configObj = config.graphicConfig.graphic.transform;
                    break;
            }

            if (configObj != null)
            {
                var triggerObjs = configObj.GetComponents<HSVTriggerObject>();
                for (int i = 0; i < triggerObjs.Length; i++)
                {
                    if (triggerObjs[i].IsStage && triggerObjs[i].IsStartTrigger == startTrigger)
                    {
                        Destroy(triggerObjs[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Clear tutorial object trigger object
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="startTrigger"></param>
        private void ClearTutorialTriggerConfig(HSVTutorialObject tObj, bool startTrigger = true)
        {
            var config = startTrigger ? tObj.startTrigger : tObj.endTrigger;
            Transform configObj = null;

            switch (config.triggerType)
            {
                case TriggerType.Collider:
                    configObj = config.colliderConfig.collider.transform;
                    break;
                case TriggerType.UI:
                    configObj = config.graphicConfig.graphic.transform;
                    break;
            }

            if (configObj != null)
            {
                var triggerObjs = configObj.GetComponents<HSVTriggerObject>();
                for (int i = 0; i < triggerObjs.Length; i++)
                {
                    if (!triggerObjs[i].IsStage && triggerObjs[i].IsStartTrigger == startTrigger)
                    {
                        Destroy(triggerObjs[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the trigger object event for stage, such as Key press
        /// </summary>
        /// <param name="stageObj"></param>
        /// <param name="startTrigger"></param>
        private void CheckEventTrigger(HSVTutorialStage stageObj, bool startTrigger = true)
        {
            var config = startTrigger ? stageObj.startTrigger : stageObj.endTrigger;
            if (config != null)
            {
                switch (config.triggerType)
                {
                    case TriggerType.KeyCode:
                        if (HSVInput.GetKeyDown(config.keyCode))
                        {
                            if (startTrigger)
                            {
                                PlayStage(stageObj.index);
                            }
                            else
                            {
                                StopStage();
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Check if the trigger object event for tutorial object, such as Key press
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="startTrigger"></param>
        private void CheckEventTrigger(HSVTutorialObject tObj, bool startTrigger = true)
        {
            var config = startTrigger ? tObj.startTrigger : tObj.endTrigger;
            if (config != null)
            {
                switch (config.triggerType)
                {
                    case TriggerType.KeyCode:
                        if (HSVInput.GetKeyDown(config.keyCode))
                        {
                            if (startTrigger)
                            {
                                PlayTutorial(tObj.stageIndex, tObj.index);
                            }
                            else
                            {
                                StopTutorial(tObj.stageIndex, tObj.index);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Invoke update event for stage object if enabled
        /// </summary>
        /// <param name="stageObj"></param>
        private void UpdateEvent(HSVTutorialStage stageObj)
        {
            //Check for update event in stage object
            if (stageObj.state != PlayState.Idle && stageObj.state != PlayState.End)
            {
                OnStageUpdate?.Invoke(stageObj);

                if (RunEvent(stageObj.AllEvents, HSVEventType.OnUpdate))
                {
                    DebugLog("Invoking 'OnUpdate' event on stage: " + stageObj.stageName + ", Index: " + stageObj.index);
                }
            }
        }

        /// <summary>
        /// Invoke update event for tutorial object if enabled
        /// </summary>
        /// <param name="tObj"></param>
        private void UpdateEvent(HSVTutorialObject tObj)
        {
            //Check for update event in stage object
            if (tObj.state != PlayState.Idle && tObj.state != PlayState.End)
            {
                OnTObjUpdate?.Invoke(tObj);

                if (RunEvent(tObj.AllEvents, HSVEventType.OnUpdate))
                {
                    DebugLog("Invoking 'OnUpdate' event on stage: " + tObj.Name + ", Index: " + tObj.index);
                }
            }
        }

        /// <summary>
        /// find and run event using event type
        /// </summary>
        /// <param name="events"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool RunEvent(Dictionary<HSVEventType, HSVEvent> events, HSVEventType type)
        {
            var curEvent = HSVEvent.GetEvent(events, type);
            if (curEvent != null && curEvent.enable)
            {
                curEvent.subEvent?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        /// find and run event using state
        /// </summary>
        /// <param name="events"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool RunEvent(Dictionary<HSVEventType, HSVEvent> events, PlayState state)
        {
            var curEvent = HSVEvent.GetEvent(events, state);
            if (curEvent != null && curEvent.enable)
            {
                curEvent.subEvent?.Invoke();
                return true;
            }
            return false;
        }

        public void OnTextDisplayChange(HSVTutorialModule module, HSVUIMask mask)
        {
            OnDisplayTextChange?.Invoke(module, mask);
        }

        /// <summary>
        /// Check if main camera state change, this is used to save performance, so the update do not run every frame
        /// </summary>
        private Matrix4x4 cameraToWorldMatrix;
        public bool CheckCameraStateChange()
        {
            if (Camera != null)
            {
                if (cameraToWorldMatrix != Camera.cameraToWorldMatrix)
                {
                    cameraToWorldMatrix = Camera.cameraToWorldMatrix;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the state, and sets state change for the editor
        /// </summary>
        /// <param name="state"></param>
        /// <param name="value"></param>
        public void SetState(ref PlayState state, PlayState value)
        {
#if UNITY_EDITOR
            if (state != value)
            {
                stateChange = true;
            }
#endif
            state = value;
        }

        /// <summary>
        /// Resets canvas
        /// </summary>
        private void ResetCanvas()
        {
            var onModules = Canvas.GetComponentsInChildren<HSVTutorialModule>(true);
            for (int i = 0; i < onModules.Length; i++)
            {
                UninitializeTargetInfo(onModules[i].m_currentTObject);
                DestroyImmediate(onModules[i].gameObject);
            }
        }

        /// <summary>
        /// Reset Effect Prefab
        /// </summary>
        private void ResetEffectPrefab()
        {
            if (previewEffect != null)
            {
                while (previewEffect.childCount > 0)
                {
                    DestroyImmediate(previewEffect.GetChild(0).gameObject);
                }
            }
        }

        private void ResetWorldObject()
        {
            if (WorldObjectContainer != null)
            {
                while (WorldObjectContainer.childCount > 0)
                {
                    DestroyImmediate(WorldObjectContainer.GetChild(0).gameObject);
                }
            }
        }

        #region Target Trigger Section
        /// <summary>
        /// Triggers specified target
        /// </summary>
        /// <param name="stageIndex"></param>
        /// <param name="tObjIndex"></param>
        /// <param name="target"></param>
        public void TriggerTarget(int stageIndex, int tObjIndex, HSVTarget target, HSVTargetTriggerObject triggerObj)
        {
            var tObj = GetCurrentTutorialObject(stageIndex - 1, tObjIndex - 1);
            if (tObj != null && currentPlayingTObjects.Contains(tObj))
            {
                if (target != null)
                {
                    target.triggerState = PlayState.End;
                    triggerObj.enabled = false;
                    if (target.effectRT != null)
                    {
                        HSVObjectPool.Instance.SetEntityAsFree(target.effectRT.transform);
                        target.effectRT = null;
                    }

                    if (outlineObjects.ContainsKey(tObj) && target.rtTarget != null && target.rtTarget.activeInHierarchy)
                    {
                        if (outlineObjects[tObj].Contains(target.rtTarget))
                        {
                            if (outlineController != null)
                            {
                                outlineController.RemoveObjects(new GameObject[] { target.rtTarget });
                            }
                            outlineObjects[tObj].Remove(target.rtTarget);
                        }
                    }

                    if (CheckTargetTriggerComplete(tObj))
                    {
                        StopTutorial(tObj.index);
                    }
                }
            }
            else
            {
                DebugLogWarning("Tutorial object: " + tObjIndex + " in stage: " + stageIndex + " is not playing, please check");
            }
        }

        private bool CheckTargetTriggerComplete(HSVTutorialObject tObj)
        {
            for (int i = 0; i < tObj.focusTargets.Length; i++)
            {
                if (tObj.focusTargets[i].useTrigger && tObj.focusTargets[i].triggerState != PlayState.End)
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearTargetTrigger(HSVTarget target)
        {
            if (target.rtTarget != null)
            {
                Transform configObj = null;
                if (target.triggerConfig != null)
                {
                    switch (target.triggerConfig.triggerType)
                    {
                        case TriggerType.Collider:
                            configObj = target.triggerConfig.colliderConfig.collider.transform;
                            break;
                        case TriggerType.UI:
                            configObj = target.triggerConfig.graphicConfig.graphic.transform;
                            break;
                    }
                }

                if (configObj != null)
                {
                    var triggerObjs = configObj.GetComponents<HSVTargetTriggerObject>();
                    foreach (var obj in triggerObjs)
                    {
                        Destroy(obj);
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Static Helper Function
        public static void DebugLog(string message)
        {
            if (Instance != null && Instance.debugMode)
            {
                Debug.Log(message);
            }
        }

        public static void DebugLogWarning(string message)
        {
            if (Instance != null && Instance.debugMode)
            {
                Debug.LogWarning(message);
            }
        }

        public static List<Type> GetNonAbstractTypesSubclassOf<T>(bool sorted = true) where T : class
        {
            Type parentType = typeof(T);
            Assembly assembly = Assembly.GetAssembly(parentType);

            List<Type> types = assembly.GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(parentType)).ToList();

            if (sorted)
                types.Sort(CompareTypesNames);

            return types;
        }

        private static int CompareTypesNames(Type a, Type b)
        {
            return a.Name.CompareTo(b.Name);
        }
        #endregion

        #region Instance Helper Function
        public string[] GetModuleConfigTypeNames()
        {
            var typeNames = new string[ModuleConfigTypes.Count];
            for (int i = 0; i < ModuleConfigTypes.Count; i++)
            {
                typeNames[i] = ModuleConfigTypes[i].Name;
            }

            return typeNames;
        }

        public static T JsonCopy<T>(T obj)
        {
            return JsonUtility.FromJson<T>(JsonUtility.ToJson(obj));
        }
        #endregion

        #region Editor Functions
#if UNITY_EDITOR
        public List<HSVTutorialObject> tmEditorList = new List<HSVTutorialObject>();

        public void UpdateTMEditorList(int page, int pageCount, int selectIndex)
        {
            //Debug.Log("Update TM Editor List");
            if (tmEditorList == null)
                tmEditorList = new List<HSVTutorialObject>();

            tmEditorList.Clear();

            var stageObj = GetCurrentStageObject(selectIndex);

            if (stageObj != null)
            {
                UpdateElementIndex(stageObj.index);
                var startIdx = (page - 1) * pageCount;
                for (int i = startIdx; i < startIdx + pageCount; i++)
                {
                    if (i >= stageObj.tutorialObjects.Length)
                    {
                        return;
                    }

                    tmEditorList.Add(stageObj.tutorialObjects[i]);
                }
            }
        }

        public void InsertElementForTMObject(int index, HSVTutorialObject obj, int selectIndex)
        {
            var stageObj = GetCurrentStageObject(selectIndex);

            if (stageObj != null)
            {
                var newArray = new HSVTutorialObject[stageObj.tutorialObjects.Length + 1];
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i < index)
                    {
                        newArray[i] = stageObj.tutorialObjects[i];
                    }
                    else if (i == index)
                    {
                        newArray[i] = obj;
                    }
                    else
                    {
                        newArray[i] = stageObj.tutorialObjects[i - 1];
                    }
                    newArray[i].index = i + 1;
                }

                stageObj.tutorialObjects = newArray;
            }
        }

        public void DeleteElementAtIndex(int index, int selectIndex)
        {

        }

        public void InsertStageElement(int index, HSVTutorialStage obj)
        {
            var newArray = new HSVTutorialStage[stageObjects.Length + 1];
            for (int i = 0; i < newArray.Length; i++)
            {
                if (i < index)
                {
                    newArray[i] = stageObjects[i];
                }
                else if (i == index)
                {
                    newArray[i] = obj;
                }
                else
                {
                    newArray[i] = stageObjects[i - 1];
                }
                newArray[i].index = i + 1;
            }

            stageObjects = newArray;
        }

        public void UpdateElementIndex(int selectIndex)
        {
            var stageObj = GetCurrentStageObject(selectIndex);
            if (stageObj != null)
            {
                for (int i = 0; i < stageObj.tutorialObjects.Length; i++)
                {
                    if (stageObj.tutorialObjects[i].index != i + 1)
                    {
                        stageObj.tutorialObjects[i].index = i + 1;
                    }
                    if (stageObj.tutorialObjects[i].stageIndex != selectIndex + 1)
                    {
                        stageObj.tutorialObjects[i].stageIndex = selectIndex + 1;
                    }
                }
            }
        }

        public void PreviewTutorialObject(int stageIndex, int tObjIndex, bool previewObject = true)
        {
            if (Application.isPlaying)
                return;

            ResetCanvas();
            ResetEffectPrefab();
            ResetWorldObject();

            if (previewObject)
            {
                var tObj = GetCurrentTutorialObject(stageIndex - 1, tObjIndex - 1);
                if (tObj != null)
                {
                    InitializeTargetInfo(tObj);
                    PreviewTargetEffect(tObj);
                    PreviewModule(tObj);
                }
            }
        }

        private void PreviewTargetEffect(HSVTutorialObject tObj)
        {
            if (previewEffect == null)
            {
                previewEffect = transform.Find("PreviewEffect");
                if (!Application.isPlaying)
                {
                    if (previewEffect == null)
                    {
                        previewEffect = new GameObject("PreviewEffect").transform;
                        previewEffect.SetParent(this.transform, false);
                    }
                }
            }

            for (int i = 0; i < tObj.focusTargets.Length; i++)
            {
                var target = tObj.focusTargets[i];
                if (target.target != null && target.spawnEffect)
                {
                    var effectRT = HSVObjectPool.Instance.GetFreeEntity(target.effectPrefab);
                    if (effectRT != null)
                    {
                        effectRT.transform.position = target.overrideEffectInfo ? target.effectSpawnInfo.spawnPos : target.target.transform.position;
                        effectRT.transform.rotation = target.overrideEffectInfo ? Quaternion.Euler(target.effectSpawnInfo.spawnRot) : target.target.transform.rotation;
                        effectRT.transform.localScale = target.overrideEffectInfo ? target.effectSpawnInfo.spawnScale : target.target.transform.localScale;

                        effectRT.transform.SetParent(previewEffect, true);
                    }
                }
            }
        }

        private void PreviewModule(HSVTutorialObject tObj)
        {
            for (int i = 0; i < tObj.moduleConfigs.Length; i++)
            {
                var mod = HSVObjectPool.Instance.GetFreeEntity(tObj.moduleConfigs[i].modulePrefab).GetComponent<HSVTutorialModule>();
                if (mod != null)
                {
                    //set mod to the child of Canvas
                    mod.config = tObj.moduleConfigs[i];
                    mod.transform.SetParent(Canvas.transform, false);
                    mod.transform.localPosition = Vector3.zero;
                    mod.transform.localRotation = Quaternion.identity;
                    mod.transform.localScale = Vector3.one;
                    (mod.transform as RectTransform).sizeDelta = Vector2.zero;
                    mod.PreviewModule(tObj);
                }
                else
                {
                    DebugLogWarning("Could not find the module prefab in the tutorial object configs :" + tObj.Name);
                }
            }
        }
#endif
        #endregion
    }
}