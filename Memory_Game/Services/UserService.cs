using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Memory_Game.Model;

namespace Memory_Game.Services
{
    public class UserService
    {
        private readonly string _usersFilePath;
        private readonly string _userImagesDirectory;

        public UserService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MemoryGame");

            _usersFilePath = Path.Combine(appDataPath, "users.json");
            _userImagesDirectory = Path.Combine(appDataPath, "UserImages");

            // Create directories if they don't exist
            Directory.CreateDirectory(appDataPath);
            Directory.CreateDirectory(_userImagesDirectory);
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
            }
            catch (Exception ex)
            {
                // Log error or handle it appropriately
                Console.WriteLine($"Error loading users: {ex.Message}");
            }

            return new ObservableCollection<User>();
        }

        public void SaveUsers(ObservableCollection<User> users)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(users, options);
                File.WriteAllText(_usersFilePath, json);
            }
            catch (Exception ex)
            {
                // Log error or handle it appropriately
                Console.WriteLine($"Error saving users: {ex.Message}");
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