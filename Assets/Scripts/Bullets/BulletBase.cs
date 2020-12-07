using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bullets
{
    public class BulletBase : MonoBehaviour
    {

        public float Speed;
        public bool Moves;
        public bool HasLiveTime;
        public float StartLiveTime;
        public OffScreenBehaviour OffScreenBehaviour;
        public int StartBounces;

        float liveTime;
        int bounces;

        public virtual void OnEnable()
        {
            liveTime = StartLiveTime;
            bounces = StartBounces;
            Moves = true;
        }

        void Update()
        {
            if (HasLiveTime)
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
            if (Moves) transform.Translate(Vector2.up * Speed * Time.deltaTime, Space.Self);
        }

        public void Reflect()
        {
            if (bounces < 0) PoolManager.current.Deactivate(gameObject);
            transform.rotation = Quaternion.Inverse(transform.rotation);
            bounces--;
        }
    }

    public enum OffScreenBehaviour
    {
        Despawn,
        Reflect,
        Death,
        None
    }
}