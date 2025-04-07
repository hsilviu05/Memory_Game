using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Memory_Game.Common;
using Memory_Game.Model;
using Memory_Game.Services;
using System.Windows;
using Memory_Game.View;

namespace Memory_Game.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly StatisticsService _statisticsService;
        private ObservableCollection<Statistics> _allStatistics;
        private Statistics _selectedUserStats;

        public ObservableCollection<Statistics> AllStatistics
        {
            get => _allStatistics;
            set
            {
                _allStatistics = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasStatistics));
                UpdateAggregateStats();
            }
        }

        public Statistics SelectedUserStats
        {
            get => _selectedUserStats;
            set
            {
                _selectedUserStats = value;
                OnPropertyChanged();
            }
        }

        private string _selectedUser;
        public string SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                    // Update selected user stats when user selection changes
                    UpdateSelectedUserStats();
                }
            }
        }

        public bool HasStatistics => AllStatistics != null && AllStatistics.Any();
        public double AverageWinRate => HasStatistics ? AllStatistics.Average(s => s.WinRate) : 0;
        public int TotalGamesPlayed => HasStatistics ? AllStatistics.Sum(s => s.GamesPlayed) : 0;
        public int TotalGamesWon => HasStatistics ? AllStatistics.Sum(s => s.GamesWon) : 0;
        public TimeSpan BestOverallTime => HasStatistics ?
            TimeSpan.FromTicks(AllStatistics.Min(s => s.BestTime.Ticks)) :
            TimeSpan.Zero;
        public double AverageMovesPerGame => HasStatistics ?
            AllStatistics.Average(s => s.AverageMovesPerGame) : 0;

        public ICommand RefreshStatisticsCommand { get; }
        public ICommand SelectUserStatsCommand { get; }
        public ICommand ClearStatisticsCommand { get; }
        public ICommand CloseCommand { get; }

        public StatisticsViewModel()
        {
            _statisticsService = new StatisticsService();
            LoadStatistics();

            // Initialize commands
            RefreshStatisticsCommand = new RelayCommand(
                execute: (object param) => ExecuteRefreshStatistics(),
                canExecute: (object param) => true
            );

            SelectUserStatsCommand = new RelayCommand<string>(
                execute: (string username) => ExecuteSelectUserStats(username),
                canExecute: (string username) => !string.IsNullOrEmpty(username)
            );

            ClearStatisticsCommand = new RelayCommand(
                execute: (object param) => ExecuteClearStatistics(),
                canExecute: (object param) => HasStatistics
            );

            CloseCommand = new RelayCommand(
                execute: (param) => ExecuteClose(),
                canExecute: (param) => true
            );
        }

        private void ExecuteRefreshStatistics()
        {
            try
            {
                LoadStatistics();
            }
            catch (Exception ex)
            {
                // Handle error
            }
        }

        private void ExecuteSelectUserStats(string username)
        {
            if (string.IsNullOrEmpty(username)) return;

            SelectedUser = username;
            UpdateSelectedUserStats();
        }

        private void ExecuteClearStatistics()
        {
            try
            {
                _statisticsService.ClearAllStatistics();
                LoadStatistics();
                SelectedUserStats = null;
                SelectedUser = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatistics()
        {
            var stats = _statisticsService.LoadStatistics();
            AllStatistics = new ObservableCollection<Statistics>(stats);
            if (HasStatistics)
            {
                SelectedUserStats = AllStatistics.First();
            }
        }

        private void UpdateSelectedUserStats()
        {
            if (string.IsNullOrEmpty(SelectedUser) || AllStatistics == null)
            {
                SelectedUserStats = null;
                return;
            }

            SelectedUserStats = AllStatistics.FirstOrDefault(s => s.Username == SelectedUser);
        }

        private void SaveStatistics()
        {
            // Save statistics to file
            // Will be implemented with statistics service
        }

        private void UpdateAggregateStats()
        {
            OnPropertyChanged(nameof(AverageWinRate));
            OnPropertyChanged(nameof(TotalGamesPlayed));
            OnPropertyChanged(nameof(TotalGamesWon));
            OnPropertyChanged(nameof(BestOverallTime));
            OnPropertyChanged(nameof(AverageMovesPerGame));
        }

        private void ExecuteClose()
        {
            var window = System.Windows.Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.Content is StatisticsView);

            window?.Close();
        }
    }
}
