using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Ring : BulletBase
{

    public GameObject Bullet;
    public int Elements = 3;
    public float SideBob = 1.25f;
    public float RotationSpeed = 3f;
    public float Distance;
    public bool RandomizedValues;

    public override void OnEnable()
    {
        base.OnEnable();
        if(RandomizedValues)
        {
            SideBob = Random.Range(-1.25f, 1.25f);
            RotationSpeed = Random.Range(-3f, 3f);
            Speed = Random.Range(1.5f, 3f);
        }
        Invoke("SpawnPetals", 0.1f);
    }
    
    public void SpawnPetals()
    {
        Color color = Color.clear;
        DestroyPetals();
        for (int i = 0; i < Elements; i++)
        {
            GameObject newPetal = PoolManager.current.Activate(Bullet, transform, transform.position);
            if (color == Color.clear) color = newPetal.GetComponent<SpriteRenderer>().color;
            newPetal.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 360f / Elements * i));
            newPetal.transform.position += newPetal.transform.up * 
                (Distance + 0.45f + (Mathf.Clamp(Elements - 4, 0, Mathf.Infinity) * 0.125f) * Mathf.Pow(1.00001f, Elements));
            newPetal.GetComponent<SpriteRenderer>().color = color;
            newPetal.GetComponent<BulletBase>().Moves = false;
        }
    }

    void DestroyPetals()
    {
        List<GameObject> CurrentPetals = new List<GameObject>();
        foreach (Transform child in transform)
        {
            CurrentPetals.Add(child.gameObject);
        }
        if (CurrentPetals.Count > 0) for (int i = 0; i < CurrentPetals.Count; i++)
        {
            PoolManager.current.Deactivate(CurrentPetals[i]);
        }
        CurrentPetals = new List<GameObject>();
    }

    public override void Move()
    {
        if(Moves)
        {
            transform.Translate(new Vector3(Mathf.Sin(Time.time * SideBob), -1) * Speed * Time.deltaTime, Space.World);
            transform.Rotate(new Vector3(0, 0, RotationSpeed));
        }
    }

    public override void Death()
    {
        List<GameObject> CurrentPetals = new List<GameObject>();
        foreach (Transform child in transform)
        {
            CurrentPetals.Add(child.gameObject);
        }
        foreach (var item in CurrentPetals)
        {
            item.GetComponent<BulletBase>().Moves = true;
            item.transform.parent = PoolManager.current.GetPool(item).transform;
        }
        base.Death();
    }

    private void OnDrawGizmosSelected()
    {
        float distance = Distance + 0.45f + Mathf.Clamp(Elements - 4, 0, Mathf.Infinity) * 0.125f * Mathf.Pow(1.00001f, Elements) / 2;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distance);
        Gizmos.color = Color.red;
        for (int i = 0; i < Elements; i++)
        {
            Gizmos.DrawWireSphere(transform.position + Quaternion.AngleAxis(360f / Elements * i, Vector3.forward * distance) * transform.up * distance, 0.1f);
        }
    }
}