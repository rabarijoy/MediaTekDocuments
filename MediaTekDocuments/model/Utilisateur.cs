namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant un utilisateur de l'application
    /// </summary>
    public class Utilisateur
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string IdService { get; set; }
        public string LibelleService { get; set; }

        public Utilisateur() { }

        public Utilisateur(string id, string login, string idService, string libelleService)
        {
            this.Id = id;
            this.Login = login;
            this.IdService = idService;
            this.LibelleService = libelleService;
        }
    }
}
