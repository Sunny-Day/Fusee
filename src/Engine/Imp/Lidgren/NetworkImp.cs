﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using Lidgren.Network;
using ProtoBuf;


namespace Fusee.Engine
{
    public class NetworkImp : INetworkImp
    {
        private NetPeer _netPeer;
        private NetServer _netServer;
        private NetClient _netClient;

        private NetConfigValues _config;
        private NetPeerConfiguration _netConfig;

        public NetConfigValues Config
        {
            get { return _config; }
            set
            {
                _netConfig.RedirectPackets = value.RedirectPackets;

                if (value.Discovery)
                {
                    _netConfig.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
                    _netConfig.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                }
                else
                {
                    _netConfig.DisableMessageType(NetIncomingMessageType.DiscoveryResponse);
                    _netConfig.DisableMessageType(NetIncomingMessageType.DiscoveryRequest);
                }
            }
        }

        public NetStatusValues Status { get; set; }

        public List<INetworkMsg> IncomingMsg { get; private set; }

        public NetworkImp()
        {
            _netConfig = new NetPeerConfiguration("FUSEE3D");

            IncomingMsg = new List<INetworkMsg>();
 
            _config = new NetConfigValues
            {
                SysType = SysType.Client,
                DefaultPort = 14242,
                Discovery = false,
                ConnectOnDiscovery = false,
                RedirectPackets = false
            };

            // _netConfig.RedirectedPacketsList.CollectionChanged += PackageCapture;

            Status = new NetStatusValues
            {
                Connected = false,
                LastStatus = ConnectionStatus.None
            };
        }

        public void StartPeer(int port)
        {
            // CHECK FOR ALREADY RUNNING
            EndPeers();
            CloseDevices();

            _netConfig = _netConfig.Clone();
            _netConfig.Port = port;

            switch (_config.SysType)
            {
                case SysType.Peer:
                    _netPeer = new NetPeer(_netConfig);
                    _netPeer.Start();
                    break;

                case SysType.Client:
                    _netClient = new NetClient(_netConfig);
                    _netClient.Start();
                    break;

                case SysType.Server:
                    _netServer = new NetServer(_netConfig);
                    _netServer.Start();
                    break;
            }
        }

        public void EndPeers()
        {
            EndPeer(SysType.Peer);
            EndPeer(SysType.Client);
            EndPeer(SysType.Server);
        }

        public void EndPeer(SysType sysType)
        {
            switch (sysType)
            {
                case SysType.Peer:
                    if (_netPeer == null) return;

                    if (_netPeer.Status == NetPeerStatus.Running)
                        _netPeer.Shutdown("Shutting Down");

                    break;

                case SysType.Client:
                    if (_netClient == null) return;

                    if (_netClient.Status == NetPeerStatus.Running)
                        _netClient.Shutdown("Shutting Down");

                    break;

                case SysType.Server:
                    if (_netServer == null) return;

                    if (_netServer.Status == NetPeerStatus.Running)
                        _netServer.Shutdown("Shutting Down");

                    break;
            }
        }

        public bool OpenConnection(SysType type, IPEndPoint ip)
        {
            return OpenConnection(type, ip.Address.ToString(), ip.Port);
        }

        public bool OpenConnection(SysType type, string host, int port)
        {
            NetConnection connection = null;

            if (type == SysType.Peer)
            {
                // START FIRST!
                var hail = _netPeer.CreateMessage("OpenConnection");
                connection = _netPeer.Connect(host, port, hail);
            }

            if (type == SysType.Client)
            {
                // START FIRST!
                var hail = _netClient.CreateMessage("OpenConnection");
                connection = _netClient.Connect(host, port, hail);
            }

            return
                (connection != null) && connection.Status == NetConnectionStatus.Connected;
        }

        public void CloseConnections()
        {
            CloseConnection(SysType.Peer);
            CloseConnection(SysType.Client);
            CloseConnection(SysType.Server);
        }

        public void CloseConnection()
        {
            CloseConnection(_config.SysType);
        }

        public void CloseConnection(SysType sysType)
        {
            switch (sysType)
            {
                case SysType.Peer:
                    foreach (var con in _netPeer.Connections)
                        con.Disconnect("Disconnecting");
                        
                    break;

                case SysType.Client:
                    foreach (var con in _netClient.Connections)
                        con.Disconnect("Disconnecting");
                    
                    break;

                case SysType.Server:
                    foreach (var con in _netServer.Connections)
                        con.Disconnect("Disconnecting");

                    break;
            }
        }

        public bool SendMessage(byte[] msg)
        {
            _netConfig.RedirectPackets = true;
            Debug.WriteLine(_netConfig.RedirectPackets + ": " + _netConfig.RedirectedPacketsList.Count);

            var sendResult = NetSendResult.Queued;

            switch (_config.SysType)
            {
                case SysType.Peer:
                    //sendMsg = _netPeer.CreateMessage(msg);
                    //sendMsg.Write(msg);

                    break;

                case SysType.Client:
                    var sendMsg = _netClient.CreateMessage();
                    sendMsg.Write(msg);

                    sendResult = _netClient.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);

                    break;

                case SysType.Server:
                    //sendMsg = _netServer.CreateMessage();
                    //_netServer.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);

                    break;
            }

            return (sendResult == NetSendResult.Sent);            
        }

        public bool SendMessage(string msg)
        {
            var enc = new System.Text.ASCIIEncoding();
            return SendMessage(enc.GetBytes(msg));
        }

        public bool SendMessage(object obj, bool compress)
        {
            byte[] data;

            /*if (compress)
                data = Compression.SerializeAndCompress(obj);
            else
            {
                var ms = new MemoryStream();
                var bf = new BinaryFormatter();

                bf.Serialize(ms, obj);

                data = ms.ToArray();
            }*/

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                data = ms.ToArray();
            }

            return SendMessage(data);
        }

        private void PackageCapture(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;

            foreach (var newIt in e.NewItems)
            {
                // Debug.WriteLine("Message: " + newIt);
            }

            _netConfig.RedirectedPacketsList.Clear();
        }

        public void SendDiscoveryMessage(int port)
        {
            switch (_config.SysType)
            {
                case SysType.Peer:
                    _netPeer.DiscoverLocalPeers(port);
                    break;

                case SysType.Client:
                    _netClient.DiscoverLocalPeers(port);
                    break;

                case SysType.Server:
                    _netServer.DiscoverLocalPeers(port);
                    break;
            }

            Debug.WriteLine("Discovery sent on port " + port);
        }

        private void SendDiscoveryResponse(IPEndPoint ip)
        {
            NetOutgoingMessage response;

            switch (_config.SysType)
            {
                case SysType.Peer:
                    response = _netPeer.CreateMessage();
                    response.Write("Peer:FUSEE3D");

                    _netPeer.SendDiscoveryResponse(response, ip);

                    break;

                case SysType.Client:
                    response = _netClient.CreateMessage();
                    response.Write("Client:FUSEE3D");

                    _netClient.SendDiscoveryResponse(response, ip);

                    break;

                case SysType.Server:
                    response = _netServer.CreateMessage();
                    response.Write("Server:FUSEE3D");

                    _netServer.SendDiscoveryResponse(response, ip);

                    break;
            }
        }

        private NetworkMessage ReadMessage(NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    Status.LastStatus = (ConnectionStatus) msg.ReadByte();

                    switch (Status.LastStatus)
                    {
                        case ConnectionStatus.Connected:
                            Status.Connected = true;
                            break;
                        case ConnectionStatus.Disconnected:
                            Status.Connected = false;
                            break;
                    }

                    return new NetworkMessage
                               {
                                   Type = (MessageType) msg.MessageType,
                                   Status = Status.LastStatus
                               };

                case NetIncomingMessageType.DiscoveryRequest:
                    SendDiscoveryResponse(msg.SenderEndPoint);

                    return new NetworkMessage
                    {
                        Type = (MessageType)msg.MessageType,
                        Sender = msg.SenderEndPoint
                    };

                case NetIncomingMessageType.DiscoveryResponse:
                    if (_config.ConnectOnDiscovery)
                        OpenConnection(_config.SysType, msg.SenderEndPoint);

                    return new NetworkMessage
                        {
                            Type = (MessageType) msg.MessageType,
                            Sender = msg.SenderEndPoint,
                            Message = msg.ReadString()
                        };

                case NetIncomingMessageType.Data:
                    return new NetworkMessage
                        {
                            Type = (MessageType) msg.MessageType,
                            Message = msg.ReadBytes(msg.LengthBytes)
                        };

                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    Debug.WriteLine(msg.MessageType + ": " + msg.ReadString());
                    break;
            }

            return new NetworkMessage();
        }

        public void OnUpdateFrame()
        {
            NetIncomingMessage msg;

            if (_netPeer != null)
            {
                while ((msg = _netPeer.ReadMessage()) != null)
                {
                    IncomingMsg.Add(ReadMessage(msg));
                    _netPeer.Recycle(msg);
                }               
            }

            if (_netClient != null)
            {
                while ((msg = _netClient.ReadMessage()) != null)
                {
                    IncomingMsg.Add(ReadMessage(msg));
                    _netClient.Recycle(msg);
                }
            }

            if (_netServer != null)
            {
                while ((msg = _netServer.ReadMessage()) != null)
                {
                    IncomingMsg.Add(ReadMessage(msg));
                    _netServer.Recycle(msg);
                }

            }
        }

        public void CloseDevices()
        {
            if (_netPeer != null)
            {
                CloseConnection(SysType.Peer);
                _netPeer = null;
            }

            if (_netClient != null)
            {
                CloseConnection(SysType.Client);
                _netClient = null;
            }

            if (_netServer != null)
            {
                CloseConnection(SysType.Server);
                _netServer = null;
            }
        }
    }
}