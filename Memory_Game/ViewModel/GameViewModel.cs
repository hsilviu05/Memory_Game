using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Memory_Game.Common;
using Memory_Game.Model;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media.Imaging;
using Memory_Game.Services;
using Memory_Game.View;

namespace Memory_Game.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private readonly StatisticsService _statisticsService;
        private readonly GameSaveService _gameSaveService;

        private Game _currentGame;
        public Game CurrentGame
        {
            get => _currentGame;
            set
            {
                if (SetProperty(ref _currentGame, value))
                {
                    OnPropertyChanged(nameof(IsGameActive));
                    OnPropertyChanged(nameof(RemainingTime));
                }
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
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

                if (SetProperty(ref _boardWidth, value))
                {
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

                if (SetProperty(ref _boardHeight, value))
                {
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

                SetProperty(ref _timeLimit, value);
            }
        }

        private User _currentPlayer;
        public User CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (_currentPlayer != value)
                {
                    _currentPlayer = value;
                    OnPropertyChanged();

                    // Automatically select a category and start a new game when user is set
                    if (_currentPlayer != null && CurrentGame == null)
                    {
                        SelectedCategory = "Jordan 1"; // Changed from Animals to Jordan 1
                        ExecuteNewGame();
                    }
                }
            }
        }

        private ObservableCollection<string> _categories;
        public ObservableCollection<string> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged();
                }
            }
        }

        private Card _firstFlippedCard;
        private Card _secondFlippedCard;
        private int _moves;
        private int _matches;

        public int Moves
        {
            get => _moves;
            set => SetProperty(ref _moves, value);
        }

        public int Matches
        {
            get => _matches;
            set
            {
                if (SetProperty(ref _matches, value))
                {
                    CheckGameEnd();
                }
            }
        }

        public bool IsGameActive => CurrentGame != null && !CurrentGame.IsGameOver;
        public TimeSpan RemainingTime => IsGameActive ? TimeLimit - CurrentGame.ElapsedTime : TimeSpan.Zero;
        public bool CanStartGame => !string.IsNullOrEmpty(SelectedCategory) && BoardWidth * BoardHeight % 2 == 0;
        public bool CanSaveGame => IsGameActive;
        public bool CanLoadGame => true;

        public ICommand NewGameCommand { get; private set; }
        public ICommand SaveGameCommand { get; private set; }
        public ICommand LoadGameCommand { get; private set; }
        public ICommand CardClickCommand { get; private set; }
        public ICommand PauseGameCommand { get; private set; }
        public ICommand ResumeGameCommand { get; private set; }
        public ICommand SelectCategoryCommand { get; private set; }
        public ICommand SetBoardSizeCommand { get; private set; }
        public ICommand ShowStatisticsCommand { get; private set; }
        public ICommand ShowAboutCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        private DispatcherTimer _gameTimer;
        private DateTime _gameStartTime;

        public GameViewModel()
        {
            _statisticsService = new StatisticsService();
            _gameSaveService = new GameSaveService();

            InitializeDefaultValues();
            InitializeCommands();
            // Don't call ExecuteNewGame() here
        }

        private void InitializeDefaultValues()
        {
            // Initialize properties
            _boardWidth = 4;
            _boardHeight = 4;
            _timeLimit = TimeSpan.FromMinutes(5);
            _moves = 0;
            _matches = 0;
            _firstFlippedCard = null;
            _secondFlippedCard = null;
            _selectedCategory = "Jordan 1"; // Changed from Animals to Jordan 1

            // Initialize categories
            Categories = new ObservableCollection<string>
            {
                "Jordan 1",  // Changed from Animals to Jordan 1
                "Fruits",
                "Vehicles",
                "Sports"
            };

            // Don't create a game immediately
            CurrentGame = null;
        }

        private void InitializeCommands()
        {
            NewGameCommand = new RelayCommand(
                execute: (param) => ExecuteNewGame(),
                canExecute: (param) => CanStartGame
            );

            SaveGameCommand = new RelayCommand(
                execute: (param) => ExecuteSaveGame(),
                canExecute: (param) => CanSaveGame
            );

            LoadGameCommand = new RelayCommand(
                execute: (param) => ExecuteLoadGame(),
                canExecute: (param) => CanLoadGame
            );

            CardClickCommand = new RelayCommand<Card>(
                execute: (card) => ExecuteCardClick(card),
                canExecute: (card) => IsGameActive && card != null && !card.IsMatched
            );

            PauseGameCommand = new RelayCommand(
                execute: (param) => ExecutePauseGame(),
                canExecute: (param) => IsGameActive
            );

            ResumeGameCommand = new RelayCommand(
                execute: (param) => ExecuteResumeGame(),
                canExecute: (param) => !IsGameActive && CurrentGame != null
            );

            SelectCategoryCommand = new RelayCommand<string>(
                execute: (category) => ExecuteSelectCategory(category),
                canExecute: (category) => !string.IsNullOrEmpty(category)
            );

            SetBoardSizeCommand = new RelayCommand(
                execute: (param) => ExecuteSetBoardSize(),
                canExecute: (param) => !IsGameActive || CurrentGame == null
            );

            ShowStatisticsCommand = new RelayCommand(
                execute: (param) => ExecuteShowStatistics(),
                canExecute: (param) => true
            );

            ShowAboutCommand = new RelayCommand(
                execute: (param) => ExecuteShowAbout(),
                canExecute: (param) => true
            );

            ExitCommand = new RelayCommand(
                execute: (param) => ExecuteExit(),
                canExecute: (param) => true
            );
        }

        private void ExecuteSelectCategory(string category)
        {
            try
            {
                if (CurrentGame != null && !CurrentGame.IsGameOver)
                {
                    var result = MessageBox.Show(
                        "Starting a new category will end the current game. Continue?",
                        "Confirm Category Change",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                        return;
                }

                SelectedCategory = category;
                ExecuteNewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSetBoardSize()
        {
            try
            {
                // Check if there's an active game
                if (CurrentGame != null && !CurrentGame.IsGameOver)
                {
                    var result = MessageBox.Show(
                        "Changing the board size will start a new game. Do you want to continue?",
                        "Confirm Board Size Change",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                        return;
                }

                var dialog = new BoardSizeDialog(BoardWidth, BoardHeight);
                if (Application.Current.MainWindow != null)
                {
                    dialog.Owner = Application.Current.MainWindow;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }

                if (dialog.ShowDialog() == true)
                {
                    // Validate the new board size
                    if (dialog.BoardWidth * dialog.BoardHeight % 2 != 0)
                    {
                        MessageBox.Show(
                            "The board must have an even number of cards for matching pairs.",
                            "Invalid Board Size",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // Update board dimensions
                    BoardWidth = dialog.BoardWidth;
                    BoardHeight = dialog.BoardHeight;

                    // Stop the current game timer if it's running
                    if (_gameTimer != null && _gameTimer.IsEnabled)
                    {
                        _gameTimer.Stop();
                    }

                    // Create new game with the new board size
                    CurrentGame = new Game(BoardWidth, BoardHeight, new ObservableCollection<Card>(), TimeLimit, SelectedCategory);

                    // Reset game state
                    _firstFlippedCard = null;
                    _secondFlippedCard = null;
                    Moves = 0;
                    Matches = 0;

                    // Initialize cards for the new game
                    InitializeCards();

                    // Start the game timer
                    StartGameTimer();

                    // Notify UI of changes
                    OnPropertyChanged(nameof(IsGameActive));
                    OnPropertyChanged(nameof(RemainingTime));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ExecuteSetBoardSize: {ex}");
                MessageBox.Show(
                    $"Error setting board size: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExecuteShowStatistics()
        {
            try
            {
                var statisticsWindow = new Window
                {
                    Title = "Statistics",
                    Content = new StatisticsView(),
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Application.Current.MainWindow,
                    ResizeMode = ResizeMode.NoResize
                };

                statisticsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteShowAbout()
        {
            MessageBox.Show(
                "Memory Game\n\n" +
                "Created by: Hermeneanu Ionut Silviu\n" +
                "Email: her.slviu.i@gmail.com\n" +
                "Group: LF232\n" +
                "Specialization: Informatica",
                "About",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ExecuteExit()
        {
            try
            {
                bool canExit = true;

                if (CurrentGame != null && !CurrentGame.IsGameOver)
                {
                    var result = MessageBox.Show(
                        "Do you want to save the current game before exiting?",
                        "Exit Game",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            ExecuteSaveGame();
                            break;
                        case MessageBoxResult.Cancel:
                            canExit = false;
                            break;
                            // For No, just continue with exit
                    }
                }

                if (canExit)
                {
                    // Stop the game timer if it's running
                    if (_gameTimer != null && _gameTimer.IsEnabled)
                    {
                        _gameTimer.Stop();
                    }

                    // Clear game state
                    CurrentGame = null;
                    _firstFlippedCard = null;
                    _secondFlippedCard = null;
                    Moves = 0;
                    Matches = 0;

                    // Navigate back to login view
                    NavigationService.NavigateTo("LoginView");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exiting game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteNewGame()
        {
            try
            {
                // Stop existing timer if running
                if (_gameTimer != null && _gameTimer.IsEnabled)
                {
                    _gameTimer.Stop();
                }

                // Reset game state
                _firstFlippedCard = null;
                _secondFlippedCard = null;
                Moves = 0;
                Matches = 0;

                // Create new game
                CurrentGame = new Game(BoardWidth, BoardHeight, new ObservableCollection<Card>(), TimeLimit, SelectedCategory);
                CurrentGame.IsGameOver = false;

                // Initialize cards but don't start timer yet
                InitializeCards();

                // Notify UI of changes
                OnPropertyChanged(nameof(IsGameActive));
                OnPropertyChanged(nameof(RemainingTime));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ExecuteNewGame: {ex}");
                MessageBox.Show($"Error starting new game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSaveGame()
        {
            try
            {
                if (CurrentGame == null || CurrentGame.IsGameOver)
                {
                    MessageBox.Show("No active game to save.", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (CurrentPlayer == null)
                {
                    MessageBox.Show("Please select a player before saving the game.", "Save Game", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Pause the timer while saving
                bool wasTimerRunning = false;
                if (_gameTimer != null && _gameTimer.IsEnabled)
                {
                    wasTimerRunning = true;
                    _gameTimer.Stop();
                }

                var gameState = new GameState
                {
                    Username = CurrentPlayer.Username,
                    Category = SelectedCategory,
                    BoardWidth = BoardWidth,
                    BoardHeight = BoardHeight,
                    TimeLimit = TimeLimit,
                    ElapsedTime = CurrentGame.ElapsedTime,
                    RemainingTime = RemainingTime,
                    IsTimerPaused = !wasTimerRunning,
                    Moves = Moves,
                    Matches = Matches,
                    Cards = CurrentGame.Cards.Select(c => new CardState
                    {
                        ImagePath = c.ImagePath,
                        IsFlipped = c.IsFlipped,
                        IsMatched = c.IsMatched,
                        Position = c.Position,
                        CardId = c.CardId
                    }).ToList()
                };

                var savedFilePath = _gameSaveService.SaveGame(gameState);

                // Resume timer if it was running
                if (wasTimerRunning)
                {
                    _gameTimer.Start();
                }

                MessageBox.Show($"Game saved successfully!\nFile: {savedFilePath}", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Save game error: {ex}");
            }
        }

        private void ExecuteLoadGame()
        {
            try
            {
                if (CurrentPlayer == null)
                {
                    MessageBox.Show("Please select a player first.", "Load Game", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var savedGames = _gameSaveService.GetSavedGames(CurrentPlayer.Username);
                if (!savedGames.Any())
                {
                    MessageBox.Show("No saved games found.", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var dialog = new OpenFileDialog
                {
                    Filter = "Game files (*.json)|*.json|All files (*.*)|*.*",
                    InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedGames"),
                    Title = "Select a saved game to load"
                };

                if (dialog.ShowDialog() == true)
                {
                    var gameState = _gameSaveService.LoadGame(dialog.FileName);

                    // Verify the game belongs to the current player
                    if (gameState.Username != CurrentPlayer.Username)
                    {
                        MessageBox.Show("You can only load your own saved games.", "Load Game", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Stop current game timer if running
                    if (_gameTimer != null && _gameTimer.IsEnabled)
                    {
                        _gameTimer.Stop();
                    }

                    // Restore game state
                    SelectedCategory = gameState.Category;
                    BoardWidth = gameState.BoardWidth;
                    BoardHeight = gameState.BoardHeight;
                    TimeLimit = gameState.TimeLimit;
                    Moves = gameState.Moves;
                    Matches = gameState.Matches;

                    // Create new game with restored cards
                    CurrentGame = new Game(BoardWidth, BoardHeight,
                        new ObservableCollection<Card>(
                            gameState.Cards.Select(c => new Card(
                                c.ImagePath, c.IsFlipped, c.IsMatched, c.Position, c.CardId))),
                        TimeLimit, SelectedCategory)
                    {
                        ElapsedTime = gameState.ElapsedTime
                    };

                    // Restore timer state
                    _gameStartTime = DateTime.Now - gameState.ElapsedTime;
                    StartGameTimer();

                    // If the game was paused when saved, pause it now
                    if (gameState.IsTimerPaused)
                    {
                        _gameTimer.Stop();
                    }

                    MessageBox.Show("Game loaded successfully!", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCardClick(Card card)
        {
            if (card == null || !IsGameActive || card.IsMatched)
                return;

            // Start timer on first card flip if it's not running
            if (_gameTimer == null || !_gameTimer.IsEnabled)
            {
                StartGameTimer();
            }

            // Rest of the card click logic
            if (_firstFlippedCard == null)
            {
                _firstFlippedCard = card;
                card.IsFlipped = true;
            }
            else if (_secondFlippedCard == null && card != _firstFlippedCard)
            {
                _secondFlippedCard = card;
                card.IsFlipped = true;
                Moves++;

                // Check for match
                if (_firstFlippedCard.ImagePath == _secondFlippedCard.ImagePath)
                {
                    _firstFlippedCard.IsMatched = true;
                    _secondFlippedCard.IsMatched = true;
                    Matches++;
                    _firstFlippedCard = null;
                    _secondFlippedCard = null;
                }
                else
                {
                    // Hide cards after delay
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                    timer.Tick += (s, e) =>
                    {
                        _firstFlippedCard.IsFlipped = false;
                        _secondFlippedCard.IsFlipped = false;
                        _firstFlippedCard = null;
                        _secondFlippedCard = null;
                        timer.Stop();
                    };
                    timer.Start();
                }
            }
        }

        private void ExecutePauseGame()
        {
            if (_gameTimer != null && _gameTimer.IsEnabled)
            {
                _gameTimer.Stop();
            }
        }

        private void ExecuteResumeGame()
        {
            if (_gameTimer != null && !_gameTimer.IsEnabled)
            {
                _gameTimer.Start();
            }
        }

        private void InitializeCards()
        {
            try
            {
                if (CurrentGame == null)
                {
                    CurrentGame = new Game(BoardWidth, BoardHeight, new ObservableCollection<Card>(), TimeLimit, SelectedCategory);
                }

                // Clear existing cards
                CurrentGame.Cards.Clear();

                // Get the base path for images
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.FullName;
                string basePath = Path.Combine(projectRoot, "Images", SelectedCategory);

                Debug.WriteLine($"Loading images from: {basePath}");

                if (!Directory.Exists(basePath))
                {
                    MessageBox.Show($"Category folder not found: {basePath}\nPlease create the folder and add images.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get all image files in the category directory
                var imageFiles = Directory.GetFiles(basePath, "*.png");
                Debug.WriteLine($"Found {imageFiles.Length} images");

                // Calculate required pairs based on board size
                int requiredPairs = (BoardWidth * BoardHeight) / 2;
                Debug.WriteLine($"Required pairs for {BoardWidth}x{BoardHeight} board: {requiredPairs}");

                if (imageFiles.Length < requiredPairs)
                {
                    MessageBox.Show($"Not enough images in the {SelectedCategory} category.\nFound {imageFiles.Length} images, need at least {requiredPairs}.\nPlease add more images to: {basePath}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create pairs of cards with images
                var cards = new List<Card>();
                int position = 0;
                for (int i = 0; i < requiredPairs; i++)
                {
                    string imagePath = imageFiles[i];
                    try
                    {
                        // Create a safe URI for the image
                        string safeImagePath = "file:///" + imagePath.Replace("\\", "/");
                        Debug.WriteLine($"Creating card pair with image: {safeImagePath}");

                        // Create two cards with the same image
                        var card1 = new Card(safeImagePath, false, false, position, i);
                        var card2 = new Card(safeImagePath, false, false, position + 1, i);
                        Debug.WriteLine($"Created pair - Card1(Pos:{position}, Id:{i}), Card2(Pos:{position + 1}, Id:{i})");

                        cards.Add(card1);
                        cards.Add(card2);
                        position += 2;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image {imagePath}: {ex.Message}",
                            "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                }

                if (cards.Count < BoardWidth * BoardHeight)
                {
                    MessageBox.Show($"Could not create enough valid card pairs. Found {cards.Count / 2} valid pairs, need {requiredPairs}.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Shuffle the cards
                var random = new Random();
                var shuffledCards = cards.OrderBy(c => random.Next()).ToList();

                // Update positions after shuffling
                for (int i = 0; i < shuffledCards.Count; i++)
                {
                    shuffledCards[i].Position = i;
                    Debug.WriteLine($"Shuffled card at position {i}: Image={shuffledCards[i].ImagePath}, Id={shuffledCards[i].CardId}");
                }

                // Add cards to the game
                foreach (var card in shuffledCards)
                {
                    CurrentGame.Cards.Add(card);
                }

                // Force UI update
                OnPropertyChanged(nameof(CurrentGame));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing cards: {ex.Message}\nStack trace: {ex.StackTrace}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartGameTimer()
        {
            _gameStartTime = DateTime.Now;
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += (s, e) =>
            {
                if (CurrentGame != null)
                {
                    CurrentGame.ElapsedTime = DateTime.Now - _gameStartTime;
                    OnPropertyChanged(nameof(RemainingTime));

                    if (RemainingTime <= TimeSpan.Zero)
                    {
                        _gameTimer.Stop();
                        CurrentGame.IsGameOver = true;
                        CurrentGame.IsWon = false;

                        // Update statistics for time-out loss
                        if (CurrentPlayer != null)
                        {
                            _statisticsService.UpdatePlayerStats(
                                CurrentPlayer.Username,
                                false,
                                CurrentGame.ElapsedTime,
                                Moves
                            );
                        }

                        MessageBox.Show("Time's up! Game Over!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                        OnPropertyChanged(nameof(IsGameActive));
                    }
                }
            };
            _gameTimer.Start();
        }

        private void CheckGameEnd()
        {
            if (Matches == 8) // All pairs matched
            {
                if (_gameTimer != null && _gameTimer.IsEnabled)
                {
                    _gameTimer.Stop();
                }

                CurrentGame.IsGameOver = true;
                CurrentGame.IsWon = true;

                // Save statistics
                if (CurrentPlayer != null)
                {
                    _statisticsService.UpdatePlayerStats(
                        CurrentPlayer.Username,
                        true,
                        CurrentGame.ElapsedTime,
                        Moves
                    );
                }

                MessageBox.Show(
                    $"Congratulations! You won!\nMoves: {Moves}\nTime: {CurrentGame.ElapsedTime:mm\\:ss}",
                    "Game Over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
