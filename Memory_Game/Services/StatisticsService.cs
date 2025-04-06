using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Memory_Game.Model;

namespace Memory_Game.Services
{
    public class StatisticsService
    {
        private readonly string _statsFilePath;

        public StatisticsService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MemoryGame"
            );
            Directory.CreateDirectory(appDataPath);
            _statsFilePath = Path.Combine(appDataPath, "statistics.json");
        }

        public Dictionary<string, Statistics> LoadStatistics()
        {
            try
            {
                if (File.Exists(_statsFilePath))
                {
                    string jsonString = File.ReadAllText(_statsFilePath);
                    return JsonSerializer.Deserialize<Dictionary<string, Statistics>>(jsonString)
                           ?? new Dictionary<string, Statistics>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading statistics: {ex.Message}");
            }
            return new Dictionary<string, Statistics>();
        }

        public void SaveStatistics(Dictionary<string, Statistics> statistics)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(statistics, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_statsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving statistics: {ex.Message}");
            }
        }

        public void UpdatePlayerStats(string username, bool isWon, TimeSpan gameTime, int moves)
        {
            var statistics = LoadStatistics();
            if (!statistics.ContainsKey(username))
            {
                statistics[username] = new Statistics(username);
            }
            statistics[username].UpdateStats(isWon, gameTime, moves);
            SaveStatistics(statistics);
        }

        public Statistics GetPlayerStats(string username)
        {
            var statistics = LoadStatistics();
            return statistics.ContainsKey(username)
                ? statistics[username]
                : new Statistics(username);
        }
    }
}