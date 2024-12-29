using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] public string audioPath0;
    public TrailRenderer bulletTrail;
    public InputAction reloadKey;
    public InputAction attack;
    FishGuard fishGuardMovement;
    void Awake()
    {
        fishGuardMovement = new FishGuard();
    }
    void OnEnable()
    {
        reloadKey = fishGuardMovement.Player.Reload;
        attack = fishGuardMovement.Player.Fire;
        reloadKey.Enable();
        attack.Enable();
        PlayerUIManager.Instance.SwitchWeapon(gameObject);
    }

    void OnDisable()
    {
        reloadKey.Disable();
        attack.Disable();
    }

    public void Start()
    {
        currentBullets = magazineSize;
        FirePoint = ComponentManager.Instance.playerCam.transform;
    }
}
