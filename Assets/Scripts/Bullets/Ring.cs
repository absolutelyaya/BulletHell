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

    void Start()
    {
        if(RandomizedValues)
        {
            SideBob = Random.Range(-1.25f, 1.25f);
            RotationSpeed = Random.Range(-3f, 3f);
            Speed = Random.Range(1.5f, 3f);
        }
        SpawnPetals();
    }
    
    [ExecuteAlways]
    public void SpawnPetals()
    {
        Color color = Color.clear;
        DestroyPetals();
        for (int i = 0; i < Elements; i++)
        {
            GameObject newPetal = (GameObject)PrefabUtility.InstantiatePrefab(Bullet, transform);
            if (color == Color.clear) color = newPetal.GetComponent<SpriteRenderer>().color;
            newPetal.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 360f / Elements * i));
            newPetal.transform.position += newPetal.transform.up * (Distance + 0.45f + (Mathf.Clamp(Elements - 4, 0, Mathf.Infinity) * 0.125f) * Mathf.Pow(1.00001f, Elements));
            newPetal.GetComponent<SpriteRenderer>().color = color;
        }
    }

    [ExecuteAlways]
    void DestroyPetals()
    {
        List<GameObject> CurrentPetals = new List<GameObject>();
        foreach (Transform child in transform)
        {
            CurrentPetals.Add(child.gameObject);
        }
        if (CurrentPetals.Count > 0) for (int i = 0; i < CurrentPetals.Count; i++)
        {
            DestroyImmediate(CurrentPetals[i]);
        }
        CurrentPetals = new List<GameObject>();
    }

    public override void Move()
    {
        transform.Translate(new Vector3(Mathf.Sin(Time.time * SideBob), - 1) * Speed * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(0, 0, RotationSpeed));
    }

    public override void Death()
    {
        transform.DetachChildren();
        base.Death();
    }
}

[CustomEditor(typeof(Ring))]
public class BlossomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Ring script = (Ring)target;
        if(GUILayout.Button("Preview"))
        {
            script.SpawnPetals();
        }
    }
}