using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class BonusAttribute
    {
        int _level;
        int _skillId;

        public int Level { get { return _level; } }
        public int SkillId { get { return _skillId; } }

        public BonusAttribute(int level, int skillId)
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