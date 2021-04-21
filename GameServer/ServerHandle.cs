using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerHandle
    {
        public static async void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();
            string password = packet.ReadString();
            string result = await Server.clients[fromClient].Login(username, password);
            if (result == "fail")
            {
                //TODO send failed login to client, stop connection
            }
            else
            {
                Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} ({username}, {password}) connected successfully and is now player {fromClient}");
                if (fromClient != clientIdCheck)
                {
                    Console.WriteLine($"Player\"{username}\" (ID: {fromClient}) has assumed wrong client ID ({clientIdCheck})!");
                }

                Server.clients[fromClient].SendIntoGame(username);
            }
        }

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            Vector3 destination = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();

            Server.clients[fromClient].player.SetInput(destination, rotation);
        }

        public static void PlayerHit(int fromClient, Packet packet)
        {
            Vector3 position = packet.ReadVector3();
            int damage = packet.ReadInt();

            Console.WriteLine("ServerHandle.PlayerHit");
            Server.clients[fromClient].player.DoDamage(position, damage);
        }
    }
}
