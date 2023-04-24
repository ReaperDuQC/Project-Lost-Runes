using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

namespace NetcodePlus
{
    public class NetworkActionSimple : NetworkAction
    {
        public UnityAction callback;

        public void TriggerAction(SNetworkActions handler)
        {
            if (handler == null)
                return;

            NetworkActionSimpleData data = new NetworkActionSimpleData();
            SendToTarget(handler, data);

            if (ShouldRun())
                RunAction();
        }

        public void RunAction()
        {
            callback?.Invoke();
        }

        public override void RunAction(FastBufferReader reader)
        {
            NetworkActionSimpleData data;
            reader.ReadNetworkSerializable(out data);
            RunAction();
        }

        public struct NetworkActionSimpleData : INetworkSerializable
        {
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                
            }
        }
    }
}
