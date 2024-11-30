using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public TextMeshProUGUI currentAmmo;
    public TextMeshProUGUI reloading;
    public Image explosionAmmo1;
    public Image explosionAmmo2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentAmmo.text = Gun.currentBullets + "/" + Gun.magazineSize;

        if(Gun2.currentBullets == 1){
            explosionAmmo2.gameObject.SetActive(false);
            explosionAmmo1.gameObject.SetActive(true);
        }

        if(Gun2.currentBullets == 0){
            explosionAmmo2.gameObject.SetActive(false);
            explosionAmmo1.gameObject.SetActive(false);
        }

         if(Gun2.currentBullets == 2){
            explosionAmmo2.gameObject.SetActive(true);
            explosionAmmo1.gameObject.SetActive(true);
        }
        if(Gun.currentBullets == 0){
            reloading.gameObject.SetActive(true);

        }else{
             reloading.gameObject.SetActive(false);
        }
    }
}
