using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bullets;

namespace Bullets
{
    [CustomEditor(typeof(Petal))]
    public class PetalEditor : BulletBaseEditor
    {

        protected SerializedProperty PetalColors;

        public override void OnEnable()
        {
            base.OnEnable();
            PetalColors = serializedObject.FindProperty("PetalColors");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(PetalColors);
            EditorGUILayout.LabelField("Petal Specific");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
