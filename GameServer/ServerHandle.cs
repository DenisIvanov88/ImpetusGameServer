using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}");
            if (fromClient != clientIdCheck)
            {
                Console.WriteLine($"Player\"{username}\" (ID: {fromClient}) has assumed wrong client ID ({clientIdCheck})!");
            }

            //TODO send player into game
            Server.clients[fromClient].SendIntoGame(username);
        }

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            Vector3 destination = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();

            Server.clients[fromClient].player.SetInput(destination, rotation);
        }
    }
}
