using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostRunes
{
    public class LobbySaver : MonoBehaviour
    {
        public Lobby? _currentLobby;
        public static LobbySaver Instance;

        private void Awake()
        {
            Instance= this;
            DontDestroyOnLoad(gameObject);
        }
    }
}