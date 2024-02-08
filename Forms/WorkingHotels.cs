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
using System.Xml.Linq;
using Tours.DB_TOURSDataSetTableAdapters;

namespace Tours.Forms
{   
    public partial class WorkingHotels : Form
    {
        string cmdText;
        string Country = string.Empty;
        string Name = string.Empty;
        public WorkingHotels()
        {
            InitializeComponent();
        }
        private void WorkingHotels_Load(object sender, EventArgs e)
        {
            this.countryTableAdapter.Fill(this.dB_TOURSDataSet1.Country);         
            this.countryTableAdapter.Fill(this.dB_TOURSDataSet1.Country);
            if (Country != string.Empty)
                cmbCountry.SelectedValue = Country;
        }
        static public void Redacting(string id, string name, int countOfStars, string country)
        {
            var x = new WorkingHotels();
            x.txtId.Text = id;
            x.txtName.Text = name;
            x.Name = name;
            x.numStars.Value = countOfStars;
            x.Country = country;
            x.cmdText = "UPDATE Hotels SET NameHotel = @a, CountOfStars= @b, CountryName = @c WHERE IdHotel = " + x.txtId.Text;
            x.ShowDialog();
        }
        static public void New()
        {
            var x = new WorkingHotels();
            x.cmdText = "INSERT INTO Hotels (NameHotel, CountOfStars, CountryName) VALUES (@a,@b,@c)";
            x.ShowDialog();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Ваши введённые данные не сохранятся! Продолжить?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
                Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length < 1)
            {
                MessageBox.Show("Заполните все поля!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var con = countryTableAdapter.Connection;
            con.Open();
            var cmd = new SqlCommand("SELECT NameHotel FROM Hotels", con);
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (Name != string.Empty && txtName.Text.ToLower() != Name.ToLower())
                    if (rdr.GetString(0).ToLower() == txtName.Text.ToLower())
                    {
                        MessageBox.Show("Такое название отеля уже существует в базе!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        con.Close();
                        return;
                    }
            }
            rdr.Close();
            cmd.CommandText = cmdText;
            cmd.Parameters.Add("@a", SqlDbType.Text).Value = txtName.Text;
            cmd.Parameters.Add("@b", SqlDbType.Int).Value = numStars.Value;
            cmd.Parameters.Add("@c", SqlDbType.Text).Value = cmbCountry.SelectedValue;
            cmd.ExecuteNonQuery();
            MessageBox.Show("Действие прошло успешно!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            con.Close();
            Close();
        }

        
    }
}
