using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    [System.Serializable]
    public class StatBonus : MonoBehaviour
    {
        [SerializeField] int _level;
        [SerializeField] int _skillId;

        public int Level { get { return _level; } }
        public int SkillId { get { return _skillId; } }

        public StatBonus(int level, int skillId)
        {
            _level = level;
            _skillId = skillId;
        }
        public void UpdateBonusAttribute(int level, int skillId)
        {
            _level = level;
            _skillId = skillId;
        }
    }
}