namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant une étape du cycle de vie d'une commande de document
    /// (ex. : "En cours", "Relancée", "Livrée", "Réglée").
    /// </summary>
    public class Suivi
    {
        /// <summary>Identifiant unique de l'étape de suivi.</summary>
        public string Id { get; set; }

        /// <summary>Libellé de l'étape affiché à l'utilisateur.</summary>
        public string Libelle { get; set; }

        /// <summary>
        /// Initialise une étape de suivi avec son identifiant et son libellé.
        /// </summary>
        /// <param name="id">Identifiant unique de l'étape.</param>
        /// <param name="libelle">Libellé de l'étape (ex. : "Livrée").</param>
        public Suivi(string id, string libelle)
        {
            Id = id;
            Libelle = libelle;
        }

        /// <summary>
        /// Retourne le libellé de l'étape, utilisé pour l'affichage dans les ComboBox.
        /// </summary>
        /// <returns>Le libellé de l'étape de suivi.</returns>
        public override string ToString() => Libelle;
    }
}
