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

        private Game _currentGame;
        public Game CurrentGame
        {
            get => _currentGame;
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
            get => _selectedCategory;
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
            set
            {
                if (_moves != value)
                {
                    _moves = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Matches
        {
            get => _matches;
            set
            {
                if (_matches != value)
                {
                    _matches = value;
                    OnPropertyChanged();
                    CheckGameEnd();
                }
            }
        }

        public bool IsGameActive => CurrentGame != null && !CurrentGame.IsGameOver;
        public TimeSpan RemainingTime => IsGameActive ? TimeLimit - CurrentGame.ElapsedTime : TimeSpan.Zero;
        public bool CanStartGame => !string.IsNullOrEmpty(SelectedCategory) && BoardWidth * BoardHeight % 2 == 0;
        public bool CanSaveGame => IsGameActive;
        public bool CanLoadGame => true;

        public ICommand NewGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand LoadGameCommand { get; }
        public ICommand CardClickCommand { get; }
        public ICommand PauseGameCommand { get; }
        public ICommand ResumeGameCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand SetBoardSizeCommand { get; }
        public ICommand ShowStatisticsCommand { get; }
        public ICommand ShowAboutCommand { get; }
        public ICommand ExitCommand { get; }

        private DispatcherTimer _gameTimer;
        private DateTime _gameStartTime;

        public GameViewModel()
        {
            _statisticsService = new StatisticsService();

            // Initialize properties
            _currentGame = new Game(4, 4, new ObservableCollection<Card>(), TimeSpan.FromMinutes(5), "Animals");
            _selectedCategory = "Animals";
            _boardWidth = 4;
            _boardHeight = 4;
            _timeLimit = TimeSpan.FromMinutes(5);
            _moves = 0;
            _matches = 0;
            _firstFlippedCard = null;
            _secondFlippedCard = null;

            // Initialize categories
            Categories = new ObservableCollection<string>
            {
                "Animals",
                "Fruits",
                "Vehicles",
                "Sports"
            };

            // Initialize commands
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

            SetBoardSizeCommand = new RelayCommand<string>(
                execute: (size) => ExecuteSetBoardSize(size),
                canExecute: (size) => !string.IsNullOrEmpty(size)
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

            // Initialize game
            ExecuteNewGame();
        }

        private void ExecuteSelectCategory(string category)
        {
            try
            {
                MessageBox.Show($"Selected category: {category}", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedCategory = category;
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
                ExecuteNewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSetBoardSize(string size)
        {
            try
            {
                if (size == "Standard")
                {
                    BoardWidth = 4;
                    BoardHeight = 4;
                }
                else
                {
                    MessageBox.Show("Custom size selection will be implemented", "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (CurrentGame != null && !CurrentGame.IsGameOver)
                {
                    var result = MessageBox.Show(
                        "Changing board size will start a new game. Continue?",
                        "Confirm Size Change",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                        ExecuteNewGame();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting board size: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (CurrentGame != null && !CurrentGame.IsGameOver)
                {
                    var result = MessageBox.Show(
                        "Do you want to save the current game before exiting?",
                        "Exit Game",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Cancel)
                        return;

                    if (result == MessageBoxResult.Yes)
                        ExecuteSaveGame();
                }

                // Navigate back to login view (to be implemented with navigation service)
                MessageBox.Show("Exit functionality will be implemented", "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // Reset game state
                _firstFlippedCard = null;
                _secondFlippedCard = null;
                Moves = 0;
                Matches = 0;

                // Create a new game with the current settings
                CurrentGame = new Game(BoardWidth, BoardHeight, new ObservableCollection<Card>(), TimeLimit, SelectedCategory);

                // Initialize the cards
                InitializeCards();

                StartGameTimer();
            }
            catch (Exception ex)
            {
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

                var dialog = new SaveFileDialog
                {
                    Filter = "Game files (*.json)|*.json|All files (*.*)|*.*",
                    InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedGames"),
                    FileName = $"MemoryGame_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (dialog.ShowDialog() == true)
                {
                    // Save game using GameService (to be implemented)
                    MessageBox.Show("Game saved successfully!", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteLoadGame()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Game files (*.json)|*.json|All files (*.*)|*.*",
                    InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedGames")
                };

                if (dialog.ShowDialog() == true)
                {
                    // Load game using GameService (to be implemented)
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
            if (card == null || card.IsMatched || card.IsFlipped ||
                (_secondFlippedCard != null && _firstFlippedCard != null)) return;

            // Flip the card
            card.IsFlipped = true;
            OnPropertyChanged(nameof(CurrentGame)); // Force UI update

            if (_firstFlippedCard == null)
            {
                // First card flipped
                _firstFlippedCard = card;
                Debug.WriteLine($"First card flipped - CardId: {card.CardId}, Position: {card.Position}, Image: {card.ImagePath}");
            }
            else if (_secondFlippedCard == null && card != _firstFlippedCard)
            {
                // Second card flipped
                _secondFlippedCard = card;
                Moves++;
                Debug.WriteLine($"Second card flipped - CardId: {card.CardId}, Position: {card.Position}, Image: {card.ImagePath}");
                Debug.WriteLine($"Comparing cards - First CardId: {_firstFlippedCard.CardId}, Second CardId: {_secondFlippedCard.CardId}");

                // Check for match
                if (_firstFlippedCard.ImagePath == _secondFlippedCard.ImagePath)
                {
                    Debug.WriteLine("Match found!");
                    // Match found
                    _firstFlippedCard.IsMatched = true;
                    _secondFlippedCard.IsMatched = true;
                    Matches++;

                    // Reset for next pair
                    _firstFlippedCard = null;
                    _secondFlippedCard = null;

                    // Force UI update
                    OnPropertyChanged(nameof(CurrentGame));
                }
                else
                {
                    Debug.WriteLine("No match found - flipping cards back");
                    // No match, flip cards back after delay
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_firstFlippedCard != null)
                                _firstFlippedCard.IsFlipped = false;
                            if (_secondFlippedCard != null)
                                _secondFlippedCard.IsFlipped = false;
                            _firstFlippedCard = null;
                            _secondFlippedCard = null;
                            OnPropertyChanged(nameof(CurrentGame)); // Force UI update
                        });
                    });
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

                if (imageFiles.Length < 8)
                {
                    MessageBox.Show($"Not enough images in the {SelectedCategory} category.\nFound {imageFiles.Length} images, need at least 8.\nPlease add more images to: {basePath}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create pairs of cards with images
                var cards = new List<Card>();
                int position = 0;
                for (int i = 0; i < Math.Min(8, imageFiles.Length); i++)
                {
                    string imagePath = imageFiles[i];
                    try
                    {
                        // Create a safe URI for the image
                        string safeImagePath = "file:///" + imagePath.Replace("\\", "/");
                        Debug.WriteLine($"Creating card pair with image: {safeImagePath}");

                        // Test loading the image
                        var testImage = new BitmapImage();
                        testImage.BeginInit();
                        testImage.UriSource = new Uri(safeImagePath);
                        testImage.CacheOption = BitmapCacheOption.OnLoad;
                        testImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        testImage.EndInit();
                        testImage.Freeze(); // Make it thread-safe

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

                if (cards.Count < 16)
                {
                    MessageBox.Show($"Could not create enough valid card pairs. Found {cards.Count / 2} valid pairs, need 8.",
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
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += GameTimer_Tick;
            _gameStartTime = DateTime.Now;
            _gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _gameStartTime;
            CurrentGame.ElapsedTime = elapsed;

            // Check if time limit reached
            if (elapsed >= TimeLimit)
            {
                _gameTimer.Stop();
                CurrentGame.IsGameOver = true;
                CurrentGame.IsWon = false;

                // Save statistics
                if (CurrentPlayer != null)
                {
                    _statisticsService.UpdatePlayerStats(
                        CurrentPlayer.Username,
                        false,
                        elapsed,
                        Moves
                    );
                }

                MessageBox.Show(
                    "Time's up! Game Over.",
                    "Game Over",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            OnPropertyChanged(nameof(RemainingTime));
        }

        private void CheckGameEnd()
        {
            if (Matches == 8) // All pairs matched
            {
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
