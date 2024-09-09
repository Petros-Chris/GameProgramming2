using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public Camera playerCamera;
    public Transform orientation;
    public Transform weapon;
    public Vector3 weaponOffset = new Vector3(0.5f, -0.3f, 0.7f);

    void Update()
    {
        GunMovesWithPlayerDirection();
        WeaponLooksInPlayerDirection();
    }

    void WeaponLooksInPlayerDirection()
    {
        
        Quaternion target = playerCamera.transform.rotation;

        Quaternion offset = Quaternion.Euler(-90f, 0f, 0f);

        Quaternion adjustedRotation = target * offset;

        weapon.rotation = adjustedRotation;

        //Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
       // weapon.rotation = playerCamera.transform.rotation;
        //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        //{
        //    weapon.LookAt(hit.point);
        //}

    }

    void GunMovesWithPlayerDirection()
    {
        Vector3 sidePosition = orientation.position + (orientation.right / 2);
        sidePosition += orientation.forward / 2;

        weapon.position = sidePosition;
    }
}
