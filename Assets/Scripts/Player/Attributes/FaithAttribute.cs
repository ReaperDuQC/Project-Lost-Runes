using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class FaithAttribute : Attribute
    {
        public FaithAttribute(int baseLevel) : base(baseLevel)
        {
        }
    }
}