using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class Stat
    {
        [SerializeField] int _baseLevel;
        [SerializeField] List<StatBonus> _bonusLevel = new();

        public int BaseLevel { get { return _baseLevel; } }
        public int BonusLevel
        {
            get
            {
                int bonus = 0;
                foreach (StatBonus stat in _bonusLevel)
                {
                    bonus += stat.Level;
                }
                return bonus;
            }
        }
        public int TotalLevel { get { return _baseLevel + BonusLevel; } }

        public Stat(int baseLevel)
        {
            _baseLevel = baseLevel;
        }

        public void LevelUp(int pointToAdd)
        {
            _baseLevel += pointToAdd;
        }
        public void ApplyBonusLevel(int level, int skillId)
        {
            foreach (var bonus in _bonusLevel)
            {
                if (bonus.SkillId == skillId)
                {
                    bonus.UpdateBonusAttribute(level, skillId);
                    return;
                }
            }

            _bonusLevel.Add(new StatBonus(level, skillId));
        }
    }
}