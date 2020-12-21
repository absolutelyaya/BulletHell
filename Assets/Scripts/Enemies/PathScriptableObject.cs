using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyNavigation
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Path", order = 1)]
    public class PathScriptableObject : ScriptableObject
    {
        public List<PathWayPoint> Points = new List<PathWayPoint>();


    }

    [Serializable]
    public struct PathWayPoint
    {
        public Vector2 Location;
        [Tooltip("How long to stay at this point.")]
        public float Delay;
    }
}