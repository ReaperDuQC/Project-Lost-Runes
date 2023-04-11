using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    [System.Serializable]
    public class Mana : Attribute
    {
        public delegate void OnManaChanged(float mana);
        public OnManaChanged _manaChanged;

        public delegate void OnMaxManaChanged(float maxMana);
        public OnMaxManaChanged _maxManaChanged;

        public Mana() { }

        public override void Initialize(CharacterStats characterStats)
        {
            _ratePerCharacterLevel = 1f;

            _ratePerStatLevel1To20 = 5f;
            _ratePerStatLevel21To40 = 7f;
            _ratePerStatLevel41To60 = 8f;
            _ratePerStatLevel61To80 = 9f;
            _ratePerStatLevel81To100 = 10f;
            _ratePerStatLevel101ToPlus = 10f;

            int characterLevel = characterStats._characterLevel._currentLevel;
            int intelligenceLevel = characterStats._intelligenceStat.TotalLevel;
            int faithLevel = characterStats._faithStat.TotalLevel;

            _baseMax = 0;
            _baseMax += GetAmountPerPlayerLevel(characterLevel);
            _baseMax += GetAmountPerStatLevel(intelligenceLevel);
            _baseMax += GetAmountPerStatLevel(faithLevel);

            _remaining = _baseMax;
        }

    }
}