using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBasicBullet : MonoBehaviour
{
    public float timeTillDeath = 7.0f;
    public float timeTillDeathAfterHit = 0.5f;
    public int damage = 1;
    public int amountOfTimesDamageCanBeTrigged = 1; //Percing ability 
    public BoxCollider buildingToIgnore;

    private void Start()
    {
        //Hard Coded for now
        GameObject building = GameObject.Find("Tower");
        buildingToIgnore = building.GetComponentInParent<BoxCollider>();

        Destroy(gameObject, timeTillDeath);

        // Ignores building
        Physics.IgnoreCollision(buildingToIgnore, gameObject.GetComponent<SphereCollider>());
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