using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LiteralWpf {
    /// <summary>
    /// Interaction logic for ConnectionManager.xaml
    /// </summary>
    public partial class ConnectionManager : Window {
        public ConnectionManager() {
            InitializeComponent();
            DemoCManagerFill();
            ServerNotSelected();
        }
        public class DemoCManager {
            public string MultiConnect { get; set; }
            public string Network { get; set; }
            public string Server { get; set; }
            public string Port { get; set; }
            public string Password { get; set; }

        }
        public void DemoCManagerFill() {
            List<DemoCManager> DCM = new List<DemoCManager>();
            DCM.Add(new DemoCManager() { MultiConnect = "true", Network = "Azzurra", Server = "irc.azzurra.org", Port = "6667", Password = "12345" });
            DCM.Add(new DemoCManager() { MultiConnect = "false", Network = "Rizon", Server = "irc.rizon.org", Port = "6667", Password = "12345" });
            ServersListView.ItemsSource = DCM;
        }

        public void ServerSelected(string network) {
            ServerLabel.Content = "Server: " + network;
            ConnTreeView.Items.Clear();
            TreeViewItem ConnTreeItems = null;
            ConnTreeItems = new TreeViewItem();
            ConnTreeItems.Header = "Manager";
            ConnTreeItems.Items.Add(new TreeViewItem() { Header = "Server" });
            ConnTreeItems.Items.Add(new TreeViewItem() { Header = "Auto Join" });
            ConnTreeItems.Items.Add(new TreeViewItem() { Header = "Auto ID" });
            ConnTreeItems.Items.Add(new TreeViewItem() { Header = "Perform" });
            ConnTreeView.Items.Add(ConnTreeItems);
            ConnTreeItems.IsExpanded = true;
        }
        public void ServerNotSelected() {
            ServerLabel.Content = "Server";
            ConnTreeView.Items.Clear();
            TreeViewItem ConnTreeItems = null;
            ConnTreeItems = new TreeViewItem();
            ConnTreeItems.Header = "Manager";
            ConnTreeItems.Items.Add(new TreeViewItem() { Header = "Server" });
            ConnTreeView.Items.Add(ConnTreeItems);
            ConnTreeItems.IsExpanded = true;
        }

        public void ConnectDoubleClick(object sender, MouseButtonEventArgs e) {
            //Connect to the double-clicked row/server
        }
        public void SelectedServer(object sender, MouseButtonEventArgs e) {
            if (ServersListView.SelectedIndex == -1) {
                ServerNotSelected();
            }
            else {
                ServerSelected(ServersListView.SelectedItem.ToString());
                //int selnet = ServersListView.SelectedIndex;
                
                //MessageBox.Show(ServersListView.Items.CurrentItem.ToString());
            }
        }


        private void ConnSelectedItem(object sender, RoutedPropertyChangedEventArgs<Object> e) {
            ServerLabel.Content = "DBG: " + ConnTreeView.SelectedItem.ToString();
        }
    }
}
