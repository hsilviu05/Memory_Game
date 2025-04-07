using System.Windows;
using System.Windows.Controls;

namespace Memory_Game.View
{
    /// <summary>
    /// Interaction logic for BoardSizeDialog.xaml
    /// </summary>
    public partial class BoardSizeDialog : Window
    {
        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }

        public BoardSizeDialog(int currentWidth = 4, int currentHeight = 4)
        {
            InitializeComponent();
            DataContext = this;

            // Set initial values after window is loaded
            Loaded += (s, e) =>
            {
                if (WidthSlider != null)
                    WidthSlider.Value = currentWidth;
                if (HeightSlider != null)
                    HeightSlider.Value = currentHeight;
            };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            BoardWidth = (int)(WidthSlider?.Value ?? 4);
            BoardHeight = (int)(HeightSlider?.Value ?? 4);

            if (BoardWidth * BoardHeight % 2 != 0)
            {
                MessageBox.Show(
                    "The board must have an even number of cards for matching pairs.",
                    "Invalid Board Size",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}