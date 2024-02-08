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
using Tours.DB_TOURSDataSetTableAdapters;

namespace Tours.Forms
{
    public partial class Orders : Form
    {
        public Orders()
        {
            InitializeComponent();
        }
        private void ordersBindingNavigatorSaveItem_Click(object sender, EventArgs e) 
        {
            this.Validate();
            this.ordersBindingSource.EndEdit();
            this.tableAdapterManager1.UpdateAll(this.dB_TOURSDataSet1);

        }
        private void Orders_Load(object sender, EventArgs e)
        {
            this.ordersTableAdapter.Fill(this.dB_TOURSDataSet1.Orders);
            DGVAddions();
            comboBox1.SelectedIndex = -1;
        }
        private void DGVAddions()
        {
            var con = ordersTableAdapter.Connection;
            con.Open();
            var cmd = new SqlCommand("", con);
            foreach (DataGridViewRow row in dataGridViewOrders.Rows)
            {
                var splittedOrder = row.Cells[1].Value.ToString().Split(';').ToList();
                splittedOrder.RemoveAt(splittedOrder.Count - 1);
                foreach (string x in splittedOrder)
                {
                    cmd.CommandText = "SELECT TourName FROM Tours WHERE IdTour = " + x.Split(':')[0];
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        row.Cells[2].Value += $"{rdr[0]} - {x.Split(':')[1]} шт; ";
                    }
                    rdr.Close();
                }
                if (row.Cells[row.Cells.Count - 2].Value.ToString() != "GUEST")
                    row.Cells[row.Cells.Count - 1].Value = row.Cells[row.Cells.Count - 2].Value;
            }
            con.Close();
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            switch (btnSort.Text)
            {
                case "Не отсортировано":
                    btnSort.Text = "По возрастанию";
                    ordersBindingSource.Sort = "Sum ASC";
                    break;
                case "По возрастанию":
                    btnSort.Text = "По убыванию";
                    ordersBindingSource.Sort = "Sum DESC";
                    break;
                case "По убыванию":
                    btnSort.Text = "Не отсортировано";
                    ordersBindingSource.Sort = "";
                    break;
            }
            DGVAddions();
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBox1.Text.Split('-', '%').Length;
            ordersBindingSource.Filter = $"Sale >= {(n != 1 ? comboBox1.Text.Split('-', '%')[0] : "0")} " +
                $"AND Sale <= {(n > 2 ? comboBox1.Text.Split('-', '%')[2] : "100")}";
            DGVAddions();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
