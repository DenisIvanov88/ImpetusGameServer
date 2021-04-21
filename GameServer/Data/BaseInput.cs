using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Data
{
    public class BaseInput<T>
    {
        public string Status { get; set; }
        public T Output { get; set; }
    }
}
