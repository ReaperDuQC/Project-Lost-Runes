using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public interface IInteractable
    {
        public void Interact();
        public string GetInteractText();
    }
}