// Ignorance 1.3.x
// A Unity LLAPI Replacement Transport for Mirror Networking
// https://github.com/SoftwareGuy/Ignorance
// -----------------
// Ignorance Transport is licensed under the MIT license, however
// it comes with no warranty what-so-ever. However, if you do
// encounter a problem with Ignorance you can get support by
// dropping past the Mirror discord's #ignorance channel. Otherwise,
// open a issue ticket on the GitHub issues page. Ensure you provide
// lots of detail of what you were doing and the error/stack trace.
// -----------------
// Ignorance Classic. This version uses LateUpdate polling for both
// server and client hot paths.
// -----------------

using System;
using System.Collections.Generic;
using ENet;
using UnityEngine;
using Event = ENet.Event;
using EventType = ENet.EventType;

namespace Mirror
{
    public class IgnoranceClassic : Transport
    {
        // debug
        [Header("Debug Options")]
        public bool DebugEnabled = false;
        // server bind to all and addresses
        [Header("Server Binding")]
        public bool ServerBindAll = true;
        public string ServerBindAddress = "127.0.0.1";
        [Header("Connection Port")]
        public int CommunicationPort = 7777;
        // maximum packet sizes
        [Header("Security")]
        [UnityEngine.Serialization.FormerlySerializedAs("MaxPacketSize")]
        public int MaxPacketSizeInKb = 16;
        // Channels
        [Header("Channel Definitions")]
        public IgnoranceChannelTypes[] Channels;
        // custom peer limits
        [Header("Custom Peer and Timeout Settings")]
        public bool CustomMaxPeerLimit = false;
        public int CustomMaxPeers = 1000;
        // custom timeouts
        public bool CustomTimeoutLimit = false;
        public uint CustomTimeoutBaseTicks = 5000;
        public uint CustomTimeoutMultiplier = 3;
        // ping calculation timer
        [Header("Ping Calculation")]
        [Tooltip("This value (in seconds) controls how often the client peer ping value will be retrieved from the Enet world. Note that too low values can actually harm performance due to excessive polling. " +
            "Keep it frequent, but not too frequent. 3 - 5 seconds should be OK. 0 to disable.")]
        public int PingCalculationInterval = 3;

        // enet engine related things
        private bool ENETInitialized = false, ServerStarted = false, ClientStarted = false;
        private Host ENETHost = new Host(), ENETClientHost = new Host();                    // Didn't want to have to do this but i don't want to risk crashes.
        private Peer ENETPeer = new Peer();
        private Address ENETAddress = new Address();
        // lookup and reverse lookup dictionaries
        private Dictionary<int, Peer> ConnectionIDToPeers = new Dictionary<int, Peer>();
        private Dictionary<Peer, int> PeersToConnectionIDs = new Dictionary<Peer, int>();
        // mirror related things
        private byte[] PacketCache;
        private int NextConnectionID = 1; // DO NOT MODIFY "NextConnectionID"
        private uint NextPingCalculationTime = 0, CurrentClientPing = 0;

        private void Awake()
        {
            // Deprecation warning. Comment it out if desired; better to say it now rather than have a rude awakening.
            Debug.LogWarning("Ignorance Classic will be REMOVED with the release of Ignorance v1.4. Please migrate to Ignorance Threaded. If for some reason you can't, drop by the Discord and explain why.");    
        }

        #region Client
        public override void ClientConnect(string address)
        {
            if (!ENETInitialized)
            {
                if (InitializeENET())
                {
                    Debug.Log($"Ignorance successfully initialized ENET.");
                    ENETInitialized = true;
                }
                else
                {
                    Debug.LogError($"Ignorance failed to initialize ENET! Cannot continue.");
                    return;
                }
            }

            if (DebugEnabled) Debug.Log($"[DEBUGGING MODE] Ignorance: ClientConnect({address})");

            if (Channels.Length > 255)
            {
                Debug.LogError($"Ignorance: Too many channels. Channel limit is 255, you have {Channels.Length}. This would probably crash ENET. Aborting connection.");
                return;
            }

            if (CommunicationPort < ushort.MinValue || CommunicationPort > ushort.MaxValue)
            {
                Debug.LogError($"Ignorance: Bad communication port number. You need to set it between port 0 and 65535. Aborting connection.");
                return;
            }

            if (ENETClientHost == null || !ENETClientHost.IsSet) ENETClientHost.Create(null, 1, Channels.Length, 0, 0, PacketCache.Length);
            if (DebugEnabled) Debug.Log($"[DEBUGGING MODE] Ignorance: Created ENET Host object");

            ENETAddress.SetHost(address);
            ENETAddress.Port = (ushort)CommunicationPort;

            ENETPeer = ENETClientHost.Connect(ENETAddress, Channels.Length);
            if (CustomTimeoutLimit) ENETPeer.Timeout(Library.throttleScale, CustomTimeoutBaseTicks, CustomTimeoutBaseTicks * CustomTimeoutMultiplier);
            ClientStarted = true;

            if (DebugEnabled) Debug.Log($"[DEBUGGING MODE] Ignorance: Client has been started!");
        }

        public override bool ClientConnected()
        {
            // No debug here. this gets spammed many times a tick
            return ENETPeer.IsSet && ENETPeer.State == PeerState.Connected;
        }

        public override void ClientDisconnect()
        {
            if (DebugEnabled) Debug.Log($"[DEBUGGING MODE] Ignorance: ClientDisconnect()");

            if (ServerStarted)
            {
                Debug.LogWarning("MIRROR BUG: ClientDisconnect called even when we're in HostClient/Dedicated Server mode");
                return;
            }

            if (!IsValid(ENETClientHost)) return;
            if (ENETPeer.IsSet) ENETPeer.DisconnectNow(0);

            // Flush and free resources.
            if (IsValid(ENETClientHost))
            {
                ENETClientHost.Flush();
                ENETClientHost.Dispose();
            }
        }

        public override bool ClientSend(int channelId, ArraySegment<byte> data)
        {
            return ENETClientSendInternal(channelId, data);
        }

        public string GetClientPing()
        {
            return CurrentClientPing.ToString();
        }
        #endregion

        public override int GetMaxPacketSize(int channelId = 0)
        {
            return PacketCache.Length;
        }

        // Server
        #region Server
        public override bool ServerActive()
        {
            return IsValid(ENETHost);
        }

        public override bool ServerDisconnect(int connectionId)
        {
            if (ConnectionIDToPeers.TryGetValue(connectionId, out Peer result))
            {
                result.DisconnectNow(0);
                return true;
            }
            else return false;
        }

        public override string ServerGetClientAddress(int connectionId)
        {
            if (ConnectionIDToPeers.TryGetValue(connectionId, out Peer result)) return $"{result.IP}:{result.Port}";
            else return "UNKNOWN";
        }

        // Can't deprecate this because I believe Dissonance uses it still...
        public bool ServerSend(int connectionId, int channelId, ArraySegment<byte> data)
        {
            if (!ENETHost.IsSet) return false;
            if (channelId > Channels.Length)
            {
                Debug.LogWarning($"Ignorance: Attempted to send data on channel {channelId} when we only have {Channels.Length} channels defined");
                return false;
            }

            Packet payload = default;

            if (ConnectionIDToPeers.TryGetValue(connectionId, out Peer targetPeer))
            {
                payload.Create(data.Array, data.Offset, data.Count + data.Offset, (PacketFlags)Channels[channelId]);
                int returnCode = targetPeer.Send((byte)channelId, ref payload);

                if (returnCode == 0)
                {
                    return true;
                }
                else
                {
                    if (DebugEnabled) Debug.LogWarning($"Ignorance: Failed sending outgoing packet on channel {channelId} to connection id {connectionId}. Code {returnCode}");
                    return false;
                }
            }
            else
            {
                if (DebugEnabled) Debug.Log($"[DEBUGGING MODE] Ignorance: Unknown connection id {connectionId}");
                return false;
            }
        }

        public override void ServerStart()
        {
            if (!ENETInitialized)
            {
                if (InitializeENET())
                {
                    Debug.Log($"Ignorance successfully initialized ENET.");
                    ENETInitialized = true;
                }
                else
                {
                    Debug.LogError($"Ignorance failed to initialize ENET! Cannot continue.");
                    return;
                }
            }

            if (!ServerBindAll)
            {
                if (DebugEnabled) print($"Ignorance: Not binding to all interfaces, checking if supplied info is actually an IP address");
                if (System.Net.IPAddress.TryParse(ServerBindAddress, out _))
                {
                    // Looks good to us. Let's use it.
                    if (DebugEnabled) print($"Ignorance: Valid IP Address {ServerBindAddress}");
                    if (!ENETAddress.SetIP(ServerBindAddress))
                    {
                        Debug.LogError("Ignorance was unable to set the hostname or address. Was this even valid? Please check it and try again.");
                        return;
                    }

                }
                else
                {
                    // Might be a hostname.
                    if (DebugEnabled) print($"Ignorance: Doesn't look like a valid IP address, assuming it's a hostname?");
                    if (!ENETAddress.SetHost(ServerBindAddress))
                    {
                        Debug.LogError("Ignorance was unable to set the hostname or address. Was this even valid? Please check it and try again.");
                        return;
                    }
                }
            }
            else
            {
                if (DebugEnabled) print($"Ignorance: Setting address to all interfaces, port {CommunicationPort}");
#if UNITY_IOS
                // Coburn: temporary fix until I figure out if this is similar to the MacOS bug again...
                ENETAddress.SetIP("::0");
#endif
            }

            ENETAddress.Port = (ushort)CommunicationPort;
            if (ENETHost == null || !ENETHost.IsSet) ENETHost = new Host();

            // Go go go! Clear those corners!
            ENETHost.Create(ENETAddress, CustomMaxPeerLimit ? CustomMaxPeers : (int)Library.maxPeers, Channels.Length, 0, 0);

            if (DebugEnabled) Debug.Log($"[DEBUGGING MODE] Ignorance: Server should be created now... If Ignorance immediately crashes after this line, please file a bug report on the GitHub.");
            ServerStarted = true;
        }

        public override void ServerStop()
        {
            if (DebugEnabled)
            {
                Debug.Log("[DEBUGGING MODE] Ignorance: ServerStop(). Cleaning the packet cache...");
            }

            PacketCache = new byte[MaxPacketSizeInKb * 1024];

            if (DebugEnabled) Debug.Log("Ignorance: Cleaning up lookup dictonaries");
            ConnectionIDToPeers.Clear();
            PeersToConnectionIDs.Clear();

            if (IsValid(ENETHost))
            {
                ENETHost.Dispose();
            }

            ServerStarted = false;
        }
        #endregion

        public override void Shutdown()
        {
            // c6: just right at the top of shutdown "Herp I shut down nao". lol... then if thats missing obv problems
            // Debug.Log("Herp I shut down nao");
            ServerStarted = false;
            ClientStarted = false;

            ENETInitialized = false;
            Library.Deinitialize();

            // Reset the next ping calculation timer
            NextPingCalculationTime = 0;
        }

        // core
        #region Core Transport
        private bool InitializeENET()
        {
            PacketCache = new byte[MaxPacketSizeInKb * 1024];
            if (DebugEnabled) Debug.Log($"Initialized new packet cache, {MaxPacketSizeInKb * 1024} bytes capacity.");

            return Library.Initialize();
        }

        // server pump
        private bool ProcessServerMessages()
        {
            // Never attempt process anything if we're not initialized
            if (!ENETInitialized) return false;
            // Never attempt to process anything if the server is not valid.
            if (!IsValid(ENETHost)) return false;
            // Never attempt to process anything if the server is not active.
            if (!ServerStarted) return false;

            bool serverWasPolled = false;
            int newConnectionID = NextConnectionID;

            while (!serverWasPolled)
            {
                if (ENETHost.CheckEvents(out Event networkEvent) <= 0)
                {
                    if (ENETHost.Service(0, out networkEvent) <= 0) break;

                    serverWasPolled = true;
                }

                switch (networkEvent.Type)
                {
                    case EventType.Connect:
                        // A client connected to the server. Assign a new ID to them.
                        if (DebugEnabled)
                        {
                            Debug.Log($"Ignorance: New connection from {networkEvent.Peer.IP}:{networkEvent.Peer.Port}");
                            Debug.Log($"Ignorance: Map {networkEvent.Peer.IP}:{networkEvent.Peer.Port} (ENET Peer {networkEvent.Peer.ID}) => Mirror World Connection {newConnectionID}");
                        }

                        if (CustomTimeoutLimit) networkEvent.Peer.Timeout(Library.throttleScale, CustomTimeoutBaseTicks, CustomTimeoutBaseTicks * CustomTimeoutMultiplier);

                        // Map them into our dictonaries.
                        PeersToConnectionIDs.Add(networkEvent.Peer, newConnectionID);
                        ConnectionIDToPeers.Add(newConnectionID, networkEvent.Peer);

                        OnServerConnected.Invoke(newConnectionID);
                        NextConnectionID++;
                        break;

                    case EventType.Disconnect:
                    case EventType.Timeout:
                        // A client disconnected.
                        if (PeersToConnectionIDs.TryGetValue(networkEvent.Peer, out int deadPeerConnID))
                        {
                            if (DebugEnabled) Debug.Log($"Ignorance: Dead Peer. {networkEvent.Peer.ID} (Mirror connection {deadPeerConnID}) died.");
                            OnServerDisconnected.Invoke(deadPeerConnID);
                            // cleanup
                            PeersToConnectionIDs.Remove(networkEvent.Peer);
                            ConnectionIDToPeers.Remove(deadPeerConnID);
                        }
                        // We don't give a shit about any other connections. if they are bogus then Mirror doesn't need to know about them. Could be a performance impact.
                        break;
                    case EventType.Receive:
                        // Only process data from known peers.
                        if (PeersToConnectionIDs.TryGetValue(networkEvent.Peer, out int knownConnectionID))
                        {
                            if (networkEvent.Packet.Length > PacketCache.Length)
                            {
                                if (DebugEnabled) Debug.Log($"Ignorance: Packet too big to fit in buffer. {networkEvent.Packet.Length} packet bytes vs {PacketCache.Length} cache bytes {networkEvent.Peer.ID}.");
                                networkEvent.Packet.Dispose();
                            }
                            else
                            {
                                // invoke on the server.
                                try
                                {
                                    byte[] rentedBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(networkEvent.Packet.Length);
                                    networkEvent.Packet.CopyTo(rentedBuffer);
                                    networkEvent.Packet.Dispose();
                                    OnServerDataReceived.Invoke(knownConnectionID, new ArraySegment<byte>(rentedBuffer, 0, networkEvent.Packet.Length), networkEvent.ChannelID);
                                    System.Buffers.ArrayPool<byte>.Shared.Return(rentedBuffer, true);

                                }
                                catch (Exception e)
                                {
                                    Debug.LogError($"Ignorance caught an exception while trying to copy data from the unmanaged (ENET) world to managed (Mono/IL2CPP) world. Please consider reporting this to the Ignorance developer on GitHub.\n" +
                                        $"Exception returned was: {e.Message}\n" +
                                        $"Debug details: {(PacketCache == null ? "packet buffer was NULL" : $"{PacketCache.Length} byte work buffer")}, {networkEvent.Packet.Length} byte(s) network packet length\n" +
                                        $"Stack Trace: {e.StackTrace}");
                                    break;
                                }

                                // Dispose of the packet, we're done.
                                networkEvent.Packet.Dispose();
                            }
                        }
                        else
                        {
                            // Emit a warning and clean the packet. We don't want it in memory.
                            if (DebugEnabled) Debug.LogWarning($"Ignorance: Unknown packet from Peer {networkEvent.Peer.ID}. Be cautious - if you get this error too many times, you're likely being attacked.");
                            networkEvent.Packet.Dispose();
                        }
                        break;
                }

                // Some messages can disable the transport
                // If the transport was disabled by any of the messages, we have to break out of the loop and wait until we've been re-enabled.
                if (!enabled)
                {
                    break;
                }
            }

            // We're done here. Return.
            return true;
        }

        // client pump
        private bool ProcessClientMessages()
        {
            // Never do anything when ENET is not initialized
            if (!ENETInitialized)
            {
                return false;
            }

            // Never do anything when ENET is in a different mode
            if (!IsValid(ENETClientHost) || ENETPeer.State == PeerState.Uninitialized || !ClientStarted)
            {
                return false;
            }

            bool clientWasPolled = false;

            // Only process messages if the client is valid.
            while (!clientWasPolled)
            {
                if (!IsValid(ENETClientHost)) return false;

                if (ENETClientHost.CheckEvents(out Event networkEvent) <= 0)
                {
                    if (ENETClientHost.Service(0, out networkEvent) <= 0) break;
                    clientWasPolled = true;
                }

                switch (networkEvent.Type)
                {
                    case EventType.Connect:
                        // Client connected.
                        OnClientConnected.Invoke();
                        break;
                    case EventType.Timeout:
                    case EventType.Disconnect:
                        // Client disconnected.
                        OnClientDisconnected.Invoke();
                        break;
                    case EventType.Receive:
                        // Client recieving some data.
                        // Debug.Log("Data");
                        if (networkEvent.Packet.Length > PacketCache.Length)
                        {
                            if (DebugEnabled) Debug.Log($"Ignorance: Packet too big to fit in buffer. {networkEvent.Packet.Length} packet bytes vs {PacketCache.Length} cache bytes {networkEvent.Peer.ID}.");
                            networkEvent.Packet.Dispose();
                        }
                        else
                        {
                            // invoke on the client.
                            try
                            {
                                byte[] rentedBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(networkEvent.Packet.Length);
                                networkEvent.Packet.CopyTo(rentedBuffer);

                                OnClientDataReceived.Invoke(new ArraySegment<byte>(rentedBuffer, 0, networkEvent.Packet.Length), networkEvent.ChannelID);
                                System.Buffers.ArrayPool<byte>.Shared.Return(rentedBuffer, true);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"Ignorance caught an exception while trying to copy data from the unmanaged (ENET) world to managed (Mono/IL2CPP) world. Please consider reporting this to the Ignorance developer on GitHub.\n" +
                                    $"Exception returned was: {e.Message}\n" +
                                    $"Debug details: {(PacketCache == null ? "packet buffer was NULL" : $"{PacketCache.Length} byte work buffer")}, {networkEvent.Packet.Length} byte(s) network packet length\n" +
                                    $"Stack Trace: {e.StackTrace}");
                                networkEvent.Packet.Dispose();
                                break;
                            }

                            networkEvent.Packet.Dispose();

                        }
                        break;
                }
                
                // Some messages can disable the transport
                // If the transport was disabled by any of the messages, we have to break out of the loop and wait until we've been re-enabled.
                if (!enabled)
                {
                    break;
                }
            }
            // We're done here. Return.
            return true;
        }

        // utility
        private bool IsValid(Host host)
        {
            return host != null && host.IsSet;
        }
        #endregion

        // -> Moved ChannelTypes enum to it's own file, so it's easier to maintain.

        // monobehaviour specific stuff
        public void LateUpdate()
        {
            // note: we need to check enabled in case we set it to false
            // when LateUpdate already started.
            // (https://github.com/vis2k/Mirror/pull/379)

            // Coburn: does the order here really matter? Server then client?
            if (enabled)
            {
                if (ServerStarted) ProcessServerMessages();
                if (ClientStarted)
                {
                    // Is ping calculation enabled?
                    if (PingCalculationInterval > 0)
                    {
                        // Time to recalculate our ping?
                        if (NextPingCalculationTime >= Library.Time)
                        {
                            // If the peer is set, then poll it. Otherwise it might not be time to do that.
                            if (ENETPeer.IsSet) CurrentClientPing = ENETPeer.RoundTripTime;
                            NextPingCalculationTime = (uint)(Library.Time + (PingCalculationInterval * 1000));
                        }
                    }

                    ProcessClientMessages();
                }
            }
        }

        public override bool Available()
        {
#if UNITY_WEBGL
            // Ignorance is not available on these platforms.
            return false;
#else
            return true;
#endif
        }

        public override string ToString()
        {
            // A little complicated if else mess.
            if (ServerActive())
            {
                if (NetworkClient.active) return $"Ignorance {IgnoranceInternals.Version} (HostClient)";      // HostClient Mode
                else return $"Ignorance {IgnoranceInternals.Version} (Dedicated Server)";                    // Dedicated server masterrace mode
            }
            else
            {
                return $"Ignorance {IgnoranceInternals.Version} (Client)";                                   // Client mode
            }
        }

        // Sanity checks.
        private void OnValidate()
        {
            if (Channels != null && Channels.Length >= 2)
            {
                // Check to make sure that Channel 0 and 1 are correct.
                if (Channels[0] != IgnoranceChannelTypes.Reliable) Channels[0] = IgnoranceChannelTypes.Reliable;
                if (Channels[1] != IgnoranceChannelTypes.Unreliable) Channels[1] = IgnoranceChannelTypes.Unreliable;
            }
            else
            {
                Channels = new IgnoranceChannelTypes[2]
                {
                    IgnoranceChannelTypes.Reliable,
                    IgnoranceChannelTypes.Unreliable
                };
            }
        }

        // Mirror 4.x specific: ServerSend to more than one connection id
        public override bool ServerSend(List<int> connectionIds, int channelId, ArraySegment<byte> segment)
        {
            if (!ENETHost.IsSet) return false;

            foreach (int conn in connectionIds)
            {
                // Cheeky hack?
                ServerSend(conn, channelId, segment);
            }

            return true;
        }

        /// <summary>
        /// Interal function used by the transport to carry data to the actual sending functions of the wrapper.
        /// Hopefully should make it easier to fix things.
        /// </summary>
        /// <param name="channelId">The channel id you wish to send the packet on. Must be within 0 and the count of the channels array.</param>
        /// <param name="dataPayload">The array segment containing the data to send to ENET.</param>
        /// <returns></returns>
        private bool ENETClientSendInternal(int channelId, ArraySegment<byte> dataPayload)
        {
            if (!ENETClientHost.IsSet) return false;
            if (channelId > Channels.Length)
            {
                Debug.LogWarning($"Ignorance: Attempted to send data on channel {channelId} when we only have {Channels.Length} channels defined");
                return false;
            }

            Packet payload = default;
            payload.Create(dataPayload.Array, dataPayload.Offset, dataPayload.Count + dataPayload.Offset, (PacketFlags)Channels[channelId]);
            int returnCode = ENETPeer.Send((byte)channelId, ref payload);
            if (returnCode == 0)
            {
                if (DebugEnabled) Debug.Log($"[DEBUGGING MODE] Ignorance: Outgoing packet on channel {channelId} OK");
                return true;
            }
            else
            {
                if (DebugEnabled) Debug.Log($"[DEBUGGING MODE] Ignorance: Outgoing packet on channel {channelId} FAIL, code {returnCode}");
                return false;
            }
        }

        #region Mirror 6.2+ - URI Support
        public override Uri ServerUri()
        {
            UriBuilder builder = new UriBuilder
            {
                Scheme = IgnoranceInternals.Scheme,
                Host = ServerBindAddress,
                Port = CommunicationPort
            };
            return builder.Uri;
        }

        public override void ClientConnect(Uri uri)
        {
            if (uri.Scheme != IgnoranceInternals.Scheme)
                throw new ArgumentException($"Invalid uri {uri}, use {IgnoranceInternals.Scheme}://host:port instead", nameof(uri));

            if (!uri.IsDefaultPort)
            {
                // Set the communication port to the one specified.
                CommunicationPort = uri.Port;
            }

            ClientConnect(uri.Host);
        }
        #endregion
    }
}
