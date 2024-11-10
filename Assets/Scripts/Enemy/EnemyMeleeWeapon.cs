using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMeleeWeapon : MonoBehaviour
{
    public int damage = 1;


    public void Stab(float AttackRange)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AttackRange);
        //canDamage = true;
        foreach (Collider collider in hitColliders)
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable component))
            {
                component.TakeDamage(damage);
                break;
            }
        }
    }
}
