using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Memory_Game.Common;
using Memory_Game.Model;

namespace Memory_Game.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private Game _currentGame;
        public Game CurrentGame 
        { 
            get =>_currentGame;
            set
            {
                if (_currentGame != value)
                {
                    _currentGame = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsGameActive));
                    OnPropertyChanged(nameof(RemainingTime));
                }
            } 
        }

        private string _selectedCategory;
        public string SelectedCategory 
        { 
            get =>_selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanStartGame));
                }
            }
        }

        private int _boardWidth = 4;
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
                    OnPropertyChanged(nameof(CanStartGame));
                }
            }
        }

        private int _boardHeight = 4;
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
                    OnPropertyChanged(nameof(CanStartGame));
                }
            }
        }

        private TimeSpan _timeLimit = TimeSpan.FromMinutes(5);
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
                    OnPropertyChanged(nameof(CanStartGame));
                }
            }
        }
            

        public bool IsGameActive => CurrentGame != null && !CurrentGame.IsGameOver;
        public TimeSpan RemainingTime => IsGameActive ? TimeLimit - CurrentGame.ElapsedTime : TimeSpan.Zero;
        public bool CanStartGame => !string.IsNullOrEmpty(SelectedCategory) && BoardWidth * BoardHeight % 2 == 0;
        public bool CanSaveGame => IsGameActive;
        public bool CanLoadGame => true; // Will be implemented with saved games check

        public ICommand NewGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand LoadGameCommand { get; }
        public ICommand CardClickCommand { get; }
        public ICommand PauseGameCommand { get; }
        public ICommand ResumeGameCommand { get; }

        public GameViewModel()
        {
            // Initialize commands
            NewGameCommand = new RelayCommand(
                execute: (object param) => ExecuteNewGame(),
                canExecute: (object param) => CanStartGame
            );

            SaveGameCommand = new RelayCommand(
                execute: (object param) => ExecuteSaveGame(),
                canExecute: (object param) => CanSaveGame
            );

            LoadGameCommand = new RelayCommand(
                execute: (object param) => ExecuteLoadGame(),
                canExecute: (object param) => CanLoadGame
            );

            CardClickCommand = new RelayCommand<Card>(
                execute: (Card card) => ExecuteCardClick(card),
                canExecute: (Card card) => IsGameActive && card != null && !card.IsMatched
            );

            PauseGameCommand = new RelayCommand(
                execute: (object param) => ExecutePauseGame(),
                canExecute: (object param) => IsGameActive
            );

            ResumeGameCommand = new RelayCommand(
                execute: (object param) => ExecuteResumeGame(),
                canExecute: (object param) => !IsGameActive && CurrentGame != null
            );
        }
        private void ExecuteNewGame()
        {
            try
            {
                // Create new game with current settings
                CurrentGame = new Game(BoardWidth, BoardHeight, new ObservableCollection<Card>(), TimeLimit, SelectedCategory);
                // Initialize cards
                InitializeCards();
                // Start timer
                StartGameTimer();
            }
            catch (Exception ex)
            {
                // Handle error
            }
        }

        private void ExecuteSaveGame()
        {
            if (CurrentGame != null)
            {
                // Save game state
                // Will be implemented with game service
            }
        }

        private void ExecuteLoadGame()
        {
            // Load saved game
            // Will be implemented with game service
        }

        private void ExecuteCardClick(Card card)
        {
            if (card == null || card.IsMatched) return;

            // Handle card click logic
            // Will be implemented with game logic
        }

        private void ExecutePauseGame()
        {
            if (CurrentGame != null)
            {
                // Pause game timer
                // Will be implemented with timer service
            }
        }

        private void ExecuteResumeGame()
        {
            if (CurrentGame != null)
            {
                // Resume game timer
                // Will be implemented with timer service
            }
        }
        private void InitializeCards()
        {
            // Initialize cards with images from selected category
            // Will be implemented with game service
        }

        private void StartGameTimer()
        {
            // Start game timer
            // Will be implemented with timer service
        }

        private void CheckGameEnd()
        {
            // Check if game is won or lost
            // Will be implemented with game logic
        }
    }
}
