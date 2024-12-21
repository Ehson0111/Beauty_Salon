using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Beauty_Salon.Windows
{
    public partial class AddingAndEditingServices : Window
    {
        public AddingAndEditingServices()
        {
            InitializeComponent();
        }

        // Event handler for selecting the main image
        private void BtnSelectMainImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Load the selected image
                imgMain.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        // Event handler for adding an additional image
        private void BtnAddAdditionalImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Add the selected image to the additional images stack panel
                Image newImage = new Image
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(openFileDialog.FileName)),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(5)
                };
                spAdditionalImages.Children.Add(newImage);
            }
        }

        // Event handler for removing the last additional image
        private void BtnRemoveAdditionalImage_Click(object sender, RoutedEventArgs e)
        {
            if (spAdditionalImages.Children.Count > 0)
            {
                spAdditionalImages.Children.RemoveAt(spAdditionalImages.Children.Count - 1);
            }
        }

        // Event handler for saving the service details
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string title = tbTitle.Text;
            string costText = tbCost.Text;
            string durationText = tbDuration.Text;
            string description = tbDescription.Text;
            string discountText = tbDiscount.Text;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(costText) || string.IsNullOrWhiteSpace(durationText))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(costText, out decimal cost) || cost <= 0)
            {
                MessageBox.Show("Введите корректную стоимость услуги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(durationText, out int duration) || duration <= 0)
            {
                MessageBox.Show("Введите корректную длительность услуги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(discountText) && (!decimal.TryParse(discountText, out decimal discount) || discount < 0 || discount > 100))
            {
                MessageBox.Show("Введите корректную скидку (от 0 до 100%).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Perform save logic here (e.g., save to database)
            MessageBox.Show("Услуга успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        // Event handler for canceling
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
