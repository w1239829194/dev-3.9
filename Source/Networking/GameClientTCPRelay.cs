﻿using StayInTarkov.Coop.Matchmaker;
using StayInTarkov.Coop.NetworkPacket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace StayInTarkov.Networking
{
    public class GameClientTCPRelay : MonoBehaviour, IGameClient
    {
        public BlockingCollection<byte[]> PooledBytesToPost { get; } = [];
        public float DownloadSpeedKbps { get; private set; } = 0;
        public float UploadSpeedKbps { get; private set; } = 0;
        public float PacketLoss { get; private set; } = 0;
        public ushort Ping
        {
            get
            {
                return SptBackendCommunication.Instance.Ping;
            }
        }

        void Awake()
        {
            SptBackendCommunication.Instance.WebSocketClose();
            SptBackendCommunication.Instance.WebSocketCreate(SITMatchmaking.Profile);
        }

        void Start()
        {
        }

        void Update()
        {
            if (PooledBytesToPost != null)
            {
                while (PooledBytesToPost.Any())
                {
                    while (PooledBytesToPost.TryTake(out var bytes))
                    {
                        SptBackendCommunication.Instance.PostDownWebSocketImmediately(bytes);
                    }
                }
            }
        }

        public void SendData(byte[] data)
        {
            if (data == null)
                return;

            PooledBytesToPost.Add(data);
        }

        public void SendData<T>(ref T packet) where T : BasePacket
        {
            SendData(packet.Serialize());
        }

        void IGameClient.ResetStats()
        {
            DownloadSpeedKbps = Interlocked.Exchange(ref SptBackendCommunication.Instance.BytesReceived, 0) / 1024f;
            UploadSpeedKbps = Interlocked.Exchange(ref SptBackendCommunication.Instance.BytesSent, 0) / 1024f;
        }
    }
}
