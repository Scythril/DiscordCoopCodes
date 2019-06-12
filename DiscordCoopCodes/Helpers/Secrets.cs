using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCoopCodes
{
    class Secrets
    {
        public string Token { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
}
