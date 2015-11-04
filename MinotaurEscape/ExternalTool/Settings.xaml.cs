using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExternalTool
{
    /// <summary>
    /// Interaction logic for GenerateMaze.xaml
    /// </summary>
    public partial class Settings : Window
    {
        // All the textboxes in this window
        public int MazeWidth{ get {
                int width;
                int.TryParse(widthTextBox.Text, out width);
                return width*2+1;
        } }
        public int MazeHeight { get {
                int height;
                int.TryParse(heightTextBox.Text, out height);
                return height*2+1;
        } }
        public int TileSize
        {
            get
            {
                int tileSize;
                int.TryParse(sizeTextBox.Text, out tileSize);
                return tileSize;
            }
        }

        public Settings()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        // Show this window over the center of the given window with the given properties
        public bool? ShowDialog(Window owner, int width, int height, int tileSize)
        {
            Left = owner.Left + (owner.Width - Width) / 2;
            Top = owner.Top + (owner.Height - Height) / 2;
            widthTextBox.Text = (width/2).ToString();
            heightTextBox.Text = (height/2).ToString();
            sizeTextBox.Text = tileSize.ToString();
            return base.ShowDialog();
        }
    }
}
