// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using jzIRC;

namespace TestBot {
    class Program {
        static IrcConnection server;

        const string serverAddr = "irc.azzurra.org";
        const int serverPort = 6667;

        static void Main(string[] args) {
            server = new IrcConnection();
            server.Connected += (conn) => {
                conn.Join("#jcslab");
                conn.Joined += (_, chan, me) => {
                    if (!me) return;
                    conn.Message(chan, "Hi all!");
                    conn.Quit("jzIRC iz da bestu!1");
                };
            };

            server.Connect(serverAddr, serverPort);
        }
    }
}
