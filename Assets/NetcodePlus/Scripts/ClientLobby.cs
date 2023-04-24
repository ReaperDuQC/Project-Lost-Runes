using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace NetcodePlus
{

    public class ClientLobby : MonoBehaviour
    {
        public UnityAction<bool> onConnect;
        public UnityAction<LobbyGameList> onRefreshList;
        public UnityAction<LobbyGame> onRefresh;
        public UnityAction<LobbyGame> onMatchmaking;

        private Dictionary<ulong, LobbyGame> room_list = new Dictionary<ulong, LobbyGame>();
        private RelayConnectData relay_data = null;
        private ulong client_id;
        private ulong joined_game_id;
        private byte[] extra_data = new byte[0];
        private bool connected = false;

        private MatchmakingRequest current_matchmaking = null;
        private float matchmaking_timer = 0f;
        private float refresh_timer = 0f;

        private static ClientLobby instance;

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            Client.SetDefaultUrl(NetworkData.Get().lobby_host, NetworkData.Get().lobby_port);
        }

        void Update()
        {
            if (!connected)
                return;

            //Refresh lobby
            refresh_timer += Time.deltaTime;
            if (refresh_timer > 1f)
            {
                refresh_timer = 0f;
                if (joined_game_id == 0)
                    KeepAlive();
                else
                    RefreshGame();
            }

            //Refresh matchmaking
            matchmaking_timer += Time.deltaTime;
            if (IsMatchmaking() && matchmaking_timer > 2f)
            {
                matchmaking_timer = 0f;
                RefreshMatchmaking();
            }
        }

        public async Task<bool> Connect()
        {
            Debug.Log("Connect to Lobby: " + NetworkData.Get().lobby_host);

            LobbyPlayer player = new LobbyPlayer(UserID, Username);
            WebResponse res = await Client.Send("connect", player);

            if (res.success)
            {
                client_id = res.GetInt64();
                connected = true;
                joined_game_id = 0;
                Client.SetClientID(client_id);
            }

            onConnect?.Invoke(res.success);
            return res.success;
        }

        public void SetConnectionExtraData(byte[] bytes)
        {
            extra_data = bytes;
        }

        public void SetConnectionExtraData(string data)
        {
            extra_data = System.Text.Encoding.UTF8.GetBytes(data);
        }

        public void SetConnectionExtraData<T>(T data) where T : INetworkSerializable, new()
        {
            extra_data = NetworkTool.NetSerialize(data);
        }

        public async void RefreshLobby()
        {
            WebResponse res = await Client.Send("refresh_list");
            LobbyGameList list = res.GetData<LobbyGameList>();
            if (res.success)
                onRefreshList?.Invoke(list);
        }

        public async void RefreshGame()
        {
            if (joined_game_id == 0)
                return;

            WebResponse res = await Client.Send("refresh", joined_game_id);
            GameRefresh game = res.GetData<GameRefresh>();
            if (res.success)
                onRefresh?.Invoke(game.game);
        }

        public async void JoinGame(ulong game_id)
        {
            if (!connected)
                return;

            WebResponse res = await Client.Send("join", game_id);
            GameRefresh game = res.GetData<GameRefresh>();
            if (res.success && game.valid)
            {
                joined_game_id = game.game.game_id;
                onRefresh?.Invoke(game.game);
            }
        }

        public async void QuitGame()
        {
            if (joined_game_id == 0)
                return;

            WebResponse res = await Client.Send("quit", joined_game_id);
            joined_game_id = 0;
            GameRefresh game = res.GetData<GameRefresh>();
            if (res.success)
                onRefresh?.Invoke(game.game);
        }

        public async void StartGame()
        {
            if (!connected || joined_game_id == 0)
                return;

            WebResponse res = await Client.Send("start", joined_game_id);
            GameRefresh game = res.GetData<GameRefresh>();
            if (res.success)
                onRefresh?.Invoke(game.game);
        }

        public async void SendChat(string text)
        {
            ChatMsg chat = new ChatMsg(Username, text);
            WebResponse res = await Client.Send("chat", chat);
            GameRefresh game = res.GetData<GameRefresh>();
            if (res.success)
                onRefresh?.Invoke(game.game);
        }

        public async void KeepAlive()
        {
            //Keep the current game on
            WebResponse res = await Client.Send("keep");
            if (res.success)
                connected = res.GetBool(); //Server returns if that client is still connected
        }

        public async void KeepAlive(ulong game_id, string[] user_list)
        {
            KeepMsg msg = new KeepMsg(game_id, user_list);
            await Client.Send("keep_list", msg); //Keep the current game on
        }

        //Only players requesting the same group will be matched
        public void StartMatchmaking(string group)
        {
            //Scene will need to be defined by ServerMatchmaker with onMatchmaking
            StartMatchmaking(group, "", NetworkData.Get().players_max);
        }
        
        public void StartMatchmaking(string group, string scene)
        {
            StartMatchmaking(group, scene, NetworkData.Get().players_max);
        }

        public void StartMatchmaking(string group, string scene, int nb_players)
        {
            MatchmakingRequest req = new MatchmakingRequest();
            req.group = group;
            req.scene = scene;
            req.players = nb_players;
            req.extra = extra_data;
            StartMatchmaking(req);
        }

        public async void StartMatchmaking(MatchmakingRequest req)
        {
            if (!connected)
                return;

            req.is_new = true;
            current_matchmaking = req;
            matchmaking_timer = 0f;

            Debug.Log("Start Matchmaking");

            WebResponse res = await Client.Send("matchmaking", req);
            LobbyGame result = res.GetData<LobbyGame>();
            ReceiveMatchmakingResult(result);
        }

        public async void RefreshMatchmaking()
        {
            if (current_matchmaking != null)
            {
                current_matchmaking.is_new = false;
                matchmaking_timer = 0f;
                WebResponse res = await Client.Send("matchmaking", current_matchmaking);
                LobbyGame result = res.GetData<LobbyGame>();
                ReceiveMatchmakingResult(result);
            }
        }

        public async void CancelMatchmaking()
        {
            Debug.Log("Cancel Matchmaking");
            current_matchmaking = null;
            matchmaking_timer = 0f;
            await Client.Send("cancel");
        }

        public async void Disconnect()
        {
            if (joined_game_id != 0)
                await Client.Send("quit", joined_game_id);
            connected = false;
            joined_game_id = 0;
        }

        private void ReceiveMatchmakingResult(LobbyGame result)
        {
            if (result == null)
                return; //Invalid result
            if (result.game_id != 0)
                current_matchmaking = null; //Success! Stop matchmaking
            onMatchmaking?.Invoke(result);
        }

        public async Task CreateGame(CreateGameData cdata)
        {
            if (NetworkData.Get().lobby_game_type == ServerType.RelayServer)
            {
                //Before creating a game on the lobby, need to create it on the relay server to get the join_code
                relay_data = await NetworkRelay.HostGame(cdata.players_max);
                if (relay_data == null)
                    return; //Failed to create relay game
                cdata.join_code = relay_data.join_code;
            }

            WebResponse res = await Client.Send("create", cdata);
            GameRefresh game = res.GetData<GameRefresh>();
            if (res.success && game.valid)
            {
                joined_game_id = game.game.game_id;
                onRefresh?.Invoke(game.game);
            }
        }

        public async Task ConnectToGame(LobbyGame game)
        {
            TheNetwork.Get().Disconnect(); //Disconnect previous connections
            await Task.Delay(500);

            if (!connected)
                return;

            bool host = game.IsHost(Client.GetClientID());
            TheNetwork.Get().SetConnectionExtraData(extra_data);
            TheNetwork.Get().SetLobbyGame(game);

            if (game.type == ServerType.DedicatedServer)
            {
                TheNetwork.Get().StartClient(game.server_host, game.server_port, host);
            }
            else if (game.type == ServerType.PeerToPeer)
            {
                if (host)
                {
                    TheNetwork.Get().StartHost(game.server_port);
                    TheNetwork.Get().LoadScene(game.scene);
                }
                else
                {
                    TheNetwork.Get().StartClient(game.server_host, game.server_port);
                }
            }
            else if (game.type == ServerType.RelayServer)
            {
                if (host)
                {
                    TheNetwork.Get().StartHostRelay(relay_data); //Relay data was already accessed when creating the game
                    TheNetwork.Get().LoadScene(game.scene);
                }
                else
                {
                    RelayConnectData data = await NetworkRelay.JoinGame(game.join_code); //Relay data need to be retrieved now
                    TheNetwork.Get().StartClientRelay(data);
                }
            }

            while (TheNetwork.Get().IsConnecting())
                await Task.Delay(200);
        }

        public CreateGameData GetCreateData(string title, string filename, string scene)
        {
            CreateGameData cdata = new CreateGameData(title, filename, scene);
            cdata.players_max = NetworkData.Get().players_max;
            cdata.join_code = ""; //This will be set later if using relay server
            cdata.hidden = false; //Game not hidden in lobby list
            return cdata;
        }

        public LobbyGame GetGame(ulong uid)
        {
            if (room_list.ContainsKey(uid))
                return room_list[uid];
            return null;
        }

        public bool IsMatchmaking()
        {
            return current_matchmaking != null;
        }

        public bool IsConnected() { return connected; }

        public WebClient Client { get { return WebClient.Get(); } }
        public string UserID { get { return Authenticator.Get().UserID; } }
        public string Username { get { return Authenticator.Get().Username; } }
        public ulong ClientID { get { return client_id; } }

        public static ClientLobby Get()
        {
            return instance;
        }
    }
}
