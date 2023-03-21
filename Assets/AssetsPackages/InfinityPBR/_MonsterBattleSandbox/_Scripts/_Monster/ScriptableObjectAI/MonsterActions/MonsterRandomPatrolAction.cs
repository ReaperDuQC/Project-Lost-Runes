using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/RandomPatrol")]

public class MonsterRandomPatrolAction : MonsterAction
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
                = Random.Range(0, monsterController.waypointList.Count - 1);
        }
    }
}