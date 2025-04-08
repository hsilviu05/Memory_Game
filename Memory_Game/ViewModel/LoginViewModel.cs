using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memory_Game.Model;
using System.Windows.Input;
using Memory_Game.Common;
using System.Windows;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Memory_Game.View;
using Memory_Game.Services;

namespace Memory_Game.ViewModel
{
    public class LoginViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly Services.UserService _userService;
        private readonly Services.StatisticsService _statisticsService;
        private ObservableCollection<UserWithStats> _users;
        public ObservableCollection<UserWithStats> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        }

        private UserWithStats _selectedUser;
        public UserWithStats SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanDeleteUser));
                    OnPropertyChanged(nameof(CanLogin));
                }
            }
        }

        private string _newUser;
        public string NewUser
        {
            get => _newUser;
            set
            {
                if (_newUser != value)
                {
                    _newUser = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCreateUser));
                }
            }
        }

        private string _newUserImage;
        public string NewUserImage
        {
            get => _newUserImage;
            set
            {
                if (_newUserImage != value)
                {
                    _newUserImage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanCreateUser));
                }
            }
        }

        public bool CanDeleteUser => SelectedUser != null;
        public bool CanLogin => SelectedUser != null;
        public bool CanCreateUser => !string.IsNullOrWhiteSpace(NewUser) && !string.IsNullOrWhiteSpace(NewUserImage);

        public ICommand LoginCommand { get; }
        public ICommand CreateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand SelectUserCommand { get; }
        public ICommand BrowseImageCommand { get; }

        public LoginViewModel()
        {
            _userService = new Services.UserService();
            _statisticsService = new Services.StatisticsService();

            LoginCommand = new RelayCommand(
                execute: (object param) => ExecuteLogin(),
                canExecute: (object param) => CanLogin
            );

            CreateUserCommand = new RelayCommand(
                execute: (object param) => ExecuteCreateUser(),
                canExecute: (object param) => CanCreateUser
            );

            DeleteUserCommand = new RelayCommand(
                execute: (object param) => ExecuteDeleteUser(),
                canExecute: (object param) => CanDeleteUser
            );

            SelectUserCommand = new RelayCommand<UserWithStats>(
                execute: (UserWithStats user) => ExecuteSelectUser(user),
                canExecute: (UserWithStats user) => user != null
            );

            BrowseImageCommand = new RelayCommand(
                execute: (object param) => ExecuteBrowseImage(),
                canExecute: (object param) => true
            );

            Users = new ObservableCollection<UserWithStats>();
            LoadUsers();
        }

        private void ExecuteLogin()
        {
            try
            {
                if (SelectedUser == null)
                {
                    MessageBox.Show("Please select a user to login.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var gameViewModel = new GameViewModel
                {
                    CurrentPlayer = new User { Username = SelectedUser.Username, ImagePath = SelectedUser.ImagePath }
                };

                var gameView = new GameView
                {
                    DataContext = gameViewModel
                };

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Content = gameView;
                }
                else
                {
                    var window = new Window
                    {
                        Title = "Memory Game",
                        Content = gameView,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Width = 1000,
                        Height = 700
                    };
                    window.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during login: {ex.Message}", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCreateUser()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewUser))
                {
                    MessageBox.Show("Please enter a username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewUserImage))
                {
                    MessageBox.Show("Please select a profile image.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newUser = new User
                {
                    Username = NewUser,
                    ImagePath = NewUserImage
                };

                if (_userService.AddUser(newUser))
                {
                    // Initialize statistics for the new user
                    var newStats = new Statistics(newUser.Username);
                    _statisticsService.UpdateStatistics(newStats);

                    LoadUsers(); // Reload users to include the new user with stats
                    NewUser = string.Empty;
                    NewUserImage = null;
                }
                else
                {
                    MessageBox.Show("Failed to create user. Username might already exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteUser()
        {
            try
            {
                if (SelectedUser == null) return;

                var result = MessageBox.Show(
                    $"Are you sure you want to delete user {SelectedUser.Username}? This will also delete their statistics.",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    if (_userService.DeleteUser(SelectedUser.Username))
                    {
                        // Statistics are deleted in UserService.DeleteUser
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSelectUser(UserWithStats user)
        {
            SelectedUser = user;
        }

        private void ExecuteBrowseImage()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                    Title = "Select a profile image"
                };

                if (dialog.ShowDialog() == true)
                {
                    NewUserImage = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUsers()
        {
            var users = _userService.GetAllUsers();
            Users.Clear();

            foreach (var user in users)
            {
                var stats = _statisticsService.GetUserStatistics(user.Username);
                Users.Add(new UserWithStats
                {
                    Username = user.Username,
                    ImagePath = string.IsNullOrEmpty(user.ImagePath) ? "pack://application:,,,/Resources/default_user.png" : user.ImagePath,
                    GamesWon = stats.GamesWon,
                    BestTime = stats.BestTime,
                    AverageMovesPerGame = stats.AverageMovesPerGame
                });
            }
        }
    }

    public class UserWithStats
    {
        public string Username { get; set; }
        public string ImagePath { get; set; }
        public int GamesWon { get; set; }
        public TimeSpan BestTime { get; set; }
        public double AverageMovesPerGame { get; set; }
    }
}
