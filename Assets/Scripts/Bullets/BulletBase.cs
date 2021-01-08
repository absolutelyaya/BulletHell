using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bullets
{
    public class BulletBase : MonoBehaviour
    {

        public BulletType Type;
        public float Speed, StartLiveTime;
        public bool Moves, HasLiveTime, IsOffScreen;
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
            PoolManager.current.Deactivate(gameObject, true);
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
            if (bounces < 0) PoolManager.current.Deactivate(gameObject, true);
            transform.rotation = Quaternion.Inverse(transform.rotation);
            bounces--;
        }

        private void OnDrawGizmosSelected()
        {
            yayasGizmos.DrawArrow(transform.position, transform.position + Quaternion.AngleAxis(transform.rotation.z, Vector3.forward) * transform.up * 1f);
        }
    }

    [System.Serializable]
    public enum OffScreenBehaviour
    {
        Despawn,
        Reflect,
        Death,
        None
    }

    [System.Serializable]
    public enum BulletType
    {
        Base,
        Petal,
        Ring
    }
}