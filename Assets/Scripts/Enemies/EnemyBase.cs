using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyNavigation;

public class EnemyBase : MonoBehaviour
{

    public float Speed;
    public List<SpawnEntry> AttackPattern = new List<SpawnEntry>();
    public PathScriptableObject Path;

    Vector2 OriginPosition;

    void OnEnable()
    {
        OriginPosition = transform.position;
        if(Path) StartCoroutine(FollowPath());
    }

    void OnBecameVisible()
    {
        StartCoroutine(Pattern());
    }

    void OnBecameInvisible()
    {
        PoolManager.current.Deactivate(gameObject);
    }

    IEnumerator FollowPath()
    {
        for (int i = 0; i < Path.Points.Count; i++)
        {
            Vector2 Goal = Path.Points[i].Location;
            while (Vector2.Distance(transform.position, Goal + OriginPosition) > 0.1f)
            {
                yield return new WaitForFixedUpdate();
                transform.Translate(((Goal + OriginPosition) - (Vector2)transform.position).normalized * Speed * Time.deltaTime);
            }
            yield return new WaitForSeconds(Path.Points[i].Delay);
        }
    }

    IEnumerator Pattern()
    {
        while(true)
        {
            SpawnEntry curEntry;
            foreach (var item in AttackPattern)
            {
                curEntry = item;
                yield return new WaitForSeconds(curEntry.Delay);
                PoolManager.current.Activate(curEntry.Entity, (Vector2)transform.position + curEntry.Position)
                    .transform.rotation = Quaternion.Euler(new Vector3(0, 0, item.Rotation));
            }
            yield return new WaitForEndOfFrame();
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
                Gizmos.DrawLine((Vector2)transform.position + Path.Points[i - 1].Location, 
                    (Vector2)transform.position + Path.Points[i].Location);
            }
        }
    }
}
