using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bullets;

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
        Invoke("SpawnElements", 0.1f);
    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            if(child.GetComponent<BulletBase>()) PoolManager.current.Deactivate(child.gameObject);
        }
    }

    public void SpawnElements()
    {
        Color color = Color.clear;
        DestroyElements();
        for (int i = 0; i < Elements; i++)
        {
            GameObject newElement = PoolManager.current.Activate(Bullet, transform, transform.position);
            if (color == Color.clear) color = newElement.GetComponent<SpriteRenderer>().color;
            newElement.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 360f / Elements * i));
            newElement.transform.position += newElement.transform.up * 
                (Distance + 0.45f + (Mathf.Clamp(Elements - 4, 0, Mathf.Infinity) * 0.125f) * Mathf.Pow(1.00001f, Elements));
            newElement.GetComponent<SpriteRenderer>().color = color;
            newElement.GetComponent<BulletBase>().Moves = false;
            newElement.GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<CircleCollider2D>().radius = Distance + 0.45f + 
                (Mathf.Clamp(Elements - 4, 0, Mathf.Infinity) * 0.125f) * Mathf.Pow(1.00001f, Elements);
        }
    }

    void DestroyElements()
    {
        List<GameObject> CurrentElements = new List<GameObject>();
        foreach (Transform child in transform)
        {
            CurrentElements.Add(child.gameObject);
        }
        if (CurrentElements.Count > 0) for (int i = 0; i < CurrentElements.Count; i++)
        {
            PoolManager.current.Deactivate(CurrentElements[i]);
            }
        CurrentElements = new List<GameObject>();
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
        List<GameObject> CurrentElements = new List<GameObject>();
        foreach (Transform child in transform)
        {
            CurrentElements.Add(child.gameObject);
        }
        foreach (var item in CurrentElements)
        {
            if (!IsOffScreen)
            {
                item.GetComponent<BulletBase>().Moves = true;
                item.GetComponent<Rigidbody2D>().simulated = true;
                item.transform.parent = PoolManager.current.GetPool(item).transform;
            }
            else PoolManager.current.Deactivate(item);
        }
        base.Death();
    }

    private void OnDrawGizmosSelected()
    {
        yayasGizmos.DrawRing(transform.position, Elements, Distance);
    }
}