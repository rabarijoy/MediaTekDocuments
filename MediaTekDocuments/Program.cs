using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MediaTekDocuments.dal;
using MediaTekDocuments.model;
using MediaTekDocuments.view;

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

            // Affichage du formulaire de connexion
            using (FrmLogin frmLogin = new FrmLogin())
            {
                if (frmLogin.ShowDialog() != DialogResult.OK || frmLogin.UtilisateurConnecte == null)
                {
                    Application.Exit();
                    return;
                }

                Utilisateur utilisateur = frmLogin.UtilisateurConnecte;

                // L'alerte d'abonnement n'est visible que pour les services avec accès complet
                bool droitsComplets = utilisateur.IdService == "00001" || utilisateur.IdService == "00004";
                if (droitsComplets)
                {
                    List<AlerteAbonnement> alertes = Access.GetInstance().GetAbonnementsExpirantBientot();
                    if (alertes != null && alertes.Count > 0)
                    {
                        using (FrmAlerteAbonnements frmAlerte = new FrmAlerteAbonnements(alertes))
                        {
                            frmAlerte.ShowDialog();
                        }
                    }
                }

                Application.Run(new FrmMediatek(utilisateur));
            }
        }
    }
}
