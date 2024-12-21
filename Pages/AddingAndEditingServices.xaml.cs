using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Beauty_Salon.Model;

namespace Beauty_Salon.Windows
{
    public partial class AddingAndEditingServices : Window
    {
        private Service _service; // Сервис для редактирования или добавления
        private bool isEditMode = false; // Флаг для проверки редактирования

        public AddingAndEditingServices(Service service = null)
        {
            InitializeComponent();

            if (service != null)
            {
                isEditMode = true;
                _service = service;
                LoadServiceData();
            }
            else
            {
                _service = new Service();
            }
        }

        private void LoadServiceData()
        {
            tbTitle.Text = _service.Title;
            tbCost.Text = _service.Cost.ToString();
            tbDuration.Text = _service.DurationInSeconds.ToString(); // DurationInSeconds
            tbDescription.Text = _service.Description;
            tbDiscount.Text = _service.Discount?.ToString() ?? "0";

            // Загрузка основного изображения
            if (!string.IsNullOrEmpty(_service.MainImagePath))
            {
                imgMain.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_service.FullImagePath));
            }

            // Загрузка дополнительных изображений
            spAdditionalImages.Children.Clear();
            foreach (var imagePath in _service.ServicePhoto.Select(p => p.PhotoPath))
            {
                var image = new System.Windows.Controls.Image
                {
                    Width = 50,
                    Height = 50,
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath))
                };
                spAdditionalImages.Children.Add(image);
            }
        }

        private void BtnSelectMainImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                _service.MainImagePath = openFileDialog.FileName;
                imgMain.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void BtnAddAdditionalImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                var servicePhoto = new ServicePhoto
                {
                    PhotoPath = openFileDialog.FileName,
                    ServiceID = _service.ID // Привязка к сервису
                };

                _service.ServicePhoto.Add(servicePhoto);
                var image = new System.Windows.Controls.Image
                {
                    Width = 50,
                    Height = 50,
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(openFileDialog.FileName))
                };
                spAdditionalImages.Children.Add(image);
            }
        }

        private void BtnRemoveAdditionalImage_Click(object sender, RoutedEventArgs e)
        {
            if (_service.ServicePhoto.Count > 0)
            {
                var lastImage = _service.ServicePhoto.Last();
                _service.ServicePhoto.Remove(lastImage);
                spAdditionalImages.Children.RemoveAt(spAdditionalImages.Children.Count - 1);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                _service.Title = tbTitle.Text;
                _service.Cost = decimal.Parse(tbCost.Text);
                _service.DurationInSeconds = int.Parse(tbDuration.Text); // DurationInSeconds
                _service.Description = tbDescription.Text;
                _service.Discount = string.IsNullOrEmpty(tbDiscount.Text) ? 0 : decimal.Parse(tbDiscount.Text);

                try
                {
                    using (var context = Number3Entities.GetContext())
                    {
                        if (isEditMode)
                        {
                            context.Service.Update(_service);
                        }
                        else
                        {
                            if (context.Service.Any(s => s.Title == _service.Title))
                            {
                                MessageBox.Show("Услуга с таким названием уже существует.");
                                return;
                            }

                            context.Service.Add(_service);
                        }
                        context.SaveChanges();
                        MessageBox.Show("Услуга сохранена.");
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
        }

        private bool ValidateFields()
        {
            // Валидация полей
            if (string.IsNullOrEmpty(tbTitle.Text) || string.IsNullOrEmpty(tbCost.Text) || string.IsNullOrEmpty(tbDuration.Text))
            {
                MessageBox.Show("Все обязательные поля должны быть заполнены.");
                return false;
            }

            if (!int.TryParse(tbDuration.Text, out int duration) || duration <= 0 || duration > 240)
            {
                MessageBox.Show("Длительность услуги должна быть положительным числом и не превышать 4 часов.");
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
