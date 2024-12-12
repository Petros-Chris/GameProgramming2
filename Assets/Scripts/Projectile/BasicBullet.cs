using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public float timeTillDeath = 7.0f;
    public float timeTillDeathAfterHit = 0.5f;
    public int damage = 1;
    public int amountOfTimesDamageCanBeTrigged = 1;

    private void Start()
    {
        Destroy(gameObject, timeTillDeath);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<EnemyAI>(out EnemyAI enemyComponent))
        {
            if(amountOfTimesDamageCanBeTrigged > 0)
            {
                enemyComponent.TakeDamage(damage, gameObject);
                amountOfTimesDamageCanBeTrigged--;
                Destroy(gameObject, timeTillDeathAfterHit);
            }
        }
    }
}
