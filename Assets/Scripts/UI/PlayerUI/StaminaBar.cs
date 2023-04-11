using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class StaminaBar : UIBar
    {
        public override void Initialize(CharacterStats stats)
        {
            Stamina stamina = stats._stamina;
            _max = stamina.Max;
            SetMaxValue(_max);

            _current = stamina.Remaining;
            SetCurrentValue(_current);

            stamina._staminaChanged += SetCurrentValue;
            stamina._maxStaminaChanged += SetMaxValue;
        }
    }
}