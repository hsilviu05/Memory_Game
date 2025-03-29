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
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public double WinRate { 
            get
            {
                if (GamesPlayed > 0)
                {
                    return (double)GamesWon/ GamesPlayed;
                }
                else 
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
