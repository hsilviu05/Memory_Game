using System.Windows;

namespace Memory_Game.View
{
    public partial class CategoryView : Window
    {
        public CategoryView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}