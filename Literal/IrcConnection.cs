// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Literal {

    /// <summary>
    /// Connection handler to IRC servers
    /// </summary>
    public class IrcConnection {
        #region Events

        /// <summary>
        /// Called when successfully connected to a server and able to execute commands. 
        /// </summary>
        public event ConnectedHandler Connected;
        public delegate void ConnectedHandler(IrcConnection connection);

        /// <summary>
        /// Called when someone (including yourself) joins a channel you're in.
        /// </summary>
        public event JoinedHandler Joined;
        public delegate void JoinedHandler(IrcConnection connection, string channel, bool isMe);

        /// <summary>
        /// Called for every raw message received
        /// </summary>
        public event RawMessageHandler RawMessage;
        public delegate void RawMessageHandler(IrcConnection connection, string rawmessage);

        #endregion
        #region Variables

        public struct ServerInfo {
            public string address;
            public int port;
            public bool useSSL;
        }

        public ServerInfo serverInfo { get; private set; }

        private TcpClient serverSocket;
        private NetworkStream serverStream;

        #endregion
        #region Public methods / APIs
        /// <summary>
        /// Creates a IRC server connection using the provided address and port.
        /// </summary>
        /// <param name="serverAddress">Address of the server, will be DNS resolved</param>
        /// <param name="serverPort">Port to connect to</param>
        /// <param name="useSSL">Is SSL used? (not implemented yet)</param>
        public IrcConnection(string serverAddress, int serverPort, bool useSSL = false) {
            if (useSSL) throw new System.NotImplementedException();
            serverInfo = new ServerInfo { address = serverAddress, port = serverPort, useSSL = useSSL };
        }

        /// <summary>
        /// Connects to the specified server using the provided identity
        /// </summary>
        /// <param name="nickname">Nickname</param>
        /// <param name="username">Username (or identd)</param>
        /// <param name="realname">Real name</param>
        /// <returns></returns>
        public async Task Connect(string nickname, string username, string realname) {
            serverSocket = new TcpClient();
            await serverSocket.ConnectAsync(serverInfo.address, serverInfo.port);
            serverStream = serverSocket.GetStream();
            await Write("USER " + username + " 0 * :" + realname);
            await Write("NICK " + nickname);
            await ReadLoop();
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

        #endregion
        #region Private methods
        private async Task Write(string command) {
            // Prepare command for being written
            byte[] utfstring = Encoding.UTF8.GetBytes(command + "\r\n");

            // Write to network stream
            await serverStream.WriteAsync(utfstring, 0, utfstring.Length);
        }

        private async Task ReadLoop() {
            // According to RFC2812, each IRC message must be within 512 characters
            // But as our motto says "Trust is for the weak"
            byte[] bytes = new byte[512];
            string message = "";
            int read = -1;
            char[] trimChar = { ' ', '\r', '\n' };
            while (read != 0) {
                read = await serverStream.ReadAsync(bytes, 0, 512);
                string decoded = Encoding.UTF8.GetString(bytes, 0, read);
                message += decoded;

                // Message longer than 512 characters, get the rest
                if (message.IndexOf("\n") < 0) continue;

                // Get the messages we got so far, leave the rest
                string[] messages = message.Split('\n');
                message = messages.Last();

                foreach (string msg in messages.Take(messages.Length - 1)) {
                    string trimMessage = msg.TrimEnd(trimChar);
                    if (RawMessage != null) RawMessage(this, trimMessage);
                    IrcCommand command = new IrcCommand(trimMessage);
                }
                
                message = "";
            }
        }
        #endregion
    }

}
