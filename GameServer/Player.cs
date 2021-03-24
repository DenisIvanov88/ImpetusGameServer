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

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 destination;

        public float moveSpeed = 2.5f / Constants.TICKS_PER_SEC;

        public Player(int id, string username, Vector3 spawnPosition)
        {
            this.id = id;
            this.username = username;
            this.position = spawnPosition;
            this.rotation = Quaternion.Identity;

            destination = spawnPosition;
        }

        public void Update() //TODO: FIX MOVEMENT
        {
            Vector2 inputDirection = Vector2.Zero;
            if (destination.Y > position.Y)
            {
                inputDirection.Y += 1f;
            }
            if (destination.Y < position.Y)
            {
                inputDirection.Y -= 1f;
            }
            if (destination.X > position.X)
            {
                inputDirection.X += 1f;
            }
            if (destination.X < position.X)
            {
                inputDirection.X -= 1f;
            }
            Console.WriteLine(inputDirection);
            Move(inputDirection);
        }

        private void Move(Vector2 inputDirection)
        {
            Vector3 up = Vector3.Transform(new Vector3(1, 0, 0), rotation);
            Vector3 sideways = Vector3.Normalize(Vector3.Cross(up, new Vector3(0, 0, 1)));

            Vector3 moveDirection = sideways * inputDirection.X + up * inputDirection.Y;

            position += moveDirection * moveSpeed;
            Console.WriteLine(position);

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }

        public void SetInput(Vector3 destination, Quaternion rotation)
        {
            this.destination = destination;
            this.rotation = rotation;
        }
    }
}
