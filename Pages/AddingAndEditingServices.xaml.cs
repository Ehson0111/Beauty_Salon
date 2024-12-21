using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using Beauty_Salon.Model;

namespace Beauty_Salon.Pages
{
    public partial class AddingAndEditingServices : Page
    {
        private Service service;
        private List<string> additionalImages = new List<string>();
        private string mainImagePath;

        public AddingAndEditingServices(Service service)
        {
            InitializeComponent();

            if (service != null)
            {
                this.service = service;
                LoadData(service);
                tbID.Visibility = Visibility.Visible;
                tbID.IsReadOnly = true;
            }
            else
            {
                this.service = new Service();
                tbID.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData(Service service)
        {
            using (Number3Entities db = new Number3Entities())
            {
                var existingService = db.Service.FirstOrDefault(s => s.ID == service.ID);
                if (existingService != null)
                {
                    additionalImages = existingService.ServicePhoto?
                        .Select(p => p.PhotoPath)
                        .ToList() ?? new List<string>();

                    tbID.Text = existingService.ID.ToString();
                    tbTitle.Text = existingService.Title;
                    tbCost.Text = existingService.Cost.ToString("F2");
                    tbDuration.Text = (existingService.DurationInSeconds / 60).ToString();
                    tbDescription.Text = existingService.Description;
                    tbDiscount.Text = existingService.Discount.HasValue
                        ? (existingService.Discount.Value * 100).ToString("F1")
                        : string.Empty;

                    if (!string.IsNullOrWhiteSpace(existingService.MainImagePath))
                    {
                        imgMain.Source = existingService.ImageSource;
                    }
                    else
                    {
                        MessageBox.Show("Главное изображение не задано.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }


                    UpdateAdditionalImages();
                }
            }
        }

        private void UpdateAdditionalImages()
        {
            spAdditionalImages.Children.Clear();

            foreach (string imagePath in additionalImages)
            {
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(5)
                };

                Button removeButton = new Button
                {
                    Content = "Удалить",
                    Tag = imagePath,
                    Margin = new Thickness(5)
                };
                removeButton.Click += BtnRemoveAdditionalImage_Click;

                panel.Children.Add(image);
                panel.Children.Add(removeButton);
                spAdditionalImages.Children.Add(panel);
            }
        }

        private void BtnSelectMainImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Title = "Выберите главное изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                mainImagePath = openFileDialog.FileName;
                imgMain.Source = new BitmapImage(new Uri(mainImagePath, UriKind.Absolute));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbTitle.Text) ||
                string.IsNullOrWhiteSpace(tbCost.Text) ||
                string.IsNullOrWhiteSpace(tbDuration.Text))
            {
                MessageBox.Show("Заполните все обязательные поля: название, стоимость, длительность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(tbCost.Text, out decimal cost) || cost < 0)
            {
                MessageBox.Show("Стоимость должна быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(tbDuration.Text, out int duration) || duration <= 0 || duration > 240)
            {
                MessageBox.Show("Длительность должна быть положительным числом и не превышать 240 минут.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(tbDiscount.Text) &&
                (!decimal.TryParse(tbDiscount.Text, out decimal discount) || discount < 0 || discount > 100))
            {
                MessageBox.Show("Скидка должна быть числом от 0 до 100.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new Number3Entities())
            {
                if (service != null) // Редактирование услуги
                {
                    var existingService = db.Service.FirstOrDefault(s => s.ID == service.ID);
                    if (existingService != null)
                    {
                        existingService.Title = tbTitle.Text;
                        existingService.Cost = cost;
                        existingService.DurationInSeconds = duration * 60;
                        existingService.Description = tbDescription.Text;
                        existingService.Discount = (double)(int.Parse(tbDiscount.Text) / 100);

                        if (!string.IsNullOrEmpty(mainImagePath))
                            existingService.MainImagePath = mainImagePath;

                        db.SaveChanges();
                        MessageBox.Show("Услуга успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else // Добавление услуги
                {
                    if (db.Service.Any(s => s.Title == tbTitle.Text))
                    {
                        MessageBox.Show("Услуга с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(mainImagePath))
                    {
                        MessageBox.Show("Выберите главное изображение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newService = new Service
                    {
                        Title = tbTitle.Text,
                        Cost = cost,
                        DurationInSeconds = duration * 60,
                        Description = tbDescription.Text,
                        Discount = (double)(int.Parse(tbDiscount.Text) / 100),
                        MainImagePath = mainImagePath
                    };

                    db.Service.Add(newService);
                    db.SaveChanges();
                    MessageBox.Show("Услуга успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            NavigationService.GoBack();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAddAdditionalImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png",
                Multiselect = true,
                Title = "Выберите дополнительные изображения"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string imagePath in openFileDialog.FileNames)
                {
                    string destinationPath = Path.Combine(Environment.CurrentDirectory, "Images", Path.GetFileName(imagePath));
                    if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                    File.Copy(imagePath, destinationPath, true);
                    additionalImages.Add(destinationPath);
                }
                UpdateAdditionalImages();
            }
        }

        private void BtnRemoveAdditionalImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button removeButton && removeButton.Tag is string imagePath)
            {
                additionalImages.Remove(imagePath);
                UpdateAdditionalImages();
            }
        }
    }
}
