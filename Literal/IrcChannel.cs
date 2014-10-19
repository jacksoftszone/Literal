using System;
// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System.Collections.Generic;

namespace Literal {
    public class IrcChannel {
        public string name;
        public string modes;
        public string topic, topicBy;
        public List<IrcUser> users;
        public List<IrcUser> banlist;
        public List<IrcUser> exceptlist;
        public List<IrcUser> invitelist;

        public double topicWhenUnix {
            get {
                return (topicWhen - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
            }
            set {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                topicWhen = dtDateTime.AddSeconds(value).ToLocalTime();
            }
        }
        public DateTime topicWhen;

        public void Join(IrcUser user) {
            users.Add(user);
        }

        public void AddBan(string hostmask) {
            banlist.Add(IrcUser.fromOrigin(hostmask));
        }

        public void DelBan(string hostmask) {
            banlist.Remove(IrcUser.fromOrigin(hostmask)); // NOTE: check if Remove works with hashes or direct references.
        }

        public void AddExcept(string hostmask) {
            exceptlist.Add(IrcUser.fromOrigin(hostmask));
        }

        public void DelExcept(string hostmask) {
            exceptlist.Remove(IrcUser.fromOrigin(hostmask)); // NOTE: check if Remove works with hashes or direct references.
        }

        public void AddInvite(string hostmask) {
            invitelist.Add(IrcUser.fromOrigin(hostmask));
        }

        public void DelInvite(string hostmask) {
            invitelist.Remove(IrcUser.fromOrigin(hostmask)); // NOTE: check if Remove works with hashes or direct references.
        }

        internal void Mode(IrcCommand command) {
            switch (command.args[1]) {
                //TODO fix for multiple modes
                case "+b":
                    AddBan(command.args[2]);
                    break;
                case "-b":
                    DelBan(command.args[2]);
                    break;
                case "+e":
                    AddExcept(command.args[2]);
                    break;
                case "-e":
                    DelExcept(command.args[2]);
                    break;
                case "+I":
                    AddInvite(command.args[2]);
                    break;
                case "-I":
                    DelInvite(command.args[2]);
                    break;
            }
        }
    }
}
