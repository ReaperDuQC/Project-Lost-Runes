using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject caster;
    private float speed;
    private float range;
    private Vector3 travelDirection;
    private float distanceTraveled;

    public event Action<GameObject, GameObject> OnProjectileCollided;

    public void Fire(GameObject Caster, Vector3 Target, float Speed, float Range)
    {
        caster = Caster;
        speed = Speed;
        range = Range;

        travelDirection = Target - transform.position;
        //travelDirection.y = 0f;
        travelDirection.Normalize();
        Quaternion.LookRotation(Target);
        distanceTraveled = 0f;
    }

    void Update()
    {
        float distanceToTravel = speed * Time.deltaTime;
        transform.Translate(travelDirection * distanceToTravel);
        distanceTraveled += distanceToTravel;
        if(distanceTraveled > range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnProjectileCollided?.Invoke(caster, other.gameObject);
        Destroy(gameObject);
    }
}
