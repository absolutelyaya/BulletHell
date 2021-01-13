using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EnemyNavigation;
using Bullets;

public class SpawningSystem : MonoBehaviour
{
    [HideInInspector]
    public bool Previewing;
    [HideInInspector]
    public List<int> Errors;
    [HideInInspector]
    public List<int> Warnings;
    public List<SpawnEntry> Level = new List<SpawnEntry>();

    Queue<SpawnEntry> levelStack = new Queue<SpawnEntry>();

    private void OnDrawGizmosSelected()
    {
        Errors.Clear();
        Warnings.Clear();
        Previewing = false;
        int entry = 0;
        foreach (var item in Level)
        {
            if (item.Type.Equals(SpawnEntry.EntryType.Enemy) && item.Path != null && item.Previewing)
            {
                Vector2[] points = item.Path.Path.CalculateEvenlySpacedPoints(0.5f);
                for (int i = 1; i < points.Length; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(item.Position, 0.5f);
                    yayasGizmos.DrawArrow(
                        item.Position + Vector2.Scale(points[i - 1], item.FlipPath ? new Vector2(-1, 1) : Vector2.one),
                        item.Position + Vector2.Scale(points[i], item.FlipPath ? new Vector2(-1, 1) : Vector2.one),
                        25f, 0.35f);
                }
                Previewing = true;
            }
            if (item.Type.Equals(SpawnEntry.EntryType.Bullet) && item.Previewing)
            {
                Gizmos.color = Color.yellow;
                switch(item.Entity.GetComponent<BulletBase>().Type)
                {
                    case BulletType.Base: 
                    case BulletType.Petal:
                        Gizmos.DrawWireSphere(item.Position, 0.25f);
                        break;
                    case BulletType.Ring:
                        yayasGizmos.DrawRing(item.Position, item.RingElements, item.RingDistance);
                        break;
                }
                yayasGizmos.DrawArrow(item.Position, (Vector3)item.Position + Quaternion.AngleAxis(item.Rotation + 180, Vector3.forward)
                    * transform.up * 2f);
                Previewing = true;
            }
            if (item.hasErrors) Errors.Add(entry);
            if (item.hasWarnings) Warnings.Add(entry);
            entry++;
        }
    }

    void Start()
    {
        foreach (var item in Level)
        {
            levelStack.Enqueue(item);
        }
        StartCoroutine(SpawnLevel());
    }

    IEnumerator SpawnLevel()
    {
        SpawnEntry curEntry;
        while(levelStack.Count > 0)
        {
            curEntry = levelStack.Dequeue();
            yield return new WaitForSeconds(curEntry.Delay);
            switch(curEntry.Type)
            {
                case SpawnEntry.EntryType.Enemy:
                    EnemyBase enemy = PoolManager.current.Activate(curEntry.Entity, curEntry.Position, 
                        Quaternion.Euler(0, 0, curEntry.Rotation)).GetComponent<EnemyBase>();
                    enemy.Path = curEntry.Path;
                    enemy.InvertPath = curEntry.FlipPath;
                    enemy.Speed = curEntry.SpeedConst;
                    break;
                case SpawnEntry.EntryType.Bullet:
                    BulletBase bullet = PoolManager.current.Activate(curEntry.Entity, curEntry.Position, 
                        Quaternion.Euler(0, 0, curEntry.Rotation)).GetComponent<BulletBase>();
                    switch(bullet.Type)
                    {
                        case BulletType.Base:
                        case BulletType.Petal:
                            bullet.Speed = UnityEngine.Random.Range(curEntry.SpeedRange.x, curEntry.SpeedRange.y);
                            bullet.OffScreenBehaviour = curEntry.OffScrBhv;
                            bullet.StartBounces = curEntry.Bounces;
                            break;
                        case BulletType.Ring:
                            bullet.Speed = UnityEngine.Random.Range(curEntry.SpeedRange.x, curEntry.SpeedRange.y);
                            Ring ring = bullet.gameObject.GetComponent<Ring>();
                            ring.Bullet = curEntry.RingBullets;
                            ring.Elements = curEntry.RingElements;
                            ring.Distance = curEntry.RingDistance;
                            break;
                    }
                    break;
            }
        }
    }
}

[Serializable]
public class SpawnEntry
{
    public SpawnEntry() {}

    /// <summary>
    /// Copy
    /// </summary>
    public SpawnEntry(SpawnEntry original)
    {
        Type = original.Type;
        Previewing = original.Previewing;
        Entity = original.Entity;
        Delay = original.Delay;
        Position = original.Position;
        Rotation = original.Rotation;
        Path = original.Path;
        FlipPath = original.FlipPath;
        SpeedConst = original.SpeedConst;
        OffScrBhv = original.OffScrBhv;
        Bounces = original.Bounces;
    }

    /// <summary>
    /// Enemy
    /// </summary>
    public SpawnEntry(EntryType type, bool previewing, GameObject entity, float delay, Vector2 position, float rotation, float speed, PathScriptableObject path,
        bool flipPath)
    {
        Type = type;
        Previewing = previewing;
        Entity = entity;
        Delay = delay;
        Position = position;
        Rotation = rotation;
        Path = path;
        FlipPath = flipPath;
        SpeedConst = speed;
    }

    /// <summary>
    /// Bullet
    /// </summary>
    public SpawnEntry(EntryType type, bool previewing, GameObject entity, float delay, Vector2 position, float rotation, float speed,
        OffScreenBehaviour offScrBhv, int bounces)
    {
        Type = type;
        Previewing = previewing;
        Entity = entity;
        Delay = delay;
        Position = position;
        Rotation = rotation;
        SpeedConst = speed;
        OffScrBhv = offScrBhv;
        Bounces = bounces;
    }

    //ListControl
    public Actions PendingAction;
    public float SpawnTime;
    public bool Expanded = true;
    public EntryType Type;
    public bool Previewing;
    public GameObject Entity;
    public float Delay;
    public Vector2 Position;
    public float Rotation;
    public bool SpecificsExpanded;
    //EnemySpecific
        public float SpeedConst;
        public PathScriptableObject Path;
        public bool FlipPath;
    //BulletSpecifics
        public Vector2 SpeedRange;
        public OffScreenBehaviour OffScrBhv;
        public int Bounces;
        //Ring
            public int RingElements;
            public float RingDistance;
            public GameObject RingBullets;

    public bool hasErrors;
    public bool hasWarnings;

    public enum EntryType
    {
        Enemy,
        Bullet
    }

    public enum Actions
    {
        None,
        MoveUp,
        MoveDown,
        Delete,
        Clone
    }
}

[CustomEditor(typeof(SpawningSystem))]
public class SpawningSystemInspector : Editor
{

    SpawningSystem script;

    void OnEnable()
    {
        script = (SpawningSystem)target;
    }

    public override void OnInspectorGUI()
    {
        if (script.Previewing) EditorGUILayout.HelpBox("Currently Previewing Objects", MessageType.Info);
        if (script.Errors.Count > 0)
        {
            foreach (var element in script.Errors)
            {
                EditorGUILayout.HelpBox($"Unresolved Error at Element {element}!", MessageType.Error);
            }
        }
        if (script.Warnings.Count > 0)
        {
            foreach (var element in script.Warnings)
            {
                EditorGUILayout.HelpBox($"Warning at Element {element}!", MessageType.Warning);
            }
        }
        DrawDefaultInspector();
        if(GUILayout.Button("Add Entry"))
        {
            script.Level.Add(new SpawnEntry());
        }
        if (GUILayout.Button("Remove Entry"))
        {
            script.Level.RemoveAt(script.Level.Count - 1);
        }

        for (int i = 0; i < script.Level.Count; i++)
        {
            if (i != 0)
            {
                script.Level[i].SpawnTime = script.Level[i - 1].SpawnTime + script.Level[i].Delay;
            }
            else script.Level[i].SpawnTime = 0;
            switch(script.Level[i].PendingAction)
            {
                case SpawnEntry.Actions.MoveUp:
                    if (i != 0)
                    {
                        Debug.Log(i - 1);
                        SpawnEntry tmp = script.Level[i];
                        script.Level[i] = script.Level[i - 1];
                        script.Level[i - 1] = tmp;
                        script.Level[i - 1].PendingAction = SpawnEntry.Actions.None;
                        return;
                    }
                    break;
                case SpawnEntry.Actions.MoveDown:
                    if (i != script.Level.Count - 1)
                    {
                        Debug.Log(i + 1);
                        SpawnEntry tmp = script.Level[i];
                        script.Level[i] = script.Level[i + 1];
                        script.Level[i + 1] = tmp;
                        script.Level[i + 1].PendingAction = SpawnEntry.Actions.None;
                        return;
                    }
                    break;
                case SpawnEntry.Actions.Delete:
                    script.Level.RemoveAt(i);
                    script.Level[i].PendingAction = SpawnEntry.Actions.None;
                    break;
                case SpawnEntry.Actions.Clone:
                    script.Level.Insert(i + 1, new SpawnEntry(script.Level[i]));
                    script.Level[i].PendingAction = SpawnEntry.Actions.None;
                    break;
            }
        }
    }
}