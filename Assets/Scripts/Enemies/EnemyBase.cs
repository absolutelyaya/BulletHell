using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyNavigation;

public class EnemyBase : MonoBehaviour
{

    public float Speed;
    public float FireRate;
    public float Spread;
    public GameObject Projectile;
    public Transform FireDirectionObject;
    public PathScriptableObject Path;
    public bool InvertPath;
    public bool DespawnOffscreen = true;
    public bool IsOffScreen;

    public Vector2 originPosition;

    protected void OnEnable()
    {
        originPosition = transform.position;
        if(Path) StartCoroutine(FollowPath());
    }

    protected void OnBecameVisible()
    {
        StartCoroutine(Pattern());
    }

    protected void OnBecameInvisible()
    {
        PoolManager.current.Deactivate(gameObject);
    }

    IEnumerator FollowPath()
    {
        for (int i = 0; i < Path.Points.Count; i++)
        {
            Vector2 Goal = Vector2.Scale(Path.Points[i].Location, InvertPath ? new Vector2(-1, 1) : Vector2.one);
            while (Vector2.Distance(transform.position, Goal + originPosition) > 0.1f)
            {
                yield return new WaitForFixedUpdate();
                transform.Translate(((Goal + originPosition) - (Vector2)transform.position).normalized * Speed * Time.deltaTime);
            }
            yield return new WaitForSeconds(Path.Points[i].Delay);
        }
    }

    protected virtual IEnumerator Pattern()
    {
        while(true)
        {
            yield return new WaitForSeconds(FireRate);
            PoolManager.current.Activate(Projectile, FireDirectionObject.position, 
                Quaternion.Euler(new Vector3(0, 0, FireDirectionObject.eulerAngles.z + Random.Range(-Spread, Spread))));
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDrawGizmosSelected()
    {
        if(Path)
        {
            for (int i = 1; i < Path.Points.Count; i++)
            { 
                Gizmos.DrawLine(
                    Application.isPlaying ? originPosition : (Vector2)transform.position +
                    Vector2.Scale(Path.Points[i - 1].Location, InvertPath ? new Vector2(-1, 1) : Vector2.one),
                    Application.isPlaying ? originPosition : (Vector2)transform.position + 
                    Vector2.Scale(Path.Points[i].Location, InvertPath ? new Vector2(-1, 1) : Vector2.one));
            }
        }
        if (FireDirectionObject)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(FireDirectionObject.position, FireDirectionObject.position +
                Quaternion.AngleAxis(FireDirectionObject.eulerAngles.z, Vector3.forward * 2) * transform.up * 2);
        }
    }
}
