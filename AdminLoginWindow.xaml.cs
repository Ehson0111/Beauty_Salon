using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Beauty_Salon
{
    /// <summary>
    /// Логика взаимодействия для AdminLoginWindow.xaml
    /// </summary>
    public partial class AdminLoginWindow : Window
    {
        public AdminLoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == "0000")
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                errorMessage.Text = "Неверный пароль. Попробуйте еще раз.";
            }
        }
    }
}
