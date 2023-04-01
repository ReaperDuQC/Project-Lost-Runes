using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class LuckAttribute : Attribute
    {
        public LuckAttribute(int baseLevel) : base(baseLevel)
        {
        }
    }
}