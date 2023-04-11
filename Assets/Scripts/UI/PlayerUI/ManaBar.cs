using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class ManaBar : UIBar
    {
        public override void Initialize(CharacterStats stats)
        {
            Mana mana = stats._mana;
            _max = mana.Max;
            SetMaxValue(_max);

            _current = mana.Remaining;
            SetCurrentValue(_current);

            mana._manaChanged += SetCurrentValue;
            mana._maxManaChanged += SetMaxValue;
        }
    }
}