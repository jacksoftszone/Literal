// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

namespace Literal {
    public class IrcUser {
        public string nickname;
        public string identd;
        public string hostname;
        public string realname;

        public static IrcUser fromOrigin(string origin) {
            char[] splits = { '!', '@' };
            string[] parts = origin.Split(splits, 3);
            return new IrcUser { nickname = parts[0], identd = parts[1], hostname = parts[2], realname = "" };
        }
    }
}
