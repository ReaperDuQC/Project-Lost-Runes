using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class HealthBar : UIBar
    {
        public override void Initialize(CharacterStats stats)
        {
            Health health = stats._health;
            _max = health.Max;
            SetMaxValue(_max);

            _current = health.Remaining;
            SetCurrentValue(_current);

            health._healthChanged += SetCurrentValue;
            health._maxHealthChanged += SetMaxValue;
        }
    }
}