using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Data
{
    public class AppUserOutput
    {
        public AppUserOutput(AppUser _user)
        {
            UserName = _user.Username;
            Points = _user.Points;
        }

        public string UserName { get; set; }
        public int Points { get; set; }
        public List<string> MatchHistoryIDs { get; set; }
    }
}
