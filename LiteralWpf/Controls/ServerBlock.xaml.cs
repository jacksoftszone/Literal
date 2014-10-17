// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

using System.Windows;
using System.Windows.Controls;

namespace LiteralWpf.Controls {

    public partial class ServerBlock : ItemsControl {

        public static DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(ServerBlock));

        public string Title {
            get { return GetValue(TitleProperty).ToString(); }
            set { SetValue(TitleProperty, value);  }
        }

        public ServerBlock() {
            this.InitializeComponent();
        }
    }
}