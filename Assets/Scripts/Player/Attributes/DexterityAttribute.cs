using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LostRunes
{
    public class DexterityAttribute : Attribute
    {
        public DexterityAttribute(int baseLevel) : base(baseLevel)
        {
        }
    }
}