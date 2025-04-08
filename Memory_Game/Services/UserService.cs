using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Memory_Game.Model;
using System.Diagnostics;
using System.Windows;

namespace Memory_Game.Services
{
    public class UserService
    {
        private readonly string _usersFilePath;
        private readonly string _userImagesDirectory;
        private readonly StatisticsService _statisticsService;

        public UserService()
        {
            try
            {
                // Get the correct project directory
                string projectDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (string.IsNullOrEmpty(projectDir))
                {
                    throw new DirectoryNotFoundException("Could not determine application directory");
                }

                _usersFilePath = Path.Combine(projectDir, "users.json");
                _userImagesDirectory = Path.Combine(projectDir, "UserImages");
                _statisticsService = new StatisticsService();

                // Create directories if they don't exist
                if (!Directory.Exists(_userImagesDirectory))
                {
                    Directory.CreateDirectory(_userImagesDirectory);
                }

                // Create users.json if it doesn't exist
                if (!File.Exists(_usersFilePath))
                {
                    var emptyUsers = new ObservableCollection<User>();
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(emptyUsers, options);
                    File.WriteAllText(_usersFilePath, json);
                }

                // Verify file is writable
                using (var fs = File.OpenWrite(_usersFilePath))
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UserService constructor: {ex.Message}");
                MessageBox.Show($"Error initializing user service. Please ensure the application has write permissions to:\n{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}",
                               "Initialization Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        public ObservableCollection<User> LoadUsers()
        {
            try
            {
                if (File.Exists(_usersFilePath))
                {
                    string json = File.ReadAllText(_usersFilePath);
                    var users = JsonSerializer.Deserialize<ObservableCollection<User>>(json);
                    return users ?? new ObservableCollection<User>();
                }
                return new ObservableCollection<User>();
            }
            catch (Exception ex)
            {
                return new ObservableCollection<User>();
            }
        }

        public bool SaveUsers(ObservableCollection<User> users)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(users, options);
                File.WriteAllText(_usersFilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddUser(User user)
        {
            try
            {
                var users = LoadUsers();
                if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }

                users.Add(user);
                return SaveUsers(users);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteUser(string username)
        {
            try
            {
                var users = LoadUsers();
                var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (user != null)
                {
                    // Delete user's image if it exists
                    if (!string.IsNullOrEmpty(user.ImagePath) && File.Exists(user.ImagePath))
                    {
                        File.Delete(user.ImagePath);
                    }

                    // Delete user's statistics
                    _statisticsService.DeleteUserStatistics(username);

                    users.Remove(user);
                    return SaveUsers(users);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                var users = LoadUsers();
                var existingUser = users.FirstOrDefault(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));

                if (existingUser != null)
                {
                    existingUser.ImagePath = user.ImagePath;
                    return SaveUsers(users);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string SaveUserImage(string imagePath)
        {
            try
            {
                string fileName = Path.GetFileName(imagePath);
                string newPath = Path.Combine(_userImagesDirectory, fileName);
                File.Copy(imagePath, newPath, true);
                return newPath;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void DeleteUserImage(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user image: {ex.Message}");
            }
        }

        public ObservableCollection<User> GetAllUsers()
        {
            return LoadUsers();
        }
    }
}