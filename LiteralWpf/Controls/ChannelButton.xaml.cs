// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System.Windows;
using System.Windows.Controls;

namespace LiteralWpf.Controls {
    public partial class ChannelButton : UserControl {

        public static DependencyProperty ChanNameProperty =
        DependencyProperty.Register("Channel name", typeof(string), typeof(ServerBlock));

        public string ChanName {
            get { return GetValue(ChanNameProperty).ToString(); }
            set { SetNames(value); SetValue(ChanNameProperty, value); }
        }

        private void SetNames(string channel) {
            // No prefix
            if (char.IsLetterOrDigit(channel[0])) {
                ChannelPrefix.Content = "";
                ChannelName.Content = channel;
            } else {
                ChannelPrefix.Content = channel.Substring(0, 1);
                ChannelName.Content = channel.Substring(1);
            }
        }

        public ChannelButton() {
            this.InitializeComponent();
        }
    }
}