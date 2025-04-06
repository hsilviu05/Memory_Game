using System;

namespace Memory_Game.Common
{
    public class NavigationEventArgs : EventArgs
    {
        public string ViewName { get; }
        public object Parameter { get; }

        public NavigationEventArgs(string viewName, object parameter = null)
        {
            ViewName = viewName;
            Parameter = parameter;
        }
    }

    public static class NavigationService
    {
        public static event EventHandler<NavigationEventArgs> NavigationRequested;

        public static void NavigateTo(string viewName, object parameter = null)
        {
            NavigationRequested?.Invoke(null, new NavigationEventArgs(viewName, parameter));
        }
    }
}