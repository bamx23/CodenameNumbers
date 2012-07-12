using System.Linq;
using System.Windows.Media;

namespace Client
{
    public class Hit
    {
        private readonly int number;
        private readonly Player player;
        private readonly bool correct;
        private long timestamp;
        private bool isTempory;

        public Brush Foreground
        {
            get
            {
                var color = new SolidColorBrush( correct ? Colors.Green : Colors.Red );
                if (isTempory)
                    color.Opacity *= 0.7f;
                return color;
            }
        }
        public bool IsTempory { get { return isTempory; } }
        public long Timestamp { get { return timestamp; } }

        public Hit(int number, bool correct, Player player, long timestamp, bool isTempory = false)
        {
            this.number = number;
            this.correct = correct;
            this.player = player;
            this.timestamp = timestamp;
            this.isTempory = isTempory;
        }

        public override string ToString()
        {
            return string.Format("{0}\t\t{1}", player == Game.me ? "Me" : player.UserName, number);
        }
    }
}
