using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using TMPro;
using System;
using Unity.Netcode;
using Netcode.Transports.Facepunch;

namespace LostRunes
{
    public class SteamManager : MonoBehaviour
    {
        [SerializeField] int _playerAmount;
        private void OnEnable()
        {
            SteamMatchmaking.OnLobbyCreated += LobbyCreated;
            SteamMatchmaking.OnLobbyEntered += LobbyEntered;
            SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;
        }
        private void OnDisable()
        {
            SteamMatchmaking.OnLobbyCreated -= LobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
            SteamFriends.OnGameLobbyJoinRequested -= GameLobbyJoinRequested;
        }
        private async void GameLobbyJoinRequested(Lobby lobby, SteamId steamId)
        {
            await lobby.Join();
        }
        private void LobbyEntered(Lobby lobby)
        {
            LobbySaver.Instance._currentLobby = lobby;

            if (NetworkManager.Singleton.IsHost) return;
            NetworkManager.Singleton.GetComponent<FacepunchTransport>().targetSteamId = lobby.Owner.Id;
            NetworkManager.Singleton.StartClient();
            Debug.Log("Entered");
        }
        private void LobbyCreated(Result result, Lobby lobby)
        {
            if (result == Result.OK)
            {
                lobby.SetPublic();
                lobby.SetJoinable(true);

                NetworkManager.Singleton.StartHost();

                Debug.Log("Created");
            }
        }
        public void JoinViaSteam()
        {
            // Open the Steam overlay and show the friend list
            SteamFriends.OpenOverlay("Friends");
        }

        public async void HostLobby()
        {
            await SteamMatchmaking.CreateLobbyAsync(_playerAmount);
        }
        public async void JoinLobbyWithID()
        {
            //ulong Id;
            //if (!ulong.TryParse(_lobbyIDInputField.text, out Id))
            //    return;
            //Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();

            //foreach (Lobby lobby in lobbies)
            //{
            //    if (lobby.Id == Id)
            //    {
            //        await lobby.Join();
            //        return;
            //    }
            //}
        }
        public void CopyLobbyId()
        {
            //if (_lobbyId == null) return;

            TextEditor textEditor = new TextEditor();
            //textEditor.text = _lobbyId.text;
            textEditor.SelectAll();
            textEditor.Copy();
        }
        public void LeaveLobby()
        {
            LobbySaver.Instance._currentLobby?.Leave();
            LobbySaver.Instance._currentLobby = null;
            NetworkManager.Singleton.Shutdown();
        }
    }
}