using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Tours.Forms
{
    public partial class ListOfTours : Form
    {
        SqlConnection sqlConnection = new SqlConnection("Data Source = LAPTOP-667E4RFT; Initial Catalog = DB_TOURS; Integrated Security = True; TrustServerCertificate = True; ");
        List<TourPanel> tourList = new List<TourPanel>();
        int page = 0;
        public ListOfTours()
        {
            InitializeComponent();
        }
        public void checkOrder()
        {
            toolStripSplitButton1.Visible = StaticOrder.TourPanels.Count > 0;
        }

        private void ListOfTours_Load(object sender, EventArgs e)
        {
            cmbType.SelectedIndex = 0;
            sqlConnection.Open();
            SqlCommand command = new SqlCommand("SELECT TypeTourName FROM TypeTours", sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                cmbType.Items.Add(reader.GetString(0));
            }
            reader.Close();
            sqlConnection.Close();
            DataLoad();
            checkOrder();
        }

        void DataLoad()
        {
            tableLayoutPanelTours.Controls.Clear();
            sqlConnection.Open();
            string commandText = "SELECT TourName, Price, IsActual, CountOfTickets, IdTour FROM Tours";
            if (cmbType.SelectedIndex != 0)
                commandText += $", ToursOfTypeTours WHERE ToursOfTypeTours.TourId = Tours.IdTour AND ToursOfTypeTours.TypeTourId = {cmbType.SelectedIndex}";
            if (btnSort.Text == "По убыванию")
                commandText += " ORDER BY Price DESC";
            if (btnSort.Text == "По возрастанию")
                commandText += " ORDER BY Price ASC";
            SqlCommand command = new SqlCommand(commandText, sqlConnection);

            tourList.ForEach(tour => { tour.Dispose(); });
            tourList.Clear();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                tourList.Add(new TourPanel());
                tourList.Last().Init(reader.GetString(0), reader.GetDecimal(1), reader.GetBoolean(2), reader.GetInt32(3), reader.GetInt32(4));
                if (!reader.GetBoolean(2)) tourList.Last().contextMenuStrip1.Dispose();
                tourList.Last().Tag = this;
            }

            for (int i = 0; i < tourList.Count; i++)
            {
                if (!tourList[i].lblTitle.Text.ToLower().Contains(txtSearch.Text.ToLower())||
                    (chbActual.Checked && tourList[i].lblActual.Text == "Не актуален"))
                {
                    tourList[i].Dispose();
                    tourList.RemoveAt(i);
                    i--;
                }
            }
            reader.Close();
            sqlConnection.Close();
            btnLeft.Enabled = btnRight.Enabled = tourList.Count != 0;
            
            if (tourList.Count == 0)
            {
                tableLayoutPanelTours.Controls.Clear();
                toolStripSplitButton1.Text = $"Данных нет";
                MessageBox.Show("Данный запрос не привел результатов", "Ошибка поиска", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal price = 0;
            tourList.ForEach(tour => { price += tour.tourPrice * tour.tourTickets; });
            lblFullPrice.Text = "Общая стоимость туров: " + price.ToString();
            page = 0;
            DataView(page);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataLoad();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataLoad();
        }

        private void chbActual_CheckedChanged(object sender, EventArgs e)
        {
            DataLoad();
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            switch (btnSort.Text)
            {
                case "Не сортировано":
                    btnSort.Text = "По убыванию";
                    break;
                case "По убыванию":
                    btnSort.Text = "По возрастанию";
                    break;
                case "По возрастанию":
                    btnSort.Text = "Не сортировано";
                    break;
            }
            DataLoad();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            page--;
            if (page == -1)
                page = (tourList.Count - 1) / 6;
            DataView(page);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            page++;
            if (page == -1)
                page = (tourList.Count - 1) / 6 + 1;
            DataView(page);
        }

        void DataView(int page)
        {
            tableLayoutPanelTours.Controls.Clear();
            for (int i = page * 6; i < page * 6 + 6; i++)
            {
                tableLayoutPanelTours.Controls.Add(tourList[i]);
                if (tourList.Count - 1 == i)
                    break;
            }
            toolStripStatusLabel1.Text = $"Страница: {page + 1} из {(tourList.Count - 1) / 6 + 1}";
        }      

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            Hide();
            new OrderForm() {Tag = Tag}.ShowDialog();
            checkOrder();
            Show();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
