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

        private int _gamesPlayed;
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

        private int _gamesWon;
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

        public double WinRate
        {
            get
            {
                if (GamesPlayed > 0)
                {
                    return (double)GamesWon / GamesPlayed;
                }
                return 0;
            }
        }

        public TimeSpan BestTime { get; set; }
        public int TotalMoves { get; set; }

        public double AverageMovesPerGame => GamesPlayed > 0 ? (double)TotalMoves / GamesPlayed : 0;

        public Statistics()
        {
            BestTime = TimeSpan.MaxValue;
        }

        public Statistics(string username)
        {
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
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
