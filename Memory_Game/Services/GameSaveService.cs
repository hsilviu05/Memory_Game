using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Memory_Game.Model;
using System.Diagnostics;

namespace Memory_Game.Services
{
    public class GameSaveService
    {
        private readonly string _savesDirectory;

        public GameSaveService()
        {
            try
            {
                // Get the executable's directory instead of AppData
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                _savesDirectory = Path.Combine(baseDir, "SavedGames");

                if (!Directory.Exists(_savesDirectory))
                {
                    Directory.CreateDirectory(_savesDirectory);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public string SaveGame(GameState gameState)
        {
            try
            {
                if (gameState == null)
                {
                    throw new ArgumentNullException(nameof(gameState));
                }

                if (string.IsNullOrEmpty(gameState.Username))
                {
                    throw new ArgumentException("Username cannot be empty");
                }

                string fileName = $"{gameState.Username}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(_savesDirectory, fileName);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(gameState, options);
                File.WriteAllText(filePath, json);

                return filePath;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public GameState LoadGame(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Saved game file not found", filePath);
                }

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<GameState>(json);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<string> GetSavedGames(string username)
        {
            try
            {
                if (!Directory.Exists(_savesDirectory))
                {
                    return new List<string>();
                }

                string pattern = $"{username}_*.json";
                return new List<string>(Directory.GetFiles(_savesDirectory, pattern));
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        public void DeleteSavedGames(string username)
        {
            try
            {
                foreach (string file in GetSavedGames(username))
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}