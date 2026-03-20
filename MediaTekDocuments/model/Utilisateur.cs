namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant un utilisateur authentifié de l'application.
    /// L'identifiant de service (IdService) détermine les droits d'accès dans l'interface.
    /// </summary>
    public class Utilisateur
    {
        /// <summary>Identifiant unique de l'utilisateur en base de données.</summary>
        public string Id { get; set; }

        /// <summary>Identifiant de connexion de l'utilisateur.</summary>
        public string Login { get; set; }

        /// <summary>Identifiant du service auquel appartient l'utilisateur (détermine ses droits).</summary>
        public string IdService { get; set; }

        /// <summary>Libellé du service de l'utilisateur (ex. : "Administratif", "Prêts").</summary>
        public string LibelleService { get; set; }

        /// <summary>
        /// Constructeur sans paramètre requis par Newtonsoft.Json pour la désérialisation.
        /// </summary>
        public Utilisateur() { }

        /// <summary>
        /// Initialise un utilisateur avec toutes ses propriétés.
        /// </summary>
        /// <param name="id">Identifiant unique en base de données.</param>
        /// <param name="login">Identifiant de connexion.</param>
        /// <param name="idService">Identifiant du service (droits d'accès).</param>
        /// <param name="libelleService">Libellé du service.</param>
        public Utilisateur(string id, string login, string idService, string libelleService)
        {
            this.Id = id;
            this.Login = login;
            this.IdService = idService;
            this.LibelleService = libelleService;
        }
    }
}
