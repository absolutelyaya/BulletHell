using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField]
    public GameObject Object;
    [SerializeField]
    public int StandardAmount;
    [SerializeField]
    public float ExtraTimeOut = 10f;

    public List<GameObject> entries = new List<GameObject>();
    public List<float> inactiveTime = new List<float>();

    void Start()
    {
        for (int i = 0; i < StandardAmount; i++)
        {
            GameObject entry;
            entries.Add(entry = Instantiate(Object, transform));
            entry.SetActive(false);
        }
    }

    //dis bish don't work
    //private void Update()
    //{
    //    while (entries.Count > inactiveTime.Count) inactiveTime.Add(0f);
    //    entries.RemoveAll(GameObject => GameObject == null);
    //
    //    if (entries.Count > StandardAmount)
    //    {
    //        int i = 0;
    //        List<int> delete = new List<int>();
    //        foreach (var item in entries)
    //        {
    //            if (entries.Count > StandardAmount) if (!item.activeSelf)
    //            {
    //                inactiveTime[i] += Time.deltaTime;
    //                if(inactiveTime[i] > ExtraTimeOut)
    //                {
    //                    Destroy(item, 0.1f);
    //                    inactiveTime.RemoveAt(i);
    //                }
    //            }
    //            else
    //            {
    //                inactiveTime[i] = 0f;
    //            }
    //            i++;
    //        }
    //    }
    //}

    public GameObject Activate(Vector2 position = new Vector2(), Quaternion rotation = new Quaternion())
    {
        GameObject target = null;
        foreach (var item in entries)
        {
            if(!item.activeSelf)
            {
                target = item;
                break;
            }
        }
        if (!target)
        {
            target = Instantiate(Object, transform);
            entries.Add(target);
            inactiveTime.Add(0f);
            Debug.LogWarning($"Expanding pool '{gameObject.name}'");
        }
        target.transform.position = position;
        target.transform.rotation = rotation;
        target.SetActive(true);
        return target;
    }

    public GameObject Activate(Transform parent, Vector2 position = new Vector2(), Quaternion rotation = new Quaternion())
    {
        GameObject target = null;
        foreach (var item in entries)
        {
            if (!item.activeSelf)
            {
                target = item;
                break;
            }
        }
        if (!target)
        {
            target = Instantiate(Object, transform);
            entries.Add(target);
            inactiveTime.Add(0f);
            Debug.LogWarning($"Expanding pool '{gameObject.name}'");
        }
        target.transform.parent = parent;
        target.transform.position = position;
        target.transform.rotation = rotation;
        target.GetComponent<Rigidbody2D>().simulated = true;
        target.SetActive(true);
        return target;
    }

    public void Deactivate(GameObject target, bool affectChildren)
    {
        if (!entries.Contains(target)) entries.Add(target);
        if(target.GetComponent<Bullets.BulletBase>()) //Weird fix, I know. Couldn't figure out the actual issue yet.
        {
            if (affectChildren && target.transform.childCount > 0)
            {
                while (target.transform.childCount > 0)
                {
                    var child = target.transform.GetChild(0);
                    PoolManager.current.Deactivate(child.gameObject, false);
                }
            }
        }
        target.transform.SetParent(transform);
        target.transform.position = transform.position;
        target.transform.rotation = Quaternion.identity;
        target.SetActive(false);
    }
}

public interface IPoolEntry
{
    void OnDeactivate();
}