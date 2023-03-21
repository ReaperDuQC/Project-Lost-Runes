using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    GameObject gameObject { get; }
    void OnAttack(GameObject attacker, Attack attack);
}
