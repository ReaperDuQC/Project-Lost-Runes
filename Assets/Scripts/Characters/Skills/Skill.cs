using System.Collections.Generic;
using UnityEngine;

namespace LostRunes 
{
    [CreateAssetMenu(fileName = "Skill", menuName = "LostRunes/Skill/Skill Data")]
    public class Skill : ScriptableObject
    {
        public string _skillName;
        public string _skillDescription;
        public Sprite _icon;
        public int _skillId;
        public int _level;
        
        public CharacterStatsData _minRequirement;
        public CharacterStatsData _maxRequirement;

        public List<int> _requiredSkillsId;
        public List<int> _lockedOutSkillsId;
    }
}