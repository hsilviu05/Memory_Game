using System.Windows;
using System.Windows.Controls;
using Memory_Game.View;
using Memory_Game.ViewModel;
using Memory_Game.Common;
using System.Linq;
using System.Reflection;

namespace Memory_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameViewModel _gameViewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new StartViewModel();
            MainContent.Content = new StartView();
            Common.NavigationService.NavigationRequested += NavigationService_NavigationRequested;
        }

        private void NavigationService_NavigationRequested(object sender, NavigationEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (e.ViewName)
                {
                    case "Exit":
                        Close();
                        break;
                    case "LoginView":
                        MainContent.Content = new LoginView();
                        break;
                    case "GameView":
                        var gameView = new GameView();
                        _gameViewModel = (GameViewModel)gameView.DataContext;
                        if (e.Parameter != null && e.Parameter is Memory_Game.Model.User user)
                        {
                            _gameViewModel.CurrentPlayer = user;
                        }
                        MainContent.Content = gameView;
                        break;
                    case "StartView":
                        MainContent.Content = new StartView();
                        break;
                    case "VictoryView":
                        var victoryView = new VictoryView();
                        var victoryViewModel = new VictoryViewModel();

                        // Check if parameters were passed and update the ViewModel
                        if (e.Parameter != null)
                        {
                            var properties = e.Parameter.GetType().GetProperties();
                            var movesProperty = properties.FirstOrDefault(p => p.Name == "Moves");
                            var timeProperty = properties.FirstOrDefault(p => p.Name == "Time");

                            if (movesProperty != null)
                                victoryViewModel.Moves = (int)movesProperty.GetValue(e.Parameter);
                            if (timeProperty != null)
                                victoryViewModel.Time = (TimeSpan)timeProperty.GetValue(e.Parameter);
                        }

                        victoryView.DataContext = victoryViewModel;
                        MainContent.Content = victoryView;
                        break;
                }
            });
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_gameViewModel != null && _gameViewModel.IsGameActive)
            {
                var result = MessageBox.Show(
                    "Do you want to save the current game before exiting?",
                    "Exit Game",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        _gameViewModel.SaveGameCommand.Execute(null);
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }

            base.OnClosing(e);
            // Unsubscribe from events
            Common.NavigationService.NavigationRequested -= NavigationService_NavigationRequested;
            Application.Current.Shutdown();
        }
    }
}