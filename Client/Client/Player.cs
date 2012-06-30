using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class Player
    {
        private string userName;
        private int userId;

        private int health;
        private int mana;
        private int score;

        public string UserName { get { return userName; } }

        public int Health { get { return health; } set {} }
        public int Mana { get { return mana; } set{} }
        public int Score { get { return score; } set{} }

        public Player(int userId, string userName)
        {
            this.userId = userId;
            this.userName = userName;

            health = 100;
            mana = 0;
            score = 0;
        }

        public void test()
        {
            mana += 10;
            health -= 10;
            score += 100;
        }
    }
}
