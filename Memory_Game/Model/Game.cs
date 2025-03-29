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

        private int _boardWidth;
        public int BoardWidth
        {
            get => _boardWidth;
            set
            {
                if (value < 2 || value > 6)
                    throw new ArgumentException("Board width must be between 2 and 6");
                if (_boardWidth != value)
                {
                    _boardWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _boardHeight;
        public int BoardHeight
        {
            get => _boardHeight;
            set
            {
                if (value < 2 || value > 6)
                    throw new ArgumentException("Board height must be between 2 and 6");
                if (_boardHeight != value)
                {
                    _boardHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Card> _cards;
        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (_cards != value)
                {
                    _cards = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan _timeLimit;
        public TimeSpan TimeLimit
        {
            get => _timeLimit;
            set
            {
                if (value <= TimeSpan.Zero)
                    throw new ArgumentException("Time limit must be greater than zero");
                if (_timeLimit != value)
                {
                    _timeLimit = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan _elapsedTime;
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentException("Elapsed time cannot be negative");
                if (_elapsedTime != value)
                {
                    _elapsedTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Category cannot be empty");
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isGameOver;
        public bool IsGameOver
        {
            get => _isGameOver;
            set
            {
                if (_isGameOver != value)
                {
                    _isGameOver = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isWon;
        public bool IsWon
        {
            get => _isWon;
            set
            {
                if (_isWon != value)
                {
                    _isWon = value;
                    OnPropertyChanged();
                }
            }
        }

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
