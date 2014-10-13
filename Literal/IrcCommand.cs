// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System;
namespace Literal {

    /// <summary>
    /// Class that represents an IRC command
    /// </summary>
    public class IrcCommand {
        public string origin;
        public string command;
        public string[] args;
        public string text;

        /// <summary>
        /// Creates an IRC command instance from the unparsed text
        /// </summary>
        /// <param name="command">Unparsed command</param>
        public IrcCommand(string command) {
            Parse(command);
        }

        private void Parse(string message) {
            char[] trimChar = { ' ', '\r', '\n' };
            message = message.TrimEnd(trimChar);

            // Check for prefix/origin
            if (message.StartsWith(":")) {
                int originEnd = message.IndexOf(" ");
                origin = message.Substring(1, originEnd - 1);
                message = message.Substring(originEnd + 1);
            }

            // Get command, uppercase-ify
            int commandEnd = message.IndexOf(" ");
            command = message.Substring(0, commandEnd).ToUpper();

            // Get args
            int argsEnd = message.IndexOf(" :");
            string[] argmsg = message.Substring(commandEnd).Split(new string[]{" :"}, 2, StringSplitOptions.None);
            args = argmsg[0].Length < 1 ? null : argmsg[0].Substring(1).Split(' '); // First character is a space

            // Check if there is a text part and get it
            if (argmsg.Length > 1)
                text = argmsg[1]; 
        }

        public override string ToString() {
            string final = "";
            if (origin != null && origin.Length > 0) final += ":" + origin + " ";
            final += command;
            if (args != null && args.Length > 0) final += " " + string.Join(" ", args);
            if (text != null && text.Length > 0) final += " :" + text;
            return final;
        }
    }
}
