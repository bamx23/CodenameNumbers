using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client
{
    public class Hit
    {
        private readonly int number;
        private readonly Player player;
        private readonly bool correct;
        private long timestamp;

        public Brush Foreground
        {
            get { return correct ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red); }
        }

        public Hit(int number, bool correct, Player player, long timestamp)
        {
            this.number = number;
            this.correct = correct;
            this.player = player;
            this.timestamp = timestamp;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}", player == Game.me ? "Me" : player.UserName, number);
        }
    }
}
