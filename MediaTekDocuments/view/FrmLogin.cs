using System;
using System.Windows.Forms;
using MediaTekDocuments.dal;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    /// <summary>
    /// Formulaire de connexion — affiché avant FrmMediatek
    /// </summary>
    public partial class FrmLogin : Form
    {
        /// <summary>
        /// Utilisateur authentifié après une connexion réussie (null sinon)
        /// </summary>
        public Utilisateur UtilisateurConnecte { get; private set; }

        public FrmLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Tente la connexion avec les identifiants saisis
        /// </summary>
        private void btnConnexion_Click(object sender, EventArgs e)
        {
            string login = txbLogin.Text.Trim();
            string pwd   = txbPwd.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("Veuillez saisir un identifiant et un mot de passe.",
                    "Champs manquants", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Utilisateur utilisateur = Access.GetInstance().GetUtilisateur(login, pwd);

            if (utilisateur == null)
            {
                MessageBox.Show("Identifiant ou mot de passe incorrect.",
                    "Connexion refusée", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txbPwd.Clear();
                txbPwd.Focus();
                return;
            }

            if (utilisateur.IdService == "00003")
            {
                MessageBox.Show("Droits insuffisants. L'application va se fermer.",
                    "Accès refusé", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            UtilisateurConnecte = utilisateur;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Annulation : ferme toute l'application
        /// </summary>
        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Permet de valider avec la touche Entrée depuis les champs de saisie
        /// </summary>
        private void txb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnexion_Click(sender, e);
            }
        }
    }
}
