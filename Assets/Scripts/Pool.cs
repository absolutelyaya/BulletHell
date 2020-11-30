using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField]
    public GameObject Object;
    [SerializeField]
    public int StandardAmount;

    List<GameObject> entries = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < StandardAmount; i++)
        {
            GameObject entry;
            entries.Add(entry = Instantiate(Object, transform));
            entry.SetActive(false);
        }
    }

    public GameObject Activate(Vector2 position = new Vector2())
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
            Debug.LogWarning($"Expanding pool '{gameObject.name}'");
        }
        target.transform.position = position;
        target.SetActive(true);
        return target;
    }

    public void Deactivate(GameObject target)
    {
        target.transform.parent = transform;
        target.transform.position = transform.position;
        target.transform.rotation = Quaternion.identity;
        target.SetActive(false);
    }
}