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
        private IrcUser me;

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
        public async void Connect(string nickname, string username, string realname) {
            serverSocket = new TcpClient();
            await serverSocket.ConnectAsync(serverInfo.address, serverInfo.port);
            serverStream = serverSocket.GetStream();
            await Write("NICK " + nickname);
            await Write("USER " + username + " 8 * :" + realname);
            me = new IrcUser { nickname = nickname, identd = username, hostname = "", realname = realname };
            if (Connected != null) Connected(this);
            ReadLoop();
        }

        /// <summary>
        /// Sends a QUIT message and terminates the connection.
        /// </summary>
        /// <param name="exitMessage">Message to send along the QUIT command</param>
        public async Task Quit(string exitMessage) {
            await Write("QUIT :" + exitMessage); ;
        }

        /// <summary>
        /// Joins a channel on the server
        /// </summary>
        /// <param name="channel">Channel to join, requires prefix (likely #)</param>
        public async Task Join(string channel) {
            await Write("JOIN " + channel);
        }

        /// <summary>
        /// Part from channel on the server
        /// </summary>
        /// <param name="channel">Channel to part, requires prefix (likely #)</param>
        public async Task Part(string channel) {
            await Write("PART " + channel);
        }

        /// <summary>
        /// Sends a message to a channel.
        /// </summary>
        /// <param name="channel">Channel to send the message to</param>
        /// <param name="message">Message to send</param>
        public async Task Message(string channel, string message) {
            await Write("PRIVMSG " + channel + " :" + message);
        }

        /// <summary>
        /// Sends a CTCP to a channel/user.
        /// </summary>
        /// <param name="chanusr">Channel/User to send the CTCP to</param>
        /// <param name="ctcp">CTCP to send</param>
        public async Task CTCP(string chanusr, string ctcp) {
            await Write("PRIVMSG " + chanusr + " :" + (char)1 + ctcp + (char)1);
        }

        /// <summary>
        /// Sends a notice to a channel/user.
        /// </summary>
        /// <param name="chanusr">Channel/User to send the notice to</param>
        /// <param name="notice">Notice to send</param>
        public async Task Notice(string chanusr, string notice) {
            await Write("NOTICE " + chanusr + " :" + notice);
        }

        /// <summary>
        /// Writes a raw message on the IRC network stream
        /// </summary>
        /// <param name="message">Message to write</param>
        public async Task Raw(string message) {
            await Write(message);
        }

        #endregion
        #region Private methods
        private async Task Write(string command) {
            // Prepare command for being written
            byte[] utfstring = Encoding.UTF8.GetBytes(command + "\r\n");

#if DEBUG
            System.Console.WriteLine("<- " + command);
#endif

            // Write to network stream
            await serverStream.WriteAsync(utfstring, 0, utfstring.Length);
        }

        private async void ReadLoop() {
            // According to RFC2812, each IRC message must be within 512 characters
            // IRCv3 does 1024 (with tagging), so let's use 1024 base and split on \r\n
            // But as our motto says "Trust is for the weak"
            byte[] bytes = new byte[1024];
            string message = "";
            int read = -1;
            char[] trimChar = { ' ', '\r', '\n' };
            while (read != 0) {
                read = await serverStream.ReadAsync(bytes, 0, 1024);
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
                    await Handle(command);
                }
            }
        }

        private async Task Handle(IrcCommand command) {
            switch (command.command) {
                case "PING":
                    command.command = "PONG";
                    await Write("PONG :" + command.text);
                    break;
                case "JOIN":
                    // Get user
                    IrcUser user = IrcUser.fromOrigin(command.origin);

                    // Get joined channel
                    string target = "";
                    if (command.args != null && command.args.Length > 0)
                        target = command.args[0];
                    else if (command.text != null && command.text.Length > 0)
                        target = command.text;

                    // Call event if bound
                    if (Joined != null)
                        Joined(this, target, user.nickname == me.nickname);

                    //TODO tell the channel (when IrcChannel will exist)
                    break;
                default:
                    DebugErr("Unknown command: " + command.command + " in \r\n  " + command.ToString());
                    break;
            }
        }

        private void DebugLog(string str) {
#if DEBUG
            System.ConsoleColor old = System.Console.ForegroundColor;
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.WriteLine(str);
            System.Console.ForegroundColor = old;
#endif
        }

        private void DebugErr(string str) {
#if DEBUG
            System.ConsoleColor old = System.Console.ForegroundColor;
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine(str);
            System.Console.ForegroundColor = old;
#endif
        }
        #endregion
    }

}
