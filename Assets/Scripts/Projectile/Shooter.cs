using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject Fire;
    public GameObject HitPoint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1"))
        Shooting();
    }

    public void Shooting(){
        RaycastHit hit;

       if(Physics.Raycast(FirePoint.position, transform.TransformDirection(Vector3.forward), out hit, 100))
        {
            Debug.DrawRay(FirePoint.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

        }

        GameObject fire = Instantiate(Fire, FirePoint.position, Quaternion.identity);
        GameObject particle = Instantiate(HitPoint, hit.point, Quaternion.identity);

        Destroy(fire, 1);
        Destroy(particle,1);

    }
}
