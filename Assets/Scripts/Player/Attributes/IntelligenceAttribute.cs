using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class IntelligenceAttribute : Attribute
    {
        public IntelligenceAttribute(int baseLevel) : base(baseLevel)
        {
        }
    }
}