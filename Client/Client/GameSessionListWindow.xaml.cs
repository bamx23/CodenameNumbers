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
        protected List<GameSession> gameSessionList;

        protected IAnimation updateAnimation;
        protected IAnimation connectAnimation;

        public GameSessionListWindow()
        {
            InitializeComponent();

            gameSessionList = new List<GameSession>();
            listBoxGameSessions.ItemsSource = gameSessionList;

            GameClient.Instance.GameSessionJoinEvent += OnGameSessionJoin;

            updateAnimation = new LoadingAnimation(canvasUpdate);
            connectAnimation = new LoadingAnimation(canvasConnect);
        }

        private int id = 0;
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            GameClient.Instance.GameSessionList();

            updateAnimation.Play();

            //Test:
            OnGameSessionList(this, null);
        }

        private void OnGameSessionList(object sender, object e)
        {
            updateAnimation.Stop();

            gameSessionList.Add(new GameSession { Id = id, Name = "Игра #" + id, PlayersCount = new Random((int)DateTime.Now.Ticks).Next(5), PlayersLimit = 4 });
            ++id;

            listBoxGameSessions.Items.Refresh();
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxGameSessions.SelectedIndex < 0)
            {
                if (listBoxGameSessions.Items.Count == 0)
                    MessageBox.Show("В данный момент нету активных игр.");
                else
                    MessageBox.Show("Выберите игру из списка.");
                return;
            }

            var gameSession = (GameSession) listBoxGameSessions.SelectedItem;

            GameClient.Instance.GameSessionJoin(gameSession.Id);
            connectAnimation.Play();
        }

        private void OnGameSessionJoin(object sender, BoolEventArgs e)
        {
            connectAnimation.Stop();
            if(e.Ok)
            {
                var mw = new MainWindow();
                mw.Show();
                mw.Closed += (s, o) => Show();
                mw.serverListWindow = this;
                Hide();
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к игре: " + e.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            buttonRefresh.Focus();
        }
    }
}
