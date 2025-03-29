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

        public Statistics() { }
        public Statistics(string username, int gamesPlayed, int gamesWon)
        {
            Username = username;
            GamesPlayed = gamesPlayed;
            GamesWon = gamesWon;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
