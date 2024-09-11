using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicBullet : MonoBehaviour
{
    public float timeTillDeath = 7.0f;
    public float timeTillDeathAfterHit = 0.5f;
    public int damage = 1;

    private void Start()
    {
        Destroy(gameObject, timeTillDeath);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.TryGetComponent<Player>(out Player playerComponent))
        // {
        //    playerComponent.TakeDamage(damage);
        //    Destroy(gameObject, timeTillDeathAfterHit);
        // }

        // if (collision.gameObject.TryGetComponent<FishKingdom>(out FishKingdom component))
        // {
        //    component.TakeDamage(damage);
        //    Destroy(gameObject, timeTillDeathAfterHit);
        // }
    }
}
