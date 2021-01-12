using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EnemyNavigation
{
    public class PathCreator : MonoBehaviour
    {
        [HideInInspector]
        public Path Path;
        [HideInInspector]
        public string Name;

        public void CreatePath()
        {
            Path = new Path(transform.position);
        }

        public void SavePath()
        {
            PathScriptableObject obj = ScriptableObject.CreateInstance<PathScriptableObject>();
            obj.Path = Path;

            string path = "Assets/ScriptableObjects/EnemyPaths/" + Name + ".asset";
            AssetDatabase.CreateAsset(obj, path);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = obj;
        }
    }
}
