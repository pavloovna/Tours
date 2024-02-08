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
using Tours.Forms;

namespace Tours
{
    public partial class Hotels : Form
    {
        int page;
        public Hotels()
        {
            InitializeComponent();
        }

        private void Hotels_Load(object sender, EventArgs e)
        {
            this.countryTableAdapter.Fill(this.dB_TOURSDataSet1.Country);
            this.hotelsTableAdapter.Fill(this.dB_TOURSDataSet1.Hotels);
            page = 0;
            Paging(page);
        }

        private void dataGridViewHotels_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 4)
            {
                Hide();
                DataGridViewCellCollection r = dataGridViewHotels.Rows[e.RowIndex].Cells;
                WorkingHotels.Redacting(r[r.Count - 1].Value.ToString(), r[0].Value.ToString(), (int)r[1].Value, r[2].Value.ToString());
                Show();
                hotelsTableAdapter.Fill(this.dB_TOURSDataSet1.Hotels);
                page = 0;
                Paging(page);
            }
        }

        void Paging(int page)
        {
            hotelsBindingSource.CurrencyManager.SuspendBinding();
            foreach (DataGridViewRow item in dataGridViewHotels.Rows)
            {
                item.Visible = false;
            }
            hotelsBindingSource.CurrencyManager.ResumeBinding();
            var con = countryTableAdapter.Connection;
            con.Open();
            var cmd = new SqlCommand("", con);
            for (int i = page * (int)numericUpDown1.Value; i < page * (int)numericUpDown1.Value + (int)numericUpDown1.Value && i < hotelsBindingSource.Count; i++)
            {
                dataGridViewHotels.Rows[i].Visible = true;
                cmd.CommandText = "SELECT COUNT(Hotels.NameHotel) FROM Hotels, Country, Tours WHERE Tours.Country_Code = Country.CountryCode AND Hotels.CountryName = Country.CountryCode AND IdHotel = " + dataGridViewHotels.Rows[i].Cells[dataGridViewHotels.Rows[i].Cells.Count - 1].Value;
                var rdr = cmd.ExecuteReader();
                while (rdr.Read()) dataGridViewHotels.Rows[i].Cells[3].Value = rdr[0];
                rdr.Close();
            }
            con.Close();
            bindingNavigatorPositionItem.Text = (page + 1).ToString();
            bindingNavigatorCountItem.Text = "из " + ((hotelsBindingSource.Count - 1) / (int)numericUpDown1.Value + 1);
            numericUpDown1.Maximum = hotelsBindingSource.Count;
            label2.Text = "Кол-во записей всего: " + hotelsBindingSource.Count;
        }

        private void bindingNavigatorMoveFirstItem_Click(object sender, EventArgs e)
        {
            page = 0;
            Paging(page);
        }

        private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
        {
            page = hotelsBindingSource.Count / (int)numericUpDown1.Value;
            Paging(page);
        }

        private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        {
            page -= page > 0 ? 1 : 0;
            Paging(page);
        }

        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {
            page += page < (hotelsBindingSource.Count - 1) / (int)numericUpDown1.Value ? 1 : 0;
            Paging(page);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            page = 0;
            Paging(page);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Hide();
            WorkingHotels.New();
            Show();
            this.hotelsTableAdapter.Fill(this.dB_TOURSDataSet1.Hotels);
            page = 0;
            Paging(page);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewHotels.SelectedCells.Count > 0)
            {
                var con = hotelsTableAdapter.Connection;
                con.Open();
                var cmd = new SqlCommand("SELECT * " +
                    "FROM Hotels, Country, Tours " +
                    "WHERE Tours.Country_Code = Country.CountryCode " +
                    "AND Hotels.CountryName = Country.CountryCode " +
                   $"AND Hotels.IdHotel = {dataGridViewHotels.Rows[dataGridViewHotels.SelectedCells[0].RowIndex].Cells[dataGridViewHotels.ColumnCount - 1].Value}" +
                   $"AND Tours.IsActual = 1", con);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MessageBox.Show("Данный отель невозможно удалить, на его месте есть активный тур!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    con.Close();
                    return;
                }
                rdr.Close();
                con.Close();
                if (MessageBox.Show("Вы уверены, что хотите удалить данный отель?",
                    "Внимание",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                    ==
                    DialogResult.Yes)
                {
                    hotelsBindingSource.RemoveCurrent();
                    hotelsBindingSource.EndEdit();
                    this.tableAdapterManager.UpdateAll(this.dB_TOURSDataSet1);
                };
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }        
    }
}
