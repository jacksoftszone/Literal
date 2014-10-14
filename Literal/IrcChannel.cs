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
    }
}
