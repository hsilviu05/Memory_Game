using System;
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

        public BoardSizeDialog(int currentWidth, int currentHeight)
        {
            InitializeComponent();
            BoardWidth = currentWidth;
            BoardHeight = currentHeight;

            // Set initial values
            WidthTextBox.Text = currentWidth.ToString();
            HeightTextBox.Text = currentHeight.ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateDimensions())
            {
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateDimensions()
        {
            if (int.TryParse(WidthTextBox.Text, out int width) &&
                int.TryParse(HeightTextBox.Text, out int height))
            {
                if (width >= 2 && width <= 6 && height >= 2 && height <= 6)
                {
                    BoardWidth = width;
                    BoardHeight = height;
                    return true;
                }
            }

            MessageBox.Show("Please enter valid dimensions (2-6 for both width and height).",
                          "Invalid Dimensions",
                          MessageBoxButton.OK,
                          MessageBoxImage.Warning);
            return false;
        }
    }
}