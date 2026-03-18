using System.Collections.Generic;
using System.Windows.Forms;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    /// <summary>
    /// Fenêtre modale affichant les abonnements expirant dans moins de 30 jours
    /// </summary>
    public partial class FrmAlerteAbonnements : Form
    {
        /// <summary>
        /// Constructeur : reçoit la liste des alertes et l'affiche dans le DataGridView
        /// </summary>
        /// <param name="alertes">liste des abonnements expirant bientôt</param>
        public FrmAlerteAbonnements(List<AlerteAbonnement> alertes)
        {
            InitializeComponent();
            dgvAlertes.DataSource = alertes;
            if (dgvAlertes.Columns.Count > 0)
            {
                dgvAlertes.Columns["IdRevue"].Visible = false;
                dgvAlertes.Columns["TitreRevue"].HeaderText = "Titre de la revue";
                dgvAlertes.Columns["DateFinAbonnement"].HeaderText = "Date de fin";
                dgvAlertes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void btnFermer_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
