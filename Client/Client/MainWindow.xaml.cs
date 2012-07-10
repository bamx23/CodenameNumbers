using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Timers;
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

        private readonly Game game;
        private readonly NetClient client;

        public MainWindow()
        {
            InitializeComponent();

            game = new Game(this);
            Game.me = new Player(0, "Bam");
            game.AddPlayer(Game.me);

            myStats.DataContext = Game.me;

            client = new NetClient("192.168.33.55");
            client.ResponseEvent += ((o, e) => MessageBox.Show(e.Response));

            listBoxOut.Items.Refresh();
            hitInput.SelectAll();
            KeyDown += inputKeyDown;
        }

        public void UpdateHitsList()
        {
            listBoxOut.Items.Refresh();

            if (listBoxOut.Items.Count > 1)
                listBoxOut.ScrollIntoView(listBoxOut.Items.GetItemAt(listBoxOut.Items.Count - 1));

            hitInput.Focus();
        }

        private int hitCounter = 0;
        private void inputKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            int dig = e.Key.TryParseDigit();
            if (dig != -1)
            {
                hitInput.Text += dig.ToString(CultureInfo.InvariantCulture);
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    if (hitInput.Text.Length != 0)
                    {
                        //TODO: HIT MAKES HERE
                        var hit = int.Parse(hitInput.Text);
                        game.AddHit(Game.me.UserId, hit == hitCounter + 1, hit, DateTime.UtcNow.Ticks, true);
                        if (hitCounter + 1 == hit)
                        {
                            hitCounter++;
                            Game.me.SetScore(Game.me.Score + 10);
                            //client.Send("test");
                        }
                        else
                        {
                            Game.me.SetScore(Game.me.Score - 10);
                            Game.me.Damage(5);
                        }
                        hitInput.Text = "";
                    }
                    break;

                case Key.Space:
                case Key.Back:
                    if (hitInput.Text.Length != 0)
                        hitInput.Text = hitInput.Text.Substring(0, hitInput.Text.Length - 1);
                    break;

                default:
                    e.Handled = false;
                    break;
            }
        }

        private void myHealthBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            healthBarChanged(sender, e);
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
        private Skill s;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            u = new Player(++i, "User #" + i);
            game.AddPlayer(u);
            u.test();
            s = new Skill("Frost", Key.F, 5000);
            game.AddSkill(s);
            s = new Skill("Slice", Key.S, 3000);
            game.AddSkill(s);

            UpdateHitsList();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if(u != null)
                u.test();
            game.ClearSkills();

            UpdateHitsList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client.Stop();
        }

    }
}
