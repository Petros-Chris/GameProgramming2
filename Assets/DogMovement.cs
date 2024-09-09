using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMovement : MonoBehaviour
{
    public Camera playerCamera;
    public Transform orientation;
    public Transform pissAttack;
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

        pissAttack.rotation = adjustedRotation;
    }

    void GunMovesWithPlayerDirection()
    {
        Vector3 sidePosition = orientation.position + (-orientation.up / 2);
        sidePosition += orientation.forward / 2;

        pissAttack.position = sidePosition;
    }
}
