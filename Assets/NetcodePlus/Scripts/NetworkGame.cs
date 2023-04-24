using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace NetcodePlus
{
    /// <summary>
    /// Network manager that get instantiated in the game scene only, unlike TheNetwork, it doesn't persist between scenes
    /// </summary>

    public class NetworkGame : MonoBehaviour
    {
        private NetworkSpawner spawner;
        private NetworkChat chat;
        private float update_timer = 0f;
        private float keep_timer = 0f;
        private bool keep_valid = false;

        private const float spawn_refresh_rate = 0.5f; //In seconds, interval at which SNetworkObjects are spawned/despawned
        private const float keep_refresh_rate = 10f; //Every 10 seconds, send refresh to lobby to keep the game listed

        private static NetworkGame instance;

        private void Awake()
        {
            instance = this;
            spawner = new NetworkSpawner();
            chat = new NetworkChat();
        }

        private void Start()
        {
            TheNetwork.Get().onTick += TickUpdate;
            Messaging.ListenMsg("action", ReceiveAction);
            Messaging.ListenMsg("variable", ReceiveVariable);
            Messaging.ListenMsg("spawn", ReceiveSpawnList);
            Messaging.ListenMsg("despawn", ReceiveDespawnList);
            Messaging.ListenMsg("change_owner", ReceiveChangeList);
            InitLobbyKeep();
            chat.Init();
        }

        private void OnDestroy()
        {
            TheNetwork.Get().onTick -= TickUpdate;
            Messaging?.UnListenMsg("action");
            Messaging?.UnListenMsg("variable");
            Messaging?.UnListenMsg("spawn");
            Messaging?.UnListenMsg("despawn");
            Messaging?.UnListenMsg("change_owner");
            SNetworkActions.ClearAll();
            SNetworkVariableBase.ClearAll();
            chat.Clear();
        }

        private void Update()
        {
            if (IsServer && IsReady)
            {
                UpdateVisibility();
            }

            /*if (IsClient)
            {
                KeepAliveLobby();
            }*/
            
            if (IsServer)
            {
                KeepAliveLobbyList();
            }
        }

        private void TickUpdate()
        {
            if (IsServer && IsReady)
            {
                spawner.TickUpdate();
            }

            if (IsReady)
            {
                SNetworkVariableBase.TickAll();
            }
        }

        private void InitLobbyKeep()
        {
            if (!IsOnline)
                return; //Not online

            WebClient client = WebClient.Get();
            client?.SetDefaultUrl(NetworkData.Get().lobby_host, NetworkData.Get().lobby_port);

            LobbyGame game = TheNetwork.Get().GetLobbyGame(); //Make sure we are playing a lobby game
            if (client != null && game != null)
            {
                keep_valid = IsServer && !IsClient; //Always valid if server and lobby game exists

                LobbyPlayer player = game.GetPlayer(TheNetwork.Get().UserID);
                if (player != null)
                {
                    client.SetClientID(player.client_id);
                    keep_valid = true; //Valid lobby game
                }
            }
        }

        //Send a keep alive to the lobby, to keep the current game listed on the lobby (otherwise it will get deleted if inactivity)
        private async void KeepAliveLobby()
        {
            if (!keep_valid)
                return;

            //Slow update
            keep_timer += Time.deltaTime;
            if (keep_timer < keep_refresh_rate)
                return;

            keep_timer = 0f;

            WebClient client = WebClient.Get();
            if (client != null)
            {
                await client.Send("keep");
            }
        }

        private async void KeepAliveLobbyList()
        {
            if (!keep_valid)
                return;

            //Slow update
            keep_timer += Time.deltaTime;
            if (keep_timer < keep_refresh_rate)
                return;

            keep_timer = 0f;

            WebClient web = WebClient.Get();
            LobbyGame game = TheNetwork.Get().GetLobbyGame();
            if (web != null && game != null)
            {
                int index = 0;
                string[] list = new string[TheNetwork.Get().CountClients()];
                foreach (KeyValuePair<ulong, ClientData> pair in TheNetwork.Get().GetClientsData())
                {
                    ClientData client = pair.Value;
                    if (client != null && index < list.Length)
                    {
                        list[index] = client.user_id;
                        index++;
                    }
                }

                if (list.Length > 0)
                {
                    KeepMsg msg = new KeepMsg(game.game_id, list);
                    await web.Send("keep_list", msg);
                }
            }
        }

        //Spawn Despawn objects based on distance
        private void UpdateVisibility()
        {
            //Slow update
            update_timer += Time.deltaTime;
            if (update_timer < spawn_refresh_rate)
                return;

            update_timer = 0f;

            //Optimization Loop
            List<SNetworkOptimizer> objs = SNetworkOptimizer.GetAll();
            foreach (SNetworkOptimizer obj in objs)
            {
                float dist = 999f;
                foreach (SNetworkPlayer character in SNetworkPlayer.GetAll())
                {
                    float pdist = (obj.GetPos() - character.GetPos()).magnitude;
                    dist = Mathf.Min(dist, pdist);
                }

                obj.SetActive(dist < obj.active_range);
            }
        }

        private void ReceiveAction(ulong client_id, FastBufferReader reader)
        {
            if (client_id != TheNetwork.Get().ClientID)
            {
                reader.ReadValueSafe(out ulong object_id);
                reader.ReadValueSafe(out ushort behaviour_id);
                reader.ReadValueSafe(out ushort type);
                reader.ReadValueSafe(out ushort delivery);
                SNetworkActions handler = SNetworkActions.GetHandler(object_id, behaviour_id);
                handler?.ReceiveAction(client_id, type, reader, (NetworkDelivery)delivery);
            }
        }

        private void ReceiveVariable(ulong client_id, FastBufferReader reader)
        {
            if (client_id != TheNetwork.Get().ClientID)
            {
                reader.ReadValueSafe(out ulong object_id);
                reader.ReadValueSafe(out ushort behaviour_id);
                reader.ReadValueSafe(out ushort variable_id);
                reader.ReadValueSafe(out ushort delivery);
                SNetworkVariableBase handler = SNetworkVariableBase.GetVariable(object_id, behaviour_id, variable_id);
                handler?.ReceiveVariable(client_id, reader, (NetworkDelivery)delivery);
            }
        }

        private void ReceiveSpawnList(ulong client_id, FastBufferReader reader)
        {
            //Is not server and the sender is the server
            if (!IsServer && client_id == TheNetwork.Get().ServerID)
            {
                reader.ReadNetworkSerializable(out NetSpawnList list);
                foreach (NetSpawnData data in list.data)
                {
                    spawner.SpawnClient(data);
                }
            }
        }

        private void ReceiveDespawnList(ulong client_id, FastBufferReader reader)
        {
            //Is not server and the sender is the server
            if (!IsServer && client_id == TheNetwork.Get().ServerID)
            {
                reader.ReadNetworkSerializable(out NetDespawnList list);
                foreach (NetDespawnData data in list.data)
                {
                    spawner.DespawnClient(data.network_id, data.destroy);
                }
            }
        }

        private void ReceiveChangeList(ulong client_id, FastBufferReader reader)
        {
            //Is not server and the sender is the server
            if (!IsServer && client_id == TheNetwork.Get().ServerID)
            {
                reader.ReadNetworkSerializable(out NetChangeList list);
                foreach (NetChangeData data in list.data)
                {
                    spawner.ChangeOwnerClient(data.network_id, data.owner);
                }
            }
        }

        public NetworkSpawner Spawner { get { return spawner; } }
        public NetworkChat Chat { get { return chat; } }
        public NetworkMessaging Messaging { get { return TheNetwork.Get().Messaging; } }

        public bool IsOnline { get { return TheNetwork.Get().IsOnline; } }
        public bool IsServer { get { return TheNetwork.Get().IsServer; } }
        public bool IsClient { get { return TheNetwork.Get().IsClient; } }
        public bool IsReady { get { return TheNetwork.Get().IsReady(); } }

        public static NetworkGame Get()
        {
            if (instance == null)
                instance = FindObjectOfType<NetworkGame>();
            return instance;
        }
    }
}
