using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class StrengthAttribute : Attribute
    {
        public StrengthAttribute(int baseLevel) : base(baseLevel)
        {
        }
    }
}