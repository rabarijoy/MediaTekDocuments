using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Constantes de l'interface
        private const string MSG_NUMERO_INTROUVABLE  = "numéro introuvable";
        private const string MSG_SAISIE_INCOMPLETE   = "Saisie incomplète";
        private const string MSG_SUCCES              = "Succès";
        private const string MSG_ERREUR              = "Erreur";
        private const string MSG_AUCUNE_SELECTION    = "Aucune sélection";
        private const string MSG_SELECTION_MANQUANTE = "Sélection manquante";
        private const string MSG_CONFIRMATION        = "Confirmation";
        private const string MSG_ERREUR_SUPPRESSION  = "Erreur lors de la suppression.";
        private const string MSG_SAISIE_INCORRECTE   = "Saisie incorrecte";
        private const string MSG_RECHERCHE           = "Recherche";
        private const string MSG_SELECTION_COMMANDE  = "Veuillez sélectionner une commande.";
        private const string MSG_REGLE_METIER        = "Règle métier";
        #endregion

        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        /// <summary>
        /// Calcule le premier identifiant entier non encore attribué dans la liste fournie.
        /// Détecte automatiquement le préfixe non-numérique et le formatage (zéros de remplissage).
        /// Exemple : ["LIV001","LIV003"] → "LIV002"
        /// </summary>
        private static string ProchainId(IEnumerable<string> idsExistants)
        {
            var ids = idsExistants.ToList();
            string prefixe = DetecterPrefixe(ids);
            HashSet<int> nums = CollecterNumeros(ids, prefixe, out int largeur);
            int suivant = 1;
            while (nums.Contains(suivant)) suivant++;
            string numStr = largeur > 0
                ? suivant.ToString().PadLeft(largeur, '0')
                : suivant.ToString();
            return prefixe + numStr;
        }

        /// <summary>Détecte le préfixe commun non-numérique de la liste d'ids.</summary>
        private static string DetecterPrefixe(List<string> ids)
        {
            if (ids.Count == 0) return "";
            string premier = ids[0];
            int fin = 0;
            while (fin < premier.Length && !char.IsDigit(premier[fin])) fin++;
            string candidat = premier.Substring(0, fin);
            return ids.All(id => id.StartsWith(candidat, StringComparison.OrdinalIgnoreCase))
                ? candidat
                : "";
        }

        /// <summary>Collecte les numéros entiers des ids après le préfixe et détecte la largeur de formatage.</summary>
        private static HashSet<int> CollecterNumeros(List<string> ids, string prefixe, out int largeur)
        {
            var nums = new HashSet<int>();
            largeur = 0;
            foreach (string id in ids)
            {
                string partie = id.Length > prefixe.Length ? id.Substring(prefixe.Length) : "";
                if (int.TryParse(partie, out int n) && n > 0)
                {
                    nums.Add(n);
                    if (partie.Length > largeur) largeur = partie.Length;
                }
            }
            return nums;
        }

        private readonly Utilisateur _utilisateur;

        /// <summary>
        /// Constructeur principal : reçoit l'utilisateur authentifié et applique ses droits d'accès
        /// </summary>
        internal FrmMediatek(Utilisateur utilisateur)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this._utilisateur = utilisateur;
            AppliquerDroitsAcces();
        }

        /// <summary>
        /// Applique les restrictions d'interface selon le service de l'utilisateur connecté.
        /// - 00001 / 00004 (Administratif / Administrateur) : accès complet, aucune restriction.
        /// - 00002 (Prêts) : lecture seule — masque les zones de saisie et les onglets de commandes.
        /// - 00003 (Culture) : ne doit jamais atteindre ce formulaire (géré dans FrmLogin).
        /// </summary>
        private void AppliquerDroitsAcces()
        {
            if (_utilisateur == null || _utilisateur.IdService == "00001" || _utilisateur.IdService == "00004")
                return;

            if (_utilisateur.IdService == "00002")
            {
                // Masquer les zones de gestion (ajout / modification / suppression) des 3 onglets catalogue
                grpLivresSaisie.Visible = false;
                grpDvdSaisie.Visible = false;
                grpRevuesSaisie.Visible = false;

                // Masquer les contrôles d'exemplaires (modification état / suppression)
                cbxLivresEtats.Visible = false;
                btnLivresModifierEtat.Visible = false;
                btnLivresSupprimerExemplaire.Visible = false;
                cbxDvdEtats.Visible = false;
                btnDvdModifierEtat.Visible = false;
                btnDvdSupprimerExemplaire.Visible = false;

                // Masquer les onglets commandes et parutions
                tabOngletsApplication.TabPages.Remove(tabCommandesLivres);
                tabOngletsApplication.TabPages.Remove(tabCommandesDvd);
                tabOngletsApplication.TabPages.Remove(tabCommandesRevues);
                tabOngletsApplication.TabPages.Remove(tabReceptionRevue);
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public static void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private readonly BindingSource bdgLivresExemplaires = new BindingSource();
        private List<Exemplaire> lesExemplairesLivre = new List<Exemplaire>();
        private List<Etat> lesEtats = new List<Etat>();

        // BindingSources dédiés aux ComboBox de saisie de l'onglet Livres
        private readonly BindingSource bdgLivresSaisieGenres = new BindingSource();
        private readonly BindingSource bdgLivresSaisiePublics = new BindingSource();
        private readonly BindingSource bdgLivresSaisieRayons = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            lesEtats = controller.GetAllEtats();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            // combos de saisie CRUD
            RemplirComboCategorie(controller.GetAllGenres(), bdgLivresSaisieGenres, cbxLivresSaisieGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgLivresSaisiePublics, cbxLivresSaisiePublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgLivresSaisieRayons, cbxLivresSaisieRayons);
            RemplirComboEtats(cbxLivresEtats);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show(MSG_NUMERO_INTROUVABLE);
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre + remplissage des champs de saisie CRUD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    RemplirLivresSaisie(livre);
                    AfficheExemplairesLivre(livre.Id);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
                VideLivresSaisie();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        // ─── CRUD Livres ──────────────────────────────────────────────────────

        /// <summary>
        /// Remplit les champs de saisie CRUD à partir du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre sélectionné</param>
        private void RemplirLivresSaisie(Livre livre)
        {
            txbLivresSaisieId.ReadOnly = true;
            txbLivresSaisieId.Text = livre.Id;
            btnLivresAjouter.Enabled = false;
            btnLivresModifier.Enabled = true;
            btnLivresSuppimer.Enabled = true;
            txbLivresSaisieTitre.Text = livre.Titre;
            txbLivresSaisieImage.Text = livre.Image;
            txbLivresSaisieIsbn.Text = livre.Isbn;
            txbLivresSaisieAuteur.Text = livre.Auteur;
            txbLivresSaisieCollection.Text = livre.Collection;

            List<Categorie> genres = bdgLivresSaisieGenres.DataSource as List<Categorie>;
            int idxGenre = genres?.FindIndex(c => c.Id == livre.IdGenre) ?? -1;
            cbxLivresSaisieGenres.SelectedIndex = idxGenre;

            List<Categorie> publics = bdgLivresSaisiePublics.DataSource as List<Categorie>;
            int idxPublic = publics?.FindIndex(c => c.Id == livre.IdPublic) ?? -1;
            cbxLivresSaisiePublics.SelectedIndex = idxPublic;

            List<Categorie> rayons = bdgLivresSaisieRayons.DataSource as List<Categorie>;
            int idxRayon = rayons?.FindIndex(c => c.Id == livre.IdRayon) ?? -1;
            cbxLivresSaisieRayons.SelectedIndex = idxRayon;
        }

        /// <summary>
        /// Vide les champs de saisie CRUD du groupe Livres
        /// </summary>
        private void VideLivresSaisie()
        {
            txbLivresSaisieId.ReadOnly = true;
            txbLivresSaisieId.Text = ProchainId(lesLivres.Select(l => l.Id));
            txbLivresSaisieTitre.Text = "";
            txbLivresSaisieImage.Text = "";
            txbLivresSaisieIsbn.Text = "";
            txbLivresSaisieAuteur.Text = "";
            txbLivresSaisieCollection.Text = "";
            cbxLivresSaisieGenres.SelectedIndex = -1;
            cbxLivresSaisiePublics.SelectedIndex = -1;
            cbxLivresSaisieRayons.SelectedIndex = -1;
            btnLivresAjouter.Enabled = true;
            btnLivresModifier.Enabled = false;
            btnLivresSuppimer.Enabled = false;
        }

        /// <summary>
        /// Construit un objet Livre depuis les champs de saisie
        /// Retourne null si les ComboBox de catégorie ne sont pas sélectionnés
        /// </summary>
        private Livre LivreDepuisSaisie()
        {
            if (cbxLivresSaisieGenres.SelectedItem == null ||
                cbxLivresSaisiePublics.SelectedItem == null ||
                cbxLivresSaisieRayons.SelectedItem == null)
                return null;

            Categorie genre   = (Categorie)cbxLivresSaisieGenres.SelectedItem;
            Categorie lePublic = (Categorie)cbxLivresSaisiePublics.SelectedItem;
            Categorie rayon   = (Categorie)cbxLivresSaisieRayons.SelectedItem;

            return new Livre(
                txbLivresSaisieId.Text.Trim(),
                txbLivresSaisieTitre.Text.Trim(),
                txbLivresSaisieImage.Text.Trim(),
                txbLivresSaisieIsbn.Text.Trim(),
                txbLivresSaisieAuteur.Text.Trim(),
                txbLivresSaisieCollection.Text.Trim(),
                genre.Id,    genre.Libelle,
                lePublic.Id, lePublic.Libelle,
                rayon.Id,    rayon.Libelle
            );
        }

        /// <summary>
        /// Ajout d'un livre
        /// </summary>
        private void btnLivresAjouter_Click(object sender, EventArgs e)
        {
            if (txbLivresSaisieTitre.Text.Trim().Equals("") ||
                txbLivresSaisieIsbn.Text.Trim().Equals("") ||
                txbLivresSaisieAuteur.Text.Trim().Equals(""))
            {
                MessageBox.Show("Les champs Titre, ISBN et Auteur sont obligatoires.", MSG_SAISIE_INCOMPLETE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Livre livre = LivreDepuisSaisie();
            if (livre == null)
            {
                MessageBox.Show("Veuillez sélectionner un genre, un public et un rayon.", MSG_SAISIE_INCOMPLETE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (controller.AjouterLivre(livre))
            {
                MessageBox.Show("Livre ajouté avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesLivres = controller.GetAllLivres();
                RemplirLivresListeComplete();
                VideLivresSaisie();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout du livre.", MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Modification du livre sélectionné
        /// </summary>
        private void btnLivresModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.SelectedRows.Count == 0 && dgvLivresListe.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner un livre à modifier.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Livre livre = LivreDepuisSaisie();
            if (livre == null)
            {
                MessageBox.Show("Veuillez sélectionner un genre, un public et un rayon.", MSG_SAISIE_INCOMPLETE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (controller.ModifierLivre(livre))
            {
                MessageBox.Show("Livre modifié avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesLivres = controller.GetAllLivres();
                RemplirLivresListeComplete();
                VideLivresSaisie();
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification du livre.", MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Peuple un ComboBox avec la liste des états (partagée entre Livres, DVD et Parutions)
        /// </summary>
        private void RemplirComboEtats(ComboBox cbx)
        {
            cbx.DataSource = null;
            cbx.DataSource = lesEtats;
            cbx.DisplayMember = "Libelle";
            cbx.ValueMember = "Id";
            if (cbx.Items.Count > 0) cbx.SelectedIndex = 0;
        }

        /// <summary>
        /// Charge et affiche les exemplaires d'un livre dans dgvLivresExemplaires
        /// </summary>
        private void AfficheExemplairesLivre(string idLivre)
        {
            lesExemplairesLivre = controller.GetExemplairesDocument(idLivre);
            lesExemplairesLivre = lesExemplairesLivre.OrderByDescending(e => e.DateAchat).ToList();
            RemplirLivresExemplairesListe(lesExemplairesLivre);
        }

        /// <summary>
        /// Lie la liste d'exemplaires au DataGridView des exemplaires de livres
        /// </summary>
        private void RemplirLivresExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgLivresExemplaires.DataSource = exemplaires;
            dgvLivresExemplaires.DataSource = bdgLivresExemplaires;
            if (dgvLivresExemplaires.Columns.Count > 0)
            {
                dgvLivresExemplaires.Columns["Id"].Visible = false;
                dgvLivresExemplaires.Columns["Photo"].Visible = false;
                dgvLivresExemplaires.Columns["IdEtat"].Visible = false;
                dgvLivresExemplaires.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        /// <summary>
        /// Tri sur colonne du DGV exemplaires livres
        /// </summary>
        private void dgvLivresExemplaires_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string col = dgvLivresExemplaires.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sorted;
            switch (col)
            {
                case "Numero":
                    sorted = lesExemplairesLivre.OrderBy(o => o.Numero).ToList();
                    break;
                case "DateAchat":
                    sorted = lesExemplairesLivre.OrderByDescending(o => o.DateAchat).ToList();
                    break;
                case "LibelleEtat":
                    sorted = lesExemplairesLivre.OrderBy(o => o.LibelleEtat).ToList();
                    break;
                default:
                    sorted = lesExemplairesLivre;
                    break;
            }
            RemplirLivresExemplairesListe(sorted);
        }

        /// <summary>
        /// Modifie l'état de l'exemplaire sélectionné dans dgvLivresExemplaires
        /// </summary>
        private void btnLivresModifierEtat_Click(object sender, EventArgs e)
        {
            if (dgvLivresExemplaires.CurrentCell == null || cbxLivresEtats.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire et un état.", MSG_SELECTION_MANQUANTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Exemplaire ex = (Exemplaire)bdgLivresExemplaires.List[bdgLivresExemplaires.Position];
            Etat etat = (Etat)cbxLivresEtats.SelectedItem;
            if (controller.ModifierEtatExemplaire(ex.Id, ex.Numero, etat.Id))
            {
                AfficheExemplairesLivre(ex.Id);
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification de l'état.", MSG_ERREUR,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Supprime l'exemplaire sélectionné dans dgvLivresExemplaires après confirmation
        /// </summary>
        private void btnLivresSupprimerExemplaire_Click(object sender, EventArgs e)
        {
            if (dgvLivresExemplaires.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire.", MSG_SELECTION_MANQUANTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Exemplaire ex = (Exemplaire)bdgLivresExemplaires.List[bdgLivresExemplaires.Position];
            if (MessageBox.Show("Confirmer la suppression de l'exemplaire n°" + ex.Numero + " ?",
                MSG_CONFIRMATION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (controller.SupprimerExemplaire(ex.Id, ex.Numero))
                {
                    AfficheExemplairesLivre(ex.Id);
                }
                else
                {
                    MessageBox.Show(MSG_ERREUR_SUPPRESSION, MSG_ERREUR,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Efface les champs de saisie et passe en mode « nouveau livre » avec Id auto-attribué
        /// </summary>
        private void btnLivresEffacer_Click(object sender, EventArgs e)
        {
            VideLivresSaisie();
        }

        /// <summary>
        /// Suppression du livre sélectionné
        /// </summary>
        private void btnLivresSuppimer_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.SelectedRows.Count == 0 && dgvLivresListe.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner un livre à supprimer.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string id = txbLivresSaisieId.Text.Trim();
            if (id.Equals(""))
            {
                MessageBox.Show("Aucun livre sélectionné.", MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Confirmer la suppression de ce livre ?",
                MSG_CONFIRMATION,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (controller.SupprimerLivre(id))
                {
                    MessageBox.Show("Livre supprimé avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lesLivres = controller.GetAllLivres();
                    RemplirLivresListeComplete();
                    VideLivresSaisie();
                }
                else
                {
                    MessageBox.Show(
                        "Suppression impossible : le livre possède des exemplaires ou des commandes.",
                        MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();
        private readonly BindingSource bdgDvdExemplaires = new BindingSource();
        private List<Exemplaire> lesExemplairesDvd = new List<Exemplaire>();

        // BindingSources dédiés aux ComboBox de saisie de l'onglet DVD
        private readonly BindingSource bdgDvdSaisieGenres = new BindingSource();
        private readonly BindingSource bdgDvdSaisiePublics = new BindingSource();
        private readonly BindingSource bdgDvdSaisieRayons = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            if (lesEtats.Count == 0) lesEtats = controller.GetAllEtats();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            // combos de saisie CRUD
            RemplirComboCategorie(controller.GetAllGenres(), bdgDvdSaisieGenres, cbxDvdSaisieGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgDvdSaisiePublics, cbxDvdSaisiePublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgDvdSaisieRayons, cbxDvdSaisieRayons);
            RemplirComboEtats(cbxDvdEtats);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show(MSG_NUMERO_INTROUVABLE);
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd + remplissage des champs de saisie CRUD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                    RemplirDvdSaisie(dvd);
                    AfficheExemplairesDvd(dvd.Id);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
                VideDvdSaisie();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        // ─── CRUD Dvd ─────────────────────────────────────────────────────────

        /// <summary>
        /// Remplit les champs de saisie CRUD à partir du dvd sélectionné
        /// </summary>
        private void RemplirDvdSaisie(Dvd dvd)
        {
            txbDvdSaisieId.ReadOnly = true;
            txbDvdSaisieId.Text = dvd.Id;
            btnDvdAjouter.Enabled = false;
            btnDvdModifier.Enabled = true;
            btnDvdSuppimer.Enabled = true;
            txbDvdSaisieTitre.Text = dvd.Titre;
            txbDvdSaisieImage.Text = dvd.Image;
            txbDvdSaisieSynopsis.Text = dvd.Synopsis;
            txbDvdSaisieRealisateur.Text = dvd.Realisateur;
            txbDvdSaisieDuree.Text = dvd.Duree.ToString();

            List<Categorie> genres = bdgDvdSaisieGenres.DataSource as List<Categorie>;
            cbxDvdSaisieGenres.SelectedIndex = genres?.FindIndex(c => c.Id == dvd.IdGenre) ?? -1;

            List<Categorie> publics = bdgDvdSaisiePublics.DataSource as List<Categorie>;
            cbxDvdSaisiePublics.SelectedIndex = publics?.FindIndex(c => c.Id == dvd.IdPublic) ?? -1;

            List<Categorie> rayons = bdgDvdSaisieRayons.DataSource as List<Categorie>;
            cbxDvdSaisieRayons.SelectedIndex = rayons?.FindIndex(c => c.Id == dvd.IdRayon) ?? -1;
        }

        /// <summary>
        /// Vide les champs de saisie CRUD du groupe DVD
        /// </summary>
        private void VideDvdSaisie()
        {
            txbDvdSaisieId.ReadOnly = true;
            txbDvdSaisieId.Text = ProchainId(lesDvd.Select(d => d.Id));
            txbDvdSaisieTitre.Text = "";
            txbDvdSaisieImage.Text = "";
            txbDvdSaisieSynopsis.Text = "";
            txbDvdSaisieRealisateur.Text = "";
            txbDvdSaisieDuree.Text = "";
            cbxDvdSaisieGenres.SelectedIndex = -1;
            cbxDvdSaisiePublics.SelectedIndex = -1;
            cbxDvdSaisieRayons.SelectedIndex = -1;
            btnDvdAjouter.Enabled = true;
            btnDvdModifier.Enabled = false;
            btnDvdSuppimer.Enabled = false;
        }

        /// <summary>
        /// Construit un objet Dvd depuis les champs de saisie
        /// </summary>
        private Dvd DvdDepuisSaisie()
        {
            if (cbxDvdSaisieGenres.SelectedItem == null ||
                cbxDvdSaisiePublics.SelectedItem == null ||
                cbxDvdSaisieRayons.SelectedItem == null)
                return null;

            if (!int.TryParse(txbDvdSaisieDuree.Text.Trim(), out int duree))
            {
                MessageBox.Show("La durée doit être un nombre entier.", MSG_SAISIE_INCORRECTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            Categorie genre    = (Categorie)cbxDvdSaisieGenres.SelectedItem;
            Categorie lePublic = (Categorie)cbxDvdSaisiePublics.SelectedItem;
            Categorie rayon    = (Categorie)cbxDvdSaisieRayons.SelectedItem;

            return new Dvd(
                txbDvdSaisieId.Text.Trim(),
                txbDvdSaisieTitre.Text.Trim(),
                txbDvdSaisieImage.Text.Trim(),
                duree,
                txbDvdSaisieRealisateur.Text.Trim(),
                txbDvdSaisieSynopsis.Text.Trim(),
                genre.Id,    genre.Libelle,
                lePublic.Id, lePublic.Libelle,
                rayon.Id,    rayon.Libelle
            );
        }

        /// <summary>
        /// Ajout d'un dvd
        /// </summary>
        private void btnDvdAjouter_Click(object sender, EventArgs e)
        {
            if (txbDvdSaisieTitre.Text.Trim().Equals("") ||
                txbDvdSaisieRealisateur.Text.Trim().Equals(""))
            {
                MessageBox.Show("Les champs Titre et Réalisateur sont obligatoires.", MSG_SAISIE_INCOMPLETE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Dvd dvd = DvdDepuisSaisie();
            if (dvd == null) return;

            if (controller.AjouterDvd(dvd))
            {
                MessageBox.Show("DVD ajouté avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesDvd = controller.GetAllDvd();
                RemplirDvdListeComplete();
                VideDvdSaisie();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout du DVD.", MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Modification du dvd sélectionné
        /// </summary>
        private void btnDvdModifier_Click(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner un DVD à modifier.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Dvd dvd = DvdDepuisSaisie();
            if (dvd == null) return;

            if (controller.ModifierDvd(dvd))
            {
                MessageBox.Show("DVD modifié avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesDvd = controller.GetAllDvd();
                RemplirDvdListeComplete();
                VideDvdSaisie();
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification du DVD.", MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Efface les champs de saisie et passe en mode « nouveau DVD » avec Id auto-attribué
        /// </summary>
        private void btnDvdEffacer_Click(object sender, EventArgs e)
        {
            VideDvdSaisie();
        }

        /// <summary>
        /// Suppression du dvd sélectionné
        /// </summary>
        private void btnDvdSuppimer_Click(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner un DVD à supprimer.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string id = txbDvdSaisieId.Text.Trim();
            if (id.Equals("")) return;

            DialogResult confirm = MessageBox.Show(
                "Confirmer la suppression de ce DVD ?",
                MSG_CONFIRMATION,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (controller.SupprimerDvd(id))
                {
                    MessageBox.Show("DVD supprimé avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lesDvd = controller.GetAllDvd();
                    RemplirDvdListeComplete();
                    VideDvdSaisie();
                }
                else
                {
                    MessageBox.Show(
                        "Suppression impossible : le DVD possède des exemplaires ou des commandes.",
                        MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Charge et affiche les exemplaires d'un DVD dans dgvDvdExemplaires
        /// </summary>
        private void AfficheExemplairesDvd(string idDvd)
        {
            lesExemplairesDvd = controller.GetExemplairesDocument(idDvd);
            lesExemplairesDvd = lesExemplairesDvd.OrderByDescending(e => e.DateAchat).ToList();
            RemplirDvdExemplairesListe(lesExemplairesDvd);
        }

        /// <summary>
        /// Lie la liste d'exemplaires au DataGridView des exemplaires de DVD
        /// </summary>
        private void RemplirDvdExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgDvdExemplaires.DataSource = exemplaires;
            dgvDvdExemplaires.DataSource = bdgDvdExemplaires;
            if (dgvDvdExemplaires.Columns.Count > 0)
            {
                dgvDvdExemplaires.Columns["Id"].Visible = false;
                dgvDvdExemplaires.Columns["Photo"].Visible = false;
                dgvDvdExemplaires.Columns["IdEtat"].Visible = false;
                dgvDvdExemplaires.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        /// <summary>
        /// Tri sur colonne du DGV exemplaires DVD
        /// </summary>
        private void dgvDvdExemplaires_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string col = dgvDvdExemplaires.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sorted;
            switch (col)
            {
                case "Numero":
                    sorted = lesExemplairesDvd.OrderBy(o => o.Numero).ToList();
                    break;
                case "DateAchat":
                    sorted = lesExemplairesDvd.OrderByDescending(o => o.DateAchat).ToList();
                    break;
                case "LibelleEtat":
                    sorted = lesExemplairesDvd.OrderBy(o => o.LibelleEtat).ToList();
                    break;
                default:
                    sorted = lesExemplairesDvd;
                    break;
            }
            RemplirDvdExemplairesListe(sorted);
        }

        /// <summary>
        /// Modifie l'état de l'exemplaire sélectionné dans dgvDvdExemplaires
        /// </summary>
        private void btnDvdModifierEtat_Click(object sender, EventArgs e)
        {
            if (dgvDvdExemplaires.CurrentCell == null || cbxDvdEtats.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire et un état.", MSG_SELECTION_MANQUANTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Exemplaire ex = (Exemplaire)bdgDvdExemplaires.List[bdgDvdExemplaires.Position];
            Etat etat = (Etat)cbxDvdEtats.SelectedItem;
            if (controller.ModifierEtatExemplaire(ex.Id, ex.Numero, etat.Id))
            {
                AfficheExemplairesDvd(ex.Id);
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification de l'état.", MSG_ERREUR,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Supprime l'exemplaire sélectionné dans dgvDvdExemplaires après confirmation
        /// </summary>
        private void btnDvdSupprimerExemplaire_Click(object sender, EventArgs e)
        {
            if (dgvDvdExemplaires.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire.", MSG_SELECTION_MANQUANTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Exemplaire ex = (Exemplaire)bdgDvdExemplaires.List[bdgDvdExemplaires.Position];
            if (MessageBox.Show("Confirmer la suppression de l'exemplaire n°" + ex.Numero + " ?",
                MSG_CONFIRMATION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (controller.SupprimerExemplaire(ex.Id, ex.Numero))
                {
                    AfficheExemplairesDvd(ex.Id);
                }
                else
                {
                    MessageBox.Show(MSG_ERREUR_SUPPRESSION, MSG_ERREUR,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        // BindingSources dédiés aux ComboBox de saisie de l'onglet Revues
        private readonly BindingSource bdgRevuesSaisieGenres = new BindingSource();
        private readonly BindingSource bdgRevuesSaisiePublics = new BindingSource();
        private readonly BindingSource bdgRevuesSaisieRayons = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            // combos de saisie CRUD
            RemplirComboCategorie(controller.GetAllGenres(), bdgRevuesSaisieGenres, cbxRevuesSaisieGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgRevuesSaisiePublics, cbxRevuesSaisiePublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRevuesSaisieRayons, cbxRevuesSaisieRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show(MSG_NUMERO_INTROUVABLE);
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue + remplissage des champs de saisie CRUD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                    RemplirRevuesSaisie(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
                VideRevuesSaisie();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        // ─── CRUD Revues ──────────────────────────────────────────────────────

        /// <summary>
        /// Remplit les champs de saisie CRUD à partir de la revue sélectionnée
        /// </summary>
        private void RemplirRevuesSaisie(Revue revue)
        {
            txbRevuesSaisieId.ReadOnly = true;
            txbRevuesSaisieId.Text = revue.Id;
            btnRevuesAjouter.Enabled = false;
            btnRevuesModifier.Enabled = true;
            btnRevuesSuppimer.Enabled = true;
            txbRevuesSaisieTitre.Text = revue.Titre;
            txbRevuesSaisieImage.Text = revue.Image;
            txbRevuesSaisiePeriodicite.Text = revue.Periodicite;
            txbRevuesSaisieDelai.Text = revue.DelaiMiseADispo.ToString();

            List<Categorie> genres = bdgRevuesSaisieGenres.DataSource as List<Categorie>;
            cbxRevuesSaisieGenres.SelectedIndex = genres?.FindIndex(c => c.Id == revue.IdGenre) ?? -1;

            List<Categorie> publics = bdgRevuesSaisiePublics.DataSource as List<Categorie>;
            cbxRevuesSaisiePublics.SelectedIndex = publics?.FindIndex(c => c.Id == revue.IdPublic) ?? -1;

            List<Categorie> rayons = bdgRevuesSaisieRayons.DataSource as List<Categorie>;
            cbxRevuesSaisieRayons.SelectedIndex = rayons?.FindIndex(c => c.Id == revue.IdRayon) ?? -1;
        }

        /// <summary>
        /// Vide les champs de saisie CRUD du groupe Revues
        /// </summary>
        private void VideRevuesSaisie()
        {
            txbRevuesSaisieId.ReadOnly = true;
            txbRevuesSaisieId.Text = ProchainId(lesRevues.Select(r => r.Id));
            txbRevuesSaisieTitre.Text = "";
            txbRevuesSaisieImage.Text = "";
            txbRevuesSaisiePeriodicite.Text = "";
            txbRevuesSaisieDelai.Text = "";
            cbxRevuesSaisieGenres.SelectedIndex = -1;
            cbxRevuesSaisiePublics.SelectedIndex = -1;
            cbxRevuesSaisieRayons.SelectedIndex = -1;
            btnRevuesAjouter.Enabled = true;
            btnRevuesModifier.Enabled = false;
            btnRevuesSuppimer.Enabled = false;
        }

        /// <summary>
        /// Construit un objet Revue depuis les champs de saisie
        /// </summary>
        private Revue RevueDepuisSaisie()
        {
            if (cbxRevuesSaisieGenres.SelectedItem == null ||
                cbxRevuesSaisiePublics.SelectedItem == null ||
                cbxRevuesSaisieRayons.SelectedItem == null)
                return null;

            if (!int.TryParse(txbRevuesSaisieDelai.Text.Trim(), out int delai))
            {
                MessageBox.Show("Le délai de mise à disposition doit être un nombre entier.", MSG_SAISIE_INCORRECTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            Categorie genre    = (Categorie)cbxRevuesSaisieGenres.SelectedItem;
            Categorie lePublic = (Categorie)cbxRevuesSaisiePublics.SelectedItem;
            Categorie rayon    = (Categorie)cbxRevuesSaisieRayons.SelectedItem;

            return new Revue(
                txbRevuesSaisieId.Text.Trim(),
                txbRevuesSaisieTitre.Text.Trim(),
                txbRevuesSaisieImage.Text.Trim(),
                genre.Id,    genre.Libelle,
                lePublic.Id, lePublic.Libelle,
                rayon.Id,    rayon.Libelle,
                txbRevuesSaisiePeriodicite.Text.Trim(),
                delai
            );
        }

        /// <summary>
        /// Ajout d'une revue
        /// </summary>
        private void btnRevuesAjouter_Click(object sender, EventArgs e)
        {
            if (txbRevuesSaisieTitre.Text.Trim().Equals("") ||
                txbRevuesSaisiePeriodicite.Text.Trim().Equals(""))
            {
                MessageBox.Show("Les champs Titre et Périodicité sont obligatoires.", MSG_SAISIE_INCOMPLETE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Revue revue = RevueDepuisSaisie();
            if (revue == null) return;

            if (controller.AjouterRevue(revue))
            {
                MessageBox.Show("Revue ajoutée avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesRevues = controller.GetAllRevues();
                RemplirRevuesListeComplete();
                VideRevuesSaisie();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout de la revue.", MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Modification de la revue sélectionnée
        /// </summary>
        private void btnRevuesModifier_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner une revue à modifier.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Revue revue = RevueDepuisSaisie();
            if (revue == null) return;

            if (controller.ModifierRevue(revue))
            {
                MessageBox.Show("Revue modifiée avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesRevues = controller.GetAllRevues();
                RemplirRevuesListeComplete();
                VideRevuesSaisie();
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification de la revue.", MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Efface les champs de saisie et passe en mode « nouvelle revue » avec Id auto-attribué
        /// </summary>
        private void btnRevuesEffacer_Click(object sender, EventArgs e)
        {
            VideRevuesSaisie();
        }

        /// <summary>
        /// Suppression de la revue sélectionnée
        /// </summary>
        private void btnRevuesSuppimer_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner une revue à supprimer.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string id = txbRevuesSaisieId.Text.Trim();
            if (id.Equals("")) return;

            DialogResult confirm = MessageBox.Show(
                "Confirmer la suppression de cette revue ?",
                MSG_CONFIRMATION,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (controller.SupprimerRevue(id))
                {
                    MessageBox.Show("Revue supprimée avec succès.", MSG_SUCCES, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lesRevues = controller.GetAllRevues();
                    RemplirRevuesListeComplete();
                    VideRevuesSaisie();
                }
                else
                {
                    MessageBox.Show(
                        "Suppression impossible : la revue possède des exemplaires ou des abonnements.",
                        MSG_ERREUR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            if (lesEtats.Count == 0) lesEtats = controller.GetAllEtats();
            RemplirComboEtats(cbxParutionsEtats);
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                if (dgvReceptionExemplairesListe.Columns.Count > 0)
                {
                    dgvReceptionExemplairesListe.Columns["IdEtat"].Visible = false;
                    dgvReceptionExemplairesListe.Columns["Id"].Visible = false;
                    dgvReceptionExemplairesListe.Columns["Photo"].Visible = false;
                    dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show(MSG_NUMERO_INTROUVABLE);
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesDocument(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", MSG_ERREUR);
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList;
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderByDescending(o => o.DateAchat).ToList();
                    break;
                case "LibelleEtat":
                    sortedList = lesExemplaires.OrderBy(o => o.LibelleEtat).ToList();
                    break;
                default:
                    sortedList = lesExemplaires;
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// Modifie l'état de l'exemplaire sélectionné dans la liste des parutions
        /// </summary>
        private void btnParutionsModifierEtat_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell == null || cbxParutionsEtats.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire et un état.", MSG_SELECTION_MANQUANTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Exemplaire ex = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
            Etat etat = (Etat)cbxParutionsEtats.SelectedItem;
            if (controller.ModifierEtatExemplaire(ex.Id, ex.Numero, etat.Id))
            {
                AfficheReceptionExemplairesRevue();
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification de l'état.", MSG_ERREUR,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Supprime l'exemplaire sélectionné dans la liste des parutions après confirmation
        /// </summary>
        private void btnParutionsSupprimerExemplaire_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire.", MSG_SELECTION_MANQUANTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Exemplaire ex = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
            if (MessageBox.Show("Confirmer la suppression de l'exemplaire n°" + ex.Numero + " ?",
                MSG_CONFIRMATION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (controller.SupprimerExemplaire(ex.Id, ex.Numero))
                {
                    AfficheReceptionExemplairesRevue();
                }
                else
                {
                    MessageBox.Show(MSG_ERREUR_SUPPRESSION, MSG_ERREUR,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }
        #endregion

        #region Onglet Commandes Livres

        private const string SUIVI_EN_COURS = "00001";
        private const string SUIVI_RELANCE  = "00002";
        private const string SUIVI_LIVRE    = "00003";
        private const string SUIVI_REGLE    = "00004";

        private readonly BindingSource bdgCommandesLivresListe = new BindingSource();
        private List<CommandeDocument> lesCommandesLivres = new List<CommandeDocument>();
        private List<Suivi> lesSuivi = new List<Suivi>();
        private Livre livreCommandeEnCours = null;

        /// <summary>
        /// Ouverture de l'onglet Commandes Livres : charge la liste des livres et des étapes de suivi
        /// </summary>
        private void tabCommandesLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            lesSuivi = controller.GetAllSuivi();
            RemplirCbxSuivi(cbxCommandesLivresSuivi);
            ViderCommandesLivresZones();
        }

        /// <summary>
        /// Remplit un ComboBox avec la liste des étapes de suivi
        /// </summary>
        private void RemplirCbxSuivi(System.Windows.Forms.ComboBox cbx)
        {
            cbx.DataSource = new List<Suivi>(lesSuivi);
            cbx.DisplayMember = "Libelle";
            cbx.ValueMember = "Id";
            cbx.SelectedIndex = -1;
        }

        /// <summary>
        /// Vide les informations affichées dans la zone commandes livres
        /// </summary>
        private void ViderCommandesLivresZones()
        {
            lblCommandesLivresTitre.Text = "";
            lblCommandesLivresAuteur.Text = "";
            lblCommandesLivresIsbn.Text = "";
            lesCommandesLivres = new List<CommandeDocument>();
            bdgCommandesLivresListe.DataSource = null;
            livreCommandeEnCours = null;
        }

        /// <summary>
        /// Remplit le DataGridView des commandes livres
        /// </summary>
        private void RemplirCommandesLivresListe(List<CommandeDocument> commandes)
        {
            bdgCommandesLivresListe.DataSource = commandes;
            dgvCommandesLivresListe.DataSource = bdgCommandesLivresListe;
            if (dgvCommandesLivresListe.Columns.Count > 0)
            {
                dgvCommandesLivresListe.Columns["idLivreDvd"].Visible = false;
                dgvCommandesLivresListe.Columns["idSuivi"].Visible = false;
                dgvCommandesLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        /// <summary>
        /// Recherche un livre par son numéro et charge ses commandes
        /// </summary>
        private void btnCommandesLivresRechercher_Click(object sender, EventArgs e)
        {
            string num = txbCommandesLivresNumero.Text.Trim();
            if (num.Equals(""))
            {
                MessageBox.Show("Veuillez saisir un numéro de livre.", MSG_RECHERCHE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Livre livre = lesLivres.Find(x => x.Id.Equals(num));
            if (livre != null)
            {
                livreCommandeEnCours = livre;
                lblCommandesLivresTitre.Text = livre.Titre;
                lblCommandesLivresAuteur.Text = livre.Auteur;
                lblCommandesLivresIsbn.Text = livre.Isbn;
                lesCommandesLivres = controller.GetCommandesLivreDvd(livre.Id);
                RemplirCommandesLivresListe(lesCommandesLivres);
            }
            else
            {
                MessageBox.Show("Numéro introuvable.", MSG_RECHERCHE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ViderCommandesLivresZones();
            }
        }

        /// <summary>
        /// Sélection d'une commande dans le DGV : met à jour le ComboBox de suivi
        /// </summary>
        private void dgvCommandesLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesLivresListe.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commande = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
                    int idx = lesSuivi.FindIndex(s => s.Id == commande.IdSuivi);
                    cbxCommandesLivresSuivi.SelectedIndex = idx;
                }
                catch (Exception)
                {
                    // Ignoré : la sélection peut être invalide lors du rechargement du DGV
                }
            }
        }

        /// <summary>
        /// Tri sur colonne du DGV commandes livres
        /// </summary>
        private void dgvCommandesLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string col = dgvCommandesLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList;
            switch (col)
            {
                case "Id":
                    sortedList = lesCommandesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesCommandesLivres.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesLivres.OrderBy(o => o.Montant).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCommandesLivres.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "LibelleEtape":
                    sortedList = lesCommandesLivres.OrderBy(o => o.LibelleEtape).ToList();
                    break;
                default:
                    sortedList = lesCommandesLivres;
                    break;
            }
            RemplirCommandesLivresListe(sortedList);
        }

        /// <summary>
        /// Génère un identifiant de commande unique au format Cxxxx (VARCHAR 5)
        /// </summary>
        private string GenererIdCommande()
        {
            var rng = new Random(Guid.NewGuid().GetHashCode());
            var tousIds = lesCommandesLivres.Select(c => c.Id)
                          .Concat(lesCommandesDvd.Select(c => c.Id))
                          .ToHashSet();
            string id;
            do { id = "C" + rng.Next(1000, 10000); }
            while (tousIds.Contains(id));
            return id;
        }

        /// <summary>
        /// Ajout d'une commande de livre
        /// </summary>
        private void btnCommandesLivresAjouterCommande_Click(object sender, EventArgs e)
        {
            if (livreCommandeEnCours == null)
            {
                MessageBox.Show("Veuillez d'abord rechercher un livre.", "Aucun livre sélectionné",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string montantStrLivres = txbCommandesLivresMontant.Text.Trim().Replace(',', '.');
            if (!double.TryParse(montantStrLivres, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double montant) || montant <= 0)
            {
                MessageBox.Show("Le montant doit être un nombre supérieur à 0.", MSG_SAISIE_INCORRECTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txbCommandesLivresNbExemplaires.Text.Trim(), out int nbExemplaires) || nbExemplaires <= 0)
            {
                MessageBox.Show("Le nombre d'exemplaires doit être un entier supérieur à 0.", MSG_SAISIE_INCORRECTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CommandeDocument commande = new CommandeDocument(
                GenererIdCommande(),
                dtpCommandesLivresDate.Value.Date,
                montant,
                nbExemplaires,
                SUIVI_EN_COURS,
                "",
                livreCommandeEnCours.Id
            );

            if (controller.AjouterCommandeDocument(commande))
            {
                MessageBox.Show("Commande ajoutée avec succès.", MSG_SUCCES,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesCommandesLivres = controller.GetCommandesLivreDvd(livreCommandeEnCours.Id);
                RemplirCommandesLivresListe(lesCommandesLivres);
                txbCommandesLivresMontant.Text = "";
                txbCommandesLivresNbExemplaires.Text = "";
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout de la commande.", MSG_ERREUR,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de livre
        /// Règles métier :
        ///   - Une commande livrée ou réglée ne peut pas revenir à une étape antérieure
        ///   - Une commande ne peut être réglée que si elle est livrée
        /// </summary>
        private void btnCommandesLivresModifierSuivi_Click(object sender, EventArgs e)
        {
            if (dgvCommandesLivresListe.CurrentCell == null)
            {
                MessageBox.Show(MSG_SELECTION_COMMANDE, MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbxCommandesLivresSuivi.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une étape de suivi.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CommandeDocument commande = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
            Suivi nouvelleEtape = (Suivi)cbxCommandesLivresSuivi.SelectedItem;

            // Règle 1 : livrée ou réglée → ne peut aller qu'à réglée (et uniquement depuis livrée)
            if ((commande.IdSuivi == SUIVI_LIVRE || commande.IdSuivi == SUIVI_REGLE) && nouvelleEtape.Id != SUIVI_REGLE)
            {
                MessageBox.Show(
                    "Impossible : une commande livrée ou réglée ne peut pas revenir à une étape précédente.",
                    MSG_REGLE_METIER, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Règle 2 : ne peut être réglée que si elle est actuellement livrée
            if (nouvelleEtape.Id == SUIVI_REGLE && commande.IdSuivi != SUIVI_LIVRE)
            {
                MessageBox.Show(
                    "Impossible : une commande ne peut être réglée que si elle est livrée.",
                    MSG_REGLE_METIER, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (controller.ModifierEtapeSuivi(commande.Id, nouvelleEtape.Id))
            {
                MessageBox.Show("Étape de suivi modifiée avec succès.", MSG_SUCCES,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesCommandesLivres = controller.GetCommandesLivreDvd(livreCommandeEnCours.Id);
                RemplirCommandesLivresListe(lesCommandesLivres);
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification.", MSG_ERREUR,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Suppression d'une commande de livre
        /// Uniquement si l'étape est "en cours" (00001) ou "relancée" (00002)
        /// </summary>
        private void btnCommandesLivresSupprimerCommande_Click(object sender, EventArgs e)
        {
            if (dgvCommandesLivresListe.CurrentCell == null)
            {
                MessageBox.Show(MSG_SELECTION_COMMANDE, MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CommandeDocument commande = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];

            if (commande.IdSuivi != SUIVI_EN_COURS && commande.IdSuivi != SUIVI_RELANCE)
            {
                MessageBox.Show(
                    "Suppression impossible : seules les commandes « en cours » ou « relancées » peuvent être supprimées.",
                    MSG_REGLE_METIER, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Confirmer la suppression de cette commande ?",
                MSG_CONFIRMATION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (controller.SupprimerCommandeDocument(commande.Id))
                {
                    MessageBox.Show("Commande supprimée avec succès.", MSG_SUCCES,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lesCommandesLivres = controller.GetCommandesLivreDvd(livreCommandeEnCours.Id);
                    RemplirCommandesLivresListe(lesCommandesLivres);
                }
                else
                {
                    MessageBox.Show(MSG_ERREUR_SUPPRESSION, MSG_ERREUR,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Onglet Commandes DVD

        private readonly BindingSource bdgCommandesDvdListe = new BindingSource();
        private List<CommandeDocument> lesCommandesDvd = new List<CommandeDocument>();
        private Dvd dvdCommandeEnCours = null;

        /// <summary>
        /// Ouverture de l'onglet Commandes DVD : charge la liste des DVD et des étapes de suivi
        /// </summary>
        private void tabCommandesDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            lesSuivi = controller.GetAllSuivi();
            RemplirCbxSuivi(cbxCommandesDvdSuivi);
            ViderCommandesDvdZones();
        }

        /// <summary>
        /// Vide les informations affichées dans la zone commandes DVD
        /// </summary>
        private void ViderCommandesDvdZones()
        {
            lblCommandesDvdTitre.Text = "";
            lblCommandesDvdRealisateur.Text = "";
            lblCommandesDvdDuree.Text = "";
            lesCommandesDvd = new List<CommandeDocument>();
            bdgCommandesDvdListe.DataSource = null;
            dvdCommandeEnCours = null;
        }

        /// <summary>
        /// Remplit le DataGridView des commandes DVD
        /// </summary>
        private void RemplirCommandesDvdListe(List<CommandeDocument> commandes)
        {
            bdgCommandesDvdListe.DataSource = commandes;
            dgvCommandesDvdListe.DataSource = bdgCommandesDvdListe;
            if (dgvCommandesDvdListe.Columns.Count > 0)
            {
                dgvCommandesDvdListe.Columns["idLivreDvd"].Visible = false;
                dgvCommandesDvdListe.Columns["idSuivi"].Visible = false;
                dgvCommandesDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        /// <summary>
        /// Recherche un DVD par son numéro et charge ses commandes
        /// </summary>
        private void btnCommandesDvdRechercher_Click(object sender, EventArgs e)
        {
            string num = txbCommandesDvdNumero.Text.Trim();
            if (num.Equals(""))
            {
                MessageBox.Show("Veuillez saisir un numéro de DVD.", MSG_RECHERCHE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Dvd dvd = lesDvd.Find(x => x.Id.Equals(num));
            if (dvd != null)
            {
                dvdCommandeEnCours = dvd;
                lblCommandesDvdTitre.Text = dvd.Titre;
                lblCommandesDvdRealisateur.Text = dvd.Realisateur;
                lblCommandesDvdDuree.Text = dvd.Duree.ToString() + " min";
                lesCommandesDvd = controller.GetCommandesLivreDvd(dvd.Id);
                RemplirCommandesDvdListe(lesCommandesDvd);
            }
            else
            {
                MessageBox.Show("Numéro introuvable.", MSG_RECHERCHE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ViderCommandesDvdZones();
            }
        }

        /// <summary>
        /// Sélection d'une commande DVD dans le DGV : met à jour le ComboBox de suivi
        /// </summary>
        private void dgvCommandesDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandesDvdListe.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commande = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
                    int idx = lesSuivi.FindIndex(s => s.Id == commande.IdSuivi);
                    cbxCommandesDvdSuivi.SelectedIndex = idx;
                }
                catch (Exception)
                {
                    // Ignoré : la sélection peut être invalide lors du rechargement du DGV
                }
            }
        }

        /// <summary>
        /// Tri sur colonne du DGV commandes DVD
        /// </summary>
        private void dgvCommandesDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string col = dgvCommandesDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList;
            switch (col)
            {
                case "Id":
                    sortedList = lesCommandesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesCommandesDvd.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDvd.OrderBy(o => o.Montant).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCommandesDvd.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "LibelleEtape":
                    sortedList = lesCommandesDvd.OrderBy(o => o.LibelleEtape).ToList();
                    break;
                default:
                    sortedList = lesCommandesDvd;
                    break;
            }
            RemplirCommandesDvdListe(sortedList);
        }

        /// <summary>
        /// Ajout d'une commande de DVD
        /// </summary>
        private void btnCommandesDvdAjouterCommande_Click(object sender, EventArgs e)
        {
            if (dvdCommandeEnCours == null)
            {
                MessageBox.Show("Veuillez d'abord rechercher un DVD.", "Aucun DVD sélectionné",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string montantStrDvd = txbCommandesDvdMontant.Text.Trim().Replace(',', '.');
            if (!double.TryParse(montantStrDvd, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double montant) || montant <= 0)
            {
                MessageBox.Show("Le montant doit être un nombre supérieur à 0.", MSG_SAISIE_INCORRECTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txbCommandesDvdNbExemplaires.Text.Trim(), out int nbExemplaires) || nbExemplaires <= 0)
            {
                MessageBox.Show("Le nombre d'exemplaires doit être un entier supérieur à 0.", MSG_SAISIE_INCORRECTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CommandeDocument commande = new CommandeDocument(
                GenererIdCommande(),
                dtpCommandesDvdDate.Value.Date,
                montant,
                nbExemplaires,
                SUIVI_EN_COURS,
                "",
                dvdCommandeEnCours.Id
            );

            if (controller.AjouterCommandeDocument(commande))
            {
                MessageBox.Show("Commande ajoutée avec succès.", MSG_SUCCES,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesCommandesDvd = controller.GetCommandesLivreDvd(dvdCommandeEnCours.Id);
                RemplirCommandesDvdListe(lesCommandesDvd);
                txbCommandesDvdMontant.Text = "";
                txbCommandesDvdNbExemplaires.Text = "";
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout de la commande.", MSG_ERREUR,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de DVD
        /// Mêmes règles métier que pour les livres
        /// </summary>
        private void btnCommandesDvdModifierSuivi_Click(object sender, EventArgs e)
        {
            if (dgvCommandesDvdListe.CurrentCell == null)
            {
                MessageBox.Show(MSG_SELECTION_COMMANDE, MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cbxCommandesDvdSuivi.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une étape de suivi.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CommandeDocument commande = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
            Suivi nouvelleEtape = (Suivi)cbxCommandesDvdSuivi.SelectedItem;

            if ((commande.IdSuivi == SUIVI_LIVRE || commande.IdSuivi == SUIVI_REGLE) && nouvelleEtape.Id != SUIVI_REGLE)
            {
                MessageBox.Show(
                    "Impossible : une commande livrée ou réglée ne peut pas revenir à une étape précédente.",
                    MSG_REGLE_METIER, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (nouvelleEtape.Id == SUIVI_REGLE && commande.IdSuivi != SUIVI_LIVRE)
            {
                MessageBox.Show(
                    "Impossible : une commande ne peut être réglée que si elle est livrée.",
                    MSG_REGLE_METIER, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (controller.ModifierEtapeSuivi(commande.Id, nouvelleEtape.Id))
            {
                MessageBox.Show("Étape de suivi modifiée avec succès.", MSG_SUCCES,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesCommandesDvd = controller.GetCommandesLivreDvd(dvdCommandeEnCours.Id);
                RemplirCommandesDvdListe(lesCommandesDvd);
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification.", MSG_ERREUR,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Suppression d'une commande de DVD
        /// Uniquement si l'étape est "en cours" (00001) ou "relancée" (00002)
        /// </summary>
        private void btnCommandesDvdSupprimerCommande_Click(object sender, EventArgs e)
        {
            if (dgvCommandesDvdListe.CurrentCell == null)
            {
                MessageBox.Show(MSG_SELECTION_COMMANDE, MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CommandeDocument commande = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];

            if (commande.IdSuivi != SUIVI_EN_COURS && commande.IdSuivi != SUIVI_RELANCE)
            {
                MessageBox.Show(
                    "Suppression impossible : seules les commandes « en cours » ou « relancées » peuvent être supprimées.",
                    MSG_REGLE_METIER, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Confirmer la suppression de cette commande ?",
                MSG_CONFIRMATION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (controller.SupprimerCommandeDocument(commande.Id))
                {
                    MessageBox.Show("Commande supprimée avec succès.", MSG_SUCCES,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lesCommandesDvd = controller.GetCommandesLivreDvd(dvdCommandeEnCours.Id);
                    RemplirCommandesDvdListe(lesCommandesDvd);
                }
                else
                {
                    MessageBox.Show(MSG_ERREUR_SUPPRESSION, MSG_ERREUR,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Onglet Commandes Revues

        private readonly BindingSource bdgCommandesRevuesListe = new BindingSource();
        private List<Abonnement> lesAbonnements = new List<Abonnement>();
        private Revue revueCommandeEnCours = null;

        /// <summary>
        /// Ouverture de l'onglet Commandes Revues : charge la liste des revues
        /// </summary>
        private void tabCommandesRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            ViderCommandesRevuesZones();
        }

        /// <summary>
        /// Vide les informations affichées dans la zone commandes revues
        /// </summary>
        private void ViderCommandesRevuesZones()
        {
            lblCommandesRevuesTitre.Text = "";
            lblCommandesRevuesPeriodicite.Text = "";
            lesAbonnements = new List<Abonnement>();
            bdgCommandesRevuesListe.DataSource = null;
            revueCommandeEnCours = null;
        }

        /// <summary>
        /// Remplit le DataGridView des abonnements d'une revue
        /// </summary>
        private void RemplirCommandesRevuesListe(List<Abonnement> abonnements)
        {
            bdgCommandesRevuesListe.DataSource = abonnements;
            dgvCommandesRevuesListe.DataSource = bdgCommandesRevuesListe;
            if (dgvCommandesRevuesListe.Columns.Count > 0)
            {
                dgvCommandesRevuesListe.Columns["IdRevue"].Visible = false;
                dgvCommandesRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        /// <summary>
        /// Recherche une revue par son numéro et charge ses abonnements
        /// </summary>
        private void btnCommandesRevuesRechercher_Click(object sender, EventArgs e)
        {
            string num = txbCommandesRevuesNumero.Text.Trim();
            if (num.Equals(""))
            {
                MessageBox.Show("Veuillez saisir un numéro de revue.", MSG_RECHERCHE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Revue revue = lesRevues.Find(x => x.Id.Equals(num));
            if (revue != null)
            {
                revueCommandeEnCours = revue;
                lblCommandesRevuesTitre.Text = revue.Titre;
                lblCommandesRevuesPeriodicite.Text = "Périodicité : " + revue.Periodicite;
                lesAbonnements = controller.GetAbonnementsRevue(revue.Id);
                RemplirCommandesRevuesListe(lesAbonnements);
            }
            else
            {
                MessageBox.Show("Numéro introuvable.", MSG_RECHERCHE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ViderCommandesRevuesZones();
            }
        }

        /// <summary>
        /// Tri sur colonne du DGV abonnements
        /// </summary>
        private void dgvCommandesRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string col = dgvCommandesRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList;
            switch (col)
            {
                case "Id":
                    sortedList = lesAbonnements.OrderBy(o => o.Id).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesAbonnements.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnements.OrderBy(o => o.Montant).ToList();
                    break;
                case "DateFinAbonnement":
                    sortedList = lesAbonnements.OrderBy(o => o.DateFinAbonnement).ToList();
                    break;
                default:
                    sortedList = lesAbonnements;
                    break;
            }
            RemplirCommandesRevuesListe(sortedList);
        }

        /// <summary>
        /// Ajout d'un abonnement pour la revue en cours
        /// </summary>
        private void btnCommandesRevuesAjouter_Click(object sender, EventArgs e)
        {
            if (revueCommandeEnCours == null)
            {
                MessageBox.Show("Veuillez d'abord rechercher une revue.", "Aucune revue sélectionnée",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string montantStr = txbCommandesRevuesMontant.Text.Trim().Replace(',', '.');
            if (!double.TryParse(montantStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double montant) || montant <= 0)
            {
                MessageBox.Show("Le montant doit être un nombre supérieur à 0.", MSG_SAISIE_INCORRECTE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpCommandesRevuesDateFin.Value.Date <= dtpCommandesRevuesDate.Value.Date)
            {
                MessageBox.Show("La date de fin d'abonnement doit être postérieure à la date de commande.",
                    MSG_SAISIE_INCORRECTE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Abonnement abonnement = new Abonnement(
                GenererIdCommande(),
                dtpCommandesRevuesDate.Value.Date,
                montant,
                dtpCommandesRevuesDateFin.Value.Date,
                revueCommandeEnCours.Id
            );

            if (controller.AjouterAbonnement(abonnement))
            {
                MessageBox.Show("Abonnement ajouté avec succès.", MSG_SUCCES,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                lesAbonnements = controller.GetAbonnementsRevue(revueCommandeEnCours.Id);
                RemplirCommandesRevuesListe(lesAbonnements);
                txbCommandesRevuesMontant.Text = "";
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout de l'abonnement.", MSG_ERREUR,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Suppression d'un abonnement.
        /// Vérifie côté client qu'aucun exemplaire de la revue n'a une dateAchat dans la période
        /// (en utilisant UtilitairesAbonnement.ParutionDansAbonnement), puis délègue à l'API
        /// qui effectue la même vérification côté serveur.
        /// </summary>
        private void btnCommandesRevuesSupprimerAbonnement_Click(object sender, EventArgs e)
        {
            if (dgvCommandesRevuesListe.CurrentCell == null)
            {
                MessageBox.Show("Veuillez sélectionner un abonnement.", MSG_AUCUNE_SELECTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Abonnement abonnement = (Abonnement)bdgCommandesRevuesListe.List[bdgCommandesRevuesListe.Position];

            // Vérification côté client : exemplaires reçus pendant la période
            List<Exemplaire> exemplaires = controller.GetExemplairesRevue(revueCommandeEnCours.Id);
            bool aDesParutions = exemplaires.Exists(ex =>
                UtilitairesAbonnement.ParutionDansAbonnement(
                    abonnement.DateCommande,
                    abonnement.DateFinAbonnement,
                    ex.DateAchat));

            if (aDesParutions)
            {
                MessageBox.Show(
                    "Suppression impossible : des exemplaires ont été reçus pendant la période de cet abonnement.",
                    MSG_REGLE_METIER, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Confirmer la suppression de cet abonnement ?",
                MSG_CONFIRMATION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (controller.SupprimerAbonnement(abonnement.Id))
                {
                    MessageBox.Show("Abonnement supprimé avec succès.", MSG_SUCCES,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lesAbonnements = controller.GetAbonnementsRevue(revueCommandeEnCours.Id);
                    RemplirCommandesRevuesListe(lesAbonnements);
                }
                else
                {
                    MessageBox.Show(MSG_ERREUR_SUPPRESSION, MSG_ERREUR,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
    }
}
