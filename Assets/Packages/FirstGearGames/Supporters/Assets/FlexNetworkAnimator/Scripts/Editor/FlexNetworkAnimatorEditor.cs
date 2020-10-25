#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.FlexNetworkAnimators.Editors
{

    [CustomEditor(typeof(FlexNetworkAnimator), true)]
    [CanEditMultipleObjects]
    public class FlexNetworkAnimatorEditor : Editor
    {
        private SerializedProperty _animator;
        private SerializedProperty _clientAuthoritative;
        private SerializedProperty _synchronizeToOwner;
        private SerializedProperty _smoothFloats;
        private SerializedProperty _interpolationFallbehind;

        protected virtual void OnEnable()
        {
            _animator = serializedObject.FindProperty("_animator");

            _clientAuthoritative = serializedObject.FindProperty("_clientAuthoritative");
            _synchronizeToOwner = serializedObject.FindProperty("_synchronizeToOwner");

            _smoothFloats = serializedObject.FindProperty("_smoothFloats");
            _interpolationFallbehind = serializedObject.FindProperty("_interpolationFallbehind");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((FlexNetworkAnimator)target), typeof(FlexNetworkAnimator), false);
            GUI.enabled = true;

            //Animator
            EditorGUILayout.LabelField("Animator", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_animator, new GUIContent("Animator", "The animator component to synchronize."));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            //Authority.
            EditorGUILayout.LabelField("Authority", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_clientAuthoritative, new GUIContent("Client Authoritative", "True if using client authoritative movement."));
            if (_clientAuthoritative.boolValue == false)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_synchronizeToOwner, new GUIContent("Synchronize To Owner", "True to synchronize server results back to owner. Typically used when you are sending inputs to the server and are relying on the server response to move the transform."));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            //Synchronization Processing.
            EditorGUILayout.LabelField("Synchronization Processing", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_smoothFloats, new GUIContent("Smooth Floats", "True to smooth floats on spectators rather than snap to their values immediately. Commonly set to true for smooth blend tree animations."));
            if (_smoothFloats.boolValue == true)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_interpolationFallbehind, new GUIContent("Interpolation Fallbehind", "How much time to fall behind when using smoothing. Only increase value if the smoothing is sometimes jittery. Recommended values are between 0 and 0.04."));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            //Sync settings.
            EditorGUILayout.LabelField("Sync Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("syncMode"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("syncInterval"));
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif