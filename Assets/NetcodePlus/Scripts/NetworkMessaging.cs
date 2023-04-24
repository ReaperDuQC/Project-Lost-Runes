using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace NetcodePlus
{
    /// <summary>
    /// Base class for sending and receiving network messages
    /// </summary>

    public class NetworkMessaging
    {
        private NetworkManager network;

        public NetworkMessaging(NetworkManager network)
        {
            this.network = network;
        }

        public void ListenMsg(string type, CustomMessagingManager.HandleNamedMessageDelegate callback)
        {
            if (IsOnline)
            {
                network.CustomMessagingManager.RegisterNamedMessageHandler(type, callback);
            }
        }

        public void ListenMsg(string type, System.Action<ulong, SerializedData> callback)
        {
            if (IsOnline)
            {
                network.CustomMessagingManager.RegisterNamedMessageHandler(type, (ulong client_id, FastBufferReader reader) =>
                {
                    SerializedData sdata = new SerializedData(reader);
                    callback(client_id, sdata);
                });
            }
        }

        public void UnListenMsg(string type)
        {
            if (IsOnline && network.CustomMessagingManager != null)
                network.CustomMessagingManager.UnregisterNamedMessageHandler(type);
        }

        //--------- Send Single ----------

        public void SendEmpty(string type, ulong target, NetworkDelivery delivery)
        {
            if (IsOnline && target != ClientID)
            {
                FastBufferWriter writer = new FastBufferWriter(0, Allocator.Temp);
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendBytes(string type, ulong target, byte[] msg, NetworkDelivery delivery)
        {
            if (IsOnline && target != ClientID)
            {
                FastBufferWriter writer = new FastBufferWriter(msg.Length, Allocator.Temp);
                writer.WriteBytesSafe(msg, msg.Length);
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendString(string type, ulong target, string msg, NetworkDelivery delivery)
        {
            if (IsOnline && target != ClientID)
            {
                FastBufferWriter writer = new FastBufferWriter(msg.Length, Allocator.Temp, TheNetwork.MsgSizeMax);
                writer.WriteValueSafe(msg);
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendInt(string type, ulong target, int data, NetworkDelivery delivery)
        {
            if (IsOnline && target != ClientID)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendUInt64(string type, ulong target, ulong data, NetworkDelivery delivery)
        {
            if (IsOnline && target != ClientID)
            {
                FastBufferWriter writer = new FastBufferWriter(8, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendFloat(string type, ulong target, float data, NetworkDelivery delivery)
        {
            if (IsOnline && target != ClientID)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
                writer.Dispose();
            }
        }
        
        public void SendObject<T>(string type, ulong target, T data, NetworkDelivery delivery) where T : INetworkSerializable
        {
            if (IsOnline && target != ClientID)
            {
                FastBufferWriter writer = new FastBufferWriter(256, Allocator.Temp, TheNetwork.MsgSizeMax);
                writer.WriteNetworkSerializable(data);
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendBuffer(string type, ulong target, FastBufferWriter writer, NetworkDelivery delivery)
        {
            if (IsOnline && target != ClientID)
            {
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
            }
        }

        //--------- Send Multi ----------

        public void SendEmpty(string type, IReadOnlyList<ulong> targets, NetworkDelivery delivery)
        {
            if (IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(0, Allocator.Temp);
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendBytes(string type, IReadOnlyList<ulong> targets, byte[] msg, NetworkDelivery delivery)
        {
            if (IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(msg.Length, Allocator.Temp);
                writer.WriteBytesSafe(msg, msg.Length);
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendString(string type, IReadOnlyList<ulong> targets, string msg, NetworkDelivery delivery)
        {
            if (IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(msg.Length, Allocator.Temp, TheNetwork.MsgSizeMax);
                writer.WriteValueSafe(msg);
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendInt(string type, IReadOnlyList<ulong> targets, int data, NetworkDelivery delivery)
        {
            if (IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendUInt64(string type, IReadOnlyList<ulong> targets, ulong data, NetworkDelivery delivery)
        {
            if (IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(8, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendFloat(string type, IReadOnlyList<ulong> targets, float data, NetworkDelivery delivery)
        {
            if (IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
                writer.Dispose();
            }
        }
        
        public void SendObject<T>(string type, IReadOnlyList<ulong> targets, T data, NetworkDelivery delivery) where T : INetworkSerializable
        {
            if (IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(256, Allocator.Temp, TheNetwork.MsgSizeMax);
                writer.WriteNetworkSerializable(data);
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendBuffer(string type, IReadOnlyList<ulong> targets, FastBufferWriter writer, NetworkDelivery delivery)
        {
            if (IsOnline)
            {
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
            }
        }

        //--------- Send All ----------

        public void SendEmptyAll(string type, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(0, Allocator.Temp);
                network.CustomMessagingManager.SendNamedMessage(type, ClientList, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendStringAll(string type, string msg, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(msg.Length, Allocator.Temp, TheNetwork.MsgSizeMax);
                writer.WriteValueSafe(msg);
                network.CustomMessagingManager.SendNamedMessage(type, ClientList, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendIntAll(string type, int data, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, ClientList, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendUInt64All(string type, ulong data, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(8, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, ClientList, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendFloatAll(string type, float data, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(4, Allocator.Temp);
                writer.WriteValueSafe(data);
                network.CustomMessagingManager.SendNamedMessage(type, ClientList, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendBytesAll(string type, byte[] msg, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(msg.Length, Allocator.Temp);
                writer.WriteBytesSafe(msg, msg.Length);
                network.CustomMessagingManager.SendNamedMessage(type, ClientList, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendObjectAll<T>(string type, T data, NetworkDelivery delivery) where T : INetworkSerializable
        {
            if (IsServer && IsOnline)
            {
                FastBufferWriter writer = new FastBufferWriter(256, Allocator.Temp, TheNetwork.MsgSizeMax);
                writer.WriteNetworkSerializable(data);
                network.CustomMessagingManager.SendNamedMessage(type, ClientList, writer, delivery);
                writer.Dispose();
            }
        }

        public void SendBufferAll(string type, FastBufferWriter writer, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                network.CustomMessagingManager.SendNamedMessage(type, ClientList, writer, delivery);
            }
        }

        //--------- Forward msgs ----------
		
		//Forward a client message to one client
        //Make sure you finished reading the reader before forwarding
        public void Forward(string type, ulong target, FastBufferReader reader, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                reader.Seek(0); //Reset reader
                reader.ReadValueSafe(out ulong header); //Ignore header
                byte[] bytes = new byte[reader.Length - reader.Position];
                reader.ReadBytesSafe(ref bytes, reader.Length - reader.Position);
                FastBufferWriter writer = new FastBufferWriter(bytes.Length, Allocator.Temp);
                writer.WriteBytesSafe(bytes, bytes.Length);
                network.CustomMessagingManager.SendNamedMessage(type, target, writer, delivery);
                writer.Dispose();
            }
        }

        //Forward a client message to all target clients
        //Make sure you finished reading the reader before forwarding
        public void Forward(string type, IReadOnlyList<ulong> targets, FastBufferReader reader, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                reader.Seek(0); //Reset reader
                reader.ReadValueSafe(out ulong header); //Ignore header
                byte[] bytes = new byte[reader.Length - reader.Position];
                reader.ReadBytesSafe(ref bytes, reader.Length - reader.Position);
                FastBufferWriter writer = new FastBufferWriter(bytes.Length, Allocator.Temp);
                writer.WriteBytesSafe(bytes, bytes.Length);
                network.CustomMessagingManager.SendNamedMessage(type, targets, writer, delivery);
                writer.Dispose();
            }
        }

        //Forward a client message to all other clients (other than the source)
        //Make sure you finished reading the reader before forwarding
        public void ForwardAll(string type, ulong source_client, FastBufferReader reader, NetworkDelivery delivery)
        {
            if (IsServer && IsOnline)
            {
                reader.Seek(0); //Reset reader
                reader.ReadValueSafe(out ulong header); //Ignore header
                byte[] bytes = new byte[reader.Length - reader.Position];
                reader.ReadBytesSafe(ref bytes, reader.Length - reader.Position);
                FastBufferWriter writer = new FastBufferWriter(bytes.Length, Allocator.Temp);
                writer.WriteBytesSafe(bytes, bytes.Length);

                foreach (ulong client in ClientList)
                {
                    if(client != source_client && client != ClientID)
                        network.CustomMessagingManager.SendNamedMessage(type, client, writer, delivery);
                }
                writer.Dispose();
            }
        }

        public IReadOnlyList<ulong> ClientList { get { return TheNetwork.Get().GetClientsIds(); } }
        public bool IsOnline { get { return TheNetwork.Get().IsOnline; } }
        public bool IsServer { get { return TheNetwork.Get().IsServer; } }
        public ulong ServerID { get { return TheNetwork.Get().ServerID; } }
        public ulong ClientID { get { return TheNetwork.Get().ClientID; } }


        public static NetworkMessaging Get()
        {
            return TheNetwork.Get().Messaging;
        }
    }
}
