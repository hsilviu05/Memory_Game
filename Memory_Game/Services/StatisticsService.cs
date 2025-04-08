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
    public class StatisticsService
    {
        private readonly string _statisticsFilePath;

        public StatisticsService()
        {
            try
            {
                string projectDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (string.IsNullOrEmpty(projectDir))
                {
                    throw new DirectoryNotFoundException("Could not determine application directory");
                }

                _statisticsFilePath = Path.Combine(projectDir, "statistics.json");

                // Create statistics.json if it doesn't exist
                if (!File.Exists(_statisticsFilePath))
                {
                    var emptyStatistics = new ObservableCollection<Statistics>();
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(emptyStatistics, options);
                    File.WriteAllText(_statisticsFilePath, json);
                }

                // Verify file is writable
                using (var fs = File.OpenWrite(_statisticsFilePath))
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in StatisticsService constructor: {ex.Message}");
                MessageBox.Show($"Error initializing statistics service. Please ensure the application has write permissions to:\n{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}",
                               "Initialization Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        public ObservableCollection<Statistics> LoadStatistics()
        {
            try
            {
                Debug.WriteLine($"Loading statistics from: {_statisticsFilePath}");

                if (File.Exists(_statisticsFilePath))
                {
                    string json = File.ReadAllText(_statisticsFilePath);
                    Debug.WriteLine($"Read JSON content: {json}");

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    var statistics = JsonSerializer.Deserialize<ObservableCollection<Statistics>>(json, options);

                    if (statistics != null)
                    {
                        Debug.WriteLine($"Loaded {statistics.Count} statistics records");
                        return statistics;
                    }
                }

                Debug.WriteLine("No existing statistics found, returning empty collection");
                return new ObservableCollection<Statistics>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading statistics: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new ObservableCollection<Statistics>();
            }
        }

        public bool SaveStatistics(ObservableCollection<Statistics> statistics)
        {
            try
            {
                Debug.WriteLine($"Saving statistics to: {_statisticsFilePath}");
                Debug.WriteLine($"Number of records to save: {statistics.Count}");

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                string json = JsonSerializer.Serialize(statistics, options);

                // Write to a temporary file first
                string tempPath = _statisticsFilePath + ".tmp";
                File.WriteAllText(tempPath, json);

                // If successful, move the temp file to replace the actual file
                if (File.Exists(_statisticsFilePath))
                {
                    File.Delete(_statisticsFilePath);
                }
                File.Move(tempPath, _statisticsFilePath);

                Debug.WriteLine("Statistics saved successfully");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving statistics: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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
                Debug.WriteLine($"Attempting to update statistics for user: {stats?.Username}");
                Debug.WriteLine($"Statistics file path: {_statisticsFilePath}");

                if (stats == null || string.IsNullOrEmpty(stats.Username))
                {
                    Debug.WriteLine("UpdateStatistics: Invalid statistics object");
                    return false;
                }

                var statistics = LoadStatistics();
                Debug.WriteLine($"Loaded {statistics.Count} existing statistics records");

                var existingStats = statistics.FirstOrDefault(s => s.Username.Equals(stats.Username, StringComparison.OrdinalIgnoreCase));

                if (existingStats != null)
                {
                    Debug.WriteLine($"Updating existing stats for {stats.Username}:");
                    Debug.WriteLine($"Games Played: {stats.GamesPlayed}, Games Won: {stats.GamesWon}");
                    Debug.WriteLine($"Best Time: {stats.BestTime}, Total Moves: {stats.TotalMoves}");

                    existingStats.GamesPlayed = stats.GamesPlayed;
                    existingStats.GamesWon = stats.GamesWon;
                    if (stats.BestTime < existingStats.BestTime || existingStats.BestTime == TimeSpan.MaxValue)
                    {
                        existingStats.BestTime = stats.BestTime;
                    }
                    existingStats.TotalMoves = stats.TotalMoves;
                }
                else
                {
                    Debug.WriteLine($"Adding new stats for {stats.Username}");
                    statistics.Add(stats);
                }

                var result = SaveStatistics(statistics);
                Debug.WriteLine($"Save result: {result}");

                // Verify the save worked by reading back
                if (result)
                {
                    var verifyStats = LoadStatistics();
                    var savedStats = verifyStats.FirstOrDefault(s => s.Username.Equals(stats.Username, StringComparison.OrdinalIgnoreCase));
                    if (savedStats != null)
                    {
                        Debug.WriteLine("Verification successful - statistics were saved");
                    }
                    else
                    {
                        Debug.WriteLine("Verification failed - statistics were not saved correctly");
                        result = false;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating statistics: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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

        public Statistics GetUserStatistics(string username)
        {
            try
            {
                Debug.WriteLine($"Getting statistics for user: {username}");

                if (string.IsNullOrWhiteSpace(username))
                {
                    Debug.WriteLine("GetUserStatistics: Username is null or empty");
                    throw new ArgumentException("Username cannot be empty");
                }

                var allStatistics = LoadStatistics();
                Debug.WriteLine($"Loaded {allStatistics.Count} total statistics records");

                var userStats = allStatistics.FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (userStats == null)
                {
                    Debug.WriteLine($"No existing statistics found for {username}, creating new statistics");
                    userStats = new Statistics(username);
                    allStatistics.Add(userStats);
                    var saveResult = SaveStatistics(allStatistics);
                    Debug.WriteLine($"Save result for new statistics: {saveResult}");
                }
                else
                {
                    Debug.WriteLine($"Found existing statistics for {username}:");
                    Debug.WriteLine($"Games Played: {userStats.GamesPlayed}, Games Won: {userStats.GamesWon}");
                    Debug.WriteLine($"Best Time: {userStats.BestTime}, Total Moves: {userStats.TotalMoves}");
                }

                return userStats;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetUserStatistics: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Rethrow to handle at a higher level
            }
        }
    }
}