using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bullets;

public class ScreenEdge : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch(collision.tag)
        {
            case "Enemy":
                collision.TryGetComponent(out EnemyBase enemy);
                if (enemy)
                {
                    if (enemy.DespawnOffscreen) PoolManager.current.Deactivate(collision.gameObject);
                    else enemy.IsOffScreen = true;
                }
                else throw new MissingComponentException($"{collision.name} has the Enemy tag, but no script inheriting from 'EnemyBase'");
                break;
            case "Bullet":
                collision.TryGetComponent(out BulletBase bullet);
                if (bullet)
                {
                    bullet.IsOffScreen = true;
                    switch(bullet.OffScreenBehaviour)
                    {
                        case OffScreenBehaviour.Despawn:
                            PoolManager.current.Deactivate(collision.gameObject);
                            break;
                        case OffScreenBehaviour.Reflect:
                            bullet.Reflect();
                            break;
                        case OffScreenBehaviour.Death:
                            bullet.Death();
                            break;
                        case OffScreenBehaviour.None:
                            break;
                    }
                }
                else throw new MissingComponentException($"{collision.name} has the Bullet tag, but no script inheriting from 'BulletBase'");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.tag)
        {
            case "Enemy":
                collision.TryGetComponent(out EnemyBase enemy);
                if (enemy)
                {
                    enemy.IsOffScreen = false;
                }
                else throw new MissingComponentException($"{collision.name} has the Enemy tag, but no script inheriting from 'EnemyBase'");
                break;
            case "Bullet":
                collision.TryGetComponent(out BulletBase bullet);
                if (bullet)
                {
                    bullet.IsOffScreen = false;
                }
                else throw new MissingComponentException($"{collision.name} has the Bullet tag, but no script inheriting from 'BulletBase'");
                break;
        }
    }
}
