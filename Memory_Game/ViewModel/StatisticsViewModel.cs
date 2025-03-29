using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Memory_Game.Common;
using Memory_Game.Model;

namespace Memory_Game.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        private ObservableCollection<Statistics> _allStatistics;
        public ObservableCollection<Statistics> AllStatistics 
        { 
            get => _allStatistics;
            set 
            {
                if (_allStatistics != value)
                {
                    _allStatistics = value;
                    OnPropertyChanged();
                }
            }
        }

        private Statistics _selectedUserStats;
        public Statistics SelectedUserStats 
        {
            get => _selectedUserStats;
            set 
            {
                if (_selectedUserStats != value)
                {
                    _selectedUserStats = value;
                    OnPropertyChanged();
                }
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

        public ICommand RefreshStatisticsCommand { get; }
        public ICommand SelectUserStatsCommand { get; }

        public ICommand ClearStatisticsCommand { get; }

        public StatisticsViewModel()
        {
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

            // Initialize statistics collection
            AllStatistics = new ObservableCollection<Statistics>();

            // Load initial statistics
            LoadStatistics();
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
            if (AllStatistics != null)
            {
                AllStatistics.Clear();
                SelectedUserStats = null;
                SelectedUser = null;
                // Save empty statistics
                // Will be implemented with statistics service
            }
        }

        private void LoadStatistics()
        {
            // Load statistics from file
            // Will be implemented with statistics service
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
    }
}
