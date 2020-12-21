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
            CreatePool(new PoolEntry(target, 10, 10f));
            pools.TryGetValue(target.name, out pool);
            return pool.GetComponent<Pool>();
        }
        return pool.GetComponent<Pool>();
    }

    void CreatePool(PoolEntry pattern)
    {
        var poolObject = new GameObject(pattern.Object.name);
        poolObject.transform.parent = transform;
        var pool = poolObject.AddComponent<Pool>();
        pool.Object = pattern.Object;
        pool.StandardAmount = pattern.StandardAmount;
        pools.Add(pattern.Object.name, poolObject);
    }

    public GameObject Activate(GameObject target, Vector2 position = new Vector2(), Quaternion rotation = new Quaternion())
    {
        return GetPool(target).Activate(position, rotation);
    }

    public GameObject Activate(GameObject target, Transform parent, Vector2 position = new Vector2(), Quaternion rotation = new Quaternion())
    {
        return GetPool(target).Activate(parent, position, rotation);
    }

    public void Deactivate(GameObject target)
    {
        GetPool(target).Deactivate(target);
    }
}

[Serializable]
public struct PoolEntry
{
    public PoolEntry(GameObject type, int amount, float extraTimeOut = 10)
    {
        Object = type;
        StandardAmount = amount;
        ExtraTimeOut = extraTimeOut;
    }

    public GameObject Object;
    public int StandardAmount;
    public float ExtraTimeOut;
}