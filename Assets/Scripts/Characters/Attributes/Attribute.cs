using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    [System.Serializable]
    public class Attribute 
    {
        [Header("Base Attribute")]
        [SerializeField] protected float _remaining;
        [SerializeField] protected float _baseMax;

        [Header("Bonus Attribute")]
        [SerializeField] List<float> _bonusMaxAmount = new(); // need to change the amount added to a structure that contain a float and what gave the bonus, with that I could check if a bonus giver already give its bonus, and change it if needed

        public float Remaining { get { return _remaining; } }
        public float Max { 
            get 
            {
                float bonusMax = 0f;

                foreach(var f in _bonusMaxAmount)
                {
                    bonusMax += f;
                }

                return _baseMax + bonusMax; 
            } 
        }
    
        [Header("Attribute Rate")]
        [SerializeField] protected float _ratePerCharacterLevel;

        [SerializeField] protected float _ratePerStatLevel1To20;
        [SerializeField] protected float _ratePerStatLevel21To40;
        [SerializeField] protected float _ratePerStatLevel41To60;
        [SerializeField] protected float _ratePerStatLevel61To80;
        [SerializeField] protected float _ratePerStatLevel81To100; 
        [SerializeField] protected float _ratePerStatLevel101ToPlus;

        [Header("Regen")]
        // amount given back to attribute every time it triggers
        [SerializeField] protected float _baseRegenAmount;
        // amount of time needed to trigger the regen on the attribute ex: / 2 sec
        [SerializeField] protected float _baseRegenInterval;
        // useful for creating a small delay, like after swinging a weapon, your stamina should wait x sec before starting to regen
        [SerializeField] protected float _baseRegenDelay;
        // can be used to toggle regen for that particular attribute, ex: no mana regen inside a dungeon or after getting hit by a spell
        [SerializeField] protected bool _hasRegen;

        [Header("Bonus Regen")]
        // amount given back to attribute every time it triggers
        [SerializeField] protected float _bonusRegenAmount;
        // amount of time needed to trigger the regen on the attribute ex: / 2 sec
        [SerializeField] protected float _bonusRegenInterval;
        // useful for creating a small delay, like after swinging a weapon, your stamina should wait x sec before starting to regen
        [SerializeField] protected float _bonusRegenDelay;

        public virtual void Initialize(CharacterStats characterStats)
        {

        }
        public void SetRemaining(float remaining)
        {
            _remaining = remaining;
        }
        public float GetAmountPerPlayerLevel(int playerLevel)
        {
            return playerLevel * _ratePerCharacterLevel;
        }
        public float GetAmountPerStatLevel(int statLevel)
        {
            float amount = 0f;

            amount += _ratePerStatLevel1To20 * Mathf.Min(statLevel, 20);
            amount += _ratePerStatLevel21To40 * Mathf.Clamp(statLevel - 20, 0, 20);
            amount += _ratePerStatLevel41To60 * Mathf.Clamp(statLevel - 40, 0, 20);
            amount += _ratePerStatLevel61To80 * Mathf.Clamp(statLevel - 60, 0, 20);
            amount += _ratePerStatLevel81To100 * Mathf.Clamp(statLevel - 80, 0, 20);
            amount += _ratePerStatLevel101ToPlus * Mathf.Max (statLevel - 100, 0);

            return amount;
        }

        public float GetAmountForBonus()
        {
            float amount = 0f;

            foreach(float bonus in _bonusMaxAmount)
            {
                amount += bonus;
            }

            return amount;
        }
        public void SetRegenActive(bool active)
        {
            _hasRegen = active;
        }

    }
}