using System;
using System.Windows;
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        private LoadingAnimation loginWaiting;

        public LoginWindow()
        {
            InitializeComponent();

            loginWaiting = new LoadingAnimation(canvasLoginWaiting);

            try
            {
                GameClient.Instance.Start();
                GameClient.Instance.LoginEvent += OnLoginResponse;
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
            loginWaiting.Play();
        }

        protected void OnLoginResponse(object sender, BoolEventArgs e)
        {
            loginWaiting.Stop();
            if (e.Ok)
            {
                var slw = new GameSessionListWindow();
                slw.Show();
                slw.Closed += (s, o) => Close();
                Hide();
            }
            else
            {
                MessageBox.Show("Не удалось залогиниться. Ошибка: " + e.Error);
            }
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
