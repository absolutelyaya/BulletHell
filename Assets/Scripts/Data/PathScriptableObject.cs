using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyNavigation
{
    [CreateAssetMenu(fileName = "Path", menuName = "ScriptableObjects/Path")]
    public class PathScriptableObject : ScriptableObject
    {
        public Path Path;

        PathScriptableObject(Path path)
        {
            Path = path;
        }
    }
}
