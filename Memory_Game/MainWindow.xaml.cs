using System.Windows;
using System.Windows.Controls;
using Memory_Game.View;
using Memory_Game.ViewModel;

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
            Common.NavigationService.NavigationRequested += NavigationService_NavigationRequested;

            // Start with the login view
            MainContent.Content = new LoginView();
        }

        private void NavigationService_NavigationRequested(object sender, Common.NavigationEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (e.ViewName)
                {
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
                        // Add other views as needed
                }
            });
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            // Unsubscribe from events
            Common.NavigationService.NavigationRequested -= NavigationService_NavigationRequested;
        }
    }
}