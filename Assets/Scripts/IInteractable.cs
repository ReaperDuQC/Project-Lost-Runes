using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public interface IInteractable
    {
        public void Interact();
        public string GetInteractText();
    }
}