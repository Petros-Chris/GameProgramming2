using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMeleeWeapon : MonoBehaviour
{
    public int damage = 1;


    public void Stab(float AttackRange)
    {

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, AttackRange))
        {
            Debug.Log("Kill");
            if (hit.transform.TryGetComponent<IDamageable>(out IDamageable component))
            {
                component.TakeDamage(damage);
            }
        }
        Debug.DrawRay(transform.position, transform.forward * 1f, Color.red, 1f);
    }

    public void AreaStab(float AttackRange)
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
