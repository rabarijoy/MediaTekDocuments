namespace MediaTekDocuments.view
{
    partial class FrmAlerteAbonnements
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblInfo = new System.Windows.Forms.Label();
            this.dgvAlertes = new System.Windows.Forms.DataGridView();
            this.btnFermer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertes)).BeginInit();
            this.SuspendLayout();

            // lblInfo
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(12, 12);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(350, 13);
            this.lblInfo.Text = "Les abonnements suivants expirent dans moins de 30 jours :";

            // dgvAlertes
            this.dgvAlertes.AllowUserToAddRows = false;
            this.dgvAlertes.AllowUserToDeleteRows = false;
            this.dgvAlertes.AllowUserToResizeColumns = false;
            this.dgvAlertes.AllowUserToResizeRows = false;
            this.dgvAlertes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlertes.Location = new System.Drawing.Point(12, 35);
            this.dgvAlertes.MultiSelect = false;
            this.dgvAlertes.Name = "dgvAlertes";
            this.dgvAlertes.ReadOnly = true;
            this.dgvAlertes.RowHeadersVisible = false;
            this.dgvAlertes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAlertes.Size = new System.Drawing.Size(460, 250);

            // btnFermer
            this.btnFermer.Location = new System.Drawing.Point(197, 300);
            this.btnFermer.Name = "btnFermer";
            this.btnFermer.Size = new System.Drawing.Size(90, 28);
            this.btnFermer.Text = "Fermer";
            this.btnFermer.Click += new System.EventHandler(this.btnFermer_Click);

            // FrmAlerteAbonnements
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 341);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.dgvAlertes);
            this.Controls.Add(this.btnFermer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAlerteAbonnements";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Abonnements expirant bientôt";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.DataGridView dgvAlertes;
        private System.Windows.Forms.Button btnFermer;
    }
}
