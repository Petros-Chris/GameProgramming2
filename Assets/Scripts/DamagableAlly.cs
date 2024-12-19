using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, GameObject whoOwMe);
    void Heal(float healAmount)
    {

    }
}