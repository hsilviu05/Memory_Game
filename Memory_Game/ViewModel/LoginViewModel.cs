using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memory_Game.Model;
using System.Windows.Input;
using Memory_Game.Common;

namespace Memory_Game.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
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
        public ICommand ChooseImageCommand { get; }

        public LoginViewModel()
        {
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

            ChooseImageCommand = new RelayCommand(
                execute: (object param) => ExecuteBrowseImage(),
                canExecute: (object param) => true
            );

            Users = new ObservableCollection<User>();
            LoadUsers();
        }

        private void ExecuteLogin()
        {
            // Navigate to game view
            // This will be implemented later with navigation service
        }

        private void ExecuteCreateUser()
        {
            var newUser = new User(NewUser, NewUserImage, false);
            Users.Add(newUser);
            // Save users to file
            // Clear input fields
            NewUser = string.Empty;
            NewUserImage = string.Empty;
        }

        private void ExecuteDeleteUser()
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                SelectedUser = null;
                // Save users to file
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
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*",
                Title = "Select Profile Image"
            };
            if (dialog.ShowDialog() == true)
            {
                NewUserImage = dialog.FileName;
            }
        }

        private void LoadUsers()
        {
            // Load users from file
            // This will be implemented later with user service
        }
    }
}
