using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{

    public float Speed;
    public bool Moves;
    public bool HasLiveTime;
    public float StartLiveTime;

    float liveTime;

    public virtual void OnEnable()
    {
        liveTime = StartLiveTime;
        Moves = true;
    }

    void Update()
    {
        if(HasLiveTime)
        {
            liveTime -= Time.deltaTime;
            if (liveTime <= 0) Death();
        }
    }

    public virtual void Death()
    {
        PoolManager.current.Deactivate(gameObject);
    }

    private void OnBecameInvisible()
    {
        PoolManager.current.Deactivate(gameObject);
    }

    private void FixedUpdate()
    {
        Move();
    }

    public virtual void Move()
    {
        if(Moves) transform.Translate(Vector2.up * Speed * Time.deltaTime, Space.Self);
    }
}
