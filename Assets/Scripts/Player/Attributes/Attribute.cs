using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class Attribute 
    {
        int _baseLevel;
        List<BonusAttribute> _bonusLevel;

        public int BaseLevel { get { return _baseLevel; } }
        public int BonusLevel 
        { 
            get 
            {
                int bonus = 0;
                foreach(BonusAttribute attr in _bonusLevel)
                {
                    bonus += attr.Level;
                }
                return bonus; 
            } 
        }
        public int TotalLevel {  get { return _baseLevel + BonusLevel; } }

        public Attribute(int baseLevel)
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
                if(bonus.SkillId == skillId)
                {
                    bonus.UpdateBonusAttribute(level, skillId);
                    return;
                }
            }

            _bonusLevel.Add(new BonusAttribute(level, skillId));
        }
    }
}