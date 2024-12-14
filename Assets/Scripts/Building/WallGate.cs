using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGate : Wall, IDamageable
{
    // Start is called before the first frame update
    void Start()
    {
        setHealthBar(gameObject.GetComponentInChildren<HealthBarScript>());
        initalize();
    }

    void Update()
    {

    }

    public void TakeDamage(float damage, GameObject whoOwMe = null)
    {
        health -= damage;

        getHealthBar().UpdateHealthBar(health, maxHealth);

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnDisable()
    {
        ComponentManager.Instance.buildingsDisabled.Add(gameObject);
    }
}
