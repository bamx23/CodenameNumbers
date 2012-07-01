using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    class Game
    {
        private readonly MainWindow window;

        private readonly List<Player> players;
        private readonly Dictionary<Player, PlayersStatsControl> playerControls;
        private readonly List<Hit> hits;
        
        public static Player me;

        /// <summary>
        /// Constructor for new Game
        /// </summary>
        /// <param name="window">MainWindow object of application</param>
        public Game(MainWindow window)
        {
            this.window = window;

            players = new List<Player>(); 
            playerControls = new Dictionary<Player, PlayersStatsControl>();
            

            hits = new List<Hit>();
            window.listBoxOut.ItemsSource = hits;
        }

        /// <summary>
        /// Add new player to Game
        /// </summary>
        /// <param name="player">Object of new player</param>
        /// <returns></returns>
        public bool AddPlayer(Player player)
        {
            players.Add(player);

            var pControl = new PlayersStatsControl();
            var playerRow = new RowDefinition();
            playerRow.Height = new GridLength(70);
            window.gridPlayers.RowDefinitions.Add(playerRow);
            window.gridPlayers.Children.Add(pControl);
            Grid.SetRow(pControl, players.Count-1);

            pControl.DataContext = player;
            playerControls.Add(player, pControl);

            return true;
        }

        /// <summary>
        /// Removes player from Game
        /// </summary>
        /// <param name="playerId">Id of player to remove</param>
        /// <returns></returns>
        public bool RemovePlayer(int playerId)
        {
            var player = players.Find(p => p.UserId == playerId);
            if (player == null) return false;

            for(int i = players.IndexOf(player) + 1; i < players.Count; ++i)
                Grid.SetRow(playerControls[players[i]], i - 1);
            window.gridPlayers.Children.Remove(playerControls[player]);
            playerControls.Remove(player);
            players.Remove(player);

            return true;
        }

        /// <summary>
        /// Add a Hit(new number)
        /// </summary>
        /// <param name="playerId">ID of player, who made a hit</param>
        /// <param name="correct">Flag of correct sended hit(in queue)</param>
        /// <param name="number">Sended number</param>
        /// <param name="timestamp">Time, when hit has been sent</param>
        /// <returns></returns>
        public bool AddHit(int playerId, bool correct, int number, long timestamp)
        {
            var player = players.Find(p => p.UserId == playerId);
            if (player == null) return false;

            hits.Add(new Hit(number, correct, player, timestamp));
            window.UpdateHitsList();
            return true;
        }

        /// <summary>
        /// Set HP of player
        /// </summary>
        /// <param name="playerId">ID of player to change HP</param>
        /// <param name="count">New value of HP</param>
        /// <returns></returns>
        public bool SetHealth(int playerId, int count)
        {
            var player = players.Find(p => p.UserId == playerId);
            if (player == null) return false;

            player.Damage(player.Health - count);
            return true;
        }

        /// <summary>
        /// Set MP of player
        /// </summary>
        /// <param name="playerId">ID of player to change MP</param>
        /// <param name="count">New value of MP</param>
        /// <returns></returns>
        public bool SetMana(int playerId, int count)
        {
            var player = players.Find(p => p.UserId == playerId);
            if (player == null) return false;

            player.SetMana(count);
            return true;
        }

        /// <summary>
        /// Set score of player
        /// </summary>
        /// <param name="playerId">ID of player to change score</param>
        /// <param name="count">New value of score</param>
        /// <returns></returns>
        public bool SetScore(int playerId, int count)
        {
            var player = players.Find(p => p.UserId == playerId);
            if (player == null) return false;

            player.SetScore(count);
            return true;
        }

        /// <summary>
        /// Show gameover message
        /// </summary>
        /// <param name="win">You win or you lose?</param>
        public void Gameover(bool win)
        {
            window.hitInput.IsEnabled = false;
            window.hitInput.Text = "";
            window.labelGameOverResult.Content = win ? "YOU WIN!" : "YOU LOSE!";
            window.gridGameOver.Visibility = Visibility.Visible;
        }
    }
}
