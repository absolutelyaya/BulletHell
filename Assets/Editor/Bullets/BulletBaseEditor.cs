using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bullets
{
    [CustomEditor(typeof(BulletBase))]
    public class BulletBaseEditor : Editor
    {

        protected SerializedProperty Type;
        protected SerializedProperty Speed;
        protected SerializedProperty Moves;
        protected SerializedProperty HasLiveTime;
        protected SerializedProperty StartLiveTime;
        protected SerializedProperty OffScreenBehaviour;
        protected SerializedProperty StartBounces;

        public virtual void OnEnable()
        {
            Type = serializedObject.FindProperty("Type");
            Speed = serializedObject.FindProperty("Speed");
            Moves = serializedObject.FindProperty("Moves");
            HasLiveTime = serializedObject.FindProperty("HasLiveTime");
            StartLiveTime = serializedObject.FindProperty("StartLiveTime");
            OffScreenBehaviour = serializedObject.FindProperty("OffScreenBehaviour");
            StartBounces = serializedObject.FindProperty("StartBounces");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(Type);
            EditorGUILayout.PropertyField(Speed);
            EditorGUILayout.PropertyField(Moves);
            EditorGUILayout.PropertyField(HasLiveTime);
            if (HasLiveTime.boolValue)
                EditorGUILayout.PropertyField(StartLiveTime);
            EditorGUILayout.PropertyField(OffScreenBehaviour);
            if (OffScreenBehaviour.enumValueIndex == (int)Bullets.OffScreenBehaviour.Reflect)
                EditorGUILayout.PropertyField(StartBounces);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
