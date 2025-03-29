using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Game.Model
{
    public class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public ObservableCollection<Card> Cards;
        public TimeSpan TimeLimit { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public string Category { get; set; }
        public bool IsGameOver;
        public bool IsWon;

        public Game() 
        {
            Cards = new ObservableCollection<Card>();
        }
        public Game(int boardWidth, int boardHeight, ObservableCollection<Card> cards, TimeSpan timeLimit, string category)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            Cards = cards;
            TimeLimit = timeLimit;
            Category = category;
            IsGameOver = false;
            IsWon = false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
