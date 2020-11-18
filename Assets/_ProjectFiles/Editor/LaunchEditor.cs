#if UNITY_EDITOR

using System;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Project.Editor
{
    [InitializeOnLoad]
    public class LaunchEditor : EditorWindow
    {

        static LaunchEditor()
        {
            EditorApplication.playModeStateChanged += HandleStateChange;
        }

        [SerializeField] public string lastOpenedScene;


        private static void HandleStateChange(PlayModeStateChange playModeStateChange)
        {
            // Debug.Log(playModeStateChange);
            if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                var window = GetWindow<LaunchEditor>();
                
                if (SceneView.sceneViews.Count > 0)
                {
                    SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                    sceneView.Focus();
                }

                if (window.lastOpenedScene.IsNullOrWhitespace())
                    return;
                
                // Debug.Log(_lastOpenedScene);
                EditorSceneManager.OpenScene(window.lastOpenedScene);
                window.lastOpenedScene = null;
            }
        }

        [MenuItem("Launch/Play")]
        public static void PlayFromPrelaunchScene()
        {
            if ( EditorApplication.isPlaying == true )
            {
                EditorApplication.isPlaying = false;
                return;
            }

            var window = GetWindow<LaunchEditor>();
            
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length > 0)
            {
                window.lastOpenedScene = EditorSceneManager.GetActiveScene().path;
                // Debug.Log(_lastOpenedScene);
                var preload = scenes[0].path;
                
                EditorApplication.SaveCurrentSceneIfUserWantsTo();
                EditorSceneManager.OpenScene(preload);
                EditorApplication.isPlaying = true;    
            }
        }
    }

}


#endif