// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

namespace Literal {
    public class IrcUser {
        public bool isServer;
        public string serverName;
        public string nickname;
        public string ident;
        public string hostname;
        public string realname;

        public static IrcUser fromOrigin(string origin) {
            // Check if server
            if (origin.IndexOf('@') < 0) {
                return new IrcUser { isServer = true, serverName = origin };
            }

            // Get nickname
            int start = 0;
            int index = origin.IndexOf('!', start);
            string nickname = origin.Substring(start, index);
            start = index + 1;

            // Get ident
            index = origin.IndexOf('@', start - 1);
            string ident = origin.Substring(start, index - start);
            start = index + 1;

            // Get host
            string host = origin.Substring(start);

            return new IrcUser { isServer = false, nickname = nickname, ident = ident, hostname = host, realname = "" };
        }
    }
}
