namespace Literal {
    public class IrcServer {
        // Server name (user given)
        public string host, name;

        // Server info (sent with 004)
        public string serverName, serverVersion, userModes, channelModes;
    }
}
