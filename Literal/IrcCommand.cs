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
            string fullargs = message.Substring(commandEnd + 1, (argsEnd < 0 ? message.Length : argsEnd) - 1);
            args = fullargs.Split(' ');

            // Check if there is a text part and get it
            if (argsEnd < 0) return;
            text = message.Substring(argsEnd + 2);
        }

        public override string ToString() {
            string final = "";
            if (origin.Length > 0) final += ":" + origin + " ";
            final += command;
            if (args.Length > 0) final += " " + string.Join(" ", args);
            if (text.Length > 0) final += " :" + text;
            return final;
        }
    }
}
