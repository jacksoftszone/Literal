// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.
using System.Net.Sockets;

namespace Literal {

    /// <summary>
    /// Connection handler to IRC servers
    /// </summary>
    public class IrcConnection {

        /// <summary>
        /// Called when a connection is fully established.
        /// </summary>
        public event ConnectedHandler Connected;
        public delegate void ConnectedHandler(IrcConnection connection);

        /// <summary>
        /// Called when someone (including yourself) joins a channel you're in.
        /// </summary>
        public event JoinedHandler Joined;
        public delegate void JoinedHandler(IrcConnection connection, string channel, bool isMe);

        struct ServerInfo {
            public string address;
            public int port;
            public bool useSSL;
        }

        public ServerInfo serverInfo { public get; private set; }

        private TcpClient serverSocket;

        /// <summary>
        /// Connects to a determined IRC server using the provided address and port.
        /// </summary>
        /// <param name="serverAddress">Address of the server, will be DNS resolved</param>
        /// <param name="serverPort">Port to connect to</param>
        /// <param name="useSSL">Is SSL used? (not implemented yet)</param>
        public void Connect(string serverAddress, int serverPort, bool useSSL = false) {
            if (useSSL) throw new System.NotImplementedException();
            this.serverInfo = new ServerInfo { address = serverAddress, port = serverPort, useSSL = useSSL };

            serverSocket = new TcpClient(serverInfo.address, serverInfo.port);
        }

        /// <summary>
        /// Sends a QUIT message and terminates the connection.
        /// </summary>
        /// <param name="exitMessage">Message to send along the QUIT command</param>
        public void Quit(string exitMessage) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Joins a channel on the server
        /// </summary>
        /// <param name="channel">Channel to join, requires prefix (likely #)</param>
        public void Join(string channel) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sends a message to a channel.
        /// </summary>
        /// <param name="channel">Channel to send the message to</param>
        /// <param name="message">Message to send</param>
        public void Message(string channel, string message) {
            throw new System.NotImplementedException();
        }
    }

}
