using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using GameServer.Data;

namespace GameServer
{
    class Client
    {
        private static readonly HttpClient client = new HttpClient();

        public static int dataBufferSize = 4096;

        public int id;
        public Player player;
        public TCP tcp;
        public UDP udp;

        public Client(int clientId)
        {
            id = clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                this.id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        Server.clients[id].Disconnect();
                        return;
                    }

                    byte[] _data = new byte[byteLength];
                    Array.Copy(receiveBuffer, _data, byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving TCP data: {ex}");
                    Server.clients[id].Disconnect();
                }
            }

            private bool HandleData(byte[] data)
            {
                int packetLenght = 0;

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLenght = receivedData.ReadInt();
                    if (packetLenght <= 0)
                    {
                        return true;
                    }
                }

                while (packetLenght > 0 && packetLenght <= receivedData.UnreadLength())
                {
                    byte[] packetBytes = receivedData.ReadBytes(packetLenght);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetId = packet.ReadInt();
                            Server.packetHandlers[packetId](id, packet);
                        }
                    });

                    packetLenght = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        packetLenght = receivedData.ReadInt();
                        if (packetLenght <= 0)
                        {
                            return true;
                        }
                    }
                }
                if (packetLenght <= 1)
                {
                    return true;
                }
                return false;
            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }
        public class UDP
        {
            public IPEndPoint endPoint;
            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            public void SendData(Packet packet)
            {
                Server.SendUDPData(endPoint, packet);
            }

            public void HandleData(Packet packetData)
            {
                int packetLength = packetData.ReadInt();
                byte[] packetBytes = packetData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.packetHandlers[packetId](id, packet);
                    }

                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }

        public void SendIntoGame(string playerName)
        {
            player = new Player(id, playerName, new Vector3(0, 0, 0));
            player.health = 100f;
            foreach (var client in Server.clients.Values)
            {
                if (client.player != null)
                {
                    if (client.id != id)
                    {
                        ServerSend.SpawnPlayer(id, client.player);
                    }
                }
            }

            foreach (var client in Server.clients.Values)
            {
                if (client.player != null)
                {
                    ServerSend.SpawnPlayer(client.id, player);
                }
            }
        }
        public async Task<string> Login(string username, string password)
        {
            string result = await LoginPorcess(username, password);
            if (result.Contains("UserNotFound") || result.Contains("UserNotFound"))
            {
                Console.WriteLine("successfully failed");
                return "fail";
            }
            else if (result.Contains("SuccessfulLogin"))
            {
                return "success";
            }
            Console.WriteLine(result);
            return result;
        }

        public static async Task<string> LoginPorcess(string username, string password)
        {
            InputClass log = new InputClass
            {
                Username = username,
                Password = password
            };
            var json = JsonConvert.SerializeObject(log);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://localhost:5001/api/user/login";
            using var client = new HttpClient();


            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }

        private void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

            player = null;
            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}
