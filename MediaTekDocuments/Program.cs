using MediaTekDocuments.dal;
using MediaTekDocuments.view;
using System.Collections.Generic;
using System.Windows.Forms;
using MediaTekDocuments.model;

namespace MediaTekDocuments
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            List<AlerteAbonnement> alertes = Access.GetInstance().GetAbonnementsExpirantBientot();
            if (alertes != null && alertes.Count > 0)
            {
                using (FrmAlerteAbonnements frmAlerte = new FrmAlerteAbonnements(alertes))
                {
                    frmAlerte.ShowDialog();
                }
            }

            Application.Run(new FrmMediatek());
        }
    }
}
