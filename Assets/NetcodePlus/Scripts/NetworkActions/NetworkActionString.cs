using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

namespace NetcodePlus
{
    public class NetworkActionString : NetworkAction
    {
        public UnityAction<string> callback;

        public NetworkActionStringData GetData(string value)
        {
            NetworkActionStringData data = new NetworkActionStringData();
            data.value = value;
            return data;
        }

        public void TriggerAction(SNetworkActions handler, string value)
        {
            if (handler == null)
                return;

            NetworkActionStringData data = GetData(value);
            SendToTarget(handler, data);

            if (ShouldRun())
                RunAction(value);
        }

        public void RunAction(string value)
        {
            callback?.Invoke(value);
        }

        public override void RunAction(FastBufferReader reader)
        {
            NetworkActionStringData data;
            reader.ReadNetworkSerializable(out data);
            RunAction(data.value);
        }

        public struct NetworkActionStringData : INetworkSerializable
        {
            public string value;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref value);
            }
        }
    }
}
