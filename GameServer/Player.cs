using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{

    class Player
    {
        public int id;
        public string username;
        public float health;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 destination;
        private bool isMoving;
        private float startTime;
        private float duration = 2.5f;

        public float moveSpeed = 2.5f / Constants.TICKS_PER_SEC;

        private float hitRadius = 1.5f;

        public Player(int id, string username, Vector3 spawnPosition)
        {
            this.id = id;
            this.username = username;
            this.position = spawnPosition;
            this.rotation = Quaternion.Identity;

        }

        public void Update() //TODO: FIX MOVEMENT
        {
            //if (!atDestination)
            //{
            //    Vector2 inputDirection = Vector2.Zero;
            //    if (destination.Y > position.Y)
            //    {
            //        inputDirection.Y += 1f;
            //    }
            //    if (destination.Y < position.Y)
            //    {
            //        inputDirection.Y -= 1f;
            //    }
            //    if (destination.X > position.X)
            //    {
            //        inputDirection.X += 1f;
            //    }
            //    if (destination.X < position.X)
            //    {
            //        inputDirection.X -= 1f;
            //    }
            //    Console.WriteLine(inputDirection);
            //    Console.WriteLine("Destination:");
            //    Console.WriteLine(destination);
            //    Move(inputDirection);
            //}
            if (position != destination)
            {
                isMoving = true;
                position = Vector3.Lerp(position, destination, startTime / duration);
                startTime += 0.001f;
                //Console.WriteLine("Walk");
                //Console.WriteLine(startTime);
                //Console.WriteLine(startTime / duration);
                ServerSend.PlayerPosition(this);
                ServerSend.PlayerRotation(this);
            }
            if (position == destination)
            {
                isMoving = false;
                startTime = 0f;
            }

        }

        private void Move(Vector2 inputDirection)
        {
            //if (position == destination)
            //{
            //    atDestination = true;

            //}
            //Vector3 up = Vector3.Transform(new Vector3(1, 0, 0), rotation);
            //Vector3 sideways = Vector3.Normalize(Vector3.Cross(up, new Vector3(0, 0, 1)));

            //Vector3 moveDirection = sideways * inputDirection.X + up * inputDirection.Y;

            //position += moveDirection * moveSpeed;
            //Console.WriteLine(position);

            //ServerSend.PlayerPosition(this);
            //ServerSend.PlayerRotation(this);
        }

        public void DoDamage(Vector3 hitPosition, int damage)
        {
            Console.WriteLine($"{username} hit {hitPosition} for {damage} damage.");
            //foreach (var client in Server.clients.Values)
            //{
            //    if (Vector3.Distance(client.player.position, hitPosition) <= hitRadius)
            //    {
            //        if (client.id != this.id)
            //        {
            //            client.player.TakeDamage(damage);
            //        }
            //    }
            //}
        }

        private void TakeDamage(int damage)
        {
            this.health -= damage;
            Console.WriteLine($"{username} took {damage} damage.");
        }

        public void SetInput(Vector3 destination, Quaternion rotation)
        {
            this.destination = destination;
            this.rotation = rotation;
        }
    }
}
