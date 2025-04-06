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
                    Debug.WriteLine($"Created saves directory at: {_savesDirectory}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing GameSaveService: {ex.Message}");
                throw;
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

                Debug.WriteLine($"Saving game to: {filePath}");

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonString = JsonSerializer.Serialize(gameState, options);
                File.WriteAllText(filePath, jsonString);

                Debug.WriteLine("Game saved successfully");
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving game: {ex.Message}");
                throw new Exception($"Failed to save game: {ex.Message}", ex);
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

                Debug.WriteLine($"Loading game from: {filePath}");

                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var gameState = JsonSerializer.Deserialize<GameState>(jsonString, options);
                Debug.WriteLine("Game loaded successfully");
                return gameState;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading game: {ex.Message}");
                throw new Exception($"Failed to load game: {ex.Message}", ex);
            }
        }

        public List<string> GetSavedGames(string username)
        {
            try
            {
                if (!Directory.Exists(_savesDirectory))
                {
                    Debug.WriteLine("Saves directory does not exist");
                    return new List<string>();
                }

                string[] files = Directory.GetFiles(_savesDirectory, $"{username}_*.json");
                Debug.WriteLine($"Found {files.Length} saved games for user {username}");
                return new List<string>(files);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting saved games: {ex.Message}");
                throw new Exception($"Failed to get saved games: {ex.Message}", ex);
            }
        }

        public void DeleteSavedGames(string username)
        {
            try
            {
                foreach (string file in GetSavedGames(username))
                {
                    File.Delete(file);
                    Debug.WriteLine($"Deleted saved game: {file}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting saved games: {ex.Message}");
                throw new Exception($"Failed to delete saved games: {ex.Message}", ex);
            }
        }
    }
}