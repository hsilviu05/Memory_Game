using System.Windows;
using System.Windows.Controls;
using Memory_Game.View;

namespace Memory_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                        MainContent.Content = new GameView();
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