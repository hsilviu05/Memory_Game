using System;
using System.Windows.Input;
using Memory_Game.Common;

namespace Memory_Game.ViewModel
{
    public class VictoryViewModel : ViewModelBase
    {
        private int _moves;
        public int Moves
        {
            get => _moves;
            set => SetProperty(ref _moves, value);
        }

        private TimeSpan _time;
        public TimeSpan Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        public string TimeDisplay => $"{Time:mm\\:ss}";

        public ICommand PlayAgainCommand { get; }
        public ICommand MainMenuCommand { get; }

        public VictoryViewModel()
        {
            PlayAgainCommand = new RelayCommand(ExecutePlayAgain);
            MainMenuCommand = new RelayCommand(ExecuteMainMenu);
        }

        private void ExecutePlayAgain(object parameter)
        {
            NavigationService.NavigateTo("GameView");
        }

        private void ExecuteMainMenu(object parameter)
        {
            NavigationService.NavigateTo("StartView");
        }
    }
}