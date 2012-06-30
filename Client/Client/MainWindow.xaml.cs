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

            myHealthBar.DataContext = Game.me;
            myManaBar.DataContext = Game.me;

            listBoxOut.Items.Refresh();
            hitInput.SelectAll();
        }

        private void UpdateList()
        {
            listBoxOut.Items.Refresh();

            if (listBoxOut.Items.Count > 1)
            {
                listBoxOut.SelectedItem = listBoxOut.Items.GetItemAt(listBoxOut.Items.Count - 1);
                listBoxOut.ScrollIntoView(listBoxOut.SelectedItem);
                ListBoxItem item =
                    listBoxOut.ItemContainerGenerator.ContainerFromItem(listBoxOut.SelectedItem) as ListBoxItem;
                item.Focus();
            }

            hitInput.Focus();
        }

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
                        game.AddHit(Game.me.UserId, true, int.Parse(((TextBox) sender).Text), DateTime.UtcNow.Ticks);
                        ((TextBox)sender).Text = "";
                        UpdateList();
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
            UpdateList();
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
        private PlayersStatsControl p;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            p = new PlayersStatsControl();
            gridPlayers.Children.Add(p);
            Grid.SetRow(p, i++);

            u = new Player(0, "Bam");
            p.DataContext = u;

            u.test();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            u.test();
        }

    }
}
