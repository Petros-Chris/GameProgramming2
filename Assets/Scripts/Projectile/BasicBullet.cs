using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public float force = 10;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.TryGetComponent<EnemyAI>(out EnemyAI enemyComponent))
        {
            enemyComponent.TakeDamage(1);
        }
        Destroy(gameObject);
    }
}
