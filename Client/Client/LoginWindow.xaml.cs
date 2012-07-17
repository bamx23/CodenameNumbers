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
                GameClient.Instance.Start();
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
            GameClient.Instance.Login(textBoxLogin.Text, textBoxPassword.Password);

            var slw = new GameSessionListWindow();
            slw.Show();
            slw.Closed += (s, o) => Close();
            Hide();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GameClient.Instance.Client.Status != NetClientStatus.Stopped)
                GameClient.Instance.Stop();
        }

        private void buttonRegistration_Click(object sender, RoutedEventArgs e)
        {
            var rw = new RegistrationWindow();
            rw.Show();
            rw.Closed += (s, o) => Show();
            Hide();
        }

      
    }
}
