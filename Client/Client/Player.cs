using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Client
{
    public class Player : INotifyPropertyChanged
    {
        private string userName;
        private int userId;

        private int health;
        private int mana;
        private int score;

        public int UserId { get { return userId; } protected set { userId = value; NotifyPropertyChanged("UserId"); } }
        public string UserName { get { return userName; } protected set { userName = value; NotifyPropertyChanged("UserName"); } }

        public int Health { get { return health; } protected set { health = value; NotifyPropertyChanged("Health"); } }
        public int Mana { get { return mana; } protected set { mana = value; NotifyPropertyChanged("Mana");} }
        public int Score { get { return score; } protected set { score = value; NotifyPropertyChanged("Score"); } }

        public Player(int userId, string userName)
        {
            this.userId = userId;
            this.userName = userName;

            Health = 100;
            Mana = 0;
            Score = 0;
        }

        public void test()
        {
            Mana += 10;
            Health -= 10;
            Score += 100;
            UserName += ".";
        }

        public void Damage(int count)
        {
            Health = Math.Max(0, Health - count);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
