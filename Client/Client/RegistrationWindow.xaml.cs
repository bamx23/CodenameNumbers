using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        protected LoadingAnimation registrationAnimation;

        public RegistrationWindow()
        {
            InitializeComponent();

            registrationAnimation = new LoadingAnimation(canvasRegistration);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public bool CheckTextBox(TextBox box)
        {
            if(box.Text.Length == 0)
            {
                MessageBox.Show("Вы ввели не все данные.");
                box.Focus();
                return false;
            }
            return true;
        }

        public bool CheckTextBox(PasswordBox box)
        {
            if (box.Password.Length == 0)
            {
                MessageBox.Show("Вы ввели не все данные.");
                box.Focus();
                return false;
            }
            return true;
        }

        private void buttonReg_Click(object sender, RoutedEventArgs e)
        {
            if(!(CheckTextBox(textBoxLogin) && CheckTextBox(textBoxPassword) && CheckTextBox(textBoxPasswordConfirm) && CheckTextBox(textBoxEmail) && CheckTextBox(textBoxNickname)))
                return;

            if(textBoxPassword.Password != textBoxPasswordConfirm.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            //TODO: Registration process

            registrationAnimation.Play();
            /*Заглушка:*/
            new Thread(() => { Thread.Sleep(2000); Dispatcher.Invoke((Action) (() => registrationAnimation.Stop())); }).Start();
        }
    }
}
