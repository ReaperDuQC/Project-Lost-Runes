using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LostRunes.Menu
{
    public class CustomMenuPanel : MonoBehaviour
    {
        [SerializeField] Selectable _defaultSelectable;

        public void SelectDefaultSelectable()
        {
            if(_defaultSelectable != null )
            {
                _defaultSelectable.Select();
            }
        }
    }
}