using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class MenuNavigator : MonoBehaviour
    {
        UIButton _pressedButton = null;

        void SetUIButton(UIButton uIButton)
        {
            _pressedButton = uIButton;
        }
    }
}