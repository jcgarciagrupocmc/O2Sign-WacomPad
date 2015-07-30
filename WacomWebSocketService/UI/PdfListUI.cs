using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WacomWebSocketService.Entities;

namespace WacomWebSocketService.UI
{
    public partial class PdfListUI : Form
    {
        private List<DocumentData> listPdf;

        public PdfListUI()
        {
            InitializeComponent();
        }
        public void setListPdf(List<DocumentData> pList)
        {
            this.listPdf = pList;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void PdfListUI_Shown(object sender, EventArgs e)
        {
            if (listPdf != null)
            {
                this.pdfGV.DataSource = this.listPdf;
                this.pdfGV.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            }
            else
            {
                //Error
            }
        }

        private void pdfGV_DataSourceChanged(object sender, EventArgs e)
        {

        }

    }
}
