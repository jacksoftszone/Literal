// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System.ComponentModel;
using System.Windows.Controls;

namespace LiteralWpf.Controls {
    public partial class ChannelButton : UserControl {

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new string Content {
            get { return ChannelPrefix.Content.ToString() + ChannelName.Content.ToString(); }
            set {
                // No prefix
                if (char.IsLetterOrDigit(value[0])) {
                    ChannelName.Content = value;
                } else {
                    ChannelPrefix.Content = value[0];
                    ChannelName.Content = value.Substring(1);
                }
            }
        }

        public ChannelButton() {
            this.InitializeComponent();
        }
    }
}