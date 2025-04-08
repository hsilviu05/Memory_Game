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
            var aboutWindow = new AboutView
            {
                Owner = Application.Current.MainWindow
            };
            aboutWindow.ShowDialog();
        }
    }
}