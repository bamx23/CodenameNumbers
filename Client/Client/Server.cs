using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Client
{
    public struct Server
    {
        public static readonly Brush BrushServerIsFull = new SolidColorBrush(Colors.Red);
        public static readonly Brush BrushServerIsNotFull = new SolidColorBrush(Colors.Black);

        public int Id;
        public string Name;
        public int PlayersCount;
        public int PlayersLimit;

        public bool IsFull { get { return PlayersCount >= PlayersLimit; } }

        public Brush Foreground { get { return IsFull ? BrushServerIsFull : BrushServerIsNotFull; } }

        public override string ToString()
        {
            return String.Format("#{0} - {1}({2}/{3})", Id, Name, PlayersCount, PlayersLimit);
        }
    }
}
