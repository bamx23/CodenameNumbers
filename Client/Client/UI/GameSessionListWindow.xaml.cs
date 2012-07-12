using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for ServersList.xaml
    /// </summary>
    public partial class GameSessionListWindow : Window
    {
        public List<GameSession> gameSessionList;

        public GameSessionListWindow()
        {
            InitializeComponent();

            gameSessionList = new List<GameSession>();
            listBoxServers.ItemsSource = gameSessionList;
        }

        private int id = 0;
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Update Server List here

            //Test:
            gameSessionList.Add(new GameSession() { Id = id, Name = "Игра #"+id, PlayersCount = new Random((int)DateTime.Now.Ticks).Next(5), PlayersLimit = 4 });
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

            var server = (GameSession) listBoxServers.SelectedItem;

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
