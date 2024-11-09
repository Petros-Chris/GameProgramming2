using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBasicBullet : MonoBehaviour
{
    //! redundant script of BasicBullet
    public float timeTillDeath = 7.0f;
    public float timeTillDeathAfterHit = 0.5f;
    public int damage = 1;
    public int amountOfTimesDamageCanBeTrigged = 1; //Percing ability 

    private void Start()
    {
        Destroy(gameObject, timeTillDeath);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent<EnemyAI>(out EnemyAI component))
        {
            if (amountOfTimesDamageCanBeTrigged > 0)
            {
                component.TakeDamage(damage);
                amountOfTimesDamageCanBeTrigged--;
                Destroy(gameObject, timeTillDeathAfterHit);
            }
        }
    }
}