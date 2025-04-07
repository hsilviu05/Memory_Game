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
                return new ObservableCollection<Statistics>();
            }
            catch (Exception ex)
            {
                return new ObservableCollection<Statistics>();
            }
        }

        public bool SaveStatistics(ObservableCollection<Statistics> statistics)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(statistics, options);
                File.WriteAllText(_statisticsFilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddGameStatistics(Statistics statistics)
        {
            try
            {
                var allStatistics = LoadStatistics();
                allStatistics.Add(statistics);
                return SaveStatistics(allStatistics);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateStatistics(Statistics stats)
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

                return SaveStatistics(statistics);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteUserStatistics(string username)
        {
            try
            {
                var allStatistics = LoadStatistics();
                var userStatistics = allStatistics.Where(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var stat in userStatistics)
                {
                    allStatistics.Remove(stat);
                }

                return SaveStatistics(allStatistics);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ClearAllStatistics()
        {
            try
            {
                var emptyStatistics = new ObservableCollection<Statistics>();
                return SaveStatistics(emptyStatistics);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}