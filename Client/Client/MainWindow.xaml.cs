using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly Key[] AvailableKeys = new Key[]
                                                  {
                                                      Key.NumPad0,
                                                      Key.NumPad1,
                                                      Key.NumPad2,
                                                      Key.NumPad3, 
                                                      Key.NumPad4, 
                                                      Key.NumPad5, 
                                                      Key.NumPad6, 
                                                      Key.NumPad7, 
                                                      Key.NumPad8, 
                                                      Key.NumPad9,
                                                      Key.Enter, 
                                                  };

        private Game game;

        public MainWindow()
        {
            InitializeComponent();

            game = new Game(this);
            Game.me = new Player(0, "Bam");
            game.AddPlayer(Game.me);

            myStats.DataContext = Game.me;

            listBoxOut.Items.Refresh();
            hitInput.SelectAll();
        }

        public void UpdateHitsList()
        {
            listBoxOut.Items.Refresh();

            if (listBoxOut.Items.Count > 1)
                listBoxOut.ScrollIntoView(listBoxOut.Items.GetItemAt(listBoxOut.Items.Count - 1));

            hitInput.Focus();
        }

        private int hitCounter = 0;
        private void hitInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (!AvailableKeys.Contains(e.Key))
            {
                e.Handled = true;
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    if (((TextBox)sender).Text.Length != 0)
                    {
                        var hit = int.Parse(((TextBox) sender).Text);
                        game.AddHit(Game.me.UserId, hit == hitCounter + 1, hit, DateTime.UtcNow.Ticks);
                        if (hitCounter + 1 == hit)
                        {
                            hitCounter++;
                            Game.me.SetScore(Game.me.Score + 10);
                        }
                        else
                        {
                            Game.me.SetScore(Game.me.Score - 10);
                            Game.me.Damage(5);
                        }
                        ((TextBox)sender).Text = "";
                    }
                    break;
            }

        }

        private void myHealthBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            healthBarChanged(sender, e);
        }

        private void hitInput_LostFocus(object sender, RoutedEventArgs e)
        {
            //UpdateHitsList();
        }

        public static void healthBarChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var bar = sender as ProgressBar;
            bar.Foreground =
                new SolidColorBrush(Color.FromScRgb(1, 1 - (float)Math.Pow((float) e.NewValue/(float) bar.Maximum, 3),
                                                    (float)Math.Pow((float) e.NewValue/(float) bar.Maximum, 3), 0));
        }

        private int i = 0;
        private Player u;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            u = new Player(++i, "User #" + i);
            game.AddPlayer(u);
            u.test();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if(u != null)
                u.test();
            game.Gameover(false);
        }

    }
}
