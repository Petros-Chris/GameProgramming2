using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance { get; private set; }
    public Weapon gun;
    public TextMeshProUGUI currentAmmo;
    public TextMeshProUGUI reloading;
    public Image explosionAmmo1;
    public Image explosionAmmo2;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void SwitchWeapon(GameObject newWeaponHead)
    {
        gun = newWeaponHead.GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gun == null)
        {
            Debug.Log("Gun Is Missing!");
            return;
        }

        currentAmmo.text = gun.currentBullets + "/" + gun.magazineSize;

        // if (gun.currentBullets == 1)
        // {
        //     explosionAmmo2.gameObject.SetActive(false);
        //     explosionAmmo1.gameObject.SetActive(true);
        // }

        // if (gun.currentBullets == 0)
        // {
        //     explosionAmmo2.gameObject.SetActive(false);
        //     explosionAmmo1.gameObject.SetActive(false);
        // }

        // if (gun.currentBullets == 2)
        // {
        //     explosionAmmo2.gameObject.SetActive(true);
        //     explosionAmmo1.gameObject.SetActive(true);
        // }
        if (gun.isReloading == true)
        {
            reloading.gameObject.SetActive(true);

        }
        else
        {
            reloading.gameObject.SetActive(false);
        }
    }
}
