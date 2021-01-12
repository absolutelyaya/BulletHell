using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyNavigation;

public class EnemyBase : MonoBehaviour
{

    public int StartHealth;
    public float Speed, FireRate, Spread;
    public GameObject Projectile;
    public Transform FireDirectionObject;
    public PathScriptableObject Path;
    public bool InvertPath, DespawnOffscreen = true, IsOffScreen;

    private Vector2 originPosition;
    private int health;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    protected void OnEnable()
    {
        StartCoroutine(Initiate());
    }

    IEnumerator Initiate()
    {
        yield return new WaitForEndOfFrame();
        health = StartHealth;
        originPosition = transform.position;
        if (Path) StartCoroutine(FollowPath());
    }

    protected void OnBecameVisible()
    {
        StartCoroutine(Pattern());
    }

    protected void OnBecameInvisible()
    {
        PoolManager.current.Deactivate(gameObject, false);
    }

    IEnumerator FollowPath()
    {
        Vector2[] points = Path.Path.CalculateEvenlySpacedPoints(0.5f);
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 Goal = Vector2.Scale(points[i], InvertPath ? new Vector2(-1, 1) : Vector2.one);
            while (Vector2.Distance(transform.position, Goal + originPosition) > 0.1f)
            {
                yield return new WaitForFixedUpdate();
                transform.Translate(((Goal + originPosition) - (Vector2)transform.position).normalized * Speed * Time.deltaTime);
            }
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
        Vector2[] points = Path.Path.CalculateEvenlySpacedPoints(0.5f);
        if (Path)
        {
            for (int i = 1; i < points.Length; i++)
            { 
                Gizmos.DrawLine(
                    Application.isPlaying ? originPosition : (Vector2)transform.position +
                    Vector2.Scale(points[i - 1], InvertPath ? new Vector2(-1, 1) : Vector2.one),
                    Application.isPlaying ? originPosition : (Vector2)transform.position + 
                    Vector2.Scale(points[i], InvertPath ? new Vector2(-1, 1) : Vector2.one));
            }
        }
        if (FireDirectionObject)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(FireDirectionObject.position, FireDirectionObject.position +
                Quaternion.AngleAxis(FireDirectionObject.eulerAngles.z, Vector3.forward * 2) * transform.up * 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerBullet"))
        {
            health--;
            PoolManager.current.Deactivate(collision.gameObject, false);
            StartCoroutine(DamageSequence());
            if (health <= 0) Death();
        }
    }

    private IEnumerator DamageSequence()
    {
        float time = 0;
        while(time < 1)
        {
            yield return new WaitForEndOfFrame();
            sprite.color = Color.Lerp(Color.red, Color.white, time);
            time += Time.deltaTime * 4;
        }
    }

    public virtual void Death()
    {
        StopAllCoroutines();
        PoolManager.current.Deactivate(gameObject, false);
    }
}
