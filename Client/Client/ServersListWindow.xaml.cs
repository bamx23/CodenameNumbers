using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for ServersList.xaml
    /// </summary>
    public partial class ServersListWindow : Window
    {
        public List<Server> serversList;

        public ServersListWindow()
        {
            InitializeComponent();

            serversList = new List<Server>();
            listBoxServers.ItemsSource = serversList;
        }

        private int id = 0;
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Update Server List here

            //Test:
            serversList.Add(new Server() { Id = id, Name = "Сервер #"+id, PlayersCount = new Random((int)DateTime.Now.Ticks).Next(5), PlayersLimit = 4 });
            ++id;

            listBoxServers.Items.Refresh();
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxServers.SelectedIndex < 0)
            {
                if(listBoxServers.Items.Count == 0)
                    MessageBox.Show("В данный момент нету активных игр.");
                else
                    MessageBox.Show("Выберите игру из списка.");
                return;
            }

            var server = (Server) listBoxServers.SelectedItem;

            //TODO: Connect to server

            //if connected:
            var mw = new MainWindow();
            mw.Show();
            mw.Closed += (s, o) => Show();
            mw.serverListWindow = this;
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            buttonRefresh.Focus();
        }
    }
}
