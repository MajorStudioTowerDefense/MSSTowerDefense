using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HSVStudio.Tutorial.TMEditor
{
    public class HSVTutorialManagerCreator
    {
        [MenuItem("Tools/HSVStudio/TutorialManager/Setup Tutorial Manager")]
        public static void SetupTutorialManagerOnScene()
        {
            var instance = HSVTutorialManager.Instance;

            if(instance == null)
            {
                GameObject tutorialManager = new GameObject("HSVTutorialManager");
                instance = tutorialManager.AddComponent<HSVTutorialManager>();
            }

            instance.Setup();

            Selection.activeGameObject = instance.gameObject;
        }

        [MenuItem("Tools/HSVStudio/TutorialManager/Clear Scene Reference")]
        public static void ClearSceneReference()
        {
            var referenceObjs = GameObject.FindObjectsOfType<HSVExposeReferenceMgr>(true);
            for(int i = 0; i < referenceObjs.Length; i++)
            {
                GameObject.DestroyImmediate(referenceObjs[i].gameObject);
            }
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}