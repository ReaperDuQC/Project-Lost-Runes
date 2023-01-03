using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using TMPro;
using System;
using Unity.Netcode;

namespace LostRunes
{
    public class SteamManager : MonoBehaviour
    {
        [SerializeField] TMP_InputField _lobbyIDInputField;
        [SerializeField] TextMeshProUGUI _lobbyId;
        [SerializeField] TextMeshProUGUI _lobbyPlayerCount;
        [SerializeField] int _playerAmount = 4;

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

            DisplayLobbyId(lobby);
            DisplayPlayerCount(lobby);

            Debug.Log("Entered");
        }
        [ClientRpc]
        private void DisplayPlayerCount(Lobby lobby)
        {
            if (_lobbyPlayerCount != null)
            {
                _lobbyPlayerCount.gameObject.SetActive(true);
                _lobbyPlayerCount.text = lobby.MemberCount.ToString() + "/" + lobby.MaxMembers.ToString();
            }
        }
        private void DisplayLobbyId(Lobby lobby)
        {
            if (_lobbyId != null)
            {
                _lobbyId.gameObject.SetActive(true);
                _lobbyId.text = lobby.Id.ToString();
            }
        }
        private void LobbyCreated(Result result, Lobby lobby)
        {
            if (result == Result.OK)
            {
                lobby.SetPublic();
                lobby.SetJoinable(true);

                Debug.Log("Created");
            }
        }
        public async void HostLobby()
        {
            await SteamMatchmaking.CreateLobbyAsync(_playerAmount);
        }
        public async void JoinLobbyWithID()
        {
            ulong Id;
            if (!ulong.TryParse(_lobbyIDInputField.text, out Id))
                return;
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();

            foreach (Lobby lobby in lobbies)
            {
                if (lobby.Id == Id)
                {
                    await lobby.Join();
                    return;
                }
            }
        }
        public void CopyLobbyId()
        {
            if (_lobbyId == null) return;

            TextEditor textEditor = new TextEditor();
            textEditor.text = _lobbyId.text;
            textEditor.SelectAll();
            textEditor.Copy();
        }
        public void LeaveLobby()
        {
            LobbySaver.Instance._currentLobby?.Leave();
            LobbySaver.Instance._currentLobby = null;
        }
    }
}