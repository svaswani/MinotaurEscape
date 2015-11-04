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
using static ExternalTool.MazeGeneration;

namespace ExternalTool
{
    /// <summary>
    /// Interaction logic for GenerateMaze.xaml
    /// </summary>
    public partial class GenerateMaze : Window
    {
        // All the textboxes and comboxes in this window
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
        public Algorithm algorithm
        {
            get
            {
                return Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>().ToArray()[algorithmComboBox.SelectedIndex];
            }
        }

        // The name's of each algorithm
        public static string[] AlgorithmNames = Enum.GetNames(typeof(Algorithm)).Select(name => name.Replace("____", "/").Replace("___", "-").Replace("__", "\'s ").Replace('_', ' ')).ToArray();

        public GenerateMaze()
        {
            InitializeComponent();
        }

        private void generate_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        // Show this window over the center of the given window
        public bool? ShowDialog(Window owner)
        {
            Left = owner.Left+(owner.Width-Width)/2;
            Top = owner.Top+(owner.Height-Height)/2;
            return base.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            algorithmComboBox.ItemsSource = AlgorithmNames;
        }
    }
}
