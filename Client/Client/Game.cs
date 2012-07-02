using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    class Game
    {
        public static readonly Timer timer;
        private readonly MainWindow window;

        private readonly List<Player> players;
        private readonly Dictionary<Player, PlayersStatsControl> playerControls;
        private readonly List<Hit> hits;

        private readonly List<Skill> skills;
        private readonly Dictionary<Skill, SkillControl> skillControls;
        
        public static Player me;

        static Game()
        {
            timer = new Timer();
            timer.Interval = 500;
        }

        /// <summary>
        /// Constructor for new Game
        /// </summary>
        /// <param name="window">MainWindow object of application</param>
        public Game(MainWindow window)
        {
            
            timer.Enabled = true;

            this.window = window;

            players = new List<Player>(); 
            playerControls = new Dictionary<Player, PlayersStatsControl>();
            
            skills = new List<Skill>();
            skillControls = new Dictionary<Skill, SkillControl>();

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
            window.gridPlayers.RowDefinitions.Add(new RowDefinition { Height = new GridLength(pControl.MinHeight) });
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
        /// Add new skill to Game
        /// </summary>
        /// <param name="skill">Object of new skill</param>
        /// <returns></returns>
        public bool AddSkill(Skill skill)
        {
            skills.Add(skill);

            var sControl = new SkillControl();
            window.gridSkills.RowDefinitions.Add(new RowDefinition { Height = new GridLength(sControl.MinHeight) });
            window.gridSkills.Children.Add(sControl);
            Grid.SetRow(sControl, skills.Count - 1);

            sControl.DataContext = skill;
            skillControls.Add(skill, sControl);

            window.KeyDown += skill.KeyDown;
            window.hitInput.KeyDown += skill.KeyDown;

            return true;
        }

        /// <summary>
        /// Removes all skills from Game
        /// </summary>
        /// <returns></returns>
        public bool ClearSkills()
        {
            foreach (var skillControl in skillControls)
            {
                window.gridSkills.Children.Remove(skillControl.Value);
                window.KeyDown -= skillControl.Key.KeyDown;
                window.hitInput.KeyDown -= skillControl.Key.KeyDown;
            }
            skills.Clear();
            skillControls.Clear();

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
