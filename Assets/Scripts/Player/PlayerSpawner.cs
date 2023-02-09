using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using UnityEditor.Rendering;
using Unity.Services.Lobbies.Models;
using UnityEditor.PackageManager;

namespace LostRunes
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] GameManager _playerPrefab;
        private void Start()
        {
            DontDestroyOnLoad(this);
        }
        public override void OnNetworkSpawn()
        {
            Debug.Log("OnNetworkSpawn");
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
        }

        private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            Debug.Log("SceneLoaded");
            if (IsHost && sceneName == "Sandbox")
            {
                foreach (ulong id in clientsCompleted)
                {
                    GameManager player = Instantiate(_playerPrefab);
                    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
                }
            }
        }
    }
}