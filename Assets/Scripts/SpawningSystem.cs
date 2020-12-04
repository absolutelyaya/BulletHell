using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningSystem : MonoBehaviour
{

    public List<SpawnEntry> Level = new List<SpawnEntry>();

    Queue<SpawnEntry> levelStack = new Queue<SpawnEntry>();

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
            PoolManager.current.Activate(curEntry.Entity, curEntry.Position);
        }
    }
}

[Serializable]
public struct SpawnEntry
{
    public SpawnEntry(GameObject entity, float delay, Vector2 position, float rotation)
    {
        Entity = entity;
        Delay = delay;
        Position = position;
        Rotation = rotation;
    }

    public GameObject Entity;
    public float Delay;
    public Vector2 Position;
    public float Rotation;
}