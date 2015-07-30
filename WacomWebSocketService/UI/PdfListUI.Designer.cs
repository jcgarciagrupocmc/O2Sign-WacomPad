namespace WacomWebSocketService.UI
{
    partial class PdfListUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pdfGV = new System.Windows.Forms.DataGridView();
            this.docName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.viewButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.signButton = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pdfGV)).BeginInit();
            this.SuspendLayout();
            // 
            // pdfGV
            // 
            this.pdfGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.pdfGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.docName,
            this.state,
            this.viewButton,
            this.signButton});
            this.pdfGV.Location = new System.Drawing.Point(15, 22);
            this.pdfGV.Name = "pdfGV";
            this.pdfGV.Size = new System.Drawing.Size(447, 150);
            this.pdfGV.TabIndex = 0;
            this.pdfGV.DataSourceChanged += new System.EventHandler(this.pdfGV_DataSourceChanged);
            this.pdfGV.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // docName
            // 
            this.docName.HeaderText = "Documento";
            this.docName.Name = "docName";
            this.docName.ReadOnly = true;
            // 
            // state
            // 
            this.state.HeaderText = "Estado";
            this.state.Name = "state";
            this.state.ReadOnly = true;
            // 
            // viewButton
            // 
            this.viewButton.HeaderText = "";
            this.viewButton.Name = "viewButton";
            // 
            // signButton
            // 
            this.signButton.HeaderText = "";
            this.signButton.Name = "signButton";
            // 
            // PdfListUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 348);
            this.Controls.Add(this.pdfGV);
            this.Name = "PdfListUI";
            this.Text = "PdfListUI";
            this.Shown += new System.EventHandler(this.PdfListUI_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pdfGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView pdfGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn docName;
        private System.Windows.Forms.DataGridViewTextBoxColumn state;
        private System.Windows.Forms.DataGridViewButtonColumn viewButton;
        private System.Windows.Forms.DataGridViewButtonColumn signButton;

    }
}