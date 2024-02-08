using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tours.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnTours_Click(object sender, EventArgs e)
        {
            Hide();
            new ListOfTours() { Tag = ((string)Tag).Split('/')[1] }.ShowDialog();
            Show();
        }

        private void btnHotels_Click(object sender, EventArgs e)
        {
            Hide();
            new Hotels().ShowDialog();
            Show();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            Hide();
            new Orders().ShowDialog();
            Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            btnOrders.Visible = new string[] { "3", "2" }.Contains(((string)Tag).Split('/')[0]);
        }
    }
}
