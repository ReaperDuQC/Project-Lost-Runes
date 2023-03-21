using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterAction : ScriptableObject
{
    public abstract void MonsterAct(MonsterStateController monsterController);
}
