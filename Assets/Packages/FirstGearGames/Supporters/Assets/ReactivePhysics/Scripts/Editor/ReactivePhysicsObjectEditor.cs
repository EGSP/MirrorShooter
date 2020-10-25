#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.ReactivePhyics.Editors
{

    [CustomEditor(typeof(ReactivePhysicsObject), true)]
    public class ReactivePhysicsObjectEditor : Editor
    {
        private SerializedProperty _useLocalSpace;

        private SerializedProperty _intervalType;
        private SerializedProperty _synchronizeInterval;

        private SerializedProperty _reliable;
        private SerializedProperty _preciseSynchronization;

        private SerializedProperty _objectType;
        private SerializedProperty _strength;

        protected virtual void OnEnable()
        {
            _useLocalSpace = serializedObject.FindProperty("_useLocalSpace");

            _intervalType = serializedObject.FindProperty("_intervalType");
            _synchronizeInterval = serializedObject.FindProperty("_synchronizeInterval");

            _reliable = serializedObject.FindProperty("_reliable");
            _preciseSynchronization = serializedObject.FindProperty("_preciseSynchronization");
            _objectType = serializedObject.FindProperty("_objectType");
            _strength = serializedObject.FindProperty("_strength");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            //Transform.
            EditorGUILayout.LabelField("Space");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_useLocalSpace, new GUIContent("Use LocalSpace", "True to synchronize using localSpace rather than worldSpace. If you are to child this object throughout it's lifespan using worldspace is recommended. However, when using worldspace synchronization may not behave properly on VR. LocalSpace is the default."));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            //Timing.
            EditorGUILayout.LabelField("Timing");
            EditorGUI.indentLevel++;

            //If not reliable and interval type is set to interval.
            if (!_reliable.boolValue && _intervalType.intValue == 0)
                EditorGUILayout.HelpBox("For best results use FixedUpdate Interval Type when not using Reliable messages.", MessageType.Warning);
            EditorGUILayout.PropertyField(_intervalType, new GUIContent("Interval Type", "How to operate synchronization timings. Timed will synchronized every specified interval while FixedUpdate will synchronize every FixedUpdate."));
            if (_intervalType.intValue == 0)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_synchronizeInterval, new GUIContent("Synchronize Interval", "How often to synchronize this transform."));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            //Synchronization Processing.
            EditorGUILayout.LabelField("Synchronization Processing");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_reliable, new GUIContent("Reliable", "True to synchronize using the reliable channel. False to synchronize using the unreliable channel. Your project must use 0 as reliable, and 1 as unreliable for this to function properly. This feature is not supported on TCP transports."));
            EditorGUILayout.PropertyField(_preciseSynchronization, new GUIContent("Precise Synchronization", "True to synchronize data anytime it has changed. False to allow greater differences before synchronizing. Given that rigidbodies often shift continuously it's recommended to leave this false to not flood the network."));
            EditorGUILayout.PropertyField(_strength, new GUIContent("Strength", "How strictly to synchronize this object when owner. Lower values will still keep the object in synchronization but it may take marginally longer for the object to correct if out of synchronization. It's recommended to use higher values, such as 0.5f, when using fast intervals. Default value is 0.1f."));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            //Object Setup
            EditorGUILayout.LabelField("Object Setup");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_objectType, new GUIContent("Object Type", "Indicates if this is a 2D or 3D object."));
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif