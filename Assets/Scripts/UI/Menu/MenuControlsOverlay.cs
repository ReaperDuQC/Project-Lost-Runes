using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes.Menu
{
    public class MenuControlsOverlay : MonoBehaviour
    {
        [SerializeField] GameObject _actionsPrefab;
        public void SetActionsOverlay(List<MenuActions> actions)
        {
            if (_actionsPrefab == null) return;

            foreach(var action in actions)
            {

            }
        }
    }
}