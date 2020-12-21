using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : EnemyBase
{
    protected override IEnumerator Pattern()
    {
        while (true)
        {
            yield return new WaitForSeconds(FireRate);
            FireDirectionObject.rotation = Quaternion.LookRotation(Vector3.forward, (PlayerController.Current.transform.position - transform.position).normalized);
            PoolManager.current.Activate(Projectile, FireDirectionObject.position,
                Quaternion.Euler(new Vector3(0, 0, FireDirectionObject.eulerAngles.z + Random.Range(-Spread, Spread))));
        }
    }
}
