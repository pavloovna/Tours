using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Tours.Forms
{
    public partial class Authorization : Form
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection("Data Source = LAPTOP-667E4RFT; Initial Catalog = DB_TOURS; Integrated Security = True; TrustServerCertificate = True");
            sqlConnection.Open();
            var cmd = new SqlCommand("SELECT FIO, Login, Password, TypeUserId FROM Users ", sqlConnection);
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (rdr[1].ToString() == txtLogin.Text && rdr[2].ToString() == txtPassword.Text)
                {
                    MessageBox.Show("Вход успешен!", "Успех!", MessageBoxButtons.OK);
                    Hide();
                    new MainForm() { Tag = rdr[3].ToString() + '/' + rdr[0] }.ShowDialog();
                    Show();
                }
            }
            sqlConnection.Close();
        }

        private void lblThen_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Hide();
            new MainForm() { Tag = "/" }.ShowDialog();
            Show();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = false;
            pictureBoxOpen.Visible = true;
            pictureBoxClose.Visible = false;
        }

        private void pictureBoxOpen_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
            pictureBoxOpen.Visible = false;
            pictureBoxClose.Visible = true;
        }
    }
}
