using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Beauty_Salon.Model;

namespace Beauty_Salon.Pages
{
    public partial class Service_page : Page
    {
        private List<Service> allServices; // Все услуги
        private List<Service> filteredServices; // Отфильтрованные услуги
        private bool isAdmin = false; // Поле для хранения статуса администратора
        private int currentPage = 1; // Текущая страница
        private int pageSize = 10; // Количество услуг на странице

        public Service_page(Frame frame)
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadServices();
        }
        private void LoadServices()
        {
            UpdateAdminControlsVisibility(); // Устанавливаем начальную видимость контролов

            try
            {
                allServices = GetServicesFromDatabase();

                // Применяем скидки для отображения измененной стоимости
                foreach (var service in allServices)
                {
                    if (service.Discount.HasValue && service.Discount > 0)
                    {
                        service.DiscountedCost = service.Cost - (service.Cost * (decimal)service.Discount);
                        service.DiscountDescription = $"Скидка: {service.Discount:P0}";
                    }
                    else
                    {
                        service.DiscountedCost = service.Cost;
                        service.DiscountDescription = "Без скидки";
                    }

                    // Форматирование стоимости с двумя знаками после запятой
                    service.Cost = Math.Round(service.Cost, 2);
                    service.DiscountedCost = Math.Round(service.DiscountedCost, 2);
                }

                filteredServices = allServices;
                DisplayPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}");
            }
        }

        private void DisplayPage()
        {
            UpdateServiceList();
            UpdatePageControls();

            // Обновление текста общего числа записей
            recordCountText.Text = $"Всего записей: {filteredServices.Count}";
        }


        private List<Service> GetServicesFromDatabase()
        {
            try
            {
                using (var context = Number3Entities.GetContext())
                {
                    return context.Service.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запросе к базе данных: {ex.Message}");
                return new List<Service>();
            }
        }

        private void UpdateServiceList()
        {
            // Обновляем данные в ListBox
            servicesList.ItemsSource = filteredServices.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        private void UpdatePageControls()
        {
            btnBack.IsEnabled = currentPage > 1;
            btnNext.IsEnabled = currentPage < Math.Ceiling((double)filteredServices.Count / pageSize);
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower();

            filteredServices = allServices
                .Where(s => s.Title.ToLower().Contains(searchText))
                .ToList();

            currentPage = 1; // Сброс текущей страницы при изменении фильтра
            DisplayPage();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSort.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Tag)
                {
                    case "1": // По возрастанию стоимости
                        filteredServices = filteredServices.OrderBy(s => s.Cost).ToList();
                        break;
                    case "2": // По убыванию стоимости
                        filteredServices = filteredServices.OrderByDescending(s => s.Cost).ToList();
                        break;
                    default: // Без сортировки
                        filteredServices = allServices;
                        break;
                }

                currentPage = 1; // Сброс текущей страницы при изменении сортировки
                DisplayPage();
            }
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                double minDiscount = 0, maxDiscount = 1;

                switch (selectedItem.Tag)
                {
                    case "1":
                        minDiscount = 0;
                        maxDiscount = 0.05;
                        break;
                    case "2":
                        minDiscount = 0.05;
                        maxDiscount = 0.15;
                        break;
                    case "3":
                        minDiscount = 0.15;
                        maxDiscount = 0.30;
                        break;
                    case "4":
                        minDiscount = 0.30;
                        maxDiscount = 0.70;
                        break;
                    case "5":
                        minDiscount = 0.70;
                        maxDiscount = 1;
                        break;
                }

                filteredServices = allServices
                    .Where(s => s.Discount >= minDiscount && s.Discount <= maxDiscount)
                    .ToList();

                currentPage = 1; // Сброс текущей страницы при изменении фильтра
                DisplayPage();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                DisplayPage();
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < Math.Ceiling((double)filteredServices.Count / pageSize))
            {
                currentPage++;
                DisplayPage();
            }
        }
        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            AdminLoginWindow loginWindow = new AdminLoginWindow();
            bool? result = loginWindow.ShowDialog();
            if (result == true)
            {
                MessageBox.Show("Вы вошли в режим администратора!");
                isAdmin = true;
                UpdateAdminControlsVisibility();
            }
        }

        private void UpdateAdminControlsVisibility()
        {
            btnAdd.IsEnabled = isAdmin;
            btnDelete.IsEnabled = isAdmin;
            btnUpdate.IsEnabled = isAdmin;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление услуги еще не реализовано.");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Удаление услуги еще не реализовано.");
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Обновление услуги еще не реализовано.");
        }

      
    }
}
