using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    public List<SpawnEntry> AttackPattern = new List<SpawnEntry>();
    public float AttackDirection = 180;

    void Start()
    {
        StartCoroutine(Pattern());
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
                    .transform.rotation = Quaternion.Euler(new Vector3(0, 0, AttackDirection));
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
