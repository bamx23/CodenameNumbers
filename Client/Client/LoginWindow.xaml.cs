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
                MainWindow.Client = new NetClient("192.168.33.55");
                MainWindow.Client.Start();
                MainWindow.Client.ResponseEvent += ((o, e) => MessageBox.Show(e.Response));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //Close();
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

            var slw = new ServersListWindow();
            slw.Show();
            slw.Closed += (s, o) => Close();
            Hide();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWindow.Client != null && MainWindow.Client.Status != NetClientStatus.Stopped)
                MainWindow.Client.Stop();
        }

      
    }
}
