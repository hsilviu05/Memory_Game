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
using Memory_Game.Utilities;

namespace Memory_Game.ViewModel
{
    public class LoginViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
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

        private User _selectedUser;
        public User SelectedUser
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
            _userService = new UserService();

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

            SelectUserCommand = new RelayCommand<User>(
                execute: (User user) => ExecuteSelectUser(user),
                canExecute: (User user) => user != null
            );

            BrowseImageCommand = new RelayCommand(
                execute: (object param) => ExecuteBrowseImage(),
                canExecute: (object param) => true
            );

            Users = new ObservableCollection<User>();
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
                    CurrentPlayer = SelectedUser
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
                var newUser = new User(NewUser, NewUserImage, false);
                if (_userService.AddUser(newUser))
                {
                    Users.Add(newUser);
                    NewUser = string.Empty;
                    NewUserImage = string.Empty;
                }
                else
                {
                    MessageBox.Show("A user with this username already exists.", "Create User Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user: {ex.Message}", "Create User Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteUser()
        {
            try
            {
                if (SelectedUser != null)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete user {SelectedUser.Username}?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (_userService.DeleteUser(SelectedUser.Username))
                        {
                            Users.Remove(SelectedUser);
                            SelectedUser = null;
                        }
                        else
                        {
                            MessageBox.Show("Error deleting user.", "Delete User Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Delete User Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSelectUser(User user)
        {
            SelectedUser = user;
        }

        private void ExecuteBrowseImage()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (dialog.ShowDialog() == true)
            {
                NewUserImage = dialog.FileName;
            }
        }

        private void LoadUsers()
        {
            try
            {
                Users = _userService.LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Load Users Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Users = new ObservableCollection<User>();
            }
        }
    }
}
