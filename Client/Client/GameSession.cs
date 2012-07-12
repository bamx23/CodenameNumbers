using System;
using System.Linq;
using System.Windows.Media;

namespace Client
{
    public struct GameSession
    {
        public static readonly Brush BrushGameIsFull = new SolidColorBrush(Colors.Red);
        public static readonly Brush BrushGameIsNotFull = new SolidColorBrush(Colors.Black);

        public int Id;
        public string Name;
        public int PlayersCount;
        public int PlayersLimit;

        public bool IsFull { get { return PlayersCount >= PlayersLimit; } }

        public Brush Foreground { get { return IsFull ? BrushGameIsFull : BrushGameIsNotFull; } }

        public override string ToString()
        {
            return String.Format("#{0} - {1}({2}/{3})", Id, Name, PlayersCount, PlayersLimit);
        }
    }
}
