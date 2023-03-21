using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedForce : MonoBehaviour, IAttackable
{
    public float ForceToAdd;
    private Rigidbody myRigidbody;

    public void OnAttack(GameObject attacker, Attack attack)
    {
        var forceDirection = transform.position - attacker.transform.position;
        forceDirection.y += 0.5f;
        forceDirection.Normalize();
        myRigidbody.AddForce(forceDirection * ForceToAdd);
    }

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }
}
