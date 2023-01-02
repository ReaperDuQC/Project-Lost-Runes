using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LostRunes
{
    public class StringToClipBoard : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _lobbyId;
       public void CopyStringToClipBoard()
        {
            if(_lobbyId == null) return;

            GUIUtility.systemCopyBuffer = _lobbyId.text;
        }
    }
}