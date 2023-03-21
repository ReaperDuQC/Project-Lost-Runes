using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/Chase")]
public class MonsterChaseAction : MonsterAction
{
    public override void MonsterAct(MonsterStateController monsterController)
    {
        Chase(monsterController);
    }

    private void Chase(MonsterStateController monsterController)
    {
        if (monsterController.ChaseTarget == null) { return; }
        monsterController.LookAtTarget();
        monsterController.MonsterNavAgent.destination = monsterController.ChaseTarget.position;
        monsterController.MonsterNavAgent.isStopped = false;
    }
}
