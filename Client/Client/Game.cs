using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Client
{
    class Game
    {
        private MainWindow window;

        private List<Player> players;
        private Dictionary<Player, PlayersStatsControl> playerControls;
        private List<Hit> hits;
        
        public static Player me;

        public Game(MainWindow window)
        {
            this.window = window;

            players = new List<Player>(); 
            playerControls = new Dictionary<Player, PlayersStatsControl>();

            hits = new List<Hit>();
            window.listBoxOut.ItemsSource = hits;
        }

        public bool AddPlayer(Player player)
        {
            players.Add(player);

            var p = new PlayersStatsControl();
            window.gridPlayers.Children.Add(p);
            Grid.SetRow(p, players.Count-1);

            p.DataContext = player;

            return true;
        }

        public bool AddHit(int playerId, bool correct, int number, long timestamp)
        {
            var player = players.Find(p => p.UserId == playerId);
            if (player == null) return false;

            hits.Add(new Hit(number, correct, player, timestamp));
            return true;
        }

        public bool Damage(int playerId, int count)
        {
            var player = players.Find(p => p.UserId == playerId);
            if (player == null) return false;

            me.Damage(count);
            return true;
        }
    }
}
