﻿using System;
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
                for (int i = 1; i < item.Path.Points.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(item.Position, 0.5f);
                    yayasGizmos.DrawArrow(
                        item.Position + Vector2.Scale(item.Path.Points[i - 1].Location, item.FlipPath ? new Vector2(-1, 1) : Vector2.one),
                        item.Position + Vector2.Scale(item.Path.Points[i].Location, item.FlipPath ? new Vector2(-1, 1) : Vector2.one),
                        25f, 0.5f);
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
                    * transform.up * 2f, 15f, 0.5f);
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
                    enemy.Speed = curEntry.Speed;
                    break;
                case SpawnEntry.EntryType.Bullet:
                    BulletBase bullet = PoolManager.current.Activate(curEntry.Entity, curEntry.Position, 
                        Quaternion.Euler(0, 0, curEntry.Rotation)).GetComponent<BulletBase>();
                    switch(bullet.Type)
                    {
                        case BulletType.Base:
                        case BulletType.Petal:
                            bullet.Speed = curEntry.Speed;
                            bullet.OffScreenBehaviour = curEntry.OffScrBhv;
                            bullet.StartBounces = curEntry.Bounces;
                            break;
                        case BulletType.Ring:
                            bullet.Speed = curEntry.Speed;
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
        Speed = speed;
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
        Speed = speed;
        OffScrBhv = offScrBhv;
        Bounces = bounces;
    }

    public EntryType Type;
    public bool Previewing;
    public GameObject Entity;
    public float Delay;
    public Vector2 Position;
    public float Rotation;
    public float Speed;
    public bool SpecificsExpanded;
    //EnemySpecific
    public PathScriptableObject Path;
        public bool FlipPath;
    //BulletSpecifics
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
    }
}