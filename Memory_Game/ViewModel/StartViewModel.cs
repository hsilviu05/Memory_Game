using System.Windows.Input;
using Memory_Game.View;
using System.Windows;
using Memory_Game.Common;

namespace Memory_Game.ViewModel
{
    public class StartViewModel
    {
        public ICommand StartGameCommand { get; }
        public ICommand ShowAboutCommand { get; }

        public StartViewModel()
        {
            StartGameCommand = new RelayCommand(ExecuteStartGame);
            ShowAboutCommand = new RelayCommand(ExecuteShowAbout);
        }

        private void ExecuteStartGame(object parameter)
        {
            Common.NavigationService.NavigateTo("LoginView");
        }

        private void ExecuteShowAbout(object parameter)
        {
            MessageBox.Show(
                "Memory Game\n\n" +
                "A classic memory matching game where you need to find pairs of matching cards.\n\n" +
                "Features:\n" +
                "- Multiple categories\n" +
                "- Customizable board size\n" +
                "- Player statistics\n" +
                "- Save and load games\n\n" +
                "Created by: Your Name",
                "About Memory Game",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}