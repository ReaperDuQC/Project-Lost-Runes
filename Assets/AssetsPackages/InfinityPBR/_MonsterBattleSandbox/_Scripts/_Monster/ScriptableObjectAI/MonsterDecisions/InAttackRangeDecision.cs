using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/InAttackRange")]
public class InAttackRangeDecision : Decision
{
    public override bool Decide(MonsterStateController monsterController)
    {
        return InAttackRange(monsterController);
    }

    private bool InAttackRange(MonsterStateController monsterController)
    {
        if (monsterController.ChaseTarget.transform.position == null)
        {
            return false;
        }

        if ((monsterController.ChaseTarget.transform.position
            - monsterController.transform.position).sqrMagnitude
            < monsterController.SpecialAttackRange 
            * monsterController.SpecialAttackRange)
        {
            return true;
        }
        return false;
    }
}
