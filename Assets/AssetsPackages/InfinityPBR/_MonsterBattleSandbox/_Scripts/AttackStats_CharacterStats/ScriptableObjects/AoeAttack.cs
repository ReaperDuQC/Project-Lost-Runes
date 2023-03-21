using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AoeAttack.asset", menuName = "Attack/AOE")]
public class AoeAttack : AttackDefinition
{
    public float DamageRadius;
    public float EffectPrefabTimeInScene;
    public GameObject AoePrefab;

    public void FireAoeAttack(GameObject Caster, Vector3 Position, int layer)
    {
        var aoe = Instantiate(AoePrefab, Position, Quaternion.identity);
        Destroy(aoe, EffectPrefabTimeInScene);

        var collidedObjects = Physics.OverlapSphere(Position, DamageRadius);
        
        foreach(var collision in collidedObjects)
        {
            if (collision.isTrigger)
            {
                continue;
            }
            var collisionGameObject = collision.gameObject;
            if (Physics.GetIgnoreLayerCollision(layer, collisionGameObject.layer))
                continue;
            var casterStats = Caster.GetComponent<CharacterStats>();
            var collisionStats = collisionGameObject.GetComponent<CharacterStats>();
            var attack = CreateAttack(casterStats, collisionStats);

            var attackables = collisionGameObject.GetComponentsInChildren(typeof(IAttackable));
            foreach(IAttackable a in attackables)
            {
                a.OnAttack(Caster, attack);
            }
        }
    }
}
