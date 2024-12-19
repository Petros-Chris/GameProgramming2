using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject HitPoint;
    public GameObject Fire;
    public Transform FirePoint;
    public Transform Nozzle;
    public float range = 100f;
    public float damage = 4f;
    public int magazineSize = 50;
    public float reloadTime = 3.0f;
    public float fireRate = 0.1f;
    public int currentBullets;
    public bool isReloading = false;
    public float nextFireTime = 0f;
    [SerializeField] public string audioPath;
    public TrailRenderer bulletTrail;
    public KeyCode reloadKey = KeyCode.R;


    public void Start()
    {
        currentBullets = magazineSize;
        FirePoint = ComponentManager.Instance.playerCam.transform;
    }

    public void OnEnable()
    {
        PlayerUIManager.Instance.SwitchWeapon(gameObject);
    }
}
