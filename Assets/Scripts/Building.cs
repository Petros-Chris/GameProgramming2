using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float health;
    public float maxHealth;
    private HealthBarScript healthBar;

    public HealthBarScript getHealthBar()
    {
        return healthBar;
    }
    public void setHealthBar(HealthBarScript localhealthBar)
    {
        healthBar = localhealthBar;
    }
}

