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
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading : Window
    {

        // The value of the loading bar
        public double LoadingValue { get { return loadingBar.Value; } set { loadingBar.Value=value; } }
        public string LoadingMessage { get { return (string)loadingLabel.Content; } set { loadingLabel.Content = value; } }
        public string StartMessage { get; set; }

        public Loading()
        {
            InitializeComponent();
        }

        // Shows the loading screen over the given window with the given properties
        public void Show(Window owner, string loadingMessage, string loadingTitle, double maxLoadValue)
        {
            Left = owner.Left + (owner.Width - Width) / 2;
            Top = owner.Top + (owner.Height - Height) / 2;
            loadingLabel.Content = loadingMessage;
            StartMessage = loadingMessage;
            Title = loadingTitle;
            loadingBar.Maximum = maxLoadValue;
            base.Show();
        }
    }
}
