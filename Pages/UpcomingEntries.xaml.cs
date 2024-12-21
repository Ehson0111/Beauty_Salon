using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Beauty_Salon.Model;

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

            LoadData();
            StartAutoRefresh();

            // Подписка на событие Unloaded для остановки таймера
            this.Unloaded += (s, e) => StopAutoRefresh();
        }

        private void LoadData()
        {
            using (var context = new Number3Entities())
            {
                var today = DateTime.Now.Date;
                var tomorrow = today.AddDays(1);

                var entries = context.ClientService
                    .Where(cs => cs.StartTime.Date == today || cs.StartTime.Date == tomorrow)
                    .OrderBy(cs => cs.StartTime)
                    .Select(cs => new EntryViewModel
                    {
                        ServiceName = cs.Service.Title,
                        ClientFullName = $"{cs.Client.LastName} {cs.Client.FirstName} {cs.Client.Patronymic}",
                        ClientEmail = cs.Client.Email,
                        ClientPhone = cs.Client.Phone,
                        StartTime = cs.StartTime,
                        TimeLeft = CalculateTimeLeft(cs.StartTime)
                    }).ToList();

                _entries.Clear();
                foreach (var entry in entries)
                {
                    _entries.Add(entry);
                }
            }
        }

        private string CalculateTimeLeft(DateTime startTime)
        {
            var timeLeft = startTime - DateTime.Now;
            if (timeLeft.TotalSeconds <= 0)
                return "Услуга началась";

            var hours = timeLeft.Hours;
            var minutes = timeLeft.Minutes;

            return $"{hours} час(а/ов) {minutes} минут";
        }

        private void StartAutoRefresh()
        {
            _updateTimer = new Timer(30000); // 30 секунд
            _updateTimer.Elapsed += (s, e) => Dispatcher.Invoke(LoadData);
            _updateTimer.Start();
        }

        private void StopAutoRefresh()
        {
            _updateTimer?.Stop();
        }
    }

    public class EntryViewModel
    {
        public string ServiceName { get; set; }
        public string ClientFullName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public DateTime StartTime { get; set; }
        public string TimeLeft { get; set; }
        public Brush TimeLeftBrush => (StartTime - DateTime.Now).TotalMinutes < 60 ? Brushes.Red : Brushes.Black;
    }
}
