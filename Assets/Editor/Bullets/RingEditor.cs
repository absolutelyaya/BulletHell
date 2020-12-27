using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bullets
{
    [CustomEditor(typeof(Ring))]
    public class RingEditor : BulletBaseEditor
    {

        protected SerializedProperty Bullet;
        protected SerializedProperty Elements;
        protected SerializedProperty SideBob;
        protected SerializedProperty RotationSpeed;
        protected SerializedProperty Distance;
        protected SerializedProperty RandomizedValues;

        public override void OnEnable()
        {
            base.OnEnable();
            Bullet = serializedObject.FindProperty("Bullet");
            Elements = serializedObject.FindProperty("Elements");
            SideBob = serializedObject.FindProperty("SideBob");
            RotationSpeed = serializedObject.FindProperty("RotationSpeed");
            Distance = serializedObject.FindProperty("Distance");
            RandomizedValues = serializedObject.FindProperty("RandomizedValues");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(Type);
            EditorGUILayout.PropertyField(Moves);
            EditorGUILayout.PropertyField(HasLiveTime);
            if (HasLiveTime.boolValue)
                EditorGUILayout.PropertyField(StartLiveTime);
            EditorGUILayout.PropertyField(OffScreenBehaviour);
            if (OffScreenBehaviour.enumValueIndex == (int)Bullets.OffScreenBehaviour.Reflect)
                EditorGUILayout.PropertyField(StartBounces);
            EditorGUILayout.LabelField("Ring Specific");
            EditorGUILayout.PropertyField(Bullet);
            EditorGUILayout.PropertyField(Elements);
            EditorGUILayout.PropertyField(Distance);
            EditorGUILayout.PropertyField(RandomizedValues);
            if (!RandomizedValues.boolValue)
            {
                EditorGUILayout.PropertyField(RotationSpeed);
                EditorGUILayout.PropertyField(SideBob);
                EditorGUILayout.PropertyField(Speed);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}