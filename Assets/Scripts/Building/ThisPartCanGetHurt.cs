
using UnityEngine;

public class ThisPartCanGetHurt : MonoBehaviour, IDamageable
{
    public WallGate wallGate;

    public void TakeDamage(float damage, GameObject whoOwMe = null)
    {
        wallGate.TakeDamage(damage, whoOwMe);
    }
    void OnDestroy()
    {
        GameObject parent = gameObject.transform.parent.parent.gameObject;
        Destroy(parent);
    }
}
