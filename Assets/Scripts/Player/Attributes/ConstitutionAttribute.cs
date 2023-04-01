using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class ConstitutionAttribute : Attribute
    {
        public ConstitutionAttribute(int baseLevel) : base(baseLevel)
        {
        }
    }
}