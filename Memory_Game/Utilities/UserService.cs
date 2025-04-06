using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Memory_Game.Model;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace Memory_Game.Utilities
{
    public class UserService
    {
        private readonly string _usersFilePath;
        private readonly string _imageDirectory;

        public UserService()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _usersFilePath = Path.Combine(appDirectory, "users.txt");
            _imageDirectory = Path.Combine(appDirectory, "UserImages");
            Directory.CreateDirectory(_imageDirectory);
        }

        public ObservableCollection<User> LoadUsers()
        {
            try
            {
                if (!File.Exists(_usersFilePath))
                    return new ObservableCollection<User>();
                string json = File.ReadAllText(_usersFilePath);
                var users = JsonSerializer.Deserialize<ObservableCollection<User>>(json);
                return new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                return new ObservableCollection<User>();
            }
        }

        public void SaveUsers(ObservableCollection<User> users)
        {
            try
            {
                string json = JsonSerializer.Serialize(users);
                File.WriteAllText(_usersFilePath, json);
            }
            catch (Exception ex)
            {
                // Log exception
            }
        }

        public bool AddUser(User user)
        {
            try
            {
                var users = LoadUsers();
                if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                    return false;

                string newImagePath = CopyUserImage(user.ImagePath, user.Username);
                user.ImagePath = newImagePath;

                users.Add(user);
                SaveUsers(users);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string CopyUserImage(string sourcePath, string username)
        {
            string extension = Path.GetExtension(sourcePath);
            string newFileName = $"{username}{extension}";
            string newPath = Path.Combine(_imageDirectory, newFileName);

            File.Copy(sourcePath, newPath, true);
            return newPath;
        }

        private void DeleteUserImage(string imagePath)
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
                // Log exception
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
                    DeleteUserImage(user.ImagePath);
                    users.Remove(user);
                    SaveUsers(users);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
