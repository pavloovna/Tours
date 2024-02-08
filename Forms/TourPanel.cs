using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tours.Properties;

namespace Tours.Forms
{
    public partial class TourPanel : UserControl
    {
        public string tourName = "XXX";
        public decimal tourPrice = 0;
        public bool Actual = false;
        public int tourTickets = 0;
        public int id = -1;
        public TourPanel()
        {
            InitializeComponent();
        }

        public void Init(string tourName, decimal price, bool isActual, int countOfTickets, int idTour)
        {
            this.tourName = tourName;
            tourPrice = Math.Round(price, 2);
            Actual = isActual;
            tourTickets = countOfTickets;
            this.id = idTour;

            lblTitle.Text = tourName;            
            ComponentResourceManager resourceManager = new ComponentResourceManager(typeof(Resources));
            pictureBox1.BackgroundImage = resourceManager.GetObject(tourName.Replace(' ', '_')) != null ?
                (Image)resourceManager.GetObject(tourName.Replace(' ', '_')) :
                (Image)resourceManager.GetObject("picture");
            lblPrice.Text = Math.Round(price, 2).ToString();
            lblActual.Text = isActual ? "Актуален" : "Не актуален";
            lblActual.ForeColor = isActual ? Color.Green : Color.Red;   
            lblTickets.Text = lblTickets.Text.Replace("xxx", countOfTickets.ToString());
        }
        public TourPanel Copy()
        {
            var a = new TourPanel();
            a.Init(tourName, tourPrice, Actual, tourTickets, id);
            return a;
        }       

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            var x = StaticOrder.TourPanels;
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i].tour.tourName == tourName)
                {
                    x[i].count++;
                    ((ListOfTours)Tag).checkOrder();
                    return;
                }
            }
            x.Add(new StaticOrder.ToursWithCount { tour = Copy(), count = 1 });
            ((ListOfTours)Tag).checkOrder();
        }
    }
}
