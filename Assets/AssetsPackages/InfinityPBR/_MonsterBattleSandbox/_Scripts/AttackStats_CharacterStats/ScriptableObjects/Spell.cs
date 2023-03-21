using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Spell.asset", menuName = "Attack/Spell")]
public class Spell : AttackDefinition
{
    public Projectile ProjectileToFire;
    public float ProjectileSpeed;

    public void Cast
        (GameObject Caster, Vector3 SpellSpawnPosition, Vector3 TargetPosition, int ProjectileLayer)
    {
        Projectile projectile = Instantiate(ProjectileToFire, SpellSpawnPosition, Quaternion.identity);
        projectile.Fire(Caster, TargetPosition, ProjectileSpeed, range);
        projectile.gameObject.layer = ProjectileLayer;

        projectile.OnProjectileCollided += OnProjectileCollided;
    }

    private void OnProjectileCollided(GameObject Caster, GameObject Target)
    {
        if (Caster == null || Target == null)
            return;

        var casterStats = Caster.GetComponent<CharacterStats>();
        var targetStats = Target.GetComponent<CharacterStats>();
        var attack = CreateAttack(casterStats, targetStats);

        var attackables = Target.GetComponents(typeof(IAttackable));
        foreach(IAttackable a in attackables)
        {
            a.OnAttack(Caster, attack);
        }
    }
}
