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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Beauty_Salon.Pages
{
    /// <summary>
    /// Логика взаимодействия для CustomerRecord.xaml
    /// </summary>
    public partial class CustomerRecord : Page
    {
        public CustomerRecord()
        {
            InitializeComponent();
        }

        // Используй для перехода назад - NavigationService.Navigate(new Service_page(null));
        // (так при возвращении на страницу данные будут обновляться)

        // Работа с данными пойдет в классе ClientService

        // Проверку что данные добавились можешь черех UpcomingEntries
    }
}
