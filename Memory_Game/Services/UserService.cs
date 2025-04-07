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
                // Use a simpler path in the project directory
                string projectDir = Directory.GetCurrentDirectory();
                _usersFilePath = Path.Combine(projectDir, "users.json");
                _userImagesDirectory = Path.Combine(projectDir, "UserImages");
                _statisticsService = new StatisticsService();

                MessageBox.Show($"Project directory: {projectDir}\nUsers file path: {_usersFilePath}", "Debug Info");

                // Create directories if they don't exist
                if (!Directory.Exists(_userImagesDirectory))
                {
                    Directory.CreateDirectory(_userImagesDirectory);
                    MessageBox.Show($"Created UserImages directory at: {_userImagesDirectory}", "Debug Info");
                }

                // Create users.json if it doesn't exist
                if (!File.Exists(_usersFilePath))
                {
                    var emptyUsers = new ObservableCollection<User>();
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(emptyUsers, options);
                    File.WriteAllText(_usersFilePath, json);
                    MessageBox.Show($"Created users.json at: {_usersFilePath}", "Debug Info");
                }
                else
                {
                    MessageBox.Show($"users.json already exists at: {_usersFilePath}", "Debug Info");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in UserService constructor: {ex.Message}\nStack trace: {ex.StackTrace}", "Error");
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
                    Debug.WriteLine($"Loaded {users?.Count ?? 0} users from {_usersFilePath}");
                    return users ?? new ObservableCollection<User>();
                }
                else
                {
                    Debug.WriteLine($"Users file not found at: {_usersFilePath}");
                    return new ObservableCollection<User>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading users: {ex.Message}");
                return new ObservableCollection<User>();
            }
        }

        public void SaveUsers(ObservableCollection<User> users)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(users, options);
                File.WriteAllText(_usersFilePath, json);
                Debug.WriteLine($"Saved {users.Count} users to {_usersFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        public bool AddUser(User user)
        {
            try
            {
                var users = LoadUsers();
                if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    Debug.WriteLine($"User {user.Username} already exists");
                    return false;
                }

                string newImagePath = CopyUserImage(user.ImagePath, user.Username);
                user.ImagePath = newImagePath;

                users.Add(user);
                SaveUsers(users);
                Debug.WriteLine($"Added new user: {user.Username}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding user: {ex.Message}");
                return false;
            }
        }

        private string CopyUserImage(string sourcePath, string username)
        {
            try
            {
                string extension = Path.GetExtension(sourcePath);
                string newFileName = $"{username}{extension}";
                string newPath = Path.Combine(_userImagesDirectory, newFileName);

                File.Copy(sourcePath, newPath, true);
                Debug.WriteLine($"Copied user image to: {newPath}");
                return newPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error copying user image: {ex.Message}");
                return null;
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
                        Debug.WriteLine($"Deleted user image: {user.ImagePath}");
                    }

                    // Delete user's statistics
                    _statisticsService.DeleteUserStatistics(username);
                    Debug.WriteLine($"Deleted statistics for user: {username}");

                    users.Remove(user);
                    SaveUsers(users);
                    Debug.WriteLine($"Deleted user: {username}");
                    return true;
                }
                Debug.WriteLine($"User not found: {username}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                var users = LoadUsers();
                var existingUser = users.FirstOrDefault(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));

                if (existingUser != null)
                {
                    // If image path has changed, copy the new image
                    if (existingUser.ImagePath != user.ImagePath)
                    {
                        string newImagePath = CopyUserImage(user.ImagePath, user.Username);
                        user.ImagePath = newImagePath;
                    }

                    // Update user properties
                    existingUser.ImagePath = user.ImagePath;
                    SaveUsers(users);
                    Debug.WriteLine($"Updated user: {user.Username}");
                }
                else
                {
                    Debug.WriteLine($"User not found for update: {user.Username}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating user: {ex.Message}");
            }
        }

        public string SaveUserImage(string sourceImagePath)
        {
            try
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(sourceImagePath)}";
                string destinationPath = Path.Combine(_userImagesDirectory, fileName);
                File.Copy(sourceImagePath, destinationPath, true);
                return destinationPath;
            }
            catch (Exception ex)
            {
                // Log error or handle it appropriately
                Console.WriteLine($"Error saving user image: {ex.Message}");
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
                // Log error or handle it appropriately
                Console.WriteLine($"Error deleting user image: {ex.Message}");
            }
        }
    }
}