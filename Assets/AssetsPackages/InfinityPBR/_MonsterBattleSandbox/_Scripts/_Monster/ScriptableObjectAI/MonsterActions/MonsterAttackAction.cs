using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Attack")]

public class MonsterAttackAction : MonsterAction
{
    public override void MonsterAct(MonsterStateController monsterController)
    {
        AttackController(monsterController);
    }

    private void AttackController(MonsterStateController monsterController)
    {
        Debug.DrawRay(monsterController.EyesTransform.position, 
            monsterController.EyesTransform.forward.normalized * monsterController.AttackRange, Color.red);

        if (monsterController.ChaseTarget != null && !monsterController.IsTargetDead() &&
            InAttackRange(monsterController))
        {
            if (monsterController.CheckIfCountdownElapsed(monsterController.AttackCooldown))
            {
                monsterController.MonsterAttack.Attack
                    (monsterController.AttackForce, monsterController.AttackCooldown);
            }
        }
    }

    private bool InAttackRange(MonsterStateController monsterController)
    {
        if ((monsterController.ChaseTarget.transform.position 
            - monsterController.transform.position).sqrMagnitude 
            < monsterController.AttackRange * monsterController.AttackRange)
        {
            return true;
        }
        return false;
    }
}
