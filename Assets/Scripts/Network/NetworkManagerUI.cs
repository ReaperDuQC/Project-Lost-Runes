using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace LostRunes.Multiplayer
{
    public class NetworkManagerUI : MonoBehaviour
    {
        public void Host()
        {
            NetworkManager.Singleton.StartHost();
        }
        public void Join()
        {
            NetworkManager.Singleton.StartClient();
        }
        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
        }
        public int CheckAvailableGames()
        {
            return 0;// NetworkManager.Singleton.
        }
    }
}