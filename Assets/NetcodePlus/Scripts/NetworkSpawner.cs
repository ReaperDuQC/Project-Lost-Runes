using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace NetcodePlus
{
    public class NetworkSpawner 
    {
        private Dictionary<ulong, SNetworkObject> spawned_list = new Dictionary<ulong, SNetworkObject>();
        private Dictionary<ulong, SNetworkObject> despawned_list = new Dictionary<ulong, SNetworkObject>();
        private Dictionary<ulong, SNetworkObject> scene_list = new Dictionary<ulong, SNetworkObject>();
        private List<ulong> destroyed_scene_list = new List<ulong>();

        private Queue<SpawnQueueItem> spawn_list_target = new Queue<SpawnQueueItem>(); //Spawn list to newly connected client
        private Queue<DespawnQueueItem> despawn_list_target = new Queue<DespawnQueueItem>(); //Despawn list to newly connected client
        private List<NetSpawnData> spawn_list = new List<NetSpawnData>();              //Spawn list of last frame
        private List<NetDespawnData> despawn_list = new List<NetDespawnData>();        //Despawn list of last frame
        private List<NetChangeData> change_list = new List<NetChangeData>();        //Change owner list of last frame

        private const int spawn_list_max = 50; //Maximum number of objects that can be spawned in 1 request

        public void TickUpdate()
        {
            if (!IsServer || !IsOnline)
                return;
            
            if (spawn_list_target.Count > 0)
            {
                SpawnQueueItem item = spawn_list_target.Dequeue();
                SpawnListTarget(item);
                return;
            }

            if (despawn_list_target.Count > 0)
            {
                DespawnQueueItem item = despawn_list_target.Dequeue();
                DespawnListTarget(item);
                return;
            }

            if (spawn_list.Count > 0)
            {
                int count = Mathf.Min(spawn_list.Count, spawn_list_max);
                NetSpawnData[] data = new NetSpawnData[count];
                for (int i = 0; i < count; i++)
                    data[i] = spawn_list[i];
                spawn_list.RemoveRange(0, count);
                NetSpawnList nlist = new NetSpawnList(data);
                SpawnList(nlist);
                return;
            }

            if (despawn_list.Count > 0)
            {
                int count = Mathf.Min(despawn_list.Count, spawn_list_max);
                NetDespawnData[] data = new NetDespawnData[count];
                for (int i = 0; i < count; i++)
                    data[i] = despawn_list[i];
                despawn_list.RemoveRange(0, count);
                NetDespawnList nlist = new NetDespawnList(data);
                DespawnList(nlist);
                return;
            }

            if (change_list.Count > 0)
            {
                int count = Mathf.Min(change_list.Count, spawn_list_max);
                NetChangeData[] data = new NetChangeData[count];
                for (int i = 0; i < count; i++)
                    data[i] = change_list[i];
                change_list.RemoveRange(0, count);
                NetChangeList nlist = new NetChangeList(data);
                ChangeOwnerList(nlist);
                return;
            }
        }

        private void SpawnListTarget(SpawnQueueItem item)
        {
            if (item.list.data != null && item.list.data.Length > 0)
            {
                Messaging.SendObject("spawn", item.client_id, item.list, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void DespawnListTarget(DespawnQueueItem item)
        {
            if (item.list.data != null && item.list.data.Length > 0)
            {
                Messaging.SendObject("despawn", item.client_id, item.list, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void SpawnList(NetSpawnList list)
        {
            if (list.data != null && list.data.Length > 0)
            {
                NetworkDelivery delivery = list.data.Length > 5 ? NetworkDelivery.ReliableFragmentedSequenced : NetworkDelivery.ReliableSequenced;
                Messaging.SendObjectAll("spawn", list, delivery);
            }
        }

        private void DespawnList(NetDespawnList list)
        {
            if (list.data != null && list.data.Length > 0)
            {
                NetworkDelivery delivery = list.data.Length > 5 ? NetworkDelivery.ReliableFragmentedSequenced : NetworkDelivery.ReliableSequenced;
                Messaging.SendObjectAll("despawn", list, delivery);
            }
        }

        private void ChangeOwnerList(NetChangeList list)
        {
            if (list.data != null && list.data.Length > 0)
            {
                NetworkDelivery delivery = list.data.Length > 5 ? NetworkDelivery.ReliableFragmentedSequenced : NetworkDelivery.ReliableSequenced;
                Messaging.SendObjectAll("change_owner", list, delivery);
            }
        }

        public void Spawn(SNetworkObject nobj)
        {
            if (nobj == null || nobj.network_id == 0 || !IsServer)
                return;

            //Debug.Log("Spawn: " + nobj.name);

            spawned_list[nobj.NetworkId] = nobj;
            despawned_list.Remove(nobj.NetworkId);

            if (IsOnline)
            {
                //Send spawn request
                NetSpawnData spawn = new NetSpawnData(nobj);
                spawn.pos = nobj.transform.position;
                spawn.rot = nobj.transform.rotation;
                spawn.extra = nobj.WriteBehaviorSpawnData();
                spawn_list.Add(spawn);
            }
        }

        public void Despawn(SNetworkObject nobj, bool destroy)
        {
            if (nobj == null || !IsServer)
                return;

            //Debug.Log("Despawn: " + nobj.name);

            spawned_list.Remove(nobj.NetworkId);

            if(!destroy && !nobj.IsSceneObject)
                despawned_list[nobj.network_id] = nobj;

            if (destroy && nobj.IsSceneObject)
                destroyed_scene_list.Add(nobj.network_id); //Save destroyed scene objects to notify new connecting clients

            if (IsOnline)
            {
                //Send despawn request
                NetDespawnData spawn = new NetDespawnData(nobj.NetworkId, destroy);
                despawn_list.Add(spawn);
            }
        }

        public void DestroyScene(SNetworkObject nobj)
        {
            if (IsServer && nobj.IsSceneObject)
                destroyed_scene_list.Add(nobj.network_id); //Save destroyed scene objects to notify new connecting clients
        }

        public void ChangeOwner(SNetworkObject nobj)
        {
            if (nobj == null || !IsServer)
                return;

            if (IsOnline)
            {
                //Send change request
                NetChangeData change = new NetChangeData(nobj.NetworkId, nobj.OwnerId);
                change_list.Add(change);
            }
        }

        //Spawn and Despawn object on a newly connected client
        public void SpawnClientObjects(ulong client_id)
        {
            if (!IsServer || !IsOnline || client_id == 0)
                return;

            //Send spawns
            List<NetSpawnData> list = new List<NetSpawnData>();
            foreach (KeyValuePair<ulong, SNetworkObject> item in spawned_list)
            {
                if (item.Value != null)
                {
                    //Call OnBeforeSpawn again before spawning on new client
                    item.Value.onBeforeSpawn?.Invoke();

                    NetSpawnData spawn = new NetSpawnData(item.Value);
                    spawn.pos = item.Value.transform.position;
                    spawn.rot = item.Value.transform.rotation;
                    spawn.extra = item.Value.WriteBehaviorSpawnData();
                    list.Add(spawn);

                    if (list.Count >= spawn_list_max)
                    {
                        SpawnQueueItem qitem = new SpawnQueueItem();
                        qitem.list.data = list.ToArray();
                        qitem.client_id = client_id;
                        spawn_list_target.Enqueue(qitem);
                        list = new List<NetSpawnData>();
                    }
                }
            }

            if (list.Count > 0)
            {
                SpawnQueueItem qitem = new SpawnQueueItem();
                qitem.list.data = list.ToArray();
                qitem.client_id = client_id;
                spawn_list_target.Enqueue(qitem);
            }

            //Send despawn
            List<NetDespawnData> dlist = new List<NetDespawnData>();
            foreach (ulong id in destroyed_scene_list)
            {
                NetDespawnData despawn = new NetDespawnData(id, true);
                dlist.Add(despawn);

                if (dlist.Count >= spawn_list_max)
                {
                    DespawnQueueItem qitem = new DespawnQueueItem();
                    qitem.list.data = dlist.ToArray();
                    qitem.client_id = client_id;
                    despawn_list_target.Enqueue(qitem);
                    dlist = new List<NetDespawnData>();
                }
            }

            if (dlist.Count > 0)
            {
                DespawnQueueItem qitem = new DespawnQueueItem();
                qitem.list.data = dlist.ToArray();
                qitem.client_id = client_id;
                despawn_list_target.Enqueue(qitem);
            }
        }

        public void SpawnPrefabClient(NetSpawnData data)
        {
            if (IsServer || data.network_id == 0 || data.prefab_id == 0)
                return;

            GameObject prefab = TheNetwork.Get().GetPrefab(data.prefab_id);
            if (prefab != null)
            {
                GameObject obj = GameObject.Instantiate(prefab, data.pos, data.rot);
                SNetworkObject nobj = obj.GetComponent<SNetworkObject>();
                if (nobj != null)
                {
                    nobj.network_id = data.network_id;
                    nobj.prefab_id = data.prefab_id;
                    nobj.is_scene = false;
                    spawned_list[nobj.NetworkId] = nobj;
                    nobj.SpawnLocal(data.owner, data.extra);
                }
            }
            else
            {
                Debug.Log("Could not find Spawn Object :" + data.network_id + " " + data.prefab_id);
            }
        }

        public void SpawnClient(NetSpawnData data)
        {
            if (data.network_id == 0 || IsServer)
                return;

            SNetworkObject nobj = FindNetObject(data.network_id);
            if (nobj != null)
            {
                spawned_list[nobj.NetworkId] = nobj;
                despawned_list.Remove(nobj.NetworkId);
                nobj.SpawnLocal(data.owner, data.extra);
            }
            else
            {
                SpawnPrefabClient(data);
            }
        }

        public void DespawnClient(ulong net_id, bool destroy)
        {
            if (net_id == 0 || IsServer)
                return;

            SNetworkObject nobj = FindNetObject(net_id);
            if (nobj != null)
            {
                spawned_list.Remove(nobj.NetworkId);
                nobj.DespawnLocal(destroy);

                if (!destroy && !nobj.IsSceneObject)
                    despawned_list[nobj.network_id] = nobj;
            }
        }

        public void ChangeOwnerClient(ulong net_id, ulong owner)
        {
            if (net_id == 0 || IsServer)
                return;

            SNetworkObject nobj = GetSpawnedObject(net_id);
            if (nobj != null)
            {
                nobj.ChangeOwner(owner);
            }
        }

        public void RegisterSceneObject(SNetworkObject scene_object)
        {
            if (scene_object == null || scene_object.NetworkId == 0)
                return;

            scene_list[scene_object.NetworkId] = scene_object;
        }

        public SNetworkObject FindNetObject(ulong net_id)
        {
            SNetworkObject obj1 = GetSpawnedObject(net_id);
            if (obj1 != null)
                return obj1;
            SNetworkObject obj2 = GetDespawnedObject(net_id);
            if (obj2 != null)
                return obj2;
            SNetworkObject obj3 = GetSceneObject(net_id);
            if (obj3 != null)
                return obj3;
            return null;
        }

        public SNetworkObject GetSceneObject(ulong net_id)
        {
            if (net_id != 0 && scene_list.ContainsKey(net_id))
                return scene_list[net_id];
            return null;
        }

        public SNetworkObject GetSpawnedObject(ulong net_id)
        {
            if (net_id != 0 && spawned_list.ContainsKey(net_id))
                return spawned_list[net_id];
            return null;
        }

        public SNetworkObject GetDespawnedObject(ulong net_id)
        {
            if (net_id != 0 && despawned_list.ContainsKey(net_id))
                return despawned_list[net_id];
            return null;
        }

        public SNetworkBehaviour GetSpawnedBehaviour(ulong net_id, ushort behaviour_id)
        {
            SNetworkObject nobj = GetSpawnedObject(net_id);
            if (nobj != null)
                return nobj.GetBehaviour(behaviour_id);
            return null;
        }

        public bool IsOnline { get { return TheNetwork.Get().IsOnline; } }
        public bool IsServer { get { return TheNetwork.Get().IsServer; } }
        public NetworkMessaging Messaging { get { return TheNetwork.Get().Messaging; } }

        public static NetworkSpawner Get()
        {
            return NetworkGame.Get().Spawner;
        }
    }

    [System.Serializable]
    public struct NetSpawnData : INetworkSerializable
    {
        public ulong network_id;
        public ulong prefab_id;
        public ulong owner;
        public Vector3Data pos;
        public QuaternionData rot;
        public byte[] extra;

        public NetSpawnData(SNetworkObject nobj) { 
            network_id = nobj.network_id; 
            prefab_id = nobj.prefab_id;
            this.owner = nobj.OwnerId;
            pos = Vector3Data.Zero;
            rot = QuaternionData.Zero;
            extra = new byte[0];
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref network_id);
            serializer.SerializeValue(ref prefab_id);
            serializer.SerializeValue(ref owner);
            serializer.SerializeValue(ref pos);
            serializer.SerializeValue(ref rot);
            serializer.SerializeValue(ref extra);
        }
    }

    [System.Serializable]
    public struct NetDespawnData : INetworkSerializable
    {
        public ulong network_id;
        public bool destroy;

        public NetDespawnData(ulong id, bool destroy) { network_id = id; this.destroy = destroy; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref network_id);
            serializer.SerializeValue(ref destroy);
        }
    }

    [System.Serializable]
    public struct NetChangeData : INetworkSerializable
    {
        public ulong network_id;
        public ulong owner;

        public NetChangeData(ulong id, ulong owner) { network_id = id; this.owner = owner; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref network_id);
            serializer.SerializeValue(ref owner);
        }
    }

    [System.Serializable]
    public struct NetSpawnList : INetworkSerializable
    {
        public NetSpawnData[] data;

        public NetSpawnList(NetSpawnData[] data) { this.data = data; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = data != null ? data.Length : 0;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
                data = new NetSpawnData[count];

            for (int i = 0; i < count; i++)
            {
                serializer.SerializeValue(ref data[i]);
            }
        }
    }

    [System.Serializable]
    public struct NetDespawnList : INetworkSerializable
    {
        public NetDespawnData[] data;

        public NetDespawnList(NetDespawnData[] data) { this.data = data; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = data != null ? data.Length : 0;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
                data = new NetDespawnData[count];

            for (int i = 0; i < count; i++)
            {
                serializer.SerializeValue(ref data[i]);
            }
        }
    }

    [System.Serializable]
    public struct NetChangeList : INetworkSerializable
    {
        public NetChangeData[] data;

        public NetChangeList(NetChangeData[] data) { this.data = data; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = data != null ? data.Length : 0;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
                data = new NetChangeData[count];

            for (int i = 0; i < count; i++)
            {
                serializer.SerializeValue(ref data[i]);
            }
        }
    }

    public class SpawnQueueItem
    {
        public ulong client_id;
        public NetSpawnList list = new NetSpawnList();
    }

    public class DespawnQueueItem
    {
        public ulong client_id;
        public NetDespawnList list = new NetDespawnList();
    }
}
