namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Suivi : représente une étape de suivi de commande de document
    /// </summary>
    public class Suivi
    {
        public string Id { get; set; }
        public string Libelle { get; set; }

        public Suivi(string id, string libelle)
        {
            Id = id;
            Libelle = libelle;
        }

        public override string ToString() => Libelle;
    }
}
