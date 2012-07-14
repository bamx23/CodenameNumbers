using System;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();

            try
            {
                MainWindow.Client = new GameClient("luckygeck.dyndns-home.com");
                MainWindow.Client.Start();
                MainWindow.Client.NetworkClient.ResponseEvent += ((o, e) => MessageBox.Show(e.Message()));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            textBoxLogin.Focus();
        }

        private void ButtonLoginClick(object sender, RoutedEventArgs e)
        {
            if(textBoxLogin.Text.Length == 0)
            {
                textBoxLogin.Focus();
                return; 
            }

            if (textBoxPassword.Password.Length == 0)
            {
                textBoxPassword.Focus();
                return;
            }

            //TODO: Login check here
            MainWindow.Client.Login(textBoxLogin.Text, textBoxPassword.Password);

            var slw = new GameSessionListWindow();
            slw.Show();
            slw.Closed += (s, o) => Close();
            Hide();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWindow.Client != null && MainWindow.Client.NetworkClient.Status != NetClientStatus.Stopped)
                MainWindow.Client.Stop();
        }

      
    }
}
