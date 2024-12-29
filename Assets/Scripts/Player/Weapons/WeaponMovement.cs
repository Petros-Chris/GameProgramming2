using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public Camera playerCamera;
    public Transform orientation;
    public Transform weapon;

    void Update()
    {
        if (playerCamera == null)
            playerCamera = ComponentManager.Instance.playerCam;
        GunMovesWithPlayerDirection();
        WeaponLooksInPlayerDirection();
    }

    void WeaponLooksInPlayerDirection()
    {
        Quaternion target = playerCamera.transform.rotation;

        Quaternion offset = Quaternion.Euler(-90f, 0f, 0f);

        Quaternion adjustedRotation = target * offset;

        weapon.rotation = Quaternion.Slerp(weapon.rotation, adjustedRotation, Time.deltaTime * 32);
    }

    void GunMovesWithPlayerDirection()
    {
        Vector3 sidePosition = orientation.position + (orientation.right / 2);
        sidePosition += orientation.forward / 2;

        weapon.position = Vector3.Lerp(weapon.position, sidePosition, Time.deltaTime * 32);
    }
}
