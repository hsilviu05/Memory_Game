using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using Memory_Game.Model;


namespace Memory_Game.Utilities
{
    public class StatisticsService
    {
        private readonly string _statisticsFilePath;

        public StatisticsService()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _statisticsFilePath = Path.Combine(appDirectory, "statistics.json");
        }

        public ObservableCollection<Statistics> LoadStatistics()
        {
            try
            {
                if (!File.Exists(_statisticsFilePath))
                    return new ObservableCollection<Statistics>();

                string json = File.ReadAllText(_statisticsFilePath);
                var stats = JsonSerializer.Deserialize<ObservableCollection<Statistics>>(json);
                return new ObservableCollection<Statistics>(stats);
            }
            catch (Exception ex)
            {
                return new ObservableCollection<Statistics>();
            }
        }

        public void SaveStatistics(ObservableCollection<Statistics> statistics)
        {
            try
            {
                string json = JsonSerializer.Serialize(statistics);
                File.WriteAllText(_statisticsFilePath, json);
            }
            catch (Exception ex)
            {
                // Log exception
            }
        }

        public void UpdateStatistics(string username, bool gameWon)
        {
            try
            {
                var statistics = LoadStatistics();
                var userStats = statistics.FirstOrDefault(s => s.Username == username);

                if (userStats == null)
                {
                    userStats = new Statistics();
                    statistics.Add(userStats);
                }

                userStats.GamesPlayed++;
                if (gameWon)
                    userStats.GamesWon++;

                SaveStatistics(statistics);
            }
            catch (Exception ex)
            {
                // Log exception
            }
        }

        public Statistics GetStatistics(string username)
        {
            try
            {
                var statistics = LoadStatistics();
                
                return statistics.FirstOrDefault(s => s.Username == username);
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
