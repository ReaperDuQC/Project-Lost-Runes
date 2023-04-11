using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    [System.Serializable]
    public class Health : Attribute
    {
        public delegate void OnHealthChanged(float health);
        public OnHealthChanged _healthChanged;

        public delegate void OnMaxHealthChanged(float maxHealth);
        public OnMaxHealthChanged _maxHealthChanged;

        public delegate void OnDeath();
        public OnDeath _onDeath;

        public Health() { }

        public override void Initialize(CharacterStats characterStats)
        {
            _ratePerCharacterLevel = 5f;

            _ratePerStatLevel1To20 = 10f;
            _ratePerStatLevel21To40 = 9f;
            _ratePerStatLevel41To60 = 8f;
            _ratePerStatLevel61To80 = 7f;
            _ratePerStatLevel81To100 = 6f;
            _ratePerStatLevel101ToPlus = 5f;

            int characterLevel = characterStats._characterLevel._currentLevel;
            int vitalityLevel = characterStats._vitalityStat.TotalLevel;

            _baseMax = 0;
            _baseMax += GetAmountPerPlayerLevel(characterLevel);
            _baseMax += GetAmountPerStatLevel(vitalityLevel);

            float amount = GetAmountForBonus();

            _baseMax += amount;

            _remaining = _baseMax;
        }

        public void TakeDamage(float damage)
        {
            _remaining -= damage;

            if (_remaining <= 0)
            {
                _remaining = 0f;
                Death();
            }

            _healthChanged?.Invoke(_remaining);
        }
        void Death()
        {
            _onDeath?.Invoke();
        }
    }
}