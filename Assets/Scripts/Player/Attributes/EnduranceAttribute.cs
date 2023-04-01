using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class EnduranceAttribute : Attribute
    {
        public EnduranceAttribute(int baseLevel) : base(baseLevel) { }
    }
}