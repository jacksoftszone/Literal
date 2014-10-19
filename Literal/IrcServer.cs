// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System.Collections.Generic;
namespace Literal {
    public class IrcServer {
        // Server name (user given)
        public string host, name;

        // Server info (sent with 004)
        public string serverName, serverVersion, userModes, channelModes;

        // MOTD
        public List<string> motd = new List<string>();
    }
}
