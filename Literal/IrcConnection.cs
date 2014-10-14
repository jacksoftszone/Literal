// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Called for every server error
        /// </summary>
        public event ErrorHandler ServerError;
        public delegate void ErrorHandler(IrcConnection connection, int code, string error);

        #endregion
        #region Variables

        public struct ServerConnectionInfo {
            public string address;
            public int port;
            public bool useSSL;
        }

        /// <summary>
        /// Try a random nick if the provided one is taken
        /// If false, it may give a 433 (Nickname in use) error while connecting.
        /// </summary>
        public bool randomNickIfTaken = false;

        public ServerConnectionInfo serverConnInfo { get; private set; }
        public IrcServer serverInfo { get; private set; }
        public Dictionary<string, IrcChannel> channels { get; private set; }

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
            if (useSSL) {
                throw new System.NotImplementedException();
            }
            serverConnInfo = new ServerConnectionInfo { address = serverAddress, port = serverPort, useSSL = useSSL };
            serverInfo = new IrcServer { host = serverAddress };
            channels = new Dictionary<string, IrcChannel>();
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
            await serverSocket.ConnectAsync(serverConnInfo.address, serverConnInfo.port);
            serverStream = serverSocket.GetStream();
            await Write("NICK " + nickname);
            await Write("USER " + username + " 8 * :" + realname);
            me = new IrcUser { nickname = nickname, identd = username, hostname = "", realname = realname };
            if (Connected != null) {
                Connected(this);
            }
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
        /// Set a new nickname.
        /// </summary>
        /// <param name="newnick">The new Nickname to be set</param>
        public async Task Nick(string newnick) {
            await Write("NICK " + newnick);
        }

        /// <summary>
        /// Gets the topic from a channel in the server
        /// </summary>
        /// <param name="channel">Channel to get topic of</param>
        public async Task Topic(string channel) {
            await Write("TOPIC " + channel);
        }

        /// <summary>
        /// Sets the topic for a channel, some modes might be required
        /// </summary>
        /// <param name="channel">Channel to set topic of</param>
        /// <param name="newtopic">Topic to set</param>
        public async Task Topic(string channel, string newtopic) {
            await Write("TOPIC " + channel + " :" + newtopic);
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
            // Because like our motto says "Trust is for the weak"
            byte[] bytes = new byte[1024];
            string message = "";
            int read = -1;
            char[] trimChar = { ' ', '\r', '\n' };
            while (read != 0) {
                read = await serverStream.ReadAsync(bytes, 0, 1024);
                string decoded = Encoding.UTF8.GetString(bytes, 0, read);
                message += decoded;

                // Message longer than 1024 characters, get the rest
                if (message.IndexOf("\n") < 0) {
                    continue;
                }

                // Get the messages we got so far, leave the rest
                string[] messages = message.Split('\n');
                message = messages.Last();

                foreach (string msg in messages.Take(messages.Length - 1)) {
                    string trimMessage = msg.TrimEnd(trimChar);
                    if (RawMessage != null) {
                        RawMessage(this, trimMessage);
                    }
                    IrcCommand command = new IrcCommand(trimMessage);
                    await Handle(command);
                }
            }
        }

        private async Task Handle(IrcCommand command) {
            switch (command.command.ToUpper()) {
                #region Server Requests

                case "PING":
                    command.command = "PONG";
                    await Write("PONG :" + command.text);
                    break;

                #endregion
                #region User actions

                case "JOIN":
                    // Get user
                    IrcUser user = IrcUser.fromOrigin(command.origin);
                    bool isMe = user.nickname == me.nickname;

                    // Get joined channel
                    string target = "";
                    if (command.args != null && command.args.Length > 0) {
                        target = command.args[0];
                    } else if (command.text != null && command.text.Length > 0) {
                        target = command.text;
                    }

                    // Call event if bound
                    if (Joined != null) {
                        Joined(this, target, isMe);
                    }

                    // We joined, create channel
                    if (isMe) {
                        channels.Add(target, new IrcChannel { name = target });
                    } else {
                        if (!channels.ContainsKey(command.args[0])) {
                            Debug.Log("Received join for a non existant channel");
                            break;
                        }
                        channels[target].Join(user);
                    }
                    //TODO tell the channel (when IrcChannel will exist)
                    break;

                #endregion
                #region Server messages

                case "001": // Welcome messages
                case "002":
                case "003":
                    break;

                case "004": // Server info
                    // Format: :<SERVER> 004 <NICK> <SNAME> <VERSION> <UMODES> <CMODES> ...
                    if (command.args.Length < 4) {
                        Debug.Log("004 format is not compliant to RFC");
                        break;
                    }
                    serverInfo.serverName = command.args[1];
                    serverInfo.serverVersion = command.args[2];
                    serverInfo.userModes = command.args[3];
                    serverInfo.channelModes = command.args[4];
                    //TODO Some IRCd might have additional parameters.. do we care?
                    break;

                case "005": // RPL_ISUPPORT / Capabilities
                    //TODO Parse capabilities
                    break;

                case "251": // RPL_LUSERCLIENT / List user result
                case "252": // RPL_LUSEROP
                case "253": // RPL_LUSERUNKNOWN
                case "254": // RPL_LUSERCHANNELS
                case "255": // RPL_LUSERME
                case "265": // RPL_LOCALUSERS
                case "266": // RPL_GLOBALUSERS
                    //TODO? Server user list (do we care?)
                    break;

                case "372": // RPL_MOTD / MOTD
                case "375": // RPL_MOTDSTART
                case "376": // RPL_ENDOFMOTD
                    //TODO MOTD handling (parse and save to IrcServer)
                    break;

                case "331": // RPL_NOTOPIC / Empty topic (no need to do anything for now)
                    if (!channels.ContainsKey(command.args[1])) {
                        Debug.Log("Received topic for an unrelated channel");
                        break;
                    }
                    break;

                case "332": // RPL_TOPIC / Got topic
                    if (!channels.ContainsKey(command.args[1])) {
                        Debug.Log("Received topic for an unrelated channel");
                        break;
                    }
                    // Set topic
                    if (command.text != null && command.text.Length > 0) {
                        channels[command.args[1]].topic = command.text;
                    }
                    break;

                case "333": // RPL_TOPICWHOTIME
                    if (!channels.ContainsKey(command.args[1])) {
                        Debug.Log("Received topic for an unrelated channel");
                        break;
                    }
                    if (command.args.Length < 2) {
                        Debug.Log("333 format not compliant to standards");
                        break;
                    }
                    channels[command.args[1]].topicBy = command.args[2];
                    long time;
                    bool conv = long.TryParse(command.args[3], out time);
                    if (conv) {
                        channels[command.args[1]].topicWhenUnix = time;
                    } else {
                        Debug.Log("Couldn't parse time from 333");
                    }
                    break;


                case "353": // RPL_NAMREPLY / NAMES reply
                case "366": // RPL_ENDOFNAMES
                    //TODO User list handling (delegate to IrcChannel)
                    break;

                #endregion
                #region Server errors

                case "ERROR":
                    Debug.Error("Got ERROR from server:\r\n " + command.text);
                    break;

                case "401": // ERR_NOSUCHNICK / The specified nick doesn't exist
                    if (ServerError != null) {
                        ServerError(this, 401, "No such nick");
                    }
                    break;

                case "402": // ERR_NOSUCHSERVER / The specified server doesn't exist (Untested)
                    if (ServerError != null) {
                        ServerError(this, 402, "No such server");
                    }
                    break;

                case "403": // ERR_NOSUCHCHANNEL / The specified channel doesn't exist (Probably not used. Tested on Azzurra only, must do other tests)
                    if (ServerError != null) {
                        ServerError(this, 403, "No such channel");
                    }
                    break;

                case "404": // ERR_CANNOTSENDTOCHAN / Due to chanmodes (+m with no grades) or +b and still on chan
                    if (ServerError != null)
                    {
                        ServerError(this, 404, "Cannot send to channel"); // To be completed (Need to add which channel on the end of the string).
                    }
                    break;

                case "405": // ERR_TOOMANYCHANNELS / Too manu opened channels.
                    if (ServerError != null)
                    {
                        ServerError(this, 405, "Too many opened channels");
                    }
                    break;

                case "411": // ERR_NORECIPIENT / Command issued without a recipient, WHOIS and INVITE uses a different error, 431 and 461
                    if (ServerError != null) {
                        ServerError(this, 411, "No recipient given (" + command.command + ")");
                    }
                    break;

                case "416": // ERR_QUERYTOOLONG
                    if (ServerError != null) {
                        ServerError(this, 416, "Too many lines in the output, restrict your query");
                    }
                    break;

                case "421": // ERR_UNKNOWNCOMMAND
                    if (ServerError != null) {
                        ServerError(this, 421, command.command + ": Unknown command");
                    }
                    break;

                case "431": // ERR_NONICKNAMEGIVEN
                    if (ServerError != null) {
                        ServerError(this, 431, "No nickname given");
                    }
                    break;

                case "432": // ERR_ERRONEOUSNICKNAME
                    if (ServerError != null) {
                        int err;
                        bool errconv = int.TryParse(command.command, out err);
                        if (errconv) {
                            ServerError(this, err, "Got error: " + command.command);
                        }
                    }
                    break;

                case "433": // ERR_NICKNAMEINUSE / Nickname in use
                    if (randomNickIfTaken) {
                        Random rnd = new Random();
                        await Nick(me + rnd.Next(000, 999).ToString());
                    } else {
                        if (ServerError != null) {
                            ServerError(this, 433, "Nickname in use");
                        }
                    }
                    break;

                case "437": // ERR_BANNICKCHANGE / Attempt to change nick while banned on a chan
                    if (ServerError != null) {
                        ServerError(this, 437, "You can't change nick while banned");
                    }
                    break;

                case "471": // ERR_CHANNELISFULL / For example, when there's the mode +l and the channel is already full
                    if (ServerError != null)
                    {
                        ServerError(this, 471, "Channel is full");
                    }
                    break;

                case "473": // ERR_INVITEONLYCHAN / The channel is invite only
                    if (ServerError != null)
                    {
                        ServerError(this, 473, "This channel is invite only"); // To be completed (Need specify channel).
                    }
                    break;

                case "474": // ERR_BANNEDFROMCHAN / Cannot join on a channel while you're banned on it.
                    if (ServerError != null)
                    {
                        ServerError(this, 474, "You are banned from this channel"); // To be completed (Need specify channel).
                    }
                    break;

                case "475": // ERR_BADCHANNELKEY / Keyword to join that channel is incorrect
                    if (ServerError != null)
                    {
                        ServerError(this, 475, "The keyword for this channel is incorrect"); // To be completed (Need specify channel).
                    }
                    break;

                case "478": // ERR_BANLISTFULL / Ban list on that channel is full, cannot add more bans.
                    if (ServerError != null)
                    {
                        ServerError(this, 478, "The banlist is full"); // To be completed (Need specify channel).
                    }
                    break;

                #endregion

                default:
                    Debug.Error("Unknown command: " + command.command + " in \r\n  " + command.ToString());
                    break;
            }
        }
        #endregion

    }

}
