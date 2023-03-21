using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/StopMovement")]
public class MonsterStopMovementAction : MonsterAction
{
    public override void MonsterAct(MonsterStateController monsterController)
    {
        monsterController.MonsterNavAgent.isStopped = true;
    }
}
