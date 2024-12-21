using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Beauty_Salon.Model;
using Beauty_Salon.Windows;

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
                    service.DiscountedCost = service.Discount.HasValue && service.Discount > 0
                        ? Math.Round(service.Cost - (service.Cost * (decimal)service.Discount), 2)
                        : service.Cost;

                    service.DiscountDescription = service.Discount.HasValue && service.Discount > 0
                        ? $"Скидка: {service.Discount:P0}"
                        : "Без скидки";
                }

                ApplyFilters(); // Применяем фильтры после загрузки
                DisplayPage(); // Отображаем данные
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}");
            }
        }

        private void DisplayPage()
        {
            int totalRecords = filteredServices.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Получаем текущую страницу данных
            var pagedServices = filteredServices
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            servicesList.ItemsSource = pagedServices; // Отображаем записи

            // Обновление текста с информацией о количестве записей
            recordCountText.Text = $"{currentPage} из {totalPages}";

            // Обновление состояния кнопок навигации
            btnBack.IsEnabled = currentPage > 1;
            btnNext.IsEnabled = currentPage < totalPages;
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
            int totalPages = (int)Math.Ceiling((double)filteredServices.Count / pageSize);
            if (currentPage < totalPages)
            {
                currentPage++;
                DisplayPage();
            }
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

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters(); // Применяем фильтры при изменении текста в поиске
            currentPage = 1; // Сброс текущей страницы
            DisplayPage();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters(); // Применяем фильтры при изменении сортировки
            currentPage = 1; // Сброс текущей страницы
            DisplayPage();
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
            currentPage = 1; // Сброс текущей страницы
            DisplayPage();
        }

        private void ApplyFilters()
        {
            string searchText = tbSearch.Text.ToLower();
            filteredServices = allServices
                .Where(s => s.Title.ToLower().Contains(searchText))
                .ToList();

            if (cbFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                filteredServices = FilterByDiscount(filteredServices, selectedItem);
            }

            if (cbSort.SelectedItem is ComboBoxItem sortItem)
            {
                filteredServices = SortServices(filteredServices, sortItem);
            }
        }

        private List<Service> FilterByDiscount(List<Service> services, ComboBoxItem selectedItem)
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

            // Фильтруем услуги по скидке
            return services.Where(s => s.Discount >= minDiscount && s.Discount <= maxDiscount).ToList();
        }

        private List<Service> SortServices(List<Service> services, ComboBoxItem sortItem)
        {
            // Сортируем услуги
            switch (sortItem.Tag)
            {
                case "1":
                    return services.OrderBy(s => s.Cost).ToList(); // По возрастанию стоимости
                case "2":
                    return services.OrderByDescending(s => s.Cost).ToList(); // По убыванию стоимости
                default:
                    return services; // Без сортировки
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

            NavigationService.Navigate(new AddingAndEditingServices(null));
            //MessageBox.Show("Добавление услуги еще не реализовано.");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (servicesList.SelectedItem is Service selectedService)
            {
                try
                {
                    using (var context = Number3Entities.GetContext())
                    {
                        context.Service.Remove(selectedService);
                        context.SaveChanges();
                    }
                    MessageBox.Show("Услуга успешно удалена!");
                    LoadServices(); // Обновляем список услуг после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления услуги: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите услугу для удаления.");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (servicesList.SelectedItem is Service selectedService)
            {
                NavigationService.Navigate(new AddingAndEditingServices(selectedService));
                MessageBox.Show("Обновление услуги еще не реализовано.");
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите услугу для обновления.");
            }
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CustomerRecord());
        }

        private void btnRecords_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UpcomingEntries());
        }
    }
}
