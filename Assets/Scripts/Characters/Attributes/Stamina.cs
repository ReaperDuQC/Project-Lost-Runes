using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    [System.Serializable]
    public class Stamina : Attribute
    {
        public delegate void OnStaminaChanged(float stamina);
        public OnStaminaChanged _staminaChanged;
        public delegate void OnMaxStaminaChanged(float maxStamina);
        public OnMaxStaminaChanged _maxStaminaChanged;

        public Stamina() { }

        public override void Initialize(CharacterStats characterStats)
        {
            _ratePerCharacterLevel = 2f;

            _ratePerStatLevel1To20 = 10f;
            _ratePerStatLevel21To40 = 8f;
            _ratePerStatLevel41To60 = 6f;
            _ratePerStatLevel61To80 = 5f;
            _ratePerStatLevel81To100 = 4f;
            _ratePerStatLevel101ToPlus = 3f;

            int characterLevel = characterStats._characterLevel._currentLevel;
            int enduranceLevel = characterStats._enduranceStat.TotalLevel;

            _baseMax = 0;
            _baseMax += GetAmountPerPlayerLevel(characterLevel);
            _baseMax += GetAmountPerStatLevel(enduranceLevel);

            _remaining = _baseMax;
        }
    }
}