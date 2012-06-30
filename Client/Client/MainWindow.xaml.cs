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
        private List<Hit> hits;

        public MainWindow()
        {
            InitializeComponent();

            hits = new List<Hit>();
            hits.Add(new Hit(0, 0));

            listBoxOut.ItemsSource = hits;
            listBoxOut.Items.Refresh();

            hitInput.SelectAll();
        }

        private void UpdateList()
        {
            listBoxOut.Items.Refresh();

            listBoxOut.SelectedItem = listBoxOut.Items.GetItemAt(listBoxOut.Items.Count - 1);
            listBoxOut.ScrollIntoView(listBoxOut.SelectedItem);
            ListBoxItem item = listBoxOut.ItemContainerGenerator.ContainerFromItem(listBoxOut.SelectedItem) as ListBoxItem;
            item.Focus();

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
                        hits.Add(new Hit(int.Parse(((TextBox)sender).Text), 1));
                        ((TextBox)sender).Text = "";
                        UpdateList();
                        progressBar1.Value -= 5;
                    }
                    break;
            }

        }

        private void hitInput_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void progressBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var bar = sender as ProgressBar;
            const float k = 3f;
            bar.Foreground =
                new SolidColorBrush(Color.FromScRgb(1, 1 - (float)Math.Pow((float) e.NewValue/(float) bar.Maximum, k),
                                                    (float)Math.Pow((float) e.NewValue/(float) bar.Maximum, k), 0));
        }

        private int i = 0;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var p = new PlayersStatsControl();
            gridPlayers.Children.Add(p);
            Grid.SetRow(p, i++);

            var u = new Player(0, "Bam");
            p.DataContext = u;

            u.test();
        }
    }

    public class Hit
    {
        private int number;
        private int UserId;

        public Hit(int number, int userId)
        {
            this.number = number;
            UserId = userId;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}", UserId, number);
        }
    }
}
