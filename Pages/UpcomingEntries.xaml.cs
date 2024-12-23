using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Beauty_Salon.Model;
using System.Data.Entity;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Beauty_Salon.Pages
{
    public partial class UpcomingEntries : Page
    {
        private ObservableCollection<EntryViewModel> _entries;
        private Timer _updateTimer;

        public UpcomingEntries()
        {
            InitializeComponent();
            _entries = new ObservableCollection<EntryViewModel>();
            UpcomingEntriesDataGrid.ItemsSource = _entries;

            Loaded += async (s, e) => await LoadData();
            StartAutoRefresh();

            // Подписка на событие Unloaded для остановки таймера
            this.Unloaded += (s, e) => StopAutoRefresh();
        }

        private async Task LoadData()
        {
            using (Number3Entities context = new Number3Entities())
            {
                try
                {
                    DateTime today = DateTime.Today;
                    DateTime tomorrow = today.AddDays(1.0);

                    var rawEntries = await context.ClientService
                        .Where(cs => DbFunctions.TruncateTime(cs.StartTime) == today ||
                                     DbFunctions.TruncateTime(cs.StartTime) == tomorrow)
                        .OrderBy(cs => cs.StartTime)
                        .Select(cs => new
                        {
                            ServiceTitle = cs.Service.Title,
                            LastName = cs.Client.LastName,
                            FirstName = cs.Client.FirstName,
                            Patronymic = cs.Client.Patronymic,
                            Email = cs.Client.Email,
                            Phone = cs.Client.Phone,
                            StartTime = cs.StartTime
                        })
                        .ToListAsync();

                    _entries.Clear();
                    foreach (var entry in rawEntries)
                    {
                        var entryViewModel = new EntryViewModel
                        {
                            ServiceName = entry.ServiceTitle,
                            ClientFullName = $"{entry.LastName} {entry.FirstName} {entry.Patronymic}",
                            ClientEmail = entry.Email,
                            ClientPhone = entry.Phone,
                            StartTime = entry.StartTime,
                        };

                        // Обновляем время до начала услуги
                        entryViewModel.UpdateTimeLeft();
                        _entries.Add(entryViewModel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
                }
            }
        }

        private void StartAutoRefresh()
        {
            _updateTimer = new Timer(30000); // 30 секунд
            _updateTimer.Elapsed += (s, e) =>
            {
                _ = Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        await LoadData(); // Полная загрузка данных
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Ошибка обновления данных: {ex.Message}");
                    }
                });
            };
            _updateTimer.Start();
        }

        private void StopAutoRefresh()
        {
            _updateTimer?.Stop();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class EntryViewModel : INotifyPropertyChanged
    {
        public string ServiceName { get; set; }
        public string ClientFullName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public DateTime StartTime { get; set; }

        private string _timeLeft;
        public string TimeLeft
        {
            get => _timeLeft;
            set
            {
                _timeLeft = value;
                OnPropertyChanged(nameof(TimeLeft));
            }
        }

        private Brush _timeLeftBrush;
        public Brush TimeLeftBrush
        {
            get => _timeLeftBrush;
            set
            {
                _timeLeftBrush = value;
                OnPropertyChanged(nameof(TimeLeftBrush));
            }
        }

        public void UpdateTimeLeft()
        {
            var timeLeft = StartTime - DateTime.Now;
            if (timeLeft.TotalSeconds <= 0)
            {
                TimeLeft = "Услуга началась";
                TimeLeftBrush = Brushes.Black;
            }
            else
            {
                var hours = (int)timeLeft.TotalHours;
                var minutes = timeLeft.Minutes;
                if(hours > 0)
                {
                    TimeLeft = $"{hours} час(а/ов) {minutes} минут";
                }
                else if(hours == 0)
                {
                    TimeLeft = $"{minutes} минут";
                }
                TimeLeftBrush = timeLeft.TotalMinutes < 60 ? Brushes.Red : Brushes.Black;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
