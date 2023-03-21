using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/SpecialAttack")]

public class MonsterSpecialAttackAction : MonsterAction
{
    public override void MonsterAct(MonsterStateController monsterController)
    {
        SpecialAttackController(monsterController);
    }

    private void SpecialAttackController(MonsterStateController monsterController)
    {
        Debug.DrawRay(monsterController.EyesTransform.position,
            monsterController.EyesTransform.forward.normalized * monsterController.SpecialAttackRange, Color.red);

        if (monsterController.ChaseTarget != null && !monsterController.IsTargetDead() &&
            InAttackRange(monsterController))
        {
            monsterController.MonsterSpecialAttack.Attack
                   (monsterController.AttackForce, monsterController.SpecialAttackCooldown);
        }
    }

    private bool InAttackRange(MonsterStateController monsterController)
    {
        if ((monsterController.ChaseTarget.transform.position
            - monsterController.transform.position).sqrMagnitude
            < monsterController.SpecialAttackRange * monsterController.SpecialAttackRange)
        {
            return true;
        }
        return false;
    }
}
