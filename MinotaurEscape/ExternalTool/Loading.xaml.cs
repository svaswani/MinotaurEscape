﻿using System;
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

        public Loading()
        {
            InitializeComponent();
        }

        // Shows the loading screen over the given window with the given properties
        public void Show(Window owner,string loadingMessage, string loadingTitle)
        {
            Left = owner.Left + (owner.Width - Width) / 2;
            Top = owner.Top + (owner.Height - Height) / 2;
            loadingLabel.Content = loadingMessage;
            Title = loadingTitle;
            base.Show();
        }
    }
}