using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/SpecialAttackInPlace")]

public class SpecialAttackInPlaceAction : MonsterAction
{

    public override void MonsterAct
        (MonsterStateController monsterController)
    {
        SpecialAttack(monsterController);
    }

    private void SpecialAttack
        (MonsterStateController monsterController)
    {
        monsterController.MonsterSpecialAttack.Attack
            (monsterController.AttackForce, 
            monsterController.SpecialAttackCooldown);
    }
}
