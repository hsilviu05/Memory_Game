using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Game.Model
{
    public class Statistics : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _username;
        private int _gamesPlayed;
        private int _gamesWon;
        private TimeSpan _bestTime;
        private int _totalMoves;

        public string Username
        {
            get => _username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty");
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public int GamesPlayed
        {
            get => _gamesPlayed;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Games played cannot be negative");
                if (_gamesPlayed != value)
                {
                    _gamesPlayed = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WinRate));
                }
            }
        }

        public int GamesWon
        {
            get => _gamesWon;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Games won cannot be negative");
                if (value > GamesPlayed)
                    throw new ArgumentException("Games won cannot be greater than games played");
                if (_gamesWon != value)
                {
                    _gamesWon = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WinRate));
                }
            }
        }

        public TimeSpan BestTime
        {
            get => _bestTime;
            set
            {
                if (_bestTime != value)
                {
                    _bestTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalMoves
        {
            get => _totalMoves;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Total moves cannot be negative");
                if (_totalMoves != value)
                {
                    _totalMoves = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AverageMovesPerGame));
                }
            }
        }

        public double WinRate => GamesPlayed > 0 ? (double)GamesWon / GamesPlayed : 0;

        public double AverageMovesPerGame => GamesPlayed > 0 ? (double)TotalMoves / GamesPlayed : 0;

        public Statistics()
        {
            Username = "Unknown";
            GamesPlayed = 0;
            GamesWon = 0;
            BestTime = TimeSpan.MaxValue;
            TotalMoves = 0;
        }

        public Statistics(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            Username = username;
            GamesPlayed = 0;
            GamesWon = 0;
            BestTime = TimeSpan.MaxValue;
            TotalMoves = 0;
        }

        public void UpdateStats(bool isWon, TimeSpan gameTime, int moves)
        {
            GamesPlayed++;
            if (isWon)
            {
                GamesWon++;
                if (gameTime < BestTime)
                {
                    BestTime = gameTime;
                }
            }
            TotalMoves += moves;
            OnPropertyChanged(nameof(WinRate));
            OnPropertyChanged(nameof(AverageMovesPerGame));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
