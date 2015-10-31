using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinotaurEscape
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?","Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnEditor_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will open a new program!","Do you want to open this external program?",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Open the external tool
            }
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            //start the game as a whole
        }
    }
}
