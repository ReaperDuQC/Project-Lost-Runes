using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class PickUpItem : MonoBehaviour, IInteractable
    {
        string _interactText = "Pick Up Item";
        [SerializeField] Item _item;

        public void Interact()
        {

        }
        public string GetInteractText()
        {
            return _interactText;
        }
        public void PickItem()
        {
            if (_item == null) return;
            FadeVFX();
        }
        private void FadeVFX()
        {

        }
    }
}