using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostRunes
{
    public class GameNetworkManager : MonoBehaviour
    {
        public static GameNetworkManager Instance { get; private set; } = null;

        private FacepunchTransport _transport = null;
        [SerializeField] int _playerAmount = 4;
        public Lobby? CurrentLobby { get; private set; } = null;
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            _transport = GetComponent<FacepunchTransport>();
        }
        private void OnDestroy()
        {
            //Steamworks.SteamClient.Shutdown();
        }
        private void OnEnable()
        {
            SteamMatchmaking.OnLobbyCreated += LobbyCreated;
            SteamMatchmaking.OnLobbyEntered += LobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += LobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += LobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite += LobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated += LobbyGameCreated;

            SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;

            if (NetworkManager.Singleton == null) { return; }

            if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectCallback;
                NetworkManager.Singleton.OnServerStarted += ServerStarted;
            }
        }
        private void OnDisable()
        {
            SteamMatchmaking.OnLobbyCreated -= LobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= LobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= LobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite -= LobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated -= LobbyGameCreated;

            SteamFriends.OnGameLobbyJoinRequested -= GameLobbyJoinRequested;

            if (NetworkManager.Singleton == null) { return; }

            if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnectCallback;
                NetworkManager.Singleton.OnServerStarted -= ServerStarted;
            }
        }
        private void OnApplicationQuit() => Disconnect();

        public async void StartHost()
        {
            NetworkManager.Singleton.StartHost();

            CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(_playerAmount);
        }
        public void StartClient(SteamId id)
        {
            _transport.targetSteamId = id;

            NetworkManager.Singleton.StartClient();
        }

        public void Disconnect()
        {
            CurrentLobby?.Leave();

            if(NetworkManager.Singleton == null) { return; }

            NetworkManager.Singleton.Shutdown();
        }

        #region Netcode Callbacks

        private void ServerStarted()
        {
            Debug.Log("Server has started");
        }
        private void ClientConnectedCallback(ulong clientId)
        {
            Debug.Log($"Client connected, clientId={clientId}");
            SceneLoaderManager.Instance.LoadScene();
        }
        private void ClientDisconnectCallback(ulong clientId)
        {
            Debug.Log($"Client disconnected, clientId={clientId}");
        }

        #endregion

        #region Steam Callbacks
        private void LobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id) { }

        private void LobbyInvite(Friend friend, Lobby lobby) { }

        private void LobbyMemberLeave(Lobby lobby, Friend friend) { }

        private void LobbyMemberJoined(Lobby lobby, Friend friend) { }

        private void GameLobbyJoinRequested(Lobby lobby, SteamId steamId) => StartClient(steamId);

        private void LobbyEntered(Lobby lobby)
        {
            if (NetworkManager.Singleton.IsHost) { return; }

            StartClient(lobby.Id);
        }
        private void LobbyCreated(Result result, Lobby lobby)
        {
            if(result != Result.OK)
            {
                Debug.Log("Lobby could not be created");
                return;
            }

            lobby.SetFriendsOnly();
            lobby.SetJoinable(true);
            Debug.Log("Lobby Created");
        }
        #endregion

        public void JoinViaSteam()
        {
            // Open the Steam overlay and show the friend list
            SteamFriends.OpenOverlay("Friends");
        }

        IEnumerator WaitForNetworkManagerCoroutine()
        {
            yield return new WaitUntil(() => NetworkManager.Singleton != null);
        }
    }
}