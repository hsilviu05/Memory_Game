using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Game.Model
{
    public class Card : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Image path cannot be empty");
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFlipped;
        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                if (_isFlipped != value)
                {
                    _isFlipped = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isMatched;
        public bool IsMatched
        {
            get => _isMatched;
            set
            {
                if (_isMatched != value)
                {
                    _isMatched = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _position;
        public int Position
        {
            get => _position;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Position cannot be negative");
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _cardId;
        public int CardId
        {
            get => _cardId;
            set
            {
                if (value < 0)
                    throw new ArgumentException("CardId cannot be negative");
                if (_cardId != value)
                {
                    _cardId = value;
                    OnPropertyChanged();
                }
            }
        }

        public Card() { }
        public Card(string imagePath, bool isFlipped, bool isMatched, int position, int cardId)
        {
            ImagePath = imagePath;
            IsFlipped = isFlipped;
            IsMatched = isMatched;
            Position = position;
            CardId = cardId;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
