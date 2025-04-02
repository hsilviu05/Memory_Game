using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Memory_Game.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _newUsername;
        private string _newUserImage;
        private User _selectedUser;
        private ObservableCollection<User> _users;

        public string NewUsername
        {
            get => _newUsername;
            set
            {
                _newUsername = value;
                OnPropertyChanged();
            }
        }

        public string NewUserImage
        {
            get => _newUserImage;
            set
            {
                _newUserImage = value;
                OnPropertyChanged();
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                UpdateCommandStates();
            }
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChooseImageCommand { get; private set; }
        public ICommand CreateUserCommand { get; private set; }
        public ICommand PlayCommand { get; private set; }
        public ICommand DeleteUserCommand { get; private set; }

        public LoginViewModel()
        {
            Users = new ObservableCollection<User>();
            InitializeCommands();
            LoadUsers();
        }

        private void InitializeCommands()
        {
            ChooseImageCommand = new RelayCommand(ChooseImage);
            CreateUserCommand = new RelayCommand(CreateUser, CanCreateUser);
            PlayCommand = new RelayCommand(Play, CanPlay);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanDeleteUser);
        }

        private void ChooseImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif",
                Title = "Select Profile Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                NewUserImage = openFileDialog.FileName;
            }
        }

        private void CreateUser()
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewUserImage))
            {
                MessageBox.Show("Please enter a username and select an image.", "Validation Error");
                return;
            }

            // TODO: Add user creation logic here
            var newUser = new User { Username = NewUsername, ImagePath = NewUserImage };
            Users.Add(newUser);

            // Clear form
            NewUsername = string.Empty;
            NewUserImage = null;
        }

        private bool CanCreateUser()
        {
            return !string.IsNullOrWhiteSpace(NewUsername) && !string.IsNullOrWhiteSpace(NewUserImage);
        }

        private void Play()
        {
            // TODO: Implement game start logic
        }

        private bool CanPlay()
        {
            return SelectedUser != null;
        }

        private void DeleteUser()
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
                    Users.Remove(SelectedUser);
                    // TODO: Implement user deletion logic (remove files, statistics, etc.)
                }
            }
        }

        private bool CanDeleteUser()
        {
            return SelectedUser != null;
        }

        private void LoadUsers()
        {
            // TODO: Implement user loading logic
        }

        private void UpdateCommandStates()
        {
            (PlayCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
