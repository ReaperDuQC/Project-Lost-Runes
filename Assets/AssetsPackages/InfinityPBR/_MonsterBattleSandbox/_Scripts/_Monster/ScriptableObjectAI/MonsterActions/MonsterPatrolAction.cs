using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]

public class MonsterPatrolAction : MonsterAction
{
    public override void MonsterAct(MonsterStateController monsterController)
    {
        Patrol(monsterController);
    }

    private void Patrol(MonsterStateController monsterController)
    {
        monsterController.MonsterNavAgent.destination 
            = monsterController.waypointList[monsterController.NextWaypoint].position;
        monsterController.MonsterNavAgent.isStopped = false;

        if (monsterController.MonsterNavAgent.remainingDistance 
            <= monsterController.MonsterNavAgent.stoppingDistance 
            && !monsterController.MonsterNavAgent.pathPending)
        {
            monsterController.NextWaypoint 
                = (monsterController.NextWaypoint + 1) % monsterController.waypointList.Count;
        }
    }
}
