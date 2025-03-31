using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Memory_Game.Model;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace Memory_Game.Utilities
{
    public class GameService
    {
        private readonly string _savedGamesDirectory;

        public GameService()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _savedGamesDirectory = Path.Combine(appDirectory, "SavedGames");
            Directory.CreateDirectory(_savedGamesDirectory);
        }

        private Game CreateNewGame(int boardWidth, int boardHeight, string category)
        {
            var cards = GenerateCards(boardWidth, boardHeight, category);
            return new Game(boardWidth, boardHeight, cards, TimeSpan.FromMinutes(5), category);
        }

        private ObservableCollection<Card> GenerateCards(int width, int height, string category)
        {
            var cards = new ObservableCollection<Card>();
            var imagePaths = GetCategoryImages(category);
            var cardCount = width * height;

            for (int i = 0; i< cardCount/2;i++)
            {
                string imagePath = imagePaths[i % imagePaths.Count];
                cards.Add(new Card(imagePath, false, false, i * 2, i));
                cards.Add(new Card(imagePath, false, false, i * 2 + 1, i));
            }
        return new ObservableCollection<Card>(cards.OrderBy(c => Guid.NewGuid()));
        }

        private List<string> GetCategoryImages(string category)
        {
            return new List<string>();
        }
    }
}
