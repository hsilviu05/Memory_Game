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
        public string ImagePath { get; set; }
        public bool IsFlipped;
        public bool IsMatched;
        public int Position { get; set; }
        public int CardId { get; set; }
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
