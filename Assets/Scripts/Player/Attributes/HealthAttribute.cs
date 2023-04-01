using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class HealthAttribute : Attribute
    {
        public HealthAttribute(int baseLevel) : base(baseLevel)
        {
        }
    }
}