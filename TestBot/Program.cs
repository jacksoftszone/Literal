// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using Literal;
using System.Threading.Tasks;

namespace TestBot {
    class Program {
        static IrcConnection server;

        //const string serverAddr = "192.168.46.100";
        const string serverAddr = "irc.azzurra.org";
        const int serverPort = 6667;

        static bool hasFinished = false;

        static void Main(string[] args) {
            server = new IrcConnection(serverAddr, serverPort);

            server.Connected += (conn) => {
                conn.Joined += (_, chan, me) => {
                    if (!me) return;
                    conn.Message(chan, "Hi all!");
                    conn.Quit("Literal iz da bestu!1");
                    hasFinished = true;
                };
                conn.Join("#jcslab");
            };

#if DEBUG
            server.RawMessage += (_, msg) => {
                System.Console.WriteLine("-> " + msg);
            };
#endif

            server.Connect("TestLit", "myuser", "Testing bot");
            while (!hasFinished) ;
        }
    }
}
