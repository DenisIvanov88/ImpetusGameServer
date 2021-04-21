using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Data
{
    public class AppUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Points { get; set; }
    }
}
