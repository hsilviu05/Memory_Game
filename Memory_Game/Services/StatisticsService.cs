using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Memory_Game.Model;
using System.Diagnostics;

namespace Memory_Game.Services
{
    public class StatisticsService
    {
        private readonly string _statisticsFilePath;

        public StatisticsService()
        {
            string projectDir = Directory.GetCurrentDirectory();
            _statisticsFilePath = Path.Combine(projectDir, "statistics.json");
        }

        public ObservableCollection<Statistics> LoadStatistics()
        {
            try
            {
                if (File.Exists(_statisticsFilePath))
                {
                    string json = File.ReadAllText(_statisticsFilePath);
                    var statistics = JsonSerializer.Deserialize<ObservableCollection<Statistics>>(json);
                    return statistics ?? new ObservableCollection<Statistics>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading statistics: {ex.Message}");
            }

            return new ObservableCollection<Statistics>();
        }

        public void SaveStatistics(ObservableCollection<Statistics> statistics)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(statistics, options);
                File.WriteAllText(_statisticsFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving statistics: {ex.Message}");
            }
        }

        public void UpdateStatistics(Statistics stats)
        {
            try
            {
                var statistics = LoadStatistics();
                var existingStats = statistics.FirstOrDefault(s => s.Username.Equals(stats.Username, StringComparison.OrdinalIgnoreCase));

                if (existingStats != null)
                {
                    existingStats.GamesPlayed = stats.GamesPlayed;
                    existingStats.GamesWon = stats.GamesWon;
                    existingStats.BestTime = stats.BestTime;
                    existingStats.TotalMoves = stats.TotalMoves;
                }
                else
                {
                    statistics.Add(stats);
                }

                SaveStatistics(statistics);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating statistics: {ex.Message}");
            }
        }

        public void DeleteUserStatistics(string username)
        {
            try
            {
                var statistics = LoadStatistics();
                var userStats = statistics.FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (userStats != null)
                {
                    statistics.Remove(userStats);
                    SaveStatistics(statistics);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting user statistics: {ex.Message}");
            }
        }

        public void ClearAllStatistics()
        {
            try
            {
                var emptyStatistics = new ObservableCollection<Statistics>();
                SaveStatistics(emptyStatistics);
                Debug.WriteLine("All statistics have been cleared");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error clearing statistics: {ex.Message}");
            }
        }
    }
}