using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager current;

    public List<PoolEntry> Pools = new List<PoolEntry>();

    public Dictionary<string, GameObject> pools = new Dictionary<string, GameObject>();

    private void OnEnable()
    {
        current = this;
        foreach (var item in Pools)
        {
            CreatePool(item);
        }
    }

    public Pool GetPool(GameObject target)
    {
        pools.TryGetValue(target.name.Replace("(Clone)", ""), out GameObject pool);
        if (!pool)
        {
            Debug.LogWarning($"Pool '{target.name}' doesn't exist. Creating now.");
            CreatePool(new PoolEntry(target, 10));
            pools.TryGetValue(target.name, out pool);
            return pool.GetComponent<Pool>();
        }
        return pool.GetComponent<Pool>();
    }

    void CreatePool(PoolEntry pattern)
    {
        var poolObject = Instantiate(new GameObject(), transform);
        var pool = poolObject.AddComponent<Pool>();
        pool.Object = pattern.Object;
        pool.StandardAmount = pattern.standardAmount;
        poolObject.name = pattern.Object.name;
        pools.Add(pattern.Object.name, poolObject);
    }

    public GameObject Activate(GameObject target, Vector2 position = new Vector2(), Quaternion rotation = new Quaternion())
    {
        return GetPool(target).Activate(position);
    }

    public GameObject Activate(GameObject target, Transform parent, Vector2 position = new Vector2(), Quaternion rotation = new Quaternion())
    {
        return GetPool(target).Activate(parent, position);
    }

    public void Deactivate(GameObject target)
    {
        GetPool(target).Deactivate(target);
    }
}

[Serializable]
public struct PoolEntry
{
    public PoolEntry(GameObject type, int amount)
    {
        Object = type;
        standardAmount = amount;
    }

    public GameObject Object;
    public int standardAmount;
}