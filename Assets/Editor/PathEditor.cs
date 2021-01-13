using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EnemyNavigation
{
    [CustomEditor(typeof(PathCreator))]
    public class PathEditor : Editor
    {
        PathCreator creator;
        Path path;

        protected SerializedProperty Name;

        private void OnSceneGUI()
        {
            Input();
            Draw();
        }

        void Input()
        {
            Event guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                Undo.RecordObject(creator, "Add Segment");
                path.AddSegment(mousePos);
            }
        }

        void Draw()
        {
            for (int i = 0; i < path.SegmentCount; i++)
            {
                Vector2[] points = path.GetPointsInSegment(i);
                Handles.color = Color.white;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
                Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
            }

            Handles.color = Color.red;
            for (int i = 0; i < path.PointCount; i++)
            {
                Vector2 newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, 0.1f, Vector2.zero, Handles.CylinderHandleCap);
                if (path[i] != newPos)
                {
                    Undo.RecordObject(creator, "Move point");
                    path.MovePoint(i, newPos);
                }
            }
        }

        private void OnEnable()
        {
            creator = (PathCreator)target;
            if (creator.Path == null)
            {
                creator.CreatePath();
            }
            path = creator.Path;

            Name = serializedObject.FindProperty("Name");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(Name);
            if (GUILayout.Button("Center Curve"))
                path.TranslatePath(creator.transform.position);
            if (Name.stringValue == string.Empty)
                EditorGUILayout.HelpBox("You have to enter a name!", MessageType.Error);
            if (AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/EnemyPaths/" + Name.stringValue + ".asset", typeof(PathScriptableObject)))
            {
                EditorGUILayout.HelpBox($"A file with the name '{Name.stringValue}' exists already!", MessageType.Warning);
                if (GUILayout.Button("Load Path"))
                {
                    if (Name.stringValue != string.Empty)
                    {
                        creator.LoadPath(Name.stringValue);
                    }
                }
            }
            if (GUILayout.Button("Save Path to ScriptableObject"))
            {
                if (Name.stringValue != string.Empty)
                {
                    creator.SavePath();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
