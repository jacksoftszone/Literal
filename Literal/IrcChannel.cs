using System.Collections.Generic;

namespace Literal {
    public class IrcChannel {
        public string name;
        public string modes;
        public List<IrcUser> users;

        public void Join(IrcUser user) {
            users.Add(user);
        }
    }
}
